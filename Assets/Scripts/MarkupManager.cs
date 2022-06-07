using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MarkupManager : MonoBehaviour
{
    [Tooltip("The container which has markup, used for easing export")]
    public GameObject markupContainer;

    // Markup control
    private bool doMarkup = false;
    private bool currentlyDrawing = false;
    private Texture2D texture;
    private Sprite blankSprite;
    private Color color = Color.white;
    private int size = 1;

    // Mouse positon
    private Vector3 mousePos = new Vector3(0f,0f,0f);

    public void ToggleMarkup()
    {
        doMarkup = !doMarkup;
    }

    // Allows for controlling line thickness
    // Since drawings consist of lines, this will only consider cardinal directions, not diagonals
    void DrawPixel(Vector3 pos, Color color, int size)
    {
        texture.SetPixel((int)pos.x, (int)pos.y, color);
        for (int i = 0; i < size; ++i)
        {
            texture.SetPixel((int)pos.x+i, (int)pos.y, color);
            texture.SetPixel((int)pos.x-i, (int)pos.y, color);
            texture.SetPixel((int)pos.x, (int)pos.y+i, color);
            texture.SetPixel((int)pos.x, (int)pos.y-i, color);
        }
    }

    // Without a bezier curve implementation, the beginning and end edges look rough/jagged
    // This draws a circular cap at each end to make it appear more rounded
    void DrawCap(Vector3 pos, Color color, int size)
    {
        // Size represents the radius of the circle we draw
        // Points = {(x,y) | (x - pos.x)^2 + (y-pos.y)^2 <= size^2}
        // We can generate a circle around x = y = 0, and translate these points by pos
        // The pixels are selected by choosing the pixels in a square of size*2 that would fit in Points

        List<int> indices = new List<int>();
        int sizeSquare = size * size;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // More efficient to do multiplication over exponentiation (without CPU-optimizations)
                double distSquared = x * x + y * y;

                if (distSquared < sizeSquare)
                {
                    // ok this is a very bad way of doing this for large sizes
                    indices.Add(x);
                    indices.Add(y);

                    indices.Add(-x);
                    indices.Add(-y);

                    indices.Add(-x);
                    indices.Add(y);

                    indices.Add(x);
                    indices.Add(-y);
                }
            }
        }

        // Iterate the list, drawing the circle
        for (int i = 0; i < indices.Count; i += 2)
        {
            texture.SetPixel(indices[i] + (int)pos.x, indices[i+1] + (int)pos.y, color);
        }
        texture.Apply();
    }

    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        blankSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        markupContainer.GetComponent<UnityEngine.UI.Image>().sprite = blankSprite;


        Color invisible = new Color(0f,0f,0f,1f);
        // Initialize the texture as being blank
        for (int i = 0; i < Screen.width; i++)
        {
            for (int k = 0; k < Screen.height; k++)
            {
                texture.SetPixel(i, k, invisible);
            }
        }

        // "Apply" our new texture to the object
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        // If markup state is toggled and the mouse is down, draw at the mouse
        if (doMarkup && Input.GetKeyDown(KeyCode.Mouse0) && !IsPointeroverUIElement())
        {
            mousePos = Input.mousePosition;
            currentlyDrawing = true;
            DrawCap(Input.mousePosition, color, size);
        }
        else if (doMarkup && Input.GetKeyUp(KeyCode.Mouse0) && !IsPointeroverUIElement())
        {
            DrawCap(Input.mousePosition, color, size);
            currentlyDrawing = false;
        }

        // Draw at cursor
        if (doMarkup && currentlyDrawing)
        {
            // Only cause a change and draw if the mouse has moved; not necessarily every frame
            if (Vector3.Distance(mousePos, Input.mousePosition) >= 1)
            {
                // The mouse is likely to be moved at a greater rate than the update function executes, so
                // interpolation between previous and current points should be used when drawing
                // To do this in a hacky and poor way for now (see bezier curves as a good implementation example)
                // can just calculate the slope between two points and use that when performing steps
                // "i < granularity" determines the granularity; 100 is likely excessive but does not seem too slow
                Vector3 interPos;
                int granularity = 100;
                for (int i = 0; i < granularity; i++)
                {
                    interPos = Vector3.Lerp(mousePos, Input.mousePosition, ((float)i)/granularity);
                    DrawPixel(interPos, color, size);
                }
                mousePos = Input.mousePosition;
                texture.Apply();
            }
        }
    }

    public void SelectColor(Color newColor)
    {
        color = newColor;
    }

    public void SelectSize(int newSize)
    {
        size = newSize;
    }

    // Fills the screen with black which, for some reason, is considered clear
    public void ClearScreen()
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int k = 0; k < texture.height; k++)
            {
                texture.SetPixel(i, k, Color.black);
            }
        }
        texture.Apply();
    }


    //These functions perform a raycast to check whether the mouse is over the UI so that the user can't draw while selecting UI elements
    private bool IsPointeroverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
    {
        for(int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
