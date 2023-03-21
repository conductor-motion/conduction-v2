using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAvatar1 : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject animator;
    // Start is called before the first frame update
    void Start()
    {
        characters[PlayerPrefs.GetInt("selectedCharacter")].SetActive(true);  
    }

    // Update is called once per frame
    void Update()
    {

    }
}