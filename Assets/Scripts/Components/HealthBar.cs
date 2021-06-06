using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private float _maxHealth = 100f;
    #endregion

    #region Public Members
    public delegate void HealthEvent(HealthBar hb);
    HealthEvent OnChanged;
    HealthEvent OnHealed;
    HealthEvent OnDamaged;
    HealthEvent OnDeath;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public float Percent => _currentHealth / _maxHealth;
    #endregion

    #region Public Methods
    public float Damage(float amount)
    {
        _currentHealth -= amount;
        OnDamaged?.Invoke(this);
        OnChanged?.Invoke(this);
        if(_currentHealth <= 0f)
        {
            OnDeath?.Invoke(this);
        }
        return _currentHealth;
    }

    public float Heal(float amount)
    {
        _currentHealth += amount;
        if(_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        OnHealed?.Invoke(this);
        OnChanged?.Invoke(this);
        return _currentHealth;
    }
    #endregion


    #region Private Members
    private float _currentHealth = 0;
    #endregion

    #region Unity Functions
    private void Start()
    {
        _currentHealth = _maxHealth;
    }
    #endregion
}
