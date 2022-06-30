using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    GameObject robot;

    // Start is called before the first frame update
    void Start()
    {
        robot = this.gameObject;
    }

    public void Rotate90()
    {
        Debug.Log(Quaternion.Euler(0, 90, 0) * robot.transform.rotation);
        robot.transform.rotation = Quaternion.Euler(0, 90, 0) * robot.transform.rotation;
    }
}
