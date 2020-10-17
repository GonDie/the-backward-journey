using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _maxHealth = 5f;
    float _currentHealth;

    public SimpleEvent OnHit, OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DealDamage(float damage)
    {
        _currentHealth -= damage;


    }
}