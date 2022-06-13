using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaControls : MonoBehaviour
{
    // The Animator is the primary way to control animations in Unity
    private Animator playerAnimator = null;
    private UnityEngine.UI.Slider speedController = null;
    private string clipName;

    private bool isPlaying = true;
    private float animSpeed = 1f;
    private bool doLoop = true;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.Find("/RobotAnimated").GetComponent<Animator>();

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
        // Force a loop of the current state in the animation in a crude method
        // This is necessary if we cannot programmatically set the animationclip to loop, I think
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            // There's an interesting side effect in which the avatar does not reset between loops
            playerAnimator.Play(clipName, 0, 0f);
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
                // "StartPlayback" is in referrence to playback mode, in which the animator is manually controlled
                // This results in a pausing effect, and if we wanted allows for timing control of the animation
                playerAnimator.StartPlayback();
            }
            else
            {
                // Resume playback
                playerAnimator.StopPlayback();

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

    // Speed should likely impact hand trails as well, or offer some level of control over them
    // Changing speed while in a paused state results in strange behavior, so this is prohibited
    public void SetSpeed(float newSpeed)
    {
        if (playerAnimator && isPlaying)
        {
            if (newSpeed >= 0)
            {
                playerAnimator.speed = animSpeed = newSpeed;
            }
            else
            {
                Debug.Log("Animation speeds below 0 are not well-supported. Preferred method is moving the playhead");
            }
        }
    }
}
