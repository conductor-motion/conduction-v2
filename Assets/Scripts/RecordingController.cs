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

    // Audio recording
    private bool canRecordAudio = false;
    private string chosenMic = "";
    public AudioSource audioSource;

    // "Audio vectors" storage so this can be serialized to a file and loaded with the animation when needed
    List<float> audioRecording = new List<float>();
    public static float[] finalRecording;
    public static AudioClip recordedAudio;

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
        videoCapture.saveFolder = "Assets/Conduction/Data/" + MainManager.Instance.dirPath;

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

        // If we found any microhpone that works, we can record audio
        if (firstAcceptableMic.Length > 0)
        {
            canRecordAudio = true;

            if (chosenMic.Length == 0) chosenMic = firstAcceptableMic;
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
        if(canRecordAudio)
        {
            audioSource.clip = Microphone.Start(chosenMic, true, 60, 44100);
            //audioSource.Play();
            Invoke("ResizeRecording", 60);
        }

        // Begin recording motion
        isCountingDown = false;
        isRecording = true;
        videoCapture.StartCapture();
        StartCoroutine(SwapIcon());
    }

    // Add the next minute of recording audio to the audio vector list
    void ResizeRecording()
    {
        if (isRecording)
        {
            int length = 44100 * 60;
            float[] clipData = new float[length];
            audioSource.clip.GetData(clipData, 0);
            audioRecording.AddRange(clipData);
            Invoke("ResizeRecording", 60);
        }
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            // Halt recording audio and record its last sub-second audio to the vector list
            if (canRecordAudio)
            {
                int length = Microphone.GetPosition(chosenMic);
                Microphone.End(null);
                float[] clipData = new float[length];
                audioSource.clip.GetData(clipData, 0);

                // Create a final concatenated audio clip
                float[] fullClip = new float[clipData.Length + audioRecording.Count];
                for (int i = 0; i < fullClip.Length; i++)
                {
                    if (i < audioRecording.Count)
                    {
                        fullClip[i] = audioRecording[i];
                    }
                    else
                    {
                        fullClip[i] = clipData[i - audioRecording.Count];
                    }
                }

                finalRecording = fullClip;

                // Create a Unity audio clip from the recorded data to play in playback
                recordedAudio = AudioClip.Create("recordingAudio", finalRecording.Length, 1, 44100, false);
                recordedAudio.SetData(finalRecording, 0);
                
            }


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
