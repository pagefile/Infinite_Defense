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
    [SerializeField]
    private List<JetEngine> _engines = default;
    #endregion

    #region Accessors/Modifiers
    public float Speed { get { return _speed; } }
    public float LiftMod { get { return _liftMod; } }
    #endregion

    #region Private Members
    private Rigidbody _rb = null;
    private HealthBar _hb = null;

    private float _pitchAxis = 0f;
    private float _yawAxis = 0f;
    private float _rollAxis = 0f;
    private float _throttle = 0f;

    private float _speed = 0f;
    private float _liftMod = 0f;
    #endregion

    #region Public Methods
    // TODO: These should be part of an interface
    // MOAR TODO: Maybe also just make these properties?

    public float Throttle
    { 
        get
        {
            return _throttle;
        }
        set
        {
            _throttle = value;
            foreach(JetEngine engine in _engines)
            {
                engine.Throttle(_throttle);
            }
        }
    }

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

    public void IncreaseThrottle(float amount)
    {
        _throttle += amount;
        if(_throttle > 1f)
        {
            _throttle = 1f;
        }
        foreach(JetEngine engine in _engines)
        {
            engine.IncreaseThrottle(_throttle);
        }
    }

    public void DecreaseThrottle(float amount)
    {
        _throttle -= amount;
        if(_throttle < 0f)
        {
            _throttle = 0f;
        }
        foreach(JetEngine engine in _engines)
        {
            engine.DecreaseThrottle(amount);
        }
    }

    public float GetPitch()
    {
        return _pitchAxis;
    }

    public float GetYaw()
    {
        return _yawAxis;
    }

    public float GetRoll()
    {
        return _rollAxis;
    }
    #endregion

    #region Unity Functions
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _hb = GetComponent<HealthBar>();
        Throttle = 1f;
    }

    private void FixedUpdate()
    {
        float speedSq = _rb.velocity.sqrMagnitude;
        // Since I'm getting it every frame I might as well cache it
        _speed = _rb.velocity.magnitude;
        float minSpeedSq = _minFlightSpeed * _minFlightSpeed;
        float zeroSpeedSq = _zeroFlightSpeed * _zeroFlightSpeed;
        this._liftMod = 1f;
        float velDif = Mathf.Max(Vector3.Dot(_rb.velocity.normalized, transform.forward), 0f);

        // Check to make sure it's going fast enough
        if(speedSq < zeroSpeedSq)
        {
            _liftMod = 0f;
        }
        else if(speedSq > zeroSpeedSq)
        {
            _liftMod = (speedSq - zeroSpeedSq) / (minSpeedSq - zeroSpeedSq);
        }

        // Health of the air craft also plays a role. Damage reduces lift.
        // velDif is the difference between the velocity of the aircraft and direction it is facing
        float maxLiftMod = Mathf.Min(_hb.Percent, _hullDamageThreshold) / _hullDamageThreshold;
        _liftMod *= velDif;
        _liftMod = Mathf.Clamp(_liftMod, 0f, maxLiftMod);

        // Rotational forces due to drag
        Vector3 pitchCorrection = Vector3.Cross(transform.forward, _rb.velocity.normalized);
        _rb.AddTorque(pitchCorrection * (1f - velDif) * speedSq, ForceMode.Force);

        // Process controls
        _rb.AddRelativeTorque(Vector3.right * _pitchAxis * _pitchSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.back * _rollAxis * _rollSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.up * _yawAxis * _yawSpeed * Mathf.Deg2Rad * _rb.mass * velDif, ForceMode.Force);

        // Counter vector is used to keep the plane from acting like a space ship. It basically provides
        // lateral forces to try to keep the air craft going in the direction it's facing
        Vector3 counterVector = (transform.forward * _speed) - _rb.velocity;
        Vector3 lift = transform.up * _liftMod;
        _rb.AddForce(lift * _rb.mass, ForceMode.Force);
        _rb.AddForce(counterVector * _rb.mass * _liftMod, ForceMode.Force);

    }
    #endregion
}
