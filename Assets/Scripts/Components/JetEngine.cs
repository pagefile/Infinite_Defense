using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JetEngine : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    float _maxThrust = 100f;
    [SerializeField]
    bool _relativeForce = false;
    #endregion

    #region Private Members
    float _throttle = 0f;
    Rigidbody _rb = null;
    #endregion

    #region Unity Functions
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.AddForce(transform.forward * _maxThrust, ForceMode.Force);
    }
    #endregion
}
