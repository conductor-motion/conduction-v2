using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cap the program's FPS such that the video recorder FPS does not mismatch the actual program FPS
public class FPSCap : MonoBehaviour
{
    // Attach to FPS Controller Object in each Scene
    void Start()
    {
        Application.targetFrameRate = 30;
    }

}
