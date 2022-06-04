using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string scene;
    public bool promptBeforeGo = false;

    public void LoadScene()
    {
        SceneManager.LoadScene(this.scene);
    }
}
