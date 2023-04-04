using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryText : MonoBehaviour
{
    public Text summaryText;

    public void Awake() {
        summaryText = transform.Find("SummaryText").GetComponent<Text>();

        displaySummaryText();
    }

    public void displaySummaryText() {
        gameObject.SetActive(true);
        //hard coded, change later
        summaryText.text = "The song perfomed has (low/average/high) enegery.";
    }
}
