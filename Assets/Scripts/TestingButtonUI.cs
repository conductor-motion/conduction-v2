
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingButtonUI : MonoBehaviour
{
    [SerializeField] private string testingUOS = "Comparison of Recording Charts";

    public void analyticsButton()
    {
        SceneManager.LoadScene(testingUOS);
    }
}
