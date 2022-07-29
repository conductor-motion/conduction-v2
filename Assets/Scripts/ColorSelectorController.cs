using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Used for drawing the color selection wheel without the use of an image that may struggle to scale
public class ColorSelectorController : MonoBehaviour
{
    private MarkupManager markupManager;

    [System.NonSerialized]
    public Color currentColor = Color.white;

    // Draw a color wheel programmatically
    private Color invisible = new Color(0f,0f,0f,0f);
    private Texture2D texture;
    private Sprite blankSprite;
    private int radius = 100; // Best if half of size of containing element

    // Used for the current color selection indicator
    private GameObject colorSelectorIndicator;

    // Keep track of the mouse
    private bool isMouseDown = false;
    private Vector3 mousePos = new Vector3(0f,0f,0f);
    Vector3 rootPos;

    RaycastResult currentRaycast;

    // Initialize the color selection wheel and variables used to create it
    void Start()
    {
        colorSelectorIndicator = GameObject.Find("ColorSelectorIndicator");

        radius = (int)(transform.GetComponent<RectTransform>().sizeDelta.x / 2);

        // Create the blank texture used to render the wheel
        texture = new Texture2D(radius * 2, radius * 2);
        blankSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        this.GetComponent<UnityEngine.UI.Image>().sprite = blankSprite;

        // Initialize the texture as entirely blank
        for (int i = 0; i < radius * 2; i++)
        {
            for (int k = 0; k < radius * 2; k++)
            {
                texture.SetPixel(i, k, invisible);
            }
        }

        markupManager = GameObject.FindGameObjectWithTag("MarkupManager").GetComponent<MarkupManager>();

        this.GetComponent<Image>().SetNativeSize();
        rootPos = transform.position;

        // Gets the original position of the wheel such that it is usable even if the window is scaled
        rootPos.x += GetComponent<RectTransform>().sizeDelta.x/2 * GetComponent<RectTransform>().lossyScale.x;
        rootPos.y += GetComponent<RectTransform>().sizeDelta.y/2 * GetComponent<RectTransform>().lossyScale.y;

        colorSelectorIndicator.transform.position = transform.position;

        // Draw the color selection wheel
        drawCircle();
    }

    void Update()
    {
        // If the mouse is down over the color picker, select the color we hover over, if applicable
        if (Input.GetKeyDown(KeyCode.Mouse0) && IsPointeroverUIElement())
        {
            // We only want to keep track of the mouse if we starting holding it down over the color selector
            isMouseDown = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isMouseDown = false;
        }

        if (isMouseDown)
        {
            // Get local coordinates inside element, and get pixel color at that loation
            if (Input.mousePosition != mousePos && IsPointeroverUIElement())
            {
                if (Vector3.Distance(Input.mousePosition, rootPos) < radius)
                {
                    // Calculate local position and select the color from that pixel
                    Vector3 localPos = new Vector3(0,0,0);

                    localPos = (Input.mousePosition - rootPos) + new Vector3(radius, radius, 0);

                    currentColor = texture.GetPixel((int)localPos.x, (int)localPos.y);
                    markupManager.SelectColor(currentColor);

                    // Place the indicator at this location
                    colorSelectorIndicator.transform.position = Input.mousePosition;
                }

                mousePos = Input.mousePosition;
            }
        }
    }

    // Easier to conceptualize RGB as HSV, so allow conversion
    // hue [0,360]; saturation [0,1]
    private Color hsv2rgb(float hue, float saturation, float value)
    {
        float chroma = value * saturation;
        float hue1 = hue / 60f;
        float x = chroma * (1f - Mathf.Abs((hue1 % 2) - 1f));

        float r1 = 0;
        float g1 = 0; 
        float b1 = 0;

        if (hue1 >= 0 && hue1 <= 1)
        {
            r1 = chroma;
            g1 = x;
            b1 = 0;
        }
        else if (hue1 >= 1 && hue1 <= 2)
        {
            r1 = x;
            g1 = chroma;
            b1 = 0;
        }
        else if (hue1 >= 2 && hue1 <= 3)
        {
            r1 = 0;
            g1 = chroma;
            b1 = x;
        }
        else if (hue1 >= 3 && hue1 <= 4)
        {
            r1 = 0;
            g1 = x;
            b1 = chroma;
        }
        else if (hue1 >= 4 && hue1 <= 5)
        {
            r1 = x;
            g1 = 0;
            b1 = chroma;
        }
        else if (hue1 >= 5 && hue1 <= 6)
        {
            r1 = chroma;
            g1 = 0;
            b1 = x;
        }
        
        float m = value - chroma;

        return new Color(r1+m, g1+m, b1+m);
    }

    private (float, float) xy2polar(int x, int y)
    {
        float r = Mathf.Sqrt(x*x + y*y);
        float theta = Mathf.Atan2(y, x);
        return (r, theta);
    }

    // Draw the color picker
    // Conceptually, as we go around a circle (angle changes), the hue changes
    // As we go further from the center, saturation increases
    // Value is relatively unimportant for this implementation, and reflects brightness
    private void drawCircle()
    {
        int offset = radius;

        float r, theta;
        float deg;

        Color newRgb;

        for (int x = radius; x > -radius; x--)
        {
            for (int y = radius; y > -radius; y--)
            {
                (r, theta) = xy2polar(x, y);

                if (r > radius)
                {
                    continue;
                }

                deg = theta * Mathf.Rad2Deg;

                if (deg < 0)
                {
                    deg = deg + 360;
                }

                // hue, saturation, value
                newRgb = hsv2rgb(deg, r / radius, 1);

                texture.SetPixel(x + offset, y + offset, newRgb);
            }
        }

        texture.Apply();
    }

    private bool IsPointeroverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
    {
        for(int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.name == this.name)
            {
                currentRaycast = curRaycastResult;
                return true;
            }
        }
        return false;
    }

    // Used to check raycast on click and get current position over
    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
