using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{

    // Rotates the character 45 degrees counter clockwise
    public void TurnLeft45()
    {
        this.transform.rotation = Quaternion.Euler(0, 45, 0) * this.transform.rotation;
    }

    //Rotates the character 45 degrees clockwise
    public void TurnRight45()
    {
        this.transform.rotation = Quaternion.Euler(0, -45, 0) * this.transform.rotation;
    }
}
