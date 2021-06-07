using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _debugGUI = default;

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.f1Key.wasPressedThisFrame)
        {
            _debugGUI?.SetActive(!_debugGUI.activeSelf);
        }
        // HACK
        if(Keyboard.current.escapeKey.isPressed)
        {
            Application.Quit();
        }
    }
}
