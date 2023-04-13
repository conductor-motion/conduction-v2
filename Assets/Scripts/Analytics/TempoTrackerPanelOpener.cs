using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enables or disables panel when user clicks button to open or close panel

public class TempoTrackerPanelOpener : MonoBehaviour
{
    public GameObject Panel;
    
    public void OpenPanel() {
        if(Panel != null) {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
   
}
