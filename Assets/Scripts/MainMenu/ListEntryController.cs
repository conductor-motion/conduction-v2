using UnityEngine;
using UnityEngine.UIElements;

//Script to conrtol webcam entries listed in webcam menu
public class ListEntryController
{
    Label NameLabel;
    VisualElement check;

    //Find UIDocument elements
    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("webcamName");
        check = visualElement.Q<VisualElement>("check");
    }

    //Set the data; called from WebcamSelectController
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