using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Recording : MonoBehaviour
{
    //public string recordingName;
    public AnimationClip clip = null;
    public Text text;
    public ListController listController;

    public void open()
    {
        // Load on demand, rather than when the list is loaded
        if (!clip)
        {
            AnimationClip newClip = AnimationLoader.LoadExistingClip(ListController.shareAvatar, text.text);
            newClip.name = text.text;

            // Have to update the list, as it contains the main copy
            ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == text.text).GetComponent<Recording>().clip = newClip;

            this.clip = newClip;
        }

        MocapPlayerOurs.recordedClip = this.clip;
        MocapPlayerOurs.existingRecording = true;
        SceneManager.LoadScene("ViewingPage");
    }
    
    public void delete()
    {
        // Attempt to delete the associated .anim file for this recording
        File.Delete(Path.Combine(Application.streamingAssetsPath, text.text + ".anim"));
        try
        {
            File.Delete(Path.Combine(Application.streamingAssetsPath, text.text + ".audio"));
        }
        catch
        {
            Debug.Log("No associated audio for this animation to delete.");
        }

        ListController.savedList.Remove(ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == text.text));
        Destroy(this.gameObject);
    }
}
