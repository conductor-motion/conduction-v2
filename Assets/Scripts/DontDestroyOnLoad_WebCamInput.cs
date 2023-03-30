using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad_WebCamInput : MonoBehaviour
{
    public GameObject WebCamInput;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
