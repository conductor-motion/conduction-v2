using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayButton : MonoBehaviour // IPointerUpHandler
{
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _play, _stop;
    [SerializeField] private Button _button;
    [SerializeField] private Stopwatch _stopwatch;
    [SerializeField] private Transform[] countdown;

    private bool activeRecording = false;

    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _button.onClick.Invoke();
        }
    }

    public void ToggleRecording()
    {
        activeRecording = !activeRecording;
        if (activeRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    /*public IEnumerable TriggerCountdown()
    {
        Debug.Log("3");
        WaitForSeconds(1);
    }*/
    public void StartRecording()
    {
        StartCoroutine(CountdownAndStartRecording());
        Debug.Log("Recording!");

        _img.sprite = _stop;
    }

    public void StopRecording()
    {
        Debug.Log("Stopped!");
        _stopwatch.ToggleStopwatch();
        _img.sprite = _play;
    }


    private IEnumerator CountdownAndStartRecording()
    {
        if (countdown != null && countdown.Length > 0)
        {
            for (int i = 0; i < countdown.Length; i++)
            {
                if (countdown[i])
                    countdown[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(1f);

                if (countdown[i])
                    countdown[i].gameObject.SetActive(false);
            }
        }
        _stopwatch.ToggleStopwatch();
    }
}
