using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    // Start is called before the first frame update
    public void DoExitGame()
    {
        Debug.Log("This works I promise");
        Application.Quit();
    }
}
