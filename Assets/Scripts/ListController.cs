using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    
    //public GameObject recordingPrefab;
    public static List<GameObject> savedList = new List<GameObject>();
    public Transform recordingParent;

    // Start is called before the first frame update
    void Start()
    {
        LoadRecordings();
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
