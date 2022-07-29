using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    
    //public GameObject recordingPrefab;
    public GameObject avatarPrefab;
    public GameObject recordingPrefab;
    public static List<GameObject> savedList = new List<GameObject>();
    public Transform recordingParent;
    public string saveFile;

    // Control whether or not we search the file system for loaded animations
    private static bool hasLoaded = false;

    // Sharable avatar for loading recordings
    public static GameObject shareAvatar;

    // Start is called before the first frame update
    void Start()
    {
        shareAvatar = avatarPrefab;
        LoadFromFileSystem();
        LoadRecordings();
    }

    // Loads a list of the animation files in the StreamableAssets folder and populates the recording list
    private void LoadFromFileSystem()
    {
        if (hasLoaded)
            return;

        string[] files = Directory.GetFiles(Application.streamingAssetsPath);
        List<string> animationFiles = new List<string>();

        // Determine which files are animations
        foreach (string file in files)
        {
            if (file.Substring(file.Length - 5) == ".anim") animationFiles.Add(file);
        }

        // For each animation file, parse it and create a Recording
        foreach (string file in animationFiles)
        {
            string relName = file.Substring(Application.streamingAssetsPath.Length + 1);
            relName = relName.Substring(0, relName.Length - 5);

            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);

            rec.GetComponent<Recording>().text.text = relName;

            savedList.Add(rec);
        }

        hasLoaded = true;
    }

    private void LoadRecordings()
    {
        int len = savedList.Count;
        for(int i = len - 1; i >= 0; i--)
        {
            GameObject recordingObj = Instantiate(savedList[i], recordingParent) as GameObject;
            recordingObj.GetComponent<Recording>().listController = this;
            Debug.Log("Created List Item");
        }
    }
}
