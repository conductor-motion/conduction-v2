using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsDashSecondButtonUI : MonoBehaviour
{
    [SerializeField] private string chart = "TempoTracker";

   public void secondGraphButton() {
    SceneManager.LoadScene(chart);
   }
}
