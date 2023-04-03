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

    //public int flag = 1;

    public GameObject WebCamInput;
    VideoPlayer webCamInput;

    public bool isLooping = false;
    public bool isPlaying = false;
    public double time;
    public double length;
    public ulong frameCount;
    public long frame;

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

        WebCamInput = GameObject.Find("WebCamInput");
        
        if(WebCamInput) {
          webCamInput = WebCamInput.GetComponent<VideoPlayer>();

           isLooping = webCamInput.isLooping;
           setIsLooping(isLooping);
           Debug.Log("webCamInput.isLooping " + webCamInput.isLooping);
           isPlaying = webCamInput.isPlaying;
           setIsPlaying(isPlaying);

           time = webCamInput.time;
           setTime(time);

           frameCount = webCamInput.frameCount;
           setFrameCount(frameCount);

           frame = webCamInput.frame;
           setFrame(frame);

           length = webCamInput.length;
           setLength(length);
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

        /*if(metronome??null) {
            flag = 0;
            setFlag(flag);
        }*/

       
        
    }

    public void setMetronomePlay(bool metronomePlay) 
    {
        MainManager.Instance.metronomePlay = metronomePlay;
    }

    public void setTempo(float tempoBeat) 
    {
        MainManager.Instance.tempoBeat = tempoBeat;
    }

   /* public void setFlag(int flag) 
    {
        MainManager.Instance.flag = flag;
    }
*/
    public void setIsLooping(bool isLooping) 
    {
        MainManager.Instance.isLooping = isLooping;
    }

    public void setIsPlaying(bool isPlaying) 
    {
        MainManager.Instance.isPlaying = isPlaying;
    }

    public void setTime(double time) 
    {
        MainManager.Instance.time = time;
    }

    public void setFrameCount(ulong frameCount) 
    {
        MainManager.Instance.frameCount = frameCount;
    }

    public void setFrame(long frame) 
    {
        MainManager.Instance.frame = frame;
    }

    public void setLength(double length) 
    {
        MainManager.Instance.length = length;
    }

    public void SetDirPath(string dirPath)
    {
        MainManager.Instance.dirPath = dirPath;
    }
}
