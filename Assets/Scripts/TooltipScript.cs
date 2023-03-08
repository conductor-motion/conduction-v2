using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour
{
    public static TooltipScript instance;
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, UICamera, out localPoint);
        transform.localPosition = localPoint;
    }

    public void displayTooltip(string tooltipString) {
        gameObject.SetActive(true);
        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 background_size = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        tooltipBackground.sizeDelta = background_size;

        gameObject.transform.SetAsLastSibling();
    }

    public void hideTooltip() {
        gameObject.SetActive(false);
    }

    public static void static_displayTooltip(string tooltipString) {
        instance.displayTooltip(tooltipString); 
    }

    public static void static_hideTooltip() {
        instance.hideTooltip();
    }
}
