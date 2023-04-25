using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Controls the behavior of the controllable axis lines
public class AxisController : MonoBehaviour
{
    SpriteRenderer rendition;

    // Called once before start
    // Ensures there's only one axis
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Axis");

        if (objs.Length > 1 && SceneManager.GetActiveScene().name == "RecordingPage")
        {
            Destroy(objs[0]);
        }

        if (objs.Length > 1 && SceneManager.GetActiveScene().name != "RecordingPage")
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
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Axis");
        if (SceneManager.GetActiveScene().name == "CharacterSelection")
        {
            Debug.Log("This is happening");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }
    }

    public void AxisLock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("LockToggled");
            GetComponent<DragModulePerspective>().enabled = !GetComponent<DragModulePerspective>().enabled;
        }
    }

    public void AxisHide(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rendition.enabled = !rendition.enabled;
        }
    }

}
