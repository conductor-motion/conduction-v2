using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleHotkey : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _toggle.GetComponent<Toggle>().isOn = !_toggle.GetComponent<Toggle>().isOn;
        }
    }
}
