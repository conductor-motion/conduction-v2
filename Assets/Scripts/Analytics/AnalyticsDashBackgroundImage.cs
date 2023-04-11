using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
