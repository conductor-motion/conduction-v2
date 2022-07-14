using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    public static List<GameObject> savedList = new List<GameObject>();
    public GameObject recordingPrefab;
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
        for(int i = 0; i < len; i++)
        {
            Instantiate(recordingPrefab, recordingParent);
        }
    }
}
