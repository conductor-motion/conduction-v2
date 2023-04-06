using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Microphone recording utility used to record microphone audio for evaluation purposes
public class MicrophoneRecorder : MonoBehaviour
{
    bool play = true;
    AudioSource audioSource;
    // Start the microhpone recording with a duration of 1000 (likely more than enough)
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 1000, 44100);
        audioSource.Play();
    }
    private void Update()
    {
        if (play)
        {
            audioSource.mute = false;
        }
        else
        {
            audioSource.mute = true;
        }
    }

    public void ToggleSound()
    {
        play = !play;
    }
}
