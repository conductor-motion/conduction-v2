using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public string dirPath;

    public GameObject metronome;
    MetronomeStorage metronomeStorage;
    BeatChecker beatChecker;

    public bool metronomePlay = false;
    public float tempoBeat;

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
            string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");

            SetDirPath(dateString.Remove(dateString.Length - 3));
        }


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
}
