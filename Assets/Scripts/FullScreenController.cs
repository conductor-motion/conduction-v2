using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tool for toggling fullscreen and windowed states of the Application
public class FullScreenController : MonoBehaviour
{
    public Toggle fullscreenToggle;
    private static bool isInitialized = false;
    private static bool isWindowed, goWindowed, isFullScreen, goFullScreen;

    // Used for restoring state between fullscreen and windowed
    private int width, height;

    // Begin the program in fullscreen
    private void Start()
    {
        if (!isInitialized)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
            isFullScreen = true;
            isWindowed = false;
            goWindowed = false;
        }
        isInitialized = true;
        fullscreenToggle.isOn = isFullScreen;

        width = Screen.width;
        height = Screen.height;
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
        // Remember old resolution before we swap
        height = Screen.height;
        width = Screen.width;

        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen); // Fix UI scaling bug that occurs
        isFullScreen = true;
        isWindowed = false;
    }

    // Fulfill the request to go windowed
    public void setWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
        isFullScreen = false;
        isWindowed = true;
    }
}
