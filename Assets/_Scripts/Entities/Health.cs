﻿using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _maxHealth = 5f;
    float _currentHealth;

    bool _isDefending;
    public bool IsDefending { get => _isDefending; set => _isDefending = value; }

    public FloatEvent OnHit, OnDeath, OnHeal;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DealDamage(float damage)
    {
        _currentHealth -= damage * (_isDefending ? 0.5f : 1f);

        if (_currentHealth <= 0)
            OnDeath?.Invoke(_currentHealth / _maxHealth);
        else
            OnHit?.Invoke(_currentHealth / _maxHealth);
    }

    public void Heal(float amount)
    {
         _currentHealth = Mathf.Clamp(_currentHealth + amount, 0f, _maxHealth);

        OnHeal?.Invoke(_currentHealth / _maxHealth);
    }
}