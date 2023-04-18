using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

// Recording storage data structure
public class Recording : MonoBehaviour
{
    public string fullDir; 
    public Text text;
    public ListController listController;

    // When open button clicked, open the selected video
    public void open()
    {
        // Shift this item to the front of the saved list so that it is the most recently accessed
        GameObject newest = ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == text.text);
        ListController.savedList.Remove(newest);
        ListController.savedList.Insert(0, newest);

        //Add information to main manager
        MainManager.Instance.SetDirPath(fullDir);
        MainManager.Instance.setNewUpload(false);

        //Load selected video into viewing page
        SceneManager.LoadScene("ViewingPage");
    }
    
    // When the delete button is pressed, remove all references to the selected animation
    public void delete()
    {   
        File.Delete(fullDir);
        File.Delete(fullDir.Substring(0, fullDir.LastIndexOf("/")) + "/data.json");
        Directory.Delete(fullDir.Substring(0, fullDir.LastIndexOf("/")));

        ListController.savedList.Remove(ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == text.text));
        Destroy(this.gameObject);
    }

}
