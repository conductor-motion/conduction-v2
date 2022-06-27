using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragModulePerspective : MonoBehaviour
{
    public LayerMask draggableMask;
    GameObject selectedObject;
    private bool isDragging;

   
    void Start()
    {
        isDragging = false;
    }



    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, draggableMask);
            if(hit.collider != null)
            {
                //dosomething
                selectedObject = hit.collider.gameObject;
                isDragging = true;  
            }
        }
        if (isDragging)
        {
            Vector3 position = mousePosition();
            selectedObject.transform.position = position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    Vector3 mousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (float)3.30));
    }
}
