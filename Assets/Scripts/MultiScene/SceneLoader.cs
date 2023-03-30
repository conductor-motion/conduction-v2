using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Scene loading controller for showing a prompt to the user if specified to do so
public class SceneLoader : MonoBehaviour
{
    public string scene;
    public bool promptBeforeGo = false;
    public GameObject prompt;

    // Open a dialog to go to the desired scene or just go there
    public void LoadScene()
    {
        /*if (promptBeforeGo)
        {
            // Display the prompt
            prompt.SetActive(true);
            return;
        }*/

        // If no prompt is to be shown, just load the target scene
        ForceLoad();
    }

    // Hide the prompt
    public void HidePrompt()
    {
        prompt.SetActive(false);
    }

    // Actually load the target scene
    public void ForceLoad()
    {
        SceneManager.LoadScene(this.scene);
    }
}
