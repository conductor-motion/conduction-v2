using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;


// Our implementation of the Kinect Mocap Recorder from the Kinect v2 Examples library
// Differs greatly in that it uses legacy animations and performs audio recording operations
public class MocapRecorderOurs : MonoBehaviour
{/*
    [Tooltip("The avatar, whose motion will be captured in the animation clip.")]
    public IKController avatarModel;

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
    [HideInInspector]
    public static bool isRecording = false;

    private bool isCountingDown = false;
    private float animTime = 0;

    // human pose parameters
    private HumanPose humanPose = new HumanPose();
    private HumanPoseHandler humanPoseHandler = null;

    // animation curves to hold the recorded animation 
    private Dictionary<string, AnimationCurve> rootPoseCurves = new Dictionary<string, AnimationCurve>();

    // initial model's root position
    private Vector3 initialRootPos = Vector3.zero;

    // Control the recording button
    private bool recordButtonPressed = false;
    private bool editorOverride = false;
    private Sprite notRec;
    private Sprite isRec;
    public static AnimationClip recordedClip;

    // Elements to hide when recording
    private GameObject MetronomeUI;
    private GameObject AxisInstructionsUI;

    // Legacy animation construction data structures
    private Dictionary<GameObject, string> avatarComponents = new Dictionary<GameObject, string>();
    public static Dictionary<string, AnimationCurve> legacyCurves = new Dictionary<string, AnimationCurve>();
    public static AnimationClip legacyAnimClip;

    // Audio recording
    private bool canRecordAudio = false;
    private string chosenMic = "";
    private AudioSource audioSource;
    // "Audio vectors" storage so this can be serialized to a file and loaded with the animation when needed
    List<float> audioRecording = new List<float>();
    public static float[] finalRecording;
    public static AudioClip recordedAudio;

    void Start()
    {
        // Allow for bypassing the need for a sensor if in the Unity Editor
        if (UnityEngine.Application.isEditor)
        {
            editorOverride = true;
        }
        editorOverride = true;

        legacyCurves.Clear();

        audioSource = GetComponent<AudioSource>();
        recordedAudio = null;

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

        // Check if we can record audio in this session
        string firstAcceptableMic = "";
        foreach (string device in Microphone.devices)
        {
            // Unity does not support Microphone arrays even in the year of our lord 2022
            if (device.Contains("Microphone Array")) continue;

            // Keep the first Microphone we can theoretically use, just in case
            if (firstAcceptableMic.Length == 0) firstAcceptableMic = device;

            // Always use the Kinect microphone if possible
            if (device.Contains("Xbox NUI Sensor")) chosenMic = device;
        }

        // If we found any microhpone that works, we can record audio
        if (firstAcceptableMic.Length > 0)
        {
            canRecordAudio = true;

            if (chosenMic.Length == 0) chosenMic = firstAcceptableMic;
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
                if(!isCountingDown && avatarModel)
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

        if (isRecording && avatarModel)
        {
            print("recording");
            // record the current pose
            animTime += Time.deltaTime;
            RecordAvatarPose();

            if (infoText & ((int)(animTime * 10f) % 10) == 0)
            {
                infoText.text = string.Format("{0}:{1}", Math.Floor(animTime / 60), (animTime % 60).ToString("00"));
            }
        }

        // stop recording, if the user is lost
        /*if (avatarModel && (avatarModel.playerId == 0 && !editorOverride))
        {
            StopRecording();
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

        // Begin recording audio
        /*if (canRecordAudio)
        {
            audioSource.clip = Microphone.Start(chosenMic, true, 60, 44100);
            audioSource.Play();
            Invoke("ResizeRecording", 60);
        }
        
        // Begin recording motion
        isCountingDown = false;
        isRecording = true;
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


    // clears the animation curves before the recording starts
    private void InitAnimationCurves()
    {
        if (avatarModel == null)
            return;

        animTime = 0f;

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

            // Halt recording audio and record its last sub-second audio to the vector list
            /*if (canRecordAudio)
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

            // Realistically is impossible for nothing to be recorded when a countdown is included
            bool isAnythingRecorded = true;

            if (isAnythingRecorded)
            {
                legacyAnimClip = CreateLegacyAnimClip();

                Debug.Log("New Clip Created");
                // MocapPlayerOurs.recordedClip = recordedClip;
                MocapPlayerOurs.recordedClip = legacyAnimClip;
                MocapPlayerOurs.existingRecording = false;
                SceneManager.LoadScene("ViewingPage-orig");
            }
            else
            {
                ShowMessage("Recording stopped - nothing to save.");
            }
            // recInfo.gameObject.SetActive(true);
            StartCoroutine(SwapIcon());
        }
    }


    // records the current avatar pose to the animation curves
    private void RecordAvatarPose()
    {
        humanPoseHandler.GetHumanPose(ref humanPose);

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
    */
}