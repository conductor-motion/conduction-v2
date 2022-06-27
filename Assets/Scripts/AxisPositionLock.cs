using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisPositionLock : MonoBehaviour
{

  

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<DragModulePerspective>().enabled = !GetComponent<DragModulePerspective>().enabled;
        }
    }
}
