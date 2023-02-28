using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour
{
    public static TooltipScript instance;
    //public bool test = true;
    public Text tooltipText;
    public RectTransform tooltipBackground;

    [SerializeField]
    public Camera UICamera;

    public void Awake() {
        instance = this;
        tooltipBackground = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.Find("text").GetComponent<Text>();
    }

    // Update is called once per frame
    public void Update()
    {
        Vector2 localPoint;
        //We need UI position 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, UICamera, out localPoint);
        transform.localPosition = localPoint;
    }

    public void displayTooltip(string tooltipString/*, Vector2 anchoredPosition*/) {
        gameObject.SetActive(true);
        //gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 background_size = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        tooltipBackground.sizeDelta = background_size;

        gameObject.transform.SetAsLastSibling();
    }

    public void hideTooltip() {
        //hides game object
        gameObject.SetActive(false);
    }

    //static funcs to easily use this class from other classes

    public static void static_displayTooltip(string tooltipString/*, Vector2 anchoredPosition*/) {
        instance.displayTooltip(tooltipString/*, anchoredPosition*/); 
    }

    public static void static_hideTooltip() {
        instance.hideTooltip();
    }
}
