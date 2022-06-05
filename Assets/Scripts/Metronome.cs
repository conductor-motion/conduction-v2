using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public MetronomeStorage metronome;
    private BeatChecker beatChecker;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<MetronomeStorage>();
        beatChecker = GameObject.FindGameObjectWithTag("Metronome").GetComponent<BeatChecker>();
        audioSource = GameObject.FindGameObjectWithTag("Metronome").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (beatChecker.getPlayBeat())
        {
            Debug.Log("click");
            if (beatChecker.isFirstBeat())
            {
                audioSource.clip = metronome.metUpBeat;
            }
            else
            {
                audioSource.clip = metronome.metLowBeat;
            }
            audioSource.Play();
        }
    }
}
