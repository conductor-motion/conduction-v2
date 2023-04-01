using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

// Recording storage data structure
// Does not include audio - that is loaded on-demand to avoid large memory usage
public class Recording : MonoBehaviour
{
    public string fullDir; 
    public Text text;
    public ListController listController;

    // When open button clicked, open the selected animation
    public void open()
    {
        // Shift this item to the front of the saved list so that it is the most recently accessed
        GameObject newest = ListController.savedList.Find(item => item.GetComponent<Recording>().text.text == text.text);
        ListController.savedList.Remove(newest);
        ListController.savedList.Insert(0, newest);
        MainManager.Instance.SetDirPath(fullDir);
        MainManager.Instance.setNewUpload(false);
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
