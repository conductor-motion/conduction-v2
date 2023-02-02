using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
    public UnityEngine.UI.Text infoText;

    [Tooltip("Sprite to display while recording is in progress.")]
    public Texture2D recordingTexture;

    [Tooltip("Sprite to display while recording is not in progress.")]
    public Texture2D notRecordingTexture;

    // Control the recording button
    private bool recordButtonPressed = false;
    private bool editorOverride = false;
    private Sprite notRec;
    private Sprite isRec;

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


    void Start()
    {
        if(MainManager.Instance != null)
        {
            string dir = MainManager.Instance.dirPath;
            //device.pathToRecord = dir;

           // string fullPath = Application.dataPath + device.path + dir;
           /*
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(fullPath);
            }*/
        }

        // Instantiate sprites for icon swapping
        isRec = Sprite.Create(recordingTexture, new Rect(0.0f, 0.0f, recordingTexture.width, recordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        notRec = Sprite.Create(notRecordingTexture, new Rect(0.0f, 0.0f, notRecordingTexture.width, notRecordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

        MetronomeUI = GameObject.Find("Metronome UI");
        AxisInstructionsUI = GameObject.Find("Axes instructions");
    }

    void Update()
    {
        if (Input.GetKeyDown("space") || recordButtonPressed)
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
        recInfo.gameObject.SetActive(false);
        AxisInstructionsUI.SetActive(false);
        MetronomeUI.SetActive(false);

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
        StartCoroutine(SwapIcon());
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            MocapPlayerOurs.existingRecording = false;
            //webCamInput.SaveVideo();
            //visuallizer.SaveDataFile();
            SceneManager.LoadScene("ViewingPage");
        }
    }
}
