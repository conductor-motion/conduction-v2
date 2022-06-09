using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Metronome : MonoBehaviour
{
    public MetronomeStorage metronome;
    public Toggle metronomeToggle;
    public TMP_InputField timeSigUpTextBox;
    public TMP_Dropdown timeSigDownDropdown;
    private AudioSource audioSource;
    private BeatChecker beatChecker;
    // Start is called before the first frame update
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

    void updateTimeSig()
    {
        metronome.SetTimeSigLow((int)Mathf.Pow(2,timeSigDownDropdown.value));
        metronome.SetTimeSigUp(int.Parse(timeSigUpTextBox.text));
        beatChecker.stopMetronome();
        if (metronomeToggle.isOn)
        {
            beatChecker.startMetronome();
        }
    }
}
