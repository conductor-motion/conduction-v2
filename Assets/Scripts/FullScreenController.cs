using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenController : MonoBehaviour
{
    // Start is called before the first frame update

    public Toggle fullscreenToggle;
    private bool isWindowed, goWindowed, isFullScreen, goFullScreen;

    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        isWindowed = false;
        goWindowed = false;
    }

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

    public void setFullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        isFullScreen = true;
        isWindowed = false;

    }

    public void setWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        isFullScreen = false;
        isWindowed = true;
    }


}
