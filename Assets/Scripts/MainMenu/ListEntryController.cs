using UnityEngine;
using UnityEngine.UIElements;

public class ListEntryController
{
    Label NameLabel;
    VisualElement check;


    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("webcamName");
        check = visualElement.Q<VisualElement>("check");
    }


    public void SetWebcamData(WebCamData data)
    {
        NameLabel.text = data.name;
        if(data.selected)
        {
            check.style.display = DisplayStyle.Flex;
        } else
        {
            check.style.display = DisplayStyle.None;
        }
    }

}