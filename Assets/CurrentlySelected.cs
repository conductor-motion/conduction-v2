using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentlySelected : MonoBehaviour
{
    public CharacterSelection charSelect;
    public GameObject text;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(charSelect.selectedCharacter +" =? " + PlayerPrefs.GetInt("selectedCharacter"));
        if (charSelect.selectedCharacter == PlayerPrefs.GetInt("selectedCharacter"))
        {
            text.SetActive(true);
        }
        else
        {
            text.SetActive(false);
        }
    }
}
