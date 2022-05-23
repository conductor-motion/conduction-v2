using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SaveFileWindow : EditorWindow
{
    // Start is called before the first frame update
    
    public string openSaveWindow()
    {
        return EditorUtility.OpenFilePanel("", "", "anim");
    }

}
