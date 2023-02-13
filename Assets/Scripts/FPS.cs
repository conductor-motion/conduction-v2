using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{

    public int MaxFrames = 60;  //maximum frames to average over

    private static float lastFPSCalculated = 0f;
    private List<float> frameTimes = new List<float>();

    // Use this for initialization
    void Start()
    {
        lastFPSCalculated = 0f;
        frameTimes.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        addFrame();
        lastFPSCalculated = calculateFPS();
    }

    private void addFrame()
    {
        frameTimes.Add(Time.unscaledDeltaTime);
        if (frameTimes.Count > MaxFrames)
        {
            frameTimes.RemoveAt(0);
        }
    }

    private float calculateFPS()
    {
        float newFPS = 0f;

        float totalTimeOfAllFrames = 0f;
        foreach (float frame in frameTimes)
        {
            totalTimeOfAllFrames += frame;
        }
        newFPS = ((float)(frameTimes.Count)) / totalTimeOfAllFrames;

        return newFPS;
    }

    public static float GetCurrentFPS()
    {
        return lastFPSCalculated;
    }
}