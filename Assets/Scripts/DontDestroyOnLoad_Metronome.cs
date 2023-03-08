using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad_Metronome : MonoBehaviour
{
    public GameObject Metronome;

    void Awake()
    {
        
        gameObject.transform.parent = null;

        DontDestroyOnLoad(gameObject);
       
    }

    
}
