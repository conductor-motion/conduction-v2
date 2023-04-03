using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsDashFirstButtonUI : MonoBehaviour
{
    [SerializeField] private string chart = "UseOfSpace";

    public void firstGraphButton() {
    SceneManager.LoadScene(chart);
   }
}
