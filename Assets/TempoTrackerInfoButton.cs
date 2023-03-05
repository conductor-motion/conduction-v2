using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TempoTrackerInfoButton : MonoBehaviour
{
    TooltipScript tooltipScript;
    [SerializeField] GameObject tooltip;
    
    private void Awake() {
        string info = "This tool tracks the conductor's tempo every five seconds. The tempo is calculated by the total number of downbeats per minute.............. ";
        transform.Find("InfoButton").GetComponent<Button_UI>().MouseOverFunc = () => TooltipScript.static_displayTooltip(info);
        transform.Find("InfoButton").GetComponent<Button_UI>().MouseOutOnceFunc = () => TooltipScript.static_hideTooltip();
    }

   
}
