using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingButtonGraph : MonoBehaviour
{
    [SerializeField] private string chart = "TestingScene";

    public void XthGraphButton() {
    SceneManager.LoadScene(chart);
   }
}
