using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls the Evereal video capture utility with customizable buttons rather than the drawn
// GUI buttons used by the tool
public class CaptureController : MonoBehaviour
{
    public Evereal.VideoCapture.VideoCapture capture;
    public GameObject cancelButton;

    private Text buttonText;

    // Attach listeners to the buttons used
    private void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(onPress);
        cancelButton.GetComponent<Button>().onClick.AddListener(cancelCapture);
    }

    // Depending on the current status, either start or stop the video capture
    private void onPress()
    {
        // If we are not currently recording or processing, then when clicked we move to the STARTED state
        // We allow you to cancel in this state with the cancelButton
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

    // Update button texts depending on the state of the video capture utility
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

    // Cancel the capture, resulting in a lack of processing and merging but a halt of video capture
    private void cancelCapture()
    {
        cancelButton.SetActive(false);
        buttonText.text = "Start Recording";
        capture.CancelCapture();
    }
}