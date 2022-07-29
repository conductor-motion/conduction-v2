using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Reports the distance from the hands to the axis lines set by the user
public class ErrorAnalytics : MonoBehaviour
{
    // The parts used for collecting analytics
    public GameObject axisLines;
    public GameObject leftHand;
    public GameObject rightHand;

    // Texts where analytics are reported
    public Text LHX, LHY, RHX, RHY;

    // The analytics
    private float leftXError, leftYError, rightXError, rightYError;

    void Start()
    {
        axisLines = GameObject.Find("Axis Lines");
    }

    // Update is called once per frame
    // Finds the distance of the hands from the axis lines that have been set
    void Update()
    {
        leftXError = Mathf.Abs(leftHand.transform.position.x - axisLines.transform.position.x);
        leftYError = Mathf.Abs(leftHand.transform.position.y - axisLines.transform.position.y);
        rightXError = Mathf.Abs(rightHand.transform.position.x - axisLines.transform.position.x);
        rightYError = Mathf.Abs(rightHand.transform.position.y - axisLines.transform.position.y);

        LHX.text = leftXError.ToString();
        LHY.text = leftYError.ToString();
        RHX.text = rightXError.ToString();
        RHY.text = rightYError.ToString();
    }
}
