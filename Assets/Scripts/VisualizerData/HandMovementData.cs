using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandMovementData
{
    public int landmarkIndex;
    public float xVal;
    public float yVal;
    public float zVal;
    public float visibility;


    public HandMovementData(int index, float xVal, float yVal, float zVal, float visibility)
    {
        this.landmarkIndex = index;
        this.xVal = xVal;
        this.yVal = yVal;
        this.zVal = zVal;
        this.visibility = visibility;
    }
}

