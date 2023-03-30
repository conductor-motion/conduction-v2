using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Evereal.VideoCapture;
using System;
using TMPro;
//using UnityEditor.MemoryProfiler;
using UnityEngine.InputSystem;

public class RecordingController : MonoBehaviour
{
    [Tooltip("Sprite transforms that will be used to display the countdown, when recording starts.")]
    public Transform[] countdown;

    [Tooltip("Sprite transform that will be used to display recording in progress.")]
    public Transform recIcon;

    [Tooltip("Sprite transform for the recording button")]
    public UnityEngine.UI.Image recButton;

    [Tooltip("Tooltip information for the record button")]
    public Transform recInfo;

    [Tooltip("UI Text to display information messages.")]
    public TextMeshProUGUI infoText;

    [Tooltip("Sprite to display while recording is in progress.")]
    public Texture2D recordingTexture;

    [Tooltip("Sprite to display while recording is not in progress.")]
    public Texture2D notRecordingTexture;

    [Tooltip("HUD Elements that'll be hidden during the recording")]
    public HidableHud hideableHUD;

    [Tooltip("Metronome used during the recording")]
    public BeatChecker _metronome;

    // Control the recording button
    private bool recordButtonPressed = false;
    private bool editorOverride = false;
    private Sprite notRec;
    private Sprite isRec;
    private float recordTime = 0;

    // Elements to hide when recording
    private GameObject MetronomeUI;
    private GameObject AxisInstructionsUI;

    // recording parameters
    [HideInInspector]
    public static bool isRecording = false;
    private bool isCountingDown = false;


    // Game objects to save data and video
    public WebCamInput webCamInput;
    public PoseVisuallizer3D visuallizer;
    public VideoCapture videoCapture;
    public GameObject recordingPrefab;


    void Start()
    {
        if (MainManager.Instance != null)
            MainManager.Instance.SetMode("Recording");

        videoCapture.inputTexture = (RenderTexture)webCamInput.inputImageTexture;
        videoCapture.saveFolder = "Data/" + MainManager.Instance.dirPath;

        // Instantiate sprites for icon swapping
        isRec = Sprite.Create(recordingTexture, new Rect(0.0f, 0.0f, recordingTexture.width, recordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        notRec = Sprite.Create(notRecordingTexture, new Rect(0.0f, 0.0f, notRecordingTexture.width, notRecordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

        MetronomeUI = GameObject.Find("Metronome UI");
        AxisInstructionsUI = GameObject.Find("Axes instructions");

        string firstAcceptableMic = "";
        foreach (string device in Microphone.devices)
        {
            if (device.Contains("Microphone Array")) continue;
            if (firstAcceptableMic.Length == 0) firstAcceptableMic = device;

        }

    }

    void Update()
    {
        if (recordButtonPressed)
        {
            recordButtonPressed = false;
            if (!isRecording)
            {
                if (!isCountingDown)
                {
                    isCountingDown = true;
                    StartCoroutine(CountdownAndStartRecording());
                }
            }
            else
            {
                StopRecording();
            }
        }


        if (isRecording)
        {
            // record the current pose
            recordTime += Time.deltaTime;

            if (infoText & ((int)(recordTime * 10f) % 10) == 0)
            {
                infoText.text = string.Format("{0}:{1}", Math.Floor(recordTime / 60), (recordTime % 60).ToString("00"));
            }
        }
    }

    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RecordButton();
        }
    }

    public void RecordButton()
    {
        recordButtonPressed = !recordButtonPressed;
    }

    // displays the given message on screen and logs it to console
    private void ShowMessage(string sMessage)
    {
        if (infoText)
        {
            infoText.text = sMessage;
        }

        Debug.Log(sMessage);
    }

    // Coroutine for animating the icon of the record button
    private IEnumerator SwapIcon()
    {
        Sprite newSprite;

        if (isRecording)
        {
            newSprite = isRec;
        }
        else
        {
            newSprite = notRec;
        }

        // Change the sprite
        recButton.overrideSprite = newSprite;

        // Rudimentary "tweening" effect between sprites, similar to what's seen on an iPhone
        if (isRecording)
        {
            // Loop to take 0.5 seconds to achieve a smooth effect
            for (int i = 0; i < 10; i++)
            {
                recButton.transform.GetComponent<RectTransform>().sizeDelta -= new Vector2(1f, 1f);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                recButton.transform.GetComponent<RectTransform>().sizeDelta += new Vector2(1f, 1f);
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return 1;
    }


    // counts down (from 3 for instance), then starts the animation recording
    private IEnumerator CountdownAndStartRecording()
    {
        // Hide non-essential UI elements while recording

        hideableHUD.HideHud();

        if (countdown != null && countdown.Length > 0)
        {
            for (int i = 0; i < countdown.Length; i++)
            {
                if (countdown[i])
                    countdown[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(1f);

                if (countdown[i])
                    countdown[i].gameObject.SetActive(false);
            }
        }

        // Begin recording motion
        isCountingDown = false;
        isRecording = true;
        videoCapture.StartCapture();
        StartCoroutine(SwapIcon());
    }


    private void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            _metronome.stopMetronome();
            videoCapture.StopCapture();
            StartCoroutine(SwapIcon());
            videoCapture.OnComplete += HandleSceneChange;
        }
    }

    private void HandleSceneChange(object sender, CaptureCompleteEventArgs args)
    {
        GameObject rec = Instantiate(recordingPrefab);
        DontDestroyOnLoad(rec);
        rec.GetComponent<Recording>().text.text = MainManager.Instance.dirPath.Substring(MainManager.Instance.dirPath.LastIndexOf("/") + 1);
        rec.GetComponent<Recording>().fullDir = MainManager.Instance.dirPath;
        ListController.savedList.Insert(0, rec);
        SceneManager.LoadScene("ViewingPage");
    }
}
