using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to hold data needed for web cam menu select
public class WebCamData
{
    public string name;
    public bool selected;

    public WebCamData(string name, bool selected)
    {
        this.name = name;
        this.selected = selected;
    }
}
