using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAvatar1 : MonoBehaviour
{
    public GameObject[] characters;
    // Start is called before the first frame update
    void Start()
    {
        characters[PlayerPrefs.GetInt("selectedCharacter")].SetActive(true);
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        GameObject prefab = characters[selectedCharacter];
        GameObject clone = Instantiate(prefab);
    }
}