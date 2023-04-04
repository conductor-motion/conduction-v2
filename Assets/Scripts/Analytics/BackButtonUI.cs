using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonUI : MonoBehaviour
{
    [SerializeField] private string backToDash = "AnalyticsDash";

    public void backButton() {
        SceneManager.LoadScene(backToDash);
    }
}
