using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MediaControls : MonoBehaviour
{
    // The Animator is the primary way to control animations in Unity
    private Animator playerAnimator = null;
    private UnityEngine.UI.Slider speedController = null;
    private GameObject playButton;
    private string clipName;
    private GameObject[] trails;

    private bool isPlaying = true;
    private float animSpeed = 1f;
    private bool doLoop = true;

    private float pauseTime;
    private bool resumeTrailCoroutineRunning = false;

    private GameObject timeline = null;
    private bool scrubbing = false;
    private float lastSpeed = 1f;
    private GameObject timelineBar;
    private GameObject playhead;

    // Initial root position to reset to after every loop
    private Vector3 initialPos = Vector3.zero;
    private Quaternion initialRot = Quaternion.identity;

    // This helps determine the clip name and is set whenever we select an animation to play
    static public int clipOffset = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.Find("/RobotAnimated").GetComponent<Animator>();

        // Sets the speed multiplier to 1 so that it moves
        playerAnimator.SetFloat("Speed", 1);       

        // Get the initial avatar body position to reset to
        initialPos = playerAnimator.gameObject.transform.position;
        initialRot = playerAnimator.gameObject.transform.rotation;

        trails = GameObject.FindGameObjectsWithTag("Trail");

        playButton = GameObject.FindGameObjectWithTag("PlayButton");

        if (!playerAnimator)
        {
            Debug.Log("Animator not found by media controls.");
            isPlaying = false;
        }

        clipName = MocapPlayerOurs.recordedClip.name;

        // Attach a listener to the speed control slider
        speedController = GetComponentInChildren<UnityEngine.UI.Slider>();
        if (!speedController)
        {
            Debug.Log("Speed control slider not found as a child element.");
        }
        else
        {
            speedController.onValueChanged.AddListener(delegate { SliderChangeEvent(); });
        }

        timeline = GameObject.Find("Timeline");
        if (!timeline)
        {
            Debug.Log("Media controls could not locate the timeline. Please ensure it is named \"Timeline\".");
        }
        else
        {
            timelineBar = GameObject.Find("Timeline/Bar");
            playhead = GameObject.Find("Bar/Playhead");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse button down and up to allow for scrubbing through the timeline
        if (Input.GetKeyDown(KeyCode.Mouse0) && IsPointerOverTimeline())
        {
            scrubbing = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            scrubbing = false;
            playerAnimator.SetFloat("Speed", lastSpeed);
        }

        if (doLoop)
        {
            // Force a loop of the current state in the animation in a crude method
            // This is necessary if we cannot programmatically set the animationclip to loop
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && playerAnimator.GetFloat("Speed") == 1)
            {
                // There's an interesting side effect in which the avatar does not reset between loops
                playerAnimator.gameObject.transform.position = initialPos;
                playerAnimator.gameObject.transform.rotation = initialRot;
                playerAnimator.Play(clipName, 0, 0f);
            }
            // Used for the reversed playback
            else if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0 && playerAnimator.GetFloat("Speed") == -1)
            {
                playerAnimator.gameObject.transform.position = initialPos;
                playerAnimator.gameObject.transform.rotation = initialRot;
                playerAnimator.Play(clipName, 0, 1f);
            }
        }

        // If a timeline exists, it should also be updated based on the clip's normalizedTime
        // Note: if the timeline is used to skip to a position in a clip, then the root motion will differ slightly
        // looking into a solution for this, but it is a minor issue
        if (timeline)
        {
            // Move current playhead position to current clip timing
            int timelineBarWidth = (int)timelineBar.GetComponent<RectTransform>().sizeDelta.x;
            playhead.transform.localPosition = new Vector3(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * timelineBarWidth - timelineBarWidth / 2, 0, 0);

            // If Mouse1 is down over the timeline, skip to that position
            if (scrubbing && IsPointerOverTimeline())
            {
                // Prevent repeated application of root motion if particular frames are chosen
                playerAnimator.SetFloat("Speed", 0);

                // We only care about the x position of the mouse
                float normalizedX = (float)Input.mousePosition.x / timelineBarWidth;

                playerAnimator.Play(clipName, 0, normalizedX);
            }
        }
    }

    public void TogglePlayback()
    {
        if (playerAnimator)
        {
            isPlaying = !isPlaying;

            if (!isPlaying)
            {
                // Pause playback
                Time.timeScale = 0;

                // Updates the play button to use the pause sprite
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Play");
            }
            else
            {
                // Resume playback
                Time.timeScale = 1;

                // Updates the play button to use the play sprite
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Pause");

                // In the event that the speed was changed while paused, update it
                SliderChangeEvent();
            }
        }
    }

    private void SliderChangeEvent()
    {
        SetSpeed((float)speedController.value);
    }

    public float GetSpeed()
    {
        return animSpeed;
    }

    // Controls the speed of the trails to be relative to the inverse speed of the animation since time is how long the trail is alive
    private void UpdateTrailsLife(float newSpeed)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (newSpeed != 0)
                trails[i].GetComponent<TrailRenderer>().time = 4 / newSpeed;
            else
                trails[i].GetComponent<TrailRenderer>().time = Mathf.Infinity;
        }
    }

    public void Reverse()
    {
        playerAnimator.SetFloat("Speed", -1);
        lastSpeed = playerAnimator.GetFloat("Speed");
    }
    public void Forward()
    {
        playerAnimator.SetFloat("Speed", 1);
        lastSpeed = playerAnimator.GetFloat("Speed");
    }

    // Speed should likely impact hand trails as well, or offer some level of control over them
    // Changing speed while in a paused state results in strange behavior, so this is prohibited
    public void SetSpeed(float newSpeed)
    {
        if (playerAnimator && isPlaying)
        {
            if (newSpeed >= 0)
            {
                playerAnimator.speed = animSpeed = newSpeed;

                if(!resumeTrailCoroutineRunning)
                    UpdateTrailsLife(newSpeed);
                
            }
            else
            {
                Debug.Log("Animation speeds below 0 are not well-supported. Preferred method is moving the playhead");
            }
        }
    }

    // These functions perform a raycast to check whether the mouse is over the UI so that the user can't draw while selecting UI elements
    // Performance impact of calling this on mouse down is unknown
    private bool IsPointerOverTimeline()
    {
        return IsPointerOverTimeline(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverTimeline(List<RaycastResult> eventSystemRaycastResults)
    {
        for(int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.name == "Bar")
                return true;
        }
        return false;
    }

    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
