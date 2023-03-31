using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;

public class ReplayController : MonoBehaviour
{
    // initial position & rotation of the model
    private Vector3 initialPos = Vector3.zero;
    private Quaternion initialRot = Quaternion.identity;

    // For passing in the video
    public WebCamInput webCamInput;
    public VideoPlayer videoPlayer;

    // For ensuring that the file is possible to save
    private char[] invalidFileChars;
    private int maxFileNameLength;

    // If the recording was opened rather than just made, it was preexisting
    // If it is preexisting, we do not allow it to be re-saved, but rather renamed
    public static bool existingRecording = false;

    // For saving and renaming animation files
    public Button btnClick;
    public InputField userInput;
    public GameObject recordingPrefab;
    public Text errorField;


    // Controlling the return home prompt if we have unsaved data
    public SceneLoader sceneLoader;

    void Start()
    {
        if (MainManager.Instance != null)
        {
            string dir = MainManager.Instance.dirPath;
            MainManager.Instance.SetMode("Replaying");

            if (!File.Exists(dir))
            {
                Debug.LogError("No recording found." + dir);
            }

            videoPlayer.url = dir;
        }

        // get the characters that cannot be included in the recording name due to file system limitations
        // (these limitations can be avoided by re-doing the save/load system to work from a data source rather than the file system)
        invalidFileChars = Path.GetInvalidFileNameChars();
        maxFileNameLength = 260 - 6; // Default windows maximum (260) - the ".audio" extension

        // get initial position & rotation
        initialPos = transform.position;
        initialRot = transform.rotation;

        // If this is not a new recording, the button should instead be to rename rather than resave
        if (existingRecording)
        {
            btnClick.gameObject.GetComponentInChildren<Text>().text = "Rename Recording";
            userInput.text = MainManager.Instance.dirPath;
        }
        else
        {
            // We should prompt the user if they try to leave as they have unsaved progress
            sceneLoader.promptBeforeGo = true;
        }

        // If this is not an existing recording, we have to load the audio from the file system and build an audio clip
        /*if (existingRecording)
        {
            try
            {
                audioSource.clip = LoadRecordingFile(recordedClip.name);
            }
            catch
            {
                Debug.Log("This is a legacy animation with no included audio to play.");
            }
        }
        else
        {
            audioRecordingData = MocapRecorderOurs.finalRecording;
            audioSource.clip = MocapRecorderOurs.recordedAudio;
        }

        audioSource.loop = true;
        audioSource.Play();*/

    }
}
