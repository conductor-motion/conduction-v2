using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Recording : MonoBehaviour
{
    public string recordingName;
    public AnimationClip clip;
    public Text text;



    // Start is called before the first frame update
    void Start()
    {
        text.text = recordingName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void open()
    {
        MocapPlayerOurs.recordedClip = this.clip;
        SceneManager.LoadScene("ViewingPage");
    }
    
    public void delete()
    {
        //ListController.savedList.Remove(this);
        Destroy(this.gameObject);
    }
}
