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

    // Remember windowed resolution
    private static int height;
    private static int width;

    // Begin the program in fullscreen
    private void Start()
    {
        if (!isInitialized)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            isFullScreen = true;
            isWindowed = false;
            goWindowed = false;

            height = Screen.height;
            width = Screen.width;
        }
        isInitialized = true;
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
        // Remember old resolution
        height = Screen.currentResolution.height;
        width = Screen.currentResolution.width;

        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        isWindowed = false;

        // Fix scaling issues
        Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
    }

    // Fulfill the request to go windowed
    public void setWindowed()
    {
        // Swap resolution with old
        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        Screen.fullScreenMode = FullScreenMode.Windowed;
        isFullScreen = false;
        isWindowed = true;
    }
}
