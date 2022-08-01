using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Reports the distance from the hands to the axis lines set by the user
public class ErrorAnalytics : MonoBehaviour
{
    // The parts used for collecting analytics
    public GameObject analyticsContainer;
    public GameObject axisLines;
    public GameObject leftHand;
    public GameObject rightHand;

    // Texts where analytics are reported
    public bool isShown = true;
    public Text LHX, LHY, RHX, RHY;

    // The analytics
    private float leftXError, leftYError, rightXError, rightYError;

    void Start()
    {
        axisLines = GameObject.Find("Axis Lines");

        Debug.Log("Test Case 1");
        Debug.Log("LX = 3, LY = 5, RX = -2, RY = 4, AX = 0, AY = -1");
        Debug.Log("Expected: 3, 6, 2, 5");
        Debug.Log("Results: " + getLXVal(3f, 0f).ToString() + ", " + getLYVal(5f, -1f).ToString() + ", " + getRXVal(-2f, 0f).ToString() + ", " + getRYVal(4f, -1f).ToString());

        Debug.Log("Test Case 2");
        Debug.Log("LX = 3.33, LY = 5.55, RX = -2.22, RY = 4.44, AX = 0, AY = -1.11");
        Debug.Log("Expected: 3.33, 6.66, 2.22, 5.55");
        Debug.Log("Results: " + getLXVal(3.33f, 0f).ToString() + ", " + getLYVal(5.55f, -1.11f).ToString() + ", " + getRXVal(-2.22f, 0f).ToString() + ", " + getRYVal(4.44f, -1.11f).ToString());

        Debug.Log("Test Case 3");
        Debug.Log("LX = 2.37204, LY = -1.02671, RX = -1.81094, RY = 5.00623, AX = 3.40078, AY = 4.13643");
        Debug.Log("Expected: 1.02874, 5.16314, 5.21172, 0.8698");
        Debug.Log("Results: " + getLXVal(2.37204f, 3.40078f).ToString("n5") + ", " + getLYVal(-1.02671f, 4.13643f).ToString("n5") + ", " + getRXVal(-1.81094f, 3.40078f).ToString("n5") + ", " + getRYVal(5.00623f, 4.13643f).ToString("n5"));
    }

    // Toggle the display of the analytics pane
    public void ToggleAnalytics()
    {
        isShown = !isShown;

        if (isShown)
            analyticsContainer.SetActive(true);
        else
            analyticsContainer.SetActive(false);
    }

    // Update is called once per frame
    // Finds the distance of the hands from the axis lines that have been set
    void Update()
    {
        if (!isShown) return;

        leftXError = getLXVal(leftHand.transform.position.x, axisLines.transform.position.x);
        leftYError = getLYVal(leftHand.transform.position.y, axisLines.transform.position.y);
        rightXError = getRXVal(rightHand.transform.position.x, axisLines.transform.position.x);
        rightYError = getRYVal(rightHand.transform.position.y, axisLines.transform.position.y);

        LHX.text = leftXError.ToString();
        LHY.text = leftYError.ToString();
        RHX.text = rightXError.ToString();
        RHY.text = rightYError.ToString();
    }

    public float getLXVal(float lx, float ax)
    {
        return Mathf.Abs(lx - ax);
    }

    public float getLYVal(float ly, float ay)
    {
        return Mathf.Abs(ly - ay);
    }

    public float getRXVal(float rx, float ax)
    {
        return Mathf.Abs(rx - ax);
    }

    public float getRYVal(float ry, float ay)
    {
        return Mathf.Abs(ry - ay);
    }
}
