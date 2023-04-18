using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.SceneManagement;


//FileManager controls the File Browser on the landing page.
public class FileManager : MonoBehaviour
{
    public string dir;
    public GameObject recordingPrefab;

    void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Videos", ".mp4"));

    }

    //Called when Choose Video button is pressed
    public void OpenFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }


    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if(FileBrowser.Success)
        {
            //Grab the selected file path
            dir = FileBrowser.Result[0];

            //Create the recording prefab for the list controller
            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);

            //Place uploaded video in a folder with current datetime in /Data
            string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");
            string dataPath = Directory.GetCurrentDirectory() + "/Data/" + dateString.Remove(dateString.Length - 3);
            Directory.CreateDirectory(dataPath);
            string destinationPath = Path.Combine(dataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);


            if (MainManager.Instance != null)
            {
                //Set variables in main manager to reference in the rest of the application
                MainManager.Instance.SetDirPath(destinationPath);
                MainManager.Instance.setNewUpload(true);
            }

            //Add info to recording prefab and add to list
            rec.GetComponent<Recording>().text.text = FileBrowserHelpers.GetFilename(FileBrowser.Result[0]);
            rec.GetComponent<Recording>().fullDir = MainManager.Instance.dirPath;
            ListController.savedList.Insert(0, rec);

            //Load uploaded video into viewing page
            SceneManager.LoadScene("ViewingPage");
        }

    }
}
