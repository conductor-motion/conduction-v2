using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HidableHud: MonoBehaviour
{
    bool visibleHud = true;
    [SerializeField] private GameObject _hud;

    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HideHud();
        }
    }

    public void HideHud()
    {
        visibleHud = !visibleHud;
        _hud.SetActive(visibleHud);
    }
}
