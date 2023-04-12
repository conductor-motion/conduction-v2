using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script makes sure the axis lines aren't visible in build mode
public class AnalyticsDashBackgroundImage : MonoBehaviour
{
    SpriteRenderer axisComponent;
    GameObject axis;
     void Awake()
    {
        axis = GameObject.FindWithTag("Axis");
        axisComponent = axis.GetComponent<SpriteRenderer>();
        axisComponent.enabled = false;
    }

     void OnDestroy()
    {
        axisComponent.enabled = true;
    }
}
