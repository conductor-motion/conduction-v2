using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enables or disables panel when user clicks button to open or close panel

public class TempoTrackerVideoPanelOpener : MonoBehaviour
{
    public GameObject VideoPanel;
    
    public void OpenPanel() {
        if(VideoPanel != null) {
            bool isActive = VideoPanel.activeSelf;
            VideoPanel.SetActive(!isActive);
        }
    }
}
