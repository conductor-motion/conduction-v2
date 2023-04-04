using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnalyticsButtonUI : MonoBehaviour
{
   [SerializeField] private string analyticsDash = "AnalyticsDash";

   public void analyticsButton() {
    SceneManager.LoadScene(analyticsDash);
   }
}
