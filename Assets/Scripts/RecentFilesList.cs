using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecentFilesList : MonoBehaviour
{
    // Variables for sizing and positioning of files on the list
    [Tooltip("Maximum files we can populate and show at once")]
    public int maxFiles = 4; // Maximum files this script will allow to populate the list
    private int currentFiles = 0;
    private float offset = 150.5f;

    // Container we insert into
    [Tooltip("The container UI element where inserted elements will exist")]
    public GameObject container;

    // Container for instantiating a clone
    private GameObject original = null;
    private GameObject clone = null;

    // Remove the example person so real files can be added
    void Start()
    {
        original = GameObject.Find("ExamplePerson");
        original.SetActive(false);

        // Example like in Figma
        if (UnityEngine.Application.isEditor)
        {
            AddFileToList("Jordan McMillan", "02/26/2022");
            AddFileToList("Michael Sed", "02/20/2022");
            AddFileToList("Connor Cabrera", "01/06/2022");
            AddFileToList("Vijay Stroup", "12/25/2021");
        }
    }

    // Given a file, instantiate a new version from the example and populate the fields
    // TODO: make the button component do anything so clicking on these is possible
    public bool AddFileToList(string name, string date)
    {
        if (currentFiles < maxFiles)
        {
            currentFiles += 1;

            clone = Instantiate(original, container.transform);

            // Adjust the positioning of the new list element
            clone.transform.localPosition = new Vector3(0, offset, 0);

            // Adjust the content of the children of this element for the values passed
            // Abuse MonoBehaviour behavior so we don't have to do any weird stuff to find children
            // (this is extremely weird stuff but Unity hierarchies defeat me)
            TMP_Text TMP_Text_Name = (TMP_Text)clone.GetComponentInChildren(typeof(TMP_Text), false);
            TMP_Text_Name.text = name;
            TMP_Text_Name.gameObject.SetActive(false);
            TMP_Text TMP_Text_Date = (TMP_Text)clone.GetComponentInChildren(typeof(TMP_Text), false);
            TMP_Text_Date.text = date;
            TMP_Text_Name.gameObject.SetActive(true);

            // Make the new element visible after having made all changes
            clone.SetActive(true);

            // Adjust the offset for future list additions
            offset -= 100;

            return true;
        }

        // Our list cannot support any more files
        return false;
    }
}
