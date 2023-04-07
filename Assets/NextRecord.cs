using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextRecord : MonoBehaviour
{
public void Continue()
	{
		SceneManager.LoadScene("RecordingPage");
	}
}
