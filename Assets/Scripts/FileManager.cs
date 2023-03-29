using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using SimpleFileBrowser;
using System.IO;
using UnityEngine.SceneManagement;

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
        {
            Debug.Log(FileBrowser.Result[0]);
            dir = FileBrowser.Result[0];


            if (MainManager.Instance != null)
            {
                MainManager.Instance.SetDirPath(dir);
            }

            GameObject rec = Instantiate(recordingPrefab);
            DontDestroyOnLoad(rec);
            rec.GetComponent<Recording>().text.text = MainManager.Instance.dirPath.Substring(MainManager.Instance.dirPath.LastIndexOf("/") + 1);
            rec.GetComponent<Recording>().fullDir = MainManager.Instance.dirPath;
            ListController.savedList.Insert(0, rec);

            SceneManager.LoadScene("ViewingPage");
        }

    }
}
