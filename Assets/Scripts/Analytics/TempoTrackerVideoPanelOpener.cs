using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
