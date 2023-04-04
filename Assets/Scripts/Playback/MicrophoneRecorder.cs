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

    public void ToggleSound()
    {
        if (play)
        {
            audioSource.clip = Microphone.Start(null, true, 1000, 44100);
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
