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
        tempoSlider = this.gameObject.GetComponent<Slider>();
        tempoSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<Metronome>();
        ValueChangeCheck();
    }

    public void ValueChangeCheck()
    {
        tempoLabel.text = tempoSlider.value.ToString();
        metronome.metronome.setTempo(tempoSlider.value);
    }
}
