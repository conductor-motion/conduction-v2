using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCap : MonoBehaviour
{
    // Attach to FPS Controller Object in each Scene
    void Start()
    {
        Application.targetFrameRate = 30;
    }

}
