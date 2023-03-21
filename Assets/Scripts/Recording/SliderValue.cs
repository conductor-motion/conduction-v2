using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SliderValue : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _text;

    void Update()
    {
        _text.GetComponent<TextMeshProUGUI>().text = _slider.value.ToString("0");
        //Come back to later, attempt to make input box that would give value of to slider
        //_slider.value = float.Parse(_text.GetComponent<TMP_InputField>().text);
    }
}