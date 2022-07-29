using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tool for toggling fullscreen and windowed states of the Application
public class FullScreenController : MonoBehaviour
{
    public Toggle fullscreenToggle;
    private bool isWindowed, goWindowed, isFullScreen, goFullScreen;

    // Begin the program in fullscreen
    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        isWindowed = false;
        goWindowed = false;
    }

    // If a state change is requested, fulfill that request and reset the request
    private void Update()
    {
        goWindowed = !fullscreenToggle.isOn;

        if(isFullScreen && goWindowed)
        {
            setWindowed();
        }
        else if(isWindowed && !goWindowed)
        {
            setFullScreen();
        }
    }

    // Fulfill the request to go fullscreen
    public void setFullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        isWindowed = false;

    }

    // Fulfill the request to go windowed
    public void setWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        isFullScreen = false;
        isWindowed = true;
    }
}
