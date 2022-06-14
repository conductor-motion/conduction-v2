using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.Find("/RobotAnimated").GetComponent<Animator>();

        // Sets the speed multiplier to 1 so that it moves
        playerAnimator.SetFloat("Speed", 1);       

        trails = GameObject.FindGameObjectsWithTag("Trail");

        playButton = GameObject.FindGameObjectWithTag("PlayButton");

        if (!playerAnimator)
        {
            Debug.Log("Animator not found by media controls.");
            isPlaying = false;
        }

        clipName = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

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
    }

    // Update is called once per frame
    // Update playhead position/timeline
    void Update()
    {
        if (doLoop)
        {
            // Force a loop of the current state in the animation in a crude method
            // This is necessary if we cannot programmatically set the animationclip to loop, I think
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && playerAnimator.GetFloat("Speed") == 1)
            {
                // There's an interesting side effect in which the avatar does not reset between loops
                playerAnimator.Play(clipName, 0, 0f);
            }
            // Used for the reversed playback
            else if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0 && playerAnimator.GetFloat("Speed") == -1)
            {
                playerAnimator.Play(clipName, 0, 1f);
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
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Pause");
            }
            else
            {
                // Resume playback
                Time.timeScale = 1;

                // Updates the play button to use the play sprite
                playButton.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Play");

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
    }
    public void Forward()
    {
        playerAnimator.SetFloat("Speed", 1);
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
}
