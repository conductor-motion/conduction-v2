using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Our list or "inventory" system of managing the list of Recordings currently loaded by the application
public class ListController : MonoBehaviour
{
    public GameObject avatarPrefab;
    public GameObject recordingPrefab;
    public static List<GameObject> savedList = new List<GameObject>();
    public Transform recordingParent;
    public string saveFile;

    // Control whether or not we search the file system for loaded animations
    private static bool hasLoaded = false;

    // Sharable avatar for loading recordings
    public static GameObject shareAvatar;

    // Populate the list and display it to the user
    void Start()
    {
        // Allow other scripts to access the avatar used for loading recordings
        shareAvatar = avatarPrefab;
        LoadFromFileSystem();
        LoadRecordings();
    }

    // Loads a list of the animation files in the StreamableAssets folder and populates the recording list
    private void LoadFromFileSystem()
    {
        if (hasLoaded)
            return;

        // Get the files from the directory and sort them by time accessed
        DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] files = dirInfo.GetFiles();

        // Sort the FileInfo array
        Array.Sort(files, delegate(FileInfo file1, FileInfo file2)
        {
            return file2.LastAccessTime.CompareTo(file1.LastAccessTime);
        });

        List<string> animationFiles = new List<string>();

        // Determine which files are animations
        foreach (FileInfo file in files)
        {
            if (file.Name.Substring(file.Name.Length - 5) == ".anim") animationFiles.Add(file.Name);
        }

        // For each animation file, parse it and create a Recording
        foreach (string file in animationFiles)
        {
            string relName = file.Substring(0, file.Length - 5);

            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);

            rec.GetComponent<Recording>().text.text = relName;

            savedList.Add(rec);
        }

        hasLoaded = true;
    }

    // Using the list of Recordings, generates elements to show them to the user
    private void LoadRecordings()
    {
        int len = savedList.Count;
        for(int i = 0; i < len; i++)
        {
            GameObject recordingObj = Instantiate(savedList[i], recordingParent) as GameObject;
            recordingObj.GetComponent<Recording>().listController = this;
        }
    }
}
