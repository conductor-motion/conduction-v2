using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Frames
{
    public List<FrameData> frames = new List<FrameData>();


    public int length()
    {
        return frames.Count;
    }

}
