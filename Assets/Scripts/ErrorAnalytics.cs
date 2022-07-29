using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorAnalytics : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject axisLines;
    public GameObject leftHand;
    public GameObject rightHand;

    public Text LHX, LHY, RHX, RHY;

    private float leftXError, leftYError, rightXError, rightYError;

    void Start()
    {
        axisLines = GameObject.Find("Axis Lines");
    }

    // Update is called once per frame
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
