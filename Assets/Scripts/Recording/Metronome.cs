using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Controls the metronome behavior given to the user on the Recording page
public class Metronome : MonoBehaviour
{
    public MetronomeStorage metronome;
    public Toggle metronomeToggle;
    public TMP_InputField timeSigUpTextBox;
    public TMP_Dropdown timeSigDownDropdown;
    private AudioSource audioSource;
    private BeatChecker beatChecker;

    // Initialize the object references used to control the metronome
    void Start()
    {
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<MetronomeStorage>();
        beatChecker = GameObject.FindGameObjectWithTag("Metronome").GetComponent<BeatChecker>();
        audioSource = GameObject.FindGameObjectWithTag("Metronome").GetComponent<AudioSource>();
        timeSigUpTextBox.text = metronome.GetTimeSigUp().ToString();
        timeSigDownDropdown.value = (int)Mathf.Log(metronome.GetTimeSigLow(), 2);
        timeSigDownDropdown.onValueChanged.AddListener(delegate { updateTimeSig(); });
        timeSigUpTextBox.onValueChanged.AddListener(delegate { updateTimeSig(); });
    }

    // Update is called once per frame
    void Update()
    {
        // Checks every frame if it is an on-beat frame
        if (beatChecker.getPlayBeat())
        {
            // If it is it checks if it's the first beat of the measure to determine which metronome track to play
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

    void updateTimeSig()
    {
        // updates the metronome to be the new time signature if it's updated by the user
        metronome.SetTimeSigLow((int)Mathf.Pow(2,timeSigDownDropdown.value));
        if (!string.IsNullOrEmpty(timeSigUpTextBox.text))
            metronome.SetTimeSigUp(int.Parse(timeSigUpTextBox.text));
        beatChecker.stopMetronome();

        // restarts the metronome if it was playing while the value was updated
        if (metronomeToggle.isOn)
        {
            beatChecker.startMetronome();
        }
    }
}
