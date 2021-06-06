using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadController : MonoBehaviour
{
    FixedWing _fw;

    // Start is called before the first frame update
    void Start()
    {
        _fw = GetComponent<FixedWing>();
        Debug.Log(Gamepad.all);
    }

    // Update is called once per frame
    void Update()
    {
        Gamepad gp = Gamepad.current;
        _fw.Roll(gp.leftStick.ReadValue().x);
        _fw.Pitch(gp.leftStick.ReadValue().y);
        _fw.Yaw(gp.rightStick.ReadValue().x);
    }
}
