using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
        // Rotates the camera and grid 45 degrees counter clockwise
        public void TurnLeft45()
        {
            // Updates the angle to be 45 degrees larger
            angle += Mathf.PI / 4;
            
            // Prevents angle from getting excessively large
            if (angle == Mathf.PI * 2)
                angle = 0;
    
            // moves the camera in a circular motion
            // Mathf.Cos and Sin use radians, Quaternion.Euler uses degrees
            GetComponent<Camera>().transform.position = new Vector3(Mathf.Cos(angle) * radius, cameraHeight, Mathf.Sin(angle) * radius + 3);
            GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, -45, 0) * GetComponent<Camera>().transform.rotation;
    
            // perfoms the same movement on the axis lines so they stay in the same location
            grid.transform.position = new Vector3(Mathf.Cos(angle + Mathf.PI) * radius, cameraHeight, Mathf.Sin(angle + Mathf.PI) * radius + 3);
            grid.transform.rotation = Quaternion.Euler(0, -45, 0) * grid.transform.rotation;
        }
    
        public void TurnRight45()
        {
            this.transform.position = new Vector3(Mathf.Cos(angle) * radius, cameraHeight, Mathf.Sin(angle) * radius + 3);
            GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 45, 0) * GetComponent<Camera>().transform.rotation;
    
            // perfoms the same movement on the axis lines so they stay in the same location
            grid.transform.position = new Vector3(Mathf.Cos(angle + Mathf.PI) * radius, cameraHeight, Mathf.Sin(angle + Mathf.PI) * radius + 3);
            grid.transform.rotation = Quaternion.Euler(0, 45, 0) * grid.transform.rotation;
        }
}
