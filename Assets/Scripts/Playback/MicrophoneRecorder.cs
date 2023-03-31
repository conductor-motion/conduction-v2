using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Microphone recording utility used to record microphone audio for evaluation purposes
public class MicrophoneRecorder : MonoBehaviour
{
    // Start the microhpone recording with a duration of 1000 (likely more than enough)
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 1000, 44100);
        audioSource.Play();
    }
}
