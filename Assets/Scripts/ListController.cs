using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    
    //public GameObject recordingPrefab;
    public static List<GameObject> savedList = new List<GameObject>();
    public Transform recordingParent;
    public string saveFile;

    // Start is called before the first frame update
    void Start()
    {
        /*
        saveFile = Application.dataPath + "/data.json";
        if (File.Exists(saveFile))
        {
            string FileContents = File.ReadAllText(saveFile);
            savedList = JsonUtility.FromJson<List<GameObject>>(FileContents);
        }*/
        
        LoadRecordings();
        //string json = JsonUtility.ToJson(savedList);
        //File.WriteAllText(saveFile, json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadRecordings()
    {
        int len = savedList.Count;
        for(int i = len - 1; i >= 0; i--)
        {
            GameObject recordingObj = Instantiate(savedList[i], recordingParent) as GameObject;
            //recordingObj.GetComponent<Recording>().text.text = savedList[i].GetComponent<Recording>().text.text;
            //recordingObj.GetComponent<Recording>().clip = savedList[i].GetComponent<Recording>().clip;
            recordingObj.GetComponent<Recording>().listController = this;
            Debug.Log("Created List Item");
        }
    }
}
