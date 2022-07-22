using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Recording : MonoBehaviour
{
    //public string recordingName;
    public AnimationClip clip;
    public Text text;
    public ListController listController;

    public void open()
    {
        MocapPlayerOurs.recordedClip = this.clip;
        SceneManager.LoadScene("ViewingPage");
    }
    
    public void delete()
    {
        // Attempt to delete the associated .anim file for this recording
        File.Delete(Path.Combine(Application.streamingAssetsPath, text.text + ".anim"));

        ListController.savedList.Remove(ListController.savedList.Find(item => item.GetComponent<Recording>().clip.name == clip.name));
        Destroy(this.gameObject);
    }
}
