using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// MocapPlayer plays the recorded animation on the model it is attached to.
/// </summary>
public class MocapPlayerOurs : MonoBehaviour
{
    // initial position & rotation of the model
    private Vector3 initialPos = Vector3.zero;
    private Quaternion initialRot = Quaternion.identity;

    // For Testing with a saved file outside of client use
    public AnimationClip testing;
    public static AnimationClip recordedClip;

    // For saving and renaming animation files
    public Button btnClick;
    public InputField userInput;
    public GameObject recordingPrefab;
    public Text errorField;

    // If the recording was opened rather than just made, it was preexisting
    // If it is preexisting, we do not allow it to be re-saved, but rather renamed
    public static bool existingRecording = false;

    // For playing back audio during the recording
    private AudioSource audioSource;
    private float[] audioRecordingData;

    // For ensuring that the file is possible to save
    private char[] invalidFileChars;
    private int maxFileNameLength;

    // Controlling the return home prompt if we have unsaved data
    public SceneLoader sceneLoader;

    void Start()
    {
        // get the characters that cannot be included in the recording name due to file system limitations
        // (these limitations can be avoided by re-doing the save/load system to work from a data source rather than the file system)
        invalidFileChars = Path.GetInvalidFileNameChars();
        maxFileNameLength = 260 - 6; // Default windows maximum (260) - the ".audio" extension

        // get reference to the animator component
        Animation anim = GetComponent<Animation>();
        anim.AddClip(recordedClip, recordedClip.name);
        anim.Play(recordedClip.name);

        // get reference to the audiosource
        audioSource = GetComponent<AudioSource>();
        
        // get initial position & rotation
        initialPos = transform.position;
        initialRot = transform.rotation;

        btnClick.onClick.AddListener(saveAnimationToList);

        // If this is not a new recording, the button should instead be to rename rather than resave
        if (existingRecording)
        {
            btnClick.gameObject.GetComponentInChildren<Text>().text = "Rename Recording";
        }
        else
        {
            // We should prompt the user if they try to leave as they have unsaved progress
            sceneLoader.promptBeforeGo = true;
        }

        // If this is not an existing recording, we have to load the audio from the file system and build an audio clip
        if (existingRecording)
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
        audioSource.Play();
    }

    public void saveAnimationToList()
    {
        string input = userInput.text.Length == 0 ? DateTime.Now.ToString("mmddyyhhmmss") : userInput.text;

        // Verify this is a possible filename (which prevents a lot of bugs and "escaping" the containing folder)
        if (input.IndexOfAny(invalidFileChars) != -1)
        {
            // Set the error text and stop
            errorField.text = "Your input contains an illegal character\nOnly use characters allowed in file names.";
            return;
        }
        
        // Verify that the file name is not too long
        if (input.Length >= maxFileNameLength)
        {
            errorField.text = "The selected name is too long.\nPlease use a shorter name.";
            return;
        }
        // No error, so ensure error text is hidden
        errorField.text = "";

        // Check for duplicates and prevent overwrites
        // Potential TODO: give option to overwrite


        // If the recording already exists, we just want to rename this file
        if (existingRecording)
        {
            File.Move(Path.Combine(Application.streamingAssetsPath, recordedClip.name + ".anim"), Path.Combine(Application.streamingAssetsPath, input + ".anim"));
            
            try
            {
                File.Move(Path.Combine(Application.streamingAssetsPath, recordedClip.name + ".audio"), Path.Combine(Application.streamingAssetsPath, input + ".audio"));
            }
            catch
            {
                // Audio file does not exist for this clip
                Debug.Log("No audio file found to move for this clip. Potentially a legacy animation.");
            }

            // Find and modify the associated recording for the clip so changes are immediately reflected
            ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == recordedClip.name).GetComponent<Recording>().text.text = input;

            // Modify the clip
            recordedClip.name = input;

            return;
        }

        // Create a persistent object to store Recording data
        GameObject savedClip = Instantiate(recordingPrefab);
        DontDestroyOnLoad(savedClip);

        if (userInput.text == "")
        {
            savedClip.GetComponent<Recording>().text.text = DateTime.Now.ToString("mmddyyhhmmss");
            userInput.text = savedClip.GetComponent<Recording>().text.text;
        }
        else
        {
            savedClip.GetComponent<Recording>().text.text = userInput.text;
        }

        recordedClip.name = userInput.text;
        savedClip.GetComponent<Recording>().clip = recordedClip;

        ListController.savedList.Add(savedClip);
        SaveAnimationClip(savedClip.GetComponent<Recording>().text.text);

        // We have just made a recording, so now any future button presses must rename the recording
        existingRecording = true;
        btnClick.gameObject.GetComponentInChildren<Text>().text = "Rename Recording";
    }

    // saves the animation clip in a serialized format
    public void SaveAnimationClip(string fileName)
    {
        // Our animation is a very large collection of objects and their curves
        // To serialize this, we need a list capable of associating those gameobjects to their relative paths, and then to their actual curves

        // (GameObject relative path, list of times and values)
        Dictionary<string, List<(float, float)>> serializableCurves = new Dictionary<string, List<(float, float)>>();

        foreach (KeyValuePair<string, AnimationCurve> data in MocapRecorderOurs.legacyCurves)
        {
            // Create the container list
            List<(float, float)> temporaryKeys = new List<(float, float)>();

            foreach (Keyframe key in data.Value.keys)
            {
                temporaryKeys.Add((key.time, key.value));
            }

            serializableCurves.Add(data.Key, temporaryKeys);
        }

        // Currently ignores all root motion, as it is not necessary for evaluation

        // Serialize our data and write it to a file that can be later retrieved
        // TODO: in-between layer that compresses the serialized data so it isn't absolutely ridiculous in file size
        BinaryFormatter bf = new BinaryFormatter();
        FileStream animFile = new FileStream(Path.Combine(Application.streamingAssetsPath, fileName + ".anim"), FileMode.Create);
        bf.Serialize(animFile, serializableCurves);
        animFile.Close();

        // Save the associated audio clip if one exists
        SaveAudioClip(fileName);

        // Since we have saved, no longer need to prompt before leaving
        sceneLoader.promptBeforeGo = false;
    }

    // Loads a specified audio clip from the file system
    public AudioClip LoadRecordingFile(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream audioFile = new FileStream(Path.Combine(Application.streamingAssetsPath, fileName + ".audio"), FileMode.Open);

        audioRecordingData = (float[])bf.Deserialize(audioFile);

        AudioClip clip = AudioClip.Create("recordingAudio", audioRecordingData.Length, 1, 44100, false);
        clip.SetData(audioRecordingData, 0);

        return clip;
    }

    // saves the audio clip in a serialized format
    public void SaveAudioClip(string fileName)
    {
        // The audio is already serializable in MocapRecorder, so we just have to write it to a file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream audioFile = new FileStream(Path.Combine(Application.streamingAssetsPath, fileName + ".audio"), FileMode.Create);
        bf.Serialize(audioFile, audioRecordingData);
        audioFile.Close();
    }
}

