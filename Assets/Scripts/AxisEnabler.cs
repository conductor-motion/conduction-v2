using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisEnabler : MonoBehaviour
{
    SpriteRenderer rendition;

    void Start()
    {
        rendition = this.gameObject.GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rendition.enabled = !rendition.enabled;
        }
    }
}
