using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureController : MonoBehaviour
{
    public Evereal.VideoCapture.VideoCapture capture;
    public string function;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(onPress);
    }

    private void onPress()
    {
        if (function == "start")
        {
            capture.StartCapture();
        }
        else if (function == "stop")
        {
            capture.StopCapture();
        }
    }
}