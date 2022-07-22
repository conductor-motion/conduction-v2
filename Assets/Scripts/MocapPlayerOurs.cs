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
    // reference to the Animator-component
    private Animator playerAnimator = null;

    // initial position & rotation of the model
    private Vector3 initialPos = Vector3.zero;
    private Quaternion initialRot = Quaternion.identity;

    //Non-Vanilla (Added by us)
    //For Testing with a saved file outside of client use
    public AnimationClip testing;
    public static AnimationClip recordedClip;

    public Button btnClick;
    public InputField userInput;
    public GameObject recordingPrefab;

    // If the recording was opened rather than just made, it was preexisting
    // If it is preexisting, we do not allow it to be re-saved, but rather renamed
    public static bool existingRecording = false;

    //End Non-Vanilla


    void Start()
    {
        // get reference to the animator component
        playerAnimator = GetComponent<Animator>();

        Animation anim = GetComponent<Animation>();
        anim.AddClip(recordedClip, recordedClip.name);
        anim.Play(recordedClip.name);

        // get initial position & rotation
        initialPos = transform.position;
        initialRot = transform.rotation;

        btnClick.onClick.AddListener(saveAnimationToList);

        // If this is not a new recording, the button should instead be to rename rather than resave
        if (existingRecording)
            btnClick.gameObject.GetComponentInChildren<Text>().text = "Rename Recording";

    }

    public void returnHome()
    {
        SceneManager.LoadScene("LandingPage");
    }

    public void saveAnimationToList()
    {
        // If the recording already exists, we just want to rename this file
        // TODO: checks for duplicate file names to prevent unintended overwrites
        if (existingRecording)
        {
            string input = userInput.text.Length == 0 ? DateTime.Now.ToString("mmddyyhhmmss") : userInput.text;

            File.Move(Application.streamingAssetsPath + "/" + recordedClip.name + ".anim", Application.streamingAssetsPath + "/" + input + ".anim");

            // Find and modify the associated recording for the clip so changes are immediately reflected
            ListController.savedList.Find(item => item.GetComponent<Recording>().clip.name == recordedClip.name).GetComponent<Recording>().text.text = input;

            // Modify the clip
            recordedClip.name = input;


            return;
        }

        GameObject savedClip = Instantiate(recordingPrefab);
        DontDestroyOnLoad(savedClip);

        if (userInput.text == "")
        {
            savedClip.GetComponent<Recording>().text.text = DateTime.Now.ToString("mmddyyhhmmss");
        }
        else
        {
            savedClip.GetComponent<Recording>().text.text = userInput.text;
        }

        recordedClip.name = DateTime.Now.ToString("mmddyyhhmmss");
        savedClip.GetComponent<Recording>().clip = recordedClip;

        ListController.savedList.Add(savedClip);
        SaveAnimationClip(savedClip.GetComponent<Recording>().text.text);
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

        // Currently ignoring root motion

        // Serialize our data and write it to a file that can be later retrieved
        // TODO: in-between layer that compresses the serialized data so it isn't absolutely ridiculous in file size
        BinaryFormatter bf = new BinaryFormatter();
        FileStream animFile = new FileStream(Application.streamingAssetsPath + "/" + fileName + ".anim", FileMode.Create);
        bf.Serialize(animFile, serializableCurves);
        animFile.Close();
    }
}

