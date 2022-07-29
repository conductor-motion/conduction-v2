using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Control the size of markup with a slider
public class SizeSelectorController : MonoBehaviour
{
    private Slider slider;
    private MarkupManager markupManager;

    // Assign the event listener for the markup size slider
    void Start()
    {
        slider = this.gameObject.GetComponent<Slider>();
        markupManager = GameObject.FindGameObjectWithTag("MarkupManager").GetComponent<MarkupManager>();
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });
    }

    private void ValueChangeCheck(Slider slider)
    {
        markupManager.SelectSize((int)slider.value);
    }
}
