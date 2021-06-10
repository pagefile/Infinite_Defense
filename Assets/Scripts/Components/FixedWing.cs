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
    [Header("Flight Simulation")]
    [SerializeField]
    private float _wingArea = 3f;
    [SerializeField]
    private AnimationCurve _liftCurve = default;
    [SerializeField]
    private AnimationCurve _dragCurve = default;
    #endregion

    #region Accessors/Modifiers
    public float Speed { get { return _speed; } }
    public float LiftMod { get { return _liftMod; } }
    #endregion

    #region Private Members
    private Rigidbody _rb = null;
    private HealthBar _hb = null;

    private float _criticalAngle = 0f;

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

        // Find the critical angle of attack
        // value = lift coefficient
        // time = angle
        float lc = 0f;
        for(int i = 0; i < _liftCurve.length; i++)
        {
            Keyframe kf = _liftCurve[i];
            if(kf.value > lc)
            {
                lc = kf.value;
                _criticalAngle = kf.time;
            }
        }
    }

    private void FixedUpdate()
    {
        float speedSq = _rb.velocity.sqrMagnitude;
        // Since I'm getting it every frame I might as well cache it
        _speed = _rb.velocity.magnitude;
        Vector3 velocity = _rb.velocity;
        Vector3 forward = transform.forward;
        Vector3 projectedForward = Vector3.Project(forward, velocity);
        float angleOfAttack = Vector3.SignedAngle(transform.forward, velocity, transform.right);

        // Lift and drag
        float liftCoefficient = _liftCurve.Evaluate(angleOfAttack);
        float dragCoefficient = _dragCurve.Evaluate(angleOfAttack);
        float lift = liftCoefficient * _wingArea * speedSq * 0.5f;
        float drag = dragCoefficient * _wingArea * speedSq * 0.5f;
        _rb.AddRelativeForce(Vector3.up * lift, ForceMode.Force);
        _rb.AddForce(-velocity.normalized * drag, ForceMode.Force);

        _liftMod = liftCoefficient;

        // Controls
        float limiter = Mathf.Clamp(1f - (angleOfAttack / _criticalAngle), 0f, 1f);
        _rb.AddRelativeTorque(Vector3.right * _pitchAxis * limiter * _pitchSpeed * Mathf.Deg2Rad * _rb.mass, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.back * _rollAxis * _rollSpeed * Mathf.Deg2Rad * _rb.mass, ForceMode.Force);
        _rb.AddRelativeTorque(Vector3.up * _yawAxis * _yawSpeed * Mathf.Deg2Rad * _rb.mass, ForceMode.Force);
    }
    #endregion
}
