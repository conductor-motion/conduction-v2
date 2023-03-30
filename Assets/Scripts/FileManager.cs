using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.SceneManagement;
using Google.Protobuf;

public class FileManager : MonoBehaviour
{
    public string dir;
    public GameObject recordingPrefab;

    void Start()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Videos", ".mp4"));

    }


    public void OpenFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }


    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders);

        if(FileBrowser.Success)
        {;
            dir = FileBrowser.Result[0];


            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);
            rec.GetComponent<Recording>().text.text = MainManager.Instance.dirPath.Substring(MainManager.Instance.dirPath.LastIndexOf("/") + 1);
            rec.GetComponent<Recording>().fullDir = MainManager.Instance.dirPath;
            ListController.savedList.Insert(0, rec);

            string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");
            string dataPath = Directory.GetCurrentDirectory() + "/Data/" + dateString.Remove(dateString.Length - 3);
            Directory.CreateDirectory(dataPath);
            string destinationPath = Path.Combine(dataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            print(destinationPath);
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);



            if (MainManager.Instance != null)
            {
                MainManager.Instance.SetDirPath(destinationPath);
                MainManager.Instance.setNewUpload(true);
            }

            SceneManager.LoadScene("ViewingPage");
        }

    }
}
