using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ReplayController : MonoBehaviour
{
    //public OAKForUnity.OAKDevice device;

    void Start()
    {
        if (MainManager.Instance != null)
        {
            string dir = MainManager.Instance.dirPath;
            /*device.pathToReplay = dir;

            string fullPath = Application.dataPath + device.path + dir;
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("No recording found.");
            }

            device.replayNumFrames = MainManager.Instance.numFrames;
            device.replayFPS = MainManager.Instance.playbackFPS;*/
            
        }
    }

}
