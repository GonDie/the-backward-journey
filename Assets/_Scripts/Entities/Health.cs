using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _maxHealth = 5f;
    float _currentHealth;

    public FloatEvent OnHit, OnDeath;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void DealDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
            OnDeath?.Invoke(_currentHealth / _maxHealth);
        else
            OnHit?.Invoke(_currentHealth / _maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            DealDamage(1f);
    }
}