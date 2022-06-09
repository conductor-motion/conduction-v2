using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierManager
{
    // Points to consider when drawing a Bezier line
    private List<Vector3> points;

    // Set whenever about to draw, tells how many curves exist in the points
    private int curveCount;

    // Constructor that initializes the points list
    public BezierManager()
    {
        points = new List<Vector3>();
    }

    // Allows for clearing the points held, so that rendering can be done quickly in runtime
    public void ClearPoints()
    {
        points.Clear();
    }

    // Clearly add a specified point to the point List
    public void AddPoint(Vector3 point)
    {
        points.Add(point);
    }

    // Add points to the Bezier curve path based on mouse coordinates passed
    // Scale refers to a "wideness" of the curve
    public void BezierInterpolate(List<Vector3> mousePoints, float scale)
    {
        ClearPoints();

        // Define variables to be used in the loop to prevent repeated definitions
        Vector3 p0, p1, p2; // "input"
        Vector3 tangent;    // calculate tangent of points, used in Bezier formula
        Vector3 q0, q1;     // "output"

        // Rare case in which only one mouse coordinate is passed, which cannot be mapped to a curve
        if (mousePoints.Count == 1)
        {
            return;
        }

        // Iterate all points and perform the Bezier "function" on pairs of points given
        for (int i = 0; i < mousePoints.Count; i++)
        {
            if (i == 0)
            {
                // In the first case, we only have the point "to the right" to look at
                p0 = mousePoints[i];
                p1 = mousePoints[i + 1];

                tangent = (p1 - p0);
                q0 = p0 + scale * tangent;

                AddPoint(p0);
                AddPoint(q0);
            }
            else if (i == mousePoints.Count - 1)
            {
                // In the last case, we only have the point "to the left" to look at
                p0 = mousePoints[i - 1];
                p1 = mousePoints[i];

                tangent = (p1 - p0);
                q0 = p1 - scale * tangent;

                AddPoint(q0);
                AddPoint(p1);
            }
            else
            {
                // In general cases, we have points to the left and right to look at
                // The above cases account for when Count == 2 as well
                p0 = mousePoints[i - 1];
                p1 = mousePoints[i];
                p2 = mousePoints[i + 1];

                tangent = (p2 - p0).normalized;
                q0 = p1 - scale * tangent * (p1 - p0).magnitude;
                q1 = p1 + scale * tangent * (p2 - p1).magnitude;

                AddPoint(q0);
                AddPoint(p1);
                AddPoint(q1);
            }
        }

        curveCount = (points.Count - 1) / 3;
    }

    // Using the points obtained from interpolation, find points on the curve suitable for drawing purposes
    public List<Vector3> GetDrawingPoints()
    {
        List<Vector3> drawingPoints = new List<Vector3>();
        List<Vector3> bezierDrawingPoints;

        for (int curveIndex = 0; curveIndex < curveCount; curveIndex++)
        {
            bezierDrawingPoints = FindDrawingPoints(curveIndex);

            if (curveIndex != 0)
            {
                bezierDrawingPoints.RemoveAt(0);
            }  

            drawingPoints.AddRange(bezierDrawingPoints);
        }

        return drawingPoints;
    }

    // Calculate a point on the path based on the curve and interpolation formula
    // This is where the "magic" of a Bezier curve being drawn occurs
    // This is a recursive implementation that utilizes overloads
    private List<Vector3> FindDrawingPoints(int curveIndex)
    {
        List <Vector3> pointList = new List<Vector3>();

        Vector3 left = CalculateBezierPoint(curveIndex, 0);
        Vector3 right = CalculateBezierPoint(curveIndex, 1);

        pointList.Add(left);
        pointList.Add(right);

        // Actual recursive call
        FindDrawingPoints(curveIndex, 0, 1, pointList, 1);

        return pointList;
    }

    // Returns how many points are added by the call for debugging purposes
    private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector3> pointList, int insertionIndex)
    {
        Vector3 left = CalculateBezierPoint(curveIndex, t0);
        Vector3 right = CalculateBezierPoint(curveIndex, t1);

        // Distance is so small that there's no point in adding a point
        if ((left - right).sqrMagnitude < 0.01f)
        {
            return 0;
        }

        float tMid = (t0 + t1) / 2;
        Vector3 mid = CalculateBezierPoint(curveIndex, tMid);

        Vector3 leftDir = (left - mid).normalized;
        Vector3 rightDir = (right - mid).normalized;

        // This is some esoteric dark knowledge that I don't quite fully understand the conditions for
        if (Vector3.Dot(leftDir, rightDir) > -0.99f || Mathf.Abs(tMid - 0.5f) < 0.0001f)
        {
            int pointsAddedCount = 0;

            pointsAddedCount += FindDrawingPoints(curveIndex, t0, tMid, pointList, insertionIndex);
            pointList.Insert(insertionIndex + pointsAddedCount, mid);
            pointsAddedCount++;
            pointsAddedCount += FindDrawingPoints(curveIndex, tMid, t1, pointList, insertionIndex + pointsAddedCount);

            return pointsAddedCount;
        }

        return 0;
    }

    // Calculate the Bezier curve points based on the current interpolation time (t) value
    public Vector3 CalculateBezierPoint(int curveIndex, float t)
    {
        int pointIndex = curveIndex * 3;

        Vector3 p0, p1, p2, p3;
        p0 = points[pointIndex];
        p1 = points[pointIndex + 1];
        p2 = points[pointIndex + 2];
        p3 = points[pointIndex + 3];

        return CalculateBezierPoint(t, p0, p1, p2, p3);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t; // "unfinished"
        float u_squared = u * u;
        float u_cubed   = u_squared * u;
        float t_squared = t * t;
        float t_cubed   = t_squared * t;

        // Initialize the calculated point and transform it with Bezier transforms
        Vector3 p = u_cubed * p0;
        p += 3 * u_squared * t * p1;
        p += 3 * u * t_squared * p2;
        p += t_cubed * p3;

        return p;
    }
}
