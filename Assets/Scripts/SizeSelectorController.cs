using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SizeSelectorController : MonoBehaviour
{
    private Slider slider;
    private MarkupManager markupManager;
    // Start is called before the first frame update
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
