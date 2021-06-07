using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JetEngine : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    float _maxThrust = 100f;
    #endregion

    #region Private Members
    float _throttle = 0f;
    Rigidbody _rb = null;
    #endregion

    #region Public Methods
    public void Throttle(float value)
    {
        _throttle = value;
    }

    public void IncreaseThrottle(float amount)
    {
        _throttle += amount;
        if(_throttle > 1f)
        {
            _throttle = 1f;
        }
    }

    public void DecreaseThrottle(float amount)
    {
        _throttle -= amount;
        if(_throttle < 0f)
        {
            _throttle = 0f;
        }
    }
    #endregion

    #region Unity Functions
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.AddForce(transform.forward * _maxThrust * _throttle, ForceMode.Force);
    }
    #endregion
}
