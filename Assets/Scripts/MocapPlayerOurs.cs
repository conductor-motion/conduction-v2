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

        //Non-Vanilla (Added by us)

        //PlayAnimationClip(recordedClip);

        btnClick.onClick.AddListener(saveAnimationToList);
        //End Non-Vanilla
    }


    /// <summary>
    /// Sets the animation clip as default animator state and plays it.
    /// </summary>
    /// <param name="animClip">Animation clip to play</param>
    /// <returns>true on success, false otherwise</returns>
    public bool PlayAnimationClip(AnimationClip animClip)
    {
        string animClipName = animClip != null ? animClip.name : string.Empty;

        if (playerAnimator && animClip)
        {
#if UNITY_EDITOR
            RuntimeAnimatorController animatorCtrlRT = playerAnimator.runtimeAnimatorController;

            UnityEditor.Animations.AnimatorController animatorCtrl = animatorCtrlRT as UnityEditor.Animations.AnimatorController;
            UnityEditor.Animations.ChildAnimatorState[] animStates = animatorCtrl.layers.Length > 0 ?
                animatorCtrl.layers[0].stateMachine.states : new UnityEditor.Animations.ChildAnimatorState[0];
            bool bStateFound = false;

            for (int i = 0; i < animStates.Length; i++)
            {
                UnityEditor.Animations.ChildAnimatorState animState = animStates[i];

                if (animState.state.name == animClipName)
                {
                    animatorCtrl.layers[0].stateMachine.states[i].state.motion = animClip;
                    animatorCtrl.layers[0].stateMachine.defaultState = animatorCtrl.layers[0].stateMachine.states[i].state;

                    bStateFound = true;
                    break;
                }
            }

            if (!bStateFound && animatorCtrl.layers.Length > 0)
            {
                UnityEditor.Animations.AnimatorState animState = animatorCtrl.layers[0].stateMachine.AddState(animClipName);
                animState.motion = animClip;

                animatorCtrl.layers[0].stateMachine.defaultState = animState;
            }
#endif
        }

        if (playerAnimator && animClipName != string.Empty)
        {
            transform.position = initialPos;
            transform.rotation = initialRot;

            playerAnimator.Play(animClipName);
            return true;
        }

        return false;
    }

    public void returnHome()
    {
        SceneManager.LoadScene("LandingPage");
    }

    public void saveAnimationToList()
    {
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

