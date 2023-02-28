using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrameData
{
    public int frameNum;
    public List<HandMovementData> data = new List<HandMovementData>();

    public FrameData(int frameNum)
    {
        this.frameNum = frameNum;
    }
    public FrameData(int frameNum, List<HandMovementData> data)
    {
        this.frameNum = frameNum;
        this.data = data;
    }
}
