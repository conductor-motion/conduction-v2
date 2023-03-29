using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Media controls for the audio and animation played on the viewing page
// * Timeline
// * Start, pause, rewind, forward
// * Volume
// * Playback speed (animation speed + audio pitch)
public class MediaControls : MonoBehaviour
{
    // The Animator is the primary way to control animations in Unity
    private Animator playerAnimator = null;
    private Animation playerAnimatorLegacy = null;
    private UnityEngine.UI.Slider speedController = null;
    private GameObject playButton;
    private string clipName;

    // Control trails when changing clip timing
    private GameObject[] trails;
    private float lastTrailLength;

    private bool isPlaying = true;
    // Speed multiplier
    private float animSpeed = 1f;
    private bool isRewind = false;
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

    // Audio controls involved when controlling the animation
    private bool isMuted = false;
    private UnityEngine.UI.Slider volumeController = null;
    private UnityEngine.UI.Text volumeLabel = null;
    private AudioSource audioSource;

    // Initialize variables necessary for controlling media playback
    void Start()
    {
        //clipName = MocapPlayerOurs.recordedClip.name;
        clipName = "test";
        playerAnimatorLegacy = GameObject.Find("U_Character_REF").GetComponent<Animation>();
        audioSource = GameObject.Find("U_Character_REF").GetComponent<AudioSource>();

        // Sets the speed multiplier to 1 so that it moves
        animSpeed = 1f;

        // Get the initial avatar body position to reset to
        initialPos = playerAnimatorLegacy.gameObject.transform.position;
        initialRot = playerAnimatorLegacy.gameObject.transform.rotation;

        // Get the trails in the scene
        trails = GameObject.FindGameObjectsWithTag("Trail");

        playButton = GameObject.FindGameObjectWithTag("PlayButton");

        if(!playerAnimatorLegacy)
        {
            Debug.Log("Animator not found by media controls.");
            isPlaying = false;
        }

        // Attach a listener to the speed control slider
        speedController = GameObject.Find("Speed Controller").GetComponent<UnityEngine.UI.Slider>();
        if (!speedController)
        {
            Debug.Log("Speed control slider not found.");
        }
        else
        {
            speedController.onValueChanged.AddListener(delegate { SliderChangeEvent(); });
        }

        // Attach a listerer to the volume control slider
        volumeController = GameObject.Find("Volume Control").GetComponent<UnityEngine.UI.Slider>();
        if (!volumeController)
        {
            Debug.Log("Volume control slider not found.");
        }
        else
        {
            volumeLabel = GameObject.Find("Volume Label").GetComponent<UnityEngine.UI.Text>();
            volumeController.onValueChanged.AddListener(delegate { VolumeSliderChangeEvent(); });

            if (!audioSource.clip) 
            {
                GameObject.Find("Divide Volume and Trail").SetActive(false);
                volumeController.gameObject.SetActive(false);
            }
        }

        // Get the timeline and its elements
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

    // Disable existing trails
    private void HideTrails()
    {
        lastTrailLength = trails[0].GetComponent<TrailRenderer>().time;
        foreach (GameObject trail in trails)
        {
            trail.GetComponent<TrailRenderer>().time = 0;
        }
    }

    // Resume showing existing trails
    private void ResumeTrails()
    {
        foreach (GameObject trail in trails)
        {
            trail.GetComponent<TrailRenderer>().time = lastTrailLength;
        }
    }

    // Resume trails after a very small delay so the animator can catch up
    private IEnumerator ResumeTrailsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject trail in trails)
        {
            trail.GetComponent<TrailRenderer>().time = lastTrailLength;
        }
    }

    // Ensure an animation loops if using non-legacy animation features
    // If the user is using the timeline, pause the animation and keep changing its playback position
    void Update()
    {
        // Check for mouse button down and up to allow for scrubbing through the timeline
        if (Input.GetKeyDown(KeyCode.Mouse0) && IsPointerOverTimeline())
        {
            scrubbing = true;
            audioSource.Pause();
            
            // Hide trails when using the timeline to move
            HideTrails();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && scrubbing == true)
        {
            scrubbing = false;
            audioSource.Play();
            SetSpeed(lastSpeed);

            // Resume showing trails again
            ResumeTrails();
        }

        if (doLoop)
        {
            // Force a loop of the current state in the animation
            if(playerAnimatorLegacy[clipName].normalizedTime > 1 && !isRewind && !scrubbing)
            {
                // Upon looping, hide the existing trails for a moment
                HideTrails();

                playerAnimatorLegacy.gameObject.transform.position = initialPos;
                playerAnimatorLegacy.gameObject.transform.rotation = initialRot;
                playerAnimatorLegacy[clipName].normalizedTime = 0f;
                playerAnimatorLegacy.Play(clipName);

                // Resume showing trails
                StartCoroutine(ResumeTrailsAfterDelay(0.1f));
            }

            // Used for the reversed playback
            if (playerAnimatorLegacy[clipName].normalizedTime < 0 && isRewind && !scrubbing)
            {
                // Upon looping, hide the existing trails for a moment
                HideTrails();

                playerAnimatorLegacy.gameObject.transform.position = initialPos;
                playerAnimatorLegacy.gameObject.transform.rotation = initialRot;
                playerAnimatorLegacy[clipName].normalizedTime = 1f;
                playerAnimatorLegacy.Play(clipName);

                // Resume showing trails
                StartCoroutine(ResumeTrailsAfterDelay(0.1f));
            }

        }

        // If a timeline exists, it should also be updated based on the clip's normalizedTime
        if (timeline)
        {
            // Move current playhead position to current clip timing
            int timelineBarWidth = (int)timelineBar.GetComponent<RectTransform>().sizeDelta.x;
            playhead.transform.localPosition = new Vector3(playerAnimatorLegacy[clipName].normalizedTime * timelineBarWidth - timelineBarWidth / 2, 0, 0);

            // If Mouse1 is down over the timeline, skip to that position
            if (scrubbing && IsPointerOverTimeline())
            {
                // Prevent repeated application of root motion if particular frames are chosen
                playerAnimatorLegacy[clipName].speed = 0;

                // We only care about the x position of the mouse
                float normalizedX = (float)Input.mousePosition.x / timelineBarWidth / timelineBar.GetComponent<RectTransform>().lossyScale.x;

                if (audioSource.clip) audioSource.time = audioSource.clip.length * normalizedX;
                playerAnimatorLegacy[clipName].normalizedTime = normalizedX;
                playerAnimatorLegacy.Play(clipName);
            }
        }
    }



    // Either pause or play the animation and audio depending on the current state
    public void TogglePlayback()
    {
        if(playerAnimatorLegacy)
        {
            isPlaying = !isPlaying;

            if (!isPlaying)
            {
                // Pause playback
                Time.timeScale = 0;

                // Pause the audio
                audioSource.Pause();

                // Updates the play button to use the pause sprite
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Play");
            }
            else
            {
                // Resume playback
                Time.timeScale = 1;

                // Resume the audio
                audioSource.Play();

                // Updates the play button to use the play sprite
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Pause");

                // In the event that the speed was changed while paused, update it
                SliderChangeEvent();
            }
        }
    }

    // Speed slider change event
    private void SliderChangeEvent()
    {
        SetSpeed((float)speedController.value);
    }

    // Volume slider change event
    private void VolumeSliderChangeEvent()
    {
        audioSource.volume = (float)volumeController.value;

        // Adjust the volume label by converting from the (0,1) float to percentage
        volumeLabel.text = (volumeController.value * 100).ToString("0") + "%";
    }

    // Returns the current animation speed multiplier (never negative)
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

    // Sets the animation and audio to play in reverse
    public void Reverse()
    {
        isRewind = true;
        playerAnimatorLegacy[clipName].speed = animSpeed * -1;
        lastSpeed = animSpeed;
        audioSource.pitch = -animSpeed; // ok this is the funniest thing I've ever supported
    }

    // Sets the animation and audio to play normally
    public void Forward()
    {
        isRewind = false;
        playerAnimatorLegacy[clipName].speed = animSpeed;
        lastSpeed = animSpeed;
        audioSource.pitch = animSpeed;
    }

    // Speed should likely impact hand trails as well, or offer some level of control over them
    // Changing speed while in a paused state results in strange behavior, so this is prohibited
    public void SetSpeed(float newSpeed)
    {
        if (playerAnimatorLegacy && isPlaying)
        {
            if (newSpeed >= 0)
            {
                lastSpeed = animSpeed;
                animSpeed = newSpeed;

                if (isRewind)
                {
                    playerAnimatorLegacy[clipName].speed = animSpeed * -1;
                    audioSource.pitch = animSpeed * -1;
                }
                else
                {
                    playerAnimatorLegacy[clipName].speed = animSpeed;
                    audioSource.pitch = animSpeed;
                }

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
