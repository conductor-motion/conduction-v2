using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureController : MonoBehaviour
{
    public Evereal.VideoCapture.VideoCapture capture;
    public GameObject cancelButton;

    private Text buttonText;

    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(onPress);
        cancelButton.GetComponent<Button>().onClick.AddListener(cancelCapture);
    }

    private void onPress()
    {
        if (capture.status == Evereal.VideoCapture.CaptureStatus.READY)
        {
            cancelButton.SetActive(true);
            capture.StartCapture();
        }
        else if (capture.status == Evereal.VideoCapture.CaptureStatus.STARTED)
        {
            capture.StopCapture();
        }
    }

    private void Update()
    {
        if (capture.status == Evereal.VideoCapture.CaptureStatus.PENDING || capture.status == Evereal.VideoCapture.CaptureStatus.STOPPED)
        {
            cancelButton.SetActive(false);
            buttonText.text = "Processing";
        }
        else if (capture.status == Evereal.VideoCapture.CaptureStatus.READY)
        {
            buttonText.text = "Start Recording";
        }
        else if (capture.status == Evereal.VideoCapture.CaptureStatus.STARTED)
        {
            buttonText.text = "Stop Recording";
        }
    }

    private void cancelCapture()
    {
        cancelButton.SetActive(false);
        buttonText.text = "Start Recording";
        capture.CancelCapture();
    }
}