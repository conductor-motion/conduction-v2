using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backButtonToViewingPage : MonoBehaviour
{
    [SerializeField] private string backToViewingPage = "ViewingPage";

    public void backButton() {
        SceneManager.LoadScene(backToViewingPage);
    }
}
