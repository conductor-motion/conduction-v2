using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisController : MonoBehaviour
{
    SpriteRenderer rendition;

    // Called once before start
    // Ensures there's only one axis
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Axis");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rendition = this.gameObject.GetComponent<SpriteRenderer>();
        GetComponent<DragModulePerspective>().enabled = false; // Axis lines start locked
    }

    // Update is called once per frame
    void Update()
    {
        // Axis Movement Lock
        if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponent<DragModulePerspective>().enabled = !GetComponent<DragModulePerspective>().enabled;
        }

        // Hide/Show Axis
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rendition.enabled = !rendition.enabled;
        }
    }
}
