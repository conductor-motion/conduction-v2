using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the camera zoom and angles on the Viewing screen so the user can look around the avatar
public class CameraController : MonoBehaviour
{
    GameObject camera;
    GameObject robot;
    GameObject grid;
    float radius = 2; // Distance of the camera from the robot
    float angle = 3f*Mathf.PI/2f; // Set based on the relative position of the camera to the robot at the start of the secene
    float cameraHeight = 0;
    float zoomSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        camera = this.gameObject;
        robot = GameObject.Find("RobotAnimated");
        grid = GameObject.Find("Axis Lines");
    }

    // Moves the camera closer to the robot
    public void ZoomIn()
    {
        //radius = 1.25f;
        //cameraHeight = 1.5f;

        // Updates the camera's position
        // camera.transform.position = new Vector3(Mathf.Cos(angle) * radius, cameraHeight, Mathf.Sin(angle) * radius + 3);
        //camera.transform.position += new Vector3(0, 0, zoomSpeed);
        radius -= zoomSpeed;
        camera.transform.position += (zoomSpeed * camera.transform.forward); 
    }

    // Move the camera farther from the robot
    public void ZoomOut()
    {
        //radius = 3f;
        //cameraHeight = 1f;

        // Updates the camera's position
        //camera.transform.position = new Vector3(Mathf.Cos(angle) * radius, cameraHeight, Mathf.Sin(angle) * radius + 3);
        // camera.transform.position -= new Vector3(0, 0, zoomSpeed);
        radius += zoomSpeed;
        camera.transform.position += (-zoomSpeed * camera.transform.forward); 
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
        camera.transform.position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius );
        camera.transform.rotation = Quaternion.Euler(0, -45, 0) * camera.transform.rotation;

        // perfoms the same movement on the axis lines so they stay in the same location
        grid.transform.position = new Vector3(Mathf.Cos(angle + Mathf.PI) * radius, 0, Mathf.Sin(angle + Mathf.PI) * radius );
        grid.transform.rotation = Quaternion.Euler(0, -45, 0) * grid.transform.rotation;
    }

    public void TurnRight45()
    {
        // Updates the angle to be 45 degrees smaller
        angle -= Mathf.PI / 4;

        // Prevents angle from getting excessively large
        if (angle == 0)
            angle = Mathf.PI * 2;

        // moves the camera in a circular motion
        // Mathf.Cos and Sin use radians, Quaternion.Euler uses degrees
        camera.transform.position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius );
        camera.transform.rotation = Quaternion.Euler(0, 45, 0) * camera.transform.rotation;

        // perfoms the same movement on the axis lines so they stay in the same location
        grid.transform.position = new Vector3(Mathf.Cos(angle + Mathf.PI) * radius, 0, Mathf.Sin(angle + Mathf.PI) * radius );
        grid.transform.rotation = Quaternion.Euler(0, 45, 0) * grid.transform.rotation;
    }

    public void RotateCustom(float degrees)
    {
        // Updates the angle, Mathf.Deg2Rad is used to modified from degrees to radians
        angle += Mathf.Deg2Rad * degrees;

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
