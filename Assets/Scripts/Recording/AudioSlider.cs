using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
//using Newtonsoft.Json.Linq;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] AudioMixer Mixer;
    [SerializeField] AudioSource Metronome;
    [SerializeField] TextMeshProUGUI ValueText;
    // Start is called before the first frame update

    private void Start()
    {
        ValueText.SetText("100%");
    }

    public void OnChangeSlider(float Value)
    {
        float v = Value * 100;
        ValueText.SetText($"{v:N0}%");
        Mixer.SetFloat("Volume", Mathf.Log10(Value) * 20);

        PlayerPrefs.SetFloat("Volume", Value);
        PlayerPrefs.Save();
    }
}
