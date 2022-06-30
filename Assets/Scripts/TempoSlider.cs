using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempoSlider : MonoBehaviour
{
    private Slider tempoSlider;
    public TextMeshProUGUI tempoLabel;
    private Metronome metronome;

    // Start is called before the first frame update
    void Start()
    {
        // Assigns gameObjects
        tempoSlider = this.gameObject.GetComponent<Slider>();
        tempoSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<Metronome>();

        // Ensures that the tempo slider is the same value as the metronome tempo
        ValueChangeCheck();
    }

    public void ValueChangeCheck()
    {
        // Updates the tempo label to be the same value as the slider
        tempoLabel.text = tempoSlider.value.ToString();

        // Updates the metronome's tempo to the value of the slider
        metronome.metronome.SetTempo(tempoSlider.value);
    }
}
