using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
