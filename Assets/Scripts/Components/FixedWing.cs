using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HealthBar))]
public class FixedWing : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Minimun speed for lift to counteract gravity")]
    private float _minFlightSpeed = 10f;
    [SerializeField]
    [Tooltip("The speed below which there is no lift")]
    private float _zeroFlightSpeed = 5f;
    [SerializeField]
    [Tooltip("The percentage of health at which the aircraft starts losing lift")]
    private float _hullDamageThreshold = 0.5f;
    [SerializeField]
    private float _pitchSpeed = 1f;
    [SerializeField]
    private float _rollSpeed = 1f;
    [SerializeField]
    private float _yawSpeed = 1f;
    #endregion

    #region Private Members
    private Rigidbody _rb = null;
    private HealthBar _hb = null;

    private float _pitchAxis = 0f;
    private float _yawAxis = 0f;
    private float _rollAxis = 0f;
    #endregion

    #region Public Methods
    // TODO: These should be part of an interface
    public void Pitch(float axis)
    {
        _pitchAxis = axis;
    }

    public void Yaw(float axis)
    {
        _yawAxis = axis;
    }

    public void Roll(float axis)
    {
        _rollAxis = axis;
    }
    #endregion

    #region Unity Functions
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _hb = GetComponent<HealthBar>();
    }

    private void FixedUpdate()
    {
        float speedSq = _rb.velocity.sqrMagnitude;
        float speed = _rb.velocity.magnitude;
        float minSpeedSq = _minFlightSpeed * _minFlightSpeed;
        float zeroSpeedSq = _zeroFlightSpeed * _zeroFlightSpeed;
        float liftMod = 1f;
        float velDif = Mathf.Max(Vector3.Dot(_rb.velocity.normalized, transform.forward), 0f);
        
        _rb.AddForce(Physics.gravity, ForceMode.Acceleration);

        // Check to make sure it's going fast enough
        if(speedSq < zeroSpeedSq)
        {
            liftMod = 0f;
        }
        else if(speedSq > zeroSpeedSq)
        {
            liftMod = (speedSq - zeroSpeedSq) / (minSpeedSq - zeroSpeedSq);
        }

        // Health of the air craft also plays a role. Damage reduces lift.
        float maxLiftMod = Mathf.Min(_hb.Percent, _hullDamageThreshold) / _hullDamageThreshold;
        liftMod *= velDif;
        liftMod = Mathf.Clamp(liftMod, 0f, maxLiftMod);

        //// Maybe cache gravity? Would an if check for the value be faster than the square root?
        //_rb.AddRelativeForce(Vector3.up * Physics.gravity.magnitude * liftMod * _rb.mass, ForceMode.Force);

        //// Rotational forces due to drag
        Vector3 pitchCorrection = Vector3.Cross(transform.forward, _rb.velocity.normalized);
        //Vector3 yawCorrection = Vector3.Cross(transform.right, _rb.velocity);
        _rb.AddTorque(pitchCorrection * (1f - velDif) * 3f * speedSq * liftMod);
        //_rb.AddTorque(yawCorrection * (1f - velDif) * speedSq * 0.5f);

        // Process controls
        _rb.AddRelativeTorque(Vector3.right * _pitchAxis * _pitchSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.back * _rollAxis * _rollSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.up * _yawAxis * _yawSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);

        Debug.Log(liftMod);

        Vector3 counterVector = (transform.forward * speed) - _rb.velocity;
        Vector3 lift = transform.up * liftMod;
        _rb.AddForce(lift * _rb.mass, ForceMode.Force);
        //counterVector.y *= liftMod;
        _rb.AddForce(counterVector * _rb.mass * liftMod, ForceMode.Force);

    }
    #endregion
}
