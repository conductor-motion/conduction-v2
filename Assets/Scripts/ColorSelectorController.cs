using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorSelectorController : MonoBehaviour
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
        switch(dropdown.options[dropdown.value].text)
        {
            case "Red":
                markupManager.selectColor(Color.red);
                break;
            case "Eraser":
                markupManager.selectColor(Color.black);
                break;
            case "White":
                markupManager.selectColor(Color.white);
                break;
        }
    }
}
