using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//MainManager class holds all data that needs to be referenced across the app
public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    //Path where video is/will be stored
    public string dirPath;

    //Name of selected webcam
    public string webCamName = "";

    //Information about metronome used for tempo tracking
    public GameObject metronome;
    MetronomeStorage metronomeStorage;
    BeatChecker beatChecker;
    public bool metronomePlay = false;
    public float tempoBeat;

    //Mode the application is currently in
    public string mode = "Recording";

    //Whether or not the current video is a new upload
    public bool newUpload = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (dirPath == "" || dirPath == null)
        {
            //Set path to current datetime string, minus the seconds
            string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");
            SetDirPath(dateString.Remove(dateString.Length - 3));
        }

        //Set the metronome information, if it exists
        metronome = GameObject.Find("Metronome");
        if(metronome) {
            metronomeStorage = GetComponent<MetronomeStorage>();
            tempoBeat = metronomeStorage.GetTempo();
            setTempo(tempoBeat);

            beatChecker = GetComponent<BeatChecker>();
            metronomePlay = beatChecker.play;
            setMetronomePlay(metronomePlay);
        } 
        
    }

    public void setMetronomePlay(bool metronomePlay) 
    {
        MainManager.Instance.metronomePlay = metronomePlay;
    }

    public void setTempo(float tempoBeat) 
    {
        MainManager.Instance.tempoBeat = tempoBeat;
    }

    public void SetDirPath(string dirPath)
    {
        MainManager.Instance.dirPath = dirPath;
    }

    //Should be reset with every new recording
    public void resetDirPath()
    {
        string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");
        SetDirPath(dateString.Remove(dateString.Length - 3));
    }


    public void SetMode(string mode)
    {
        MainManager.Instance.mode = mode;
    }

    public void setNewUpload(bool newUpload)
    {
        MainManager.Instance.newUpload = newUpload;
    }

    public void setWebCamName(string webCamName)
    {
        MainManager.Instance.webCamName = webCamName;
    }
}
