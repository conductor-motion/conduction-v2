using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttontester : MonoBehaviour
{
    [SerializeField] private string testing = "AnalyticsDash";

    public void Buttontester() {
    SceneManager.LoadScene(testing);
   }
}
