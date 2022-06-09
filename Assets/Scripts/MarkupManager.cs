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

    // Constant variables
    private Color invisible = new Color(0f,0f,0f,1f);

    // Mouse positon
    private Vector3 mousePos = new Vector3(0f,0f,0f);

    // Curve settings
    private List<Vector3> bezierPoints;
    private List<Vector3> drawingPoints;
    private int threshold = 30;
    private BezierManager bezier;

    // Circle drawing
    // Is inefficient in not using Vector3, but does not currently seem to matter
    List<int> indices = new List<int>();

    // When called, enables or disable markup drawing on the screen
    public void ToggleMarkup()
    {
        doMarkup = !doMarkup;
    }

    void Start()
    {
        bezier = new BezierManager();
        bezierPoints = new List<Vector3>();

        texture = new Texture2D(Screen.width, Screen.height);
        blankSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        markupContainer.GetComponent<UnityEngine.UI.Image>().sprite = blankSprite;

        CalculateCircleCoords(size);

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

    // Predefine a circle every time the size changes - reducing runtime calculations
    void CalculateCircleCoords(int size)
    {
        // Size represents the radius of the circle we draw
        // Points = {(x,y) | (x - pos.x)^2 + (y-pos.y)^2 <= size^2}
        // We can generate a circle around x = y = 0, and translate these points by pos
        // The pixels are selected by choosing the pixels in a square of size*2 that would fit in Points

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
    }

    // Without a bezier curve implementation, the beginning and end edges look rough/jagged
    // This draws a circular cap at each end to make it appear more rounded
    void DrawCap(Vector3 pos, Color color, int size)
    {
        // Iterate the list, drawing the circle
        for (int i = 0; i < indices.Count; i += 2)
        {
            texture.SetPixel(indices[i] + (int)pos.x, indices[i+1] + (int)pos.y, color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If markup state is toggled and the mouse is down, enable drawing at the mouse
        if (doMarkup && Input.GetKeyDown(KeyCode.Mouse0) && !IsPointeroverUIElement())
        {
            mousePos = Input.mousePosition;
            currentlyDrawing = true;
        }
        else if (doMarkup && Input.GetKeyUp(KeyCode.Mouse0) && !IsPointeroverUIElement())
        {
            currentlyDrawing = false;
            bezierPoints.Clear();
        }

        // Draw at cursor if enabled
        if (doMarkup && currentlyDrawing)
        {
            // Only cause a change and draw if the mouse has moved; not necessarily every frame
            if (Vector3.Distance(mousePos, Input.mousePosition) >= 1)
            {
                // The mouse is likely to be moved at a greater rate than the update function executes, so
                // interpolation between previous and current points should be used when drawing
                // If the distance between the last mousePos and the current is too far, then add some extra point by interpolation
                Vector3 intermediate = mousePos;
                float granularity = 0.05f;
                while (Vector3.Distance(intermediate, Input.mousePosition) > 1)
                {
                    intermediate = Vector3.Lerp(mousePos, Input.mousePosition, granularity);
                    granularity += 0.05f;
                    bezierPoints.Add(intermediate);
                }

                mousePos = Input.mousePosition;
                bezierPoints.Add(mousePos);

                if (bezierPoints.Count > threshold)
                {
                    bezier.BezierInterpolate(bezierPoints, size);
                    bezierPoints.Clear();
                    drawingPoints = bezier.GetDrawingPoints();

                    for (int i = 0; i < drawingPoints.Count; i++)
                    {
                        DrawCap(drawingPoints[i], color, size);
                        if (i != drawingPoints.Count - 1 && Vector3.Distance(drawingPoints[i], drawingPoints[i + 1]) > size)
                        {
                            // Add a midpoint as a heuristic
                            DrawCap(Vector3.Lerp(drawingPoints[i], drawingPoints[i + 1], 0.5f), color, size);
                        }
                    }
                    texture.Apply();

                    // Remember last position so smoothness maintained between segments
                    bezierPoints.Add(mousePos);
                }
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
        CalculateCircleCoords(size);
    }

    // Fills the screen with pixels of alpha = 1, which will be clear for any UI texture used
    public void ClearScreen()
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int k = 0; k < texture.height; k++)
            {
                texture.SetPixel(i, k, invisible);
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
