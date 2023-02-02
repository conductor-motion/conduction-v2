using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public string dirPath;
    public float playbackFPS;

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

            SetDirPath(Application.dataPath + "/Conduction/Data/" + dateString.Remove(dateString.Length - 3));
        }
    }

    public void SetDirPath(string dirPath)
    {
        MainManager.Instance.dirPath = dirPath;
    }

    public void SetPlaybackFPS(float playbackFPS)
    {
        MainManager.Instance.playbackFPS = playbackFPS;
    }
}
