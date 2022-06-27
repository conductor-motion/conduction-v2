using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisPreserve : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Axis");

        if(objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
   
  
}
