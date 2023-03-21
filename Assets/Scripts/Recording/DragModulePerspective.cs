using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tool for dragging the axis lines by having them follow the mouse
public class DragModulePerspective : MonoBehaviour
{
    public LayerMask draggableMask;
    GameObject selectedObject;
    private bool isDragging;

    // Initially should not be able to drag the lines without enabling them
    void Start()
    {
        isDragging = false;
    }

    // If LMB is down, then have the axis lines follow it
    void Update()
    {
        // LMB down
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, draggableMask);
            if(hit.collider != null)
            {
                // If the mouse was over the axis lines, then we drag
                selectedObject = hit.collider.gameObject;
                isDragging = true;  
            }
        }

        // LMB is currently down and was pressed over the axis lines
        if (isDragging)
        {
            Vector3 position = mousePosition();
            selectedObject.transform.position = position;
        }

        // LMB up
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // Get the "world space" of the mouse by using the camera
    // The 4.01 at the end is the z-axis, which cannot be reflected in this use-case with ScreenToWorldPoint
    // 4.01 is behind the avatar used
    Vector3 mousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (float)4.01));
    }
}
