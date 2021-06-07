using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DbgFlight : MonoBehaviour
{
    [SerializeField]
    private FixedWing _player = default;
    [SerializeField]
    private TMP_Text _speedText = default;
    [SerializeField]
    private TMP_Text _liftModText = default;
    [SerializeField]
    private Slider _pitchSlider = default;
    [SerializeField]
    private Slider _rollSlider = default;
    [SerializeField]
    private Slider _yawSlider = default;
    [SerializeField]
    private TMP_Text _throttleText = default;

    private void Start()
    {
        _pitchSlider.minValue = -1f;
        _pitchSlider.maxValue = 1f;
        _rollSlider.minValue = -1f;
        _rollSlider.maxValue = 1f;
        _yawSlider.minValue = -1f;
        _yawSlider.maxValue = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        _speedText.text = $"Flight Speed: {_player.Speed:N2}";
        _liftModText.text = $"Lift Mod: {_player.LiftMod:N2}";
        _throttleText.text = $"Throttle: {_player.Throttle:N2}";
        _pitchSlider.value = _player.GetPitch();
        _rollSlider.value = _player.GetRoll();
        _yawSlider.value = _player.GetYaw();
    }
}