using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public string dirPath;
    public string mode = "Recording";

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
    }

    public void SetDirPath(string dirPath)
    {
        MainManager.Instance.dirPath = dirPath;
    }

    public void SetMode(string mode)
    {
        MainManager.Instance.mode = mode;
    }
}
