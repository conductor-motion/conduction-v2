using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject camera;
    GameObject robot;
    GameObject grid;
    float radius = 3f; // Distance of the camera from the robot
    float angle = 3f*Mathf.PI/2f; // Set based on the relative position of the camera to the robot at the start of the secene

    // Start is called before the first frame update
    void Start()
    {
        camera = this.gameObject;
        robot = GameObject.Find("RobotAnimated");
        grid = GameObject.Find("Axis Lines");
    }

    public void Rotate90()
    {
        angle += Mathf.PI / 2;
        // Prevents angle from getting excessively large
        if (angle == Mathf.PI * 2)
            angle = 0;

        // moves the camera in a circular motion
        camera.transform.position = new Vector3(Mathf.Cos(angle)*radius, 1, Mathf.Sin(angle)*radius+3);
        camera.transform.rotation = Quaternion.Euler(0, -90, 0) * camera.transform.rotation;

        // perfoms the same movement on the axis lines so they stay in the same location
        grid.transform.position = new Vector3(Mathf.Cos(angle + Mathf.PI) * radius, 1, Mathf.Sin(angle + Mathf.PI) * radius + 3);
        grid.transform.rotation = Quaternion.Euler(0, -90, 0) * grid.transform.rotation;
    }
}
