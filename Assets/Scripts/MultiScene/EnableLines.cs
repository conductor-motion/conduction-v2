using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableLines : MonoBehaviour
{
    // Forces green skeleton lines to stay on.

    private void Update()
    {
        PoseVisuallizer3D.showLines = true;
    }
}
