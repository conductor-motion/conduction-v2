using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SizeSelectorController : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private MarkupManager markupManager;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = this.gameObject.GetComponent<TMP_Dropdown>();
        markupManager = GameObject.FindGameObjectWithTag("MarkupManager").GetComponent<MarkupManager>();
        dropdown.onValueChanged.AddListener(delegate { ValueChangeCheck(dropdown); });
    }

    private void ValueChangeCheck(TMP_Dropdown dropdown)
    {
        switch (dropdown.options[dropdown.value].text)
        {
            case "Small":
                markupManager.selectSize(2);
                break;
            case "Medium":
                markupManager.selectSize(5);
                break;
            case "Large":
                markupManager.selectSize(25);
                break;
        }
    }
}
