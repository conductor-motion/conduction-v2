using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using com.rfilkov.kinect;
using System;
using System.IO;
using UnityEngine.SceneManagement;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;

/// <summary>
/// MocapRecorder records the avatar motion into the given animation clip.
/// </summary>
public class MocapRecorderOurs : MonoBehaviour
{
    [Tooltip("The avatar, whose motion will be captured in the animation clip.")]
    public AvatarController avatarModel;

    [Tooltip("Full path to the file, where the animation clip will be saved at the end of animation recording.")]
    //public string animSaveToFile = "Assets/AzureKinectExamples/KinectDemos/MocapAnimatorDemo/Animations/Recorded.anim";
    //public string animSaveToFile = "Assets/K2Examples/KinectDemos/MocapAnimatorDemo/Animations/Recorded.anim";
    private static string tempRecordSave;
    private string animSaveToFile;
    private string animSaveToFileBuilt;

    [Tooltip("Whether to capture the root motion as well.")]
    public bool captureRootMotion = true;

    [Tooltip("The model used to play the recorded animation clip.")]
    public MocapPlayerOurs mocapPlayer;

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

    // reference to the avatar's animator component
    private Animator modelAnimator = null;

    // recording parameters
    private bool isRecording = false;
    private bool isCountingDown = false;
    private float animTime = 0;

    // human pose parameters
    private HumanPose humanPose = new HumanPose();
    private HumanPoseHandler humanPoseHandler = null;

    // animation curves to hold the recorded animation 
    private Dictionary<int, AnimationCurve> muscleCurves = new Dictionary<int, AnimationCurve>();
    private Dictionary<string, AnimationCurve> rootPoseCurves = new Dictionary<string, AnimationCurve>();

    // initial model's root position
    private Vector3 initialRootPos = Vector3.zero;

    //Non-Vanilla (Added By Us)

    private bool recordButtonPressed = false;
    private bool editorOverride = false;
    private Sprite notRec;
    private Sprite isRec;
    public static AnimationClip recordedClip;

    private GameObject MetronomeUI;
    private GameObject AxisInstructionsUI;

    //End Non-Vanilla

    // : )
    private Dictionary<GameObject, string> avatarComponents = new Dictionary<GameObject, string>();
    private Dictionary<string, AnimationCurve> legacyCurves = new Dictionary<string, AnimationCurve>();
    public static AnimationClip legacyAnimClip;

    void Start()
    {

        // Allow for bypassing the need for a sensor if in the Unity Editor
        if (UnityEngine.Application.isEditor)
        {
            editorOverride = true;
        }
        editorOverride = true;

        // Instantiate sprites for icon swapping
        isRec = Sprite.Create(recordingTexture, new Rect(0.0f, 0.0f, recordingTexture.width, recordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        notRec = Sprite.Create(notRecordingTexture, new Rect(0.0f, 0.0f, notRecordingTexture.width, notRecordingTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

        MetronomeUI = GameObject.Find("Metronome UI");
        AxisInstructionsUI = GameObject.Find("Axes instructions");

        if (avatarModel)
        {
            modelAnimator = avatarModel.gameObject.GetComponent<Animator>();

            // Recursively obtain every GameObject in the model and their relative paths
            MapAvatar(avatarModel.gameObject, "");

            if (modelAnimator)
            {
                initialRootPos = avatarModel.transform.position;
                humanPoseHandler = new HumanPoseHandler(modelAnimator.avatar, avatarModel.transform);
            }
            else
            {
                ShowMessage("The AvatarModel has no Animator-component!");
            }
        }
        else
        {
            ShowMessage("The AvatarModel is not set!");
        }
    }

    // Traverse the assigned avatarModel and recursively populate the avatarComponents dictionary for legacy anims
    void MapAvatar(GameObject obj, string rel)
    {
        // First insert the current part and then call this on every child
        // Contains an exception for trails and the avatar root
        if (obj != avatarModel.gameObject && !obj.GetComponent<TrailRenderer>()) avatarComponents.Add(obj, rel);

        foreach (Transform child in obj.transform)
        {
            MapAvatar(child.gameObject, rel + (rel.Length == 0 ? "" : "/") + child.gameObject.name);
        }
    }

    void Update()
    {
        // check for Space-key
        if(Input.GetButtonDown("Jump") || recordButtonPressed)
        {
            recordButtonPressed = false;
            if(!isRecording)
            {
                if(!isCountingDown && avatarModel && (editorOverride || avatarModel.playerId != 0))
                {
                    InitAnimationCurves();
                    isCountingDown = true;
                    StartCoroutine(CountdownAndStartRecording());
                }
            }
            else
            {
                StopRecording();
            }
        }

        if (isRecording && avatarModel && (editorOverride || avatarModel.playerId != 0))
        {
            // record the current pose
            animTime += Time.deltaTime;
            RecordAvatarPose();

            if (infoText & ((int)(animTime * 10f) % 10) == 0)
            {
                infoText.text = string.Format("{0:F0}s", animTime);
            }
        }

        // stop recording, if the user is lost
        if (avatarModel && (avatarModel.playerId == 0 && !editorOverride))
        {
            StopRecording();
        }
    }


    //void LateUpdate()
    //{
    //}

    //Non-Vanilla (Added By Us)

    public void RecordButton()
    {
        recordButtonPressed = !recordButtonPressed;
    }

    //End Non-Vanilla

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
        // Instantiate a new sprite from the textures given
        // Likely leads to some memory leak but is unlikely to be truly impactful
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
            // TODO: curves for more smooth animation
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

        isCountingDown = false;
        isRecording = true;
        ShowMessage("Recording started.");
        StartCoroutine(SwapIcon());

        /* if (recIcon)
        {
            recIcon.gameObject.SetActive(true);
        } */
    }


    // clears the animation curves before the recording starts
    private void InitAnimationCurves()
    {
        if (avatarModel == null)
            return;

        animTime = 0f;
        muscleCurves.Clear();
        rootPoseCurves.Clear();

        List<HumanBodyBones> mecanimBones = avatarModel.GetMecanimBones();
        foreach (HumanBodyBones boneType in mecanimBones)
        {
            for (int i = 0; i < 3; i++)
            {
                int muscle = HumanTrait.MuscleFromBone((int)boneType, i);

                if (muscle != -1)
                {
                    muscleCurves.Add(muscle, new AnimationCurve());
                }
            }
        }

        foreach (GameObject obj in avatarComponents.Keys)
        {
            legacyCurves.Add(obj.name + ".x", new AnimationCurve());
            legacyCurves.Add(obj.name + ".y", new AnimationCurve());
            legacyCurves.Add(obj.name + ".z", new AnimationCurve());
            legacyCurves.Add(obj.name + ".Qx", new AnimationCurve());
            legacyCurves.Add(obj.name + ".Qy", new AnimationCurve());
            legacyCurves.Add(obj.name + ".Qz", new AnimationCurve());
            legacyCurves.Add(obj.name + ".Qw", new AnimationCurve());
        }

        rootPoseCurves.Add("RootT.x", new AnimationCurve());
        rootPoseCurves.Add("RootT.y", new AnimationCurve());
        rootPoseCurves.Add("RootT.z", new AnimationCurve());

        rootPoseCurves.Add("RootQ.x", new AnimationCurve());
        rootPoseCurves.Add("RootQ.y", new AnimationCurve());
        rootPoseCurves.Add("RootQ.z", new AnimationCurve());
        rootPoseCurves.Add("RootQ.w", new AnimationCurve());
    }


    // stops the recording and saves the animation clip
    private void StopRecording()
    {
        if(isRecording)
        {
            isRecording = false;
            ShowMessage(string.Format("Recording stopped - saved {0:F3}s animation.", animTime));

            /* if (recIcon)
            {
                recIcon.gameObject.SetActive(false);
            } */

            bool isAnythingRecorded = (muscleCurves.Count > 0 && muscleCurves[0].length > 0) || 
                (rootPoseCurves.Count > 0 && rootPoseCurves["RootT.x"].length > 0);

            if (isAnythingRecorded)
            {
                recordedClip = CreateAnimationClip();
                legacyAnimClip = CreateLegacyAnimClip();
                Debug.Log("New Clip Created");
                // MocapPlayerOurs.recordedClip = recordedClip;
                MocapPlayerOurs.recordedClip = legacyAnimClip;
                SceneManager.LoadScene("ViewingPage");

                if (mocapPlayer)
                {
                    mocapPlayer.PlayAnimationClip(recordedClip);
                }
            }
            else
            {
                ShowMessage("Recording stopped - nothing to save.");
            }
            recInfo.gameObject.SetActive(true);
            StartCoroutine(SwapIcon());
        }
    }


    // records the current avatar pose to the animation curves
    private void RecordAvatarPose()
    {
        humanPoseHandler.GetHumanPose(ref humanPose);

        foreach (KeyValuePair<int, AnimationCurve> data in muscleCurves)
        {
            Keyframe key = new Keyframe(animTime, humanPose.muscles[data.Key]);
            data.Value.AddKey(key);
        }

        foreach (GameObject obj in avatarComponents.Keys)
        {
            // x
            Keyframe key = new Keyframe(animTime, obj.transform.localPosition.x);
            legacyCurves[obj.name + ".x"].AddKey(key);

            // y
            key = new Keyframe(animTime, obj.transform.localPosition.y);
            legacyCurves[obj.name + ".y"].AddKey(key);

            // z
            key = new Keyframe(animTime, obj.transform.localPosition.z);
            legacyCurves[obj.name + ".z"].AddKey(key);

            // Qx
            key = new Keyframe(animTime, obj.transform.localRotation.x);
            legacyCurves[obj.name + ".Qx"].AddKey(key);

            // Qy
            key = new Keyframe(animTime, obj.transform.localRotation.y);
            legacyCurves[obj.name + ".Qy"].AddKey(key);

            // Qz
            key = new Keyframe(animTime, obj.transform.localRotation.z);
            legacyCurves[obj.name + ".Qz"].AddKey(key);

            // Qw
            key = new Keyframe(animTime, obj.transform.localRotation.w);
            legacyCurves[obj.name + ".Qw"].AddKey(key);
        }

        if(captureRootMotion)
        {
            Vector3 rootPosition = humanPose.bodyPosition - initialRootPos;
            AddRootPosKeyFrame("RootT.x", rootPosition.x);
            AddRootPosKeyFrame("RootT.y", rootPosition.y);
            AddRootPosKeyFrame("RootT.z", rootPosition.z);

            Quaternion rootRotation = humanPose.bodyRotation;
            AddRootPosKeyFrame("RootQ.x", rootRotation.x);
            AddRootPosKeyFrame("RootQ.y", rootRotation.y);
            AddRootPosKeyFrame("RootQ.z", rootRotation.z);
            AddRootPosKeyFrame("RootQ.w", rootRotation.w);
        }
    }


    // adds a key frame for the given root position coordinate
    private void AddRootPosKeyFrame(string property, float value)
    {
        Keyframe key = new Keyframe(animTime, value);
        rootPoseCurves[property].AddKey(key);
    }


    // creates animation clip out of the recorded animation curves
    private AnimationClip CreateAnimationClip()
    {
        AnimationClip animClip = new AnimationClip();

        foreach (KeyValuePair<int, AnimationCurve> data in muscleCurves)
        {
            animClip.SetCurve(string.Empty, typeof(Animator), HumanTrait.MuscleName[data.Key], data.Value);
        }

        if(captureRootMotion)
        {
            foreach (KeyValuePair<string, AnimationCurve> data in rootPoseCurves)
            {
                animClip.SetCurve(string.Empty, typeof(Animator), data.Key, data.Value);
            }

        }

        return animClip;
    }

    private AnimationClip CreateLegacyAnimClip()
    {
        AnimationClip animClip = new AnimationClip();
        animClip.legacy = true;

        // Mapping x, y, z, Qx, Qy, Qz, and Qw to parameters
        foreach (KeyValuePair<string, AnimationCurve> data in legacyCurves)
        {
            GameObject originalObj = null;
            if (GameObject.Find(data.Key.Substring(0, data.Key.Length - 3)))
            {
                originalObj = GameObject.Find(data.Key.Substring(0, data.Key.Length - 3));
            }
            else if (GameObject.Find(data.Key.Substring(0, data.Key.Length - 2)))
            {
                originalObj = GameObject.Find(data.Key.Substring(0, data.Key.Length - 2));
            }

            string property = "";
            if (data.Key.Substring(data.Key.Length - 2) == "Qw")
            {
                property = "localRotation.w";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qz")
            {
                property = "localRotation.z";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qy")
            {
                property = "localRotation.y";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qx")
            {
                property = "localRotation.x";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "z")
            {
                property = "localPosition.z";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "y")
            {
                property = "localPosition.y";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "x")
            {
                property = "localPosition.x";
            }
            
            animClip.SetCurve(avatarComponents[originalObj], typeof(Transform), property, data.Value);
        }

        animClip.wrapMode = WrapMode.Loop;

        return animClip;
    }

    // saves the animation clip to the specified save-file
    public void SaveAnimationClip(AnimationClip animClip)
    {
        // clear the animation curves
        muscleCurves.Clear();
        rootPoseCurves.Clear();
    }
}
