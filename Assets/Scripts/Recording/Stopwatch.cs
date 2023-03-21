using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class Stopwatch : MonoBehaviour
{
    bool stopwatchActive = false;
    float currentTime;
    public GameObject _stopwatch;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive)
        {
            currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        _stopwatch.GetComponent<TextMeshProUGUI>().text = time.ToString(@"mm\:ss");
    }

    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleStopwatch();
        }
    }

    public void ToggleStopwatch()
    {
        stopwatchActive = !stopwatchActive;
        currentTime = 0;
    }
}
