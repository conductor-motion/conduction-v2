using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Our list or "inventory" system of managing the list of Recordings currently loaded by the application
public class ListController : MonoBehaviour
{
    public GameObject recordingPrefab;
    public static List<GameObject> savedList = new List<GameObject>();
    public Transform recordingParent;
    public string saveFile;

    // Control whether or not we search the file system for loaded animations
    private static bool hasLoaded = false;


    // Populate the list and display it to the user
    void Start()
    {
        MainManager.Instance.resetDirPath();
        LoadFromFileSystem();
        LoadRecordings();
    }

    // Loads a list of the video files in the Data folder and populates the recording list
    private void LoadFromFileSystem()
    {
        if (hasLoaded)
            return;

        // Get the files from the directory and sort them by time accessed
        DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "/Data/");
        FileInfo[] files = dirInfo.GetFiles("*.mp4", SearchOption.AllDirectories);

        // Sort the FileInfo array
        Array.Sort(files, delegate (FileInfo file1, FileInfo file2)
        {
            return file2.LastAccessTime.CompareTo(file1.LastAccessTime);
        });

        foreach (FileInfo file in files)
        {
            string name = file.Name.Substring(0, file.Name.Length - 4);

            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);
            rec.GetComponent<Recording>().text.text = name;
            rec.GetComponent<Recording>().fullDir = file.FullName;

            savedList.Add(rec);
        }

        hasLoaded = true;
    }

    // Using the list of Recordings, generates elements to show them to the user
    private void LoadRecordings()
    {
        int len = savedList.Count;
        for (int i = 0; i < len; i++)
        {
            GameObject recordingObj = Instantiate(savedList[i], recordingParent) as GameObject;
            recordingObj.GetComponent<Recording>().listController = this;
        }
    }
}