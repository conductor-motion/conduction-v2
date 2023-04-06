using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WebcamSelectController : MonoBehaviour
{
    // UXML template for list entries
    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView webcamList;
    Button backButton;
    VisualElement root;

    List<WebCamData> AllWebcams = new List<WebCamData>();
    bool firstTime = true;


    public void InitializeWebcamList()
    {
        AllWebcams.Clear();
        EnumerateAllWebcams();

        UIDocument doc = GetComponent<UIDocument>();
        // Store a reference to the template for the list entries
        //ListEntryTemplate = listElementTemplate;

        // Store a reference to the webcam list element
        root = doc.rootVisualElement;
        webcamList = root.Q<ListView>("webcamList");
        backButton = root.Q<Button>("backButton");
        backButton.clicked += BackButtonClicked;

        FillWebcamList();

        // Register to get a callback when an item is selected
        webcamList.onSelectionChange += OnWebcamSelected;
    }

    void EnumerateAllWebcams()
    {
        WebCamDevice[] deviceList = new WebCamDevice[10];
        deviceList = WebCamTexture.devices;
        for(int i = 0; i < deviceList.Length; i++)
        {
            WebCamData data = new WebCamData(deviceList[i].name, false);
            if ((MainManager.Instance.webCamName == "" && i == 0) || data.name == MainManager.Instance.webCamName)
            {
                data.selected = true; //default webcam
            }
            AllWebcams.Add(data);
        }
    }

    void FillWebcamList()
    {
        // Set up a make item function for a list entry
        webcamList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new ListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        webcamList.bindItem = (item, index) =>
        {
            (item.userData as ListEntryController).SetWebcamData(AllWebcams[index]);
        };

        // Set a fixed item height
        webcamList.fixedItemHeight = 45;

        // Set the actual item's source list/array
        webcamList.itemsSource = AllWebcams;
    }


    void OnWebcamSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        WebCamData selectedWebcam = webcamList.selectedItem as WebCamData;
        selectedWebcam.selected = true;
        foreach(WebCamData webcam in AllWebcams)
        {
            if (webcam.name != selectedWebcam.name) //deselect all other webcams
                webcam.selected = false;
        }
        webcamList.Rebuild();


        MainManager.Instance.setWebCamName(selectedWebcam.name);
    }

    void BackButtonClicked()
    {
        GetComponent<UIDocument>().enabled = false;
    }

    public void OpenMenu()
    {
        GetComponent<UIDocument>().enabled = true;
        InitializeWebcamList();
    }
}