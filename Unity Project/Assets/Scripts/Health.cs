using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth = 100f;
    [SerializeField]
    private float _currentHealth = 100f;

    public UnityEvent<float> OnMaxHealthSet;
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    public bool IsAlive => _currentHealth > 0f;
    public float MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth);
        }
    }

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            float clamped = Mathf.Clamp(value, 0f, _maxHealth);
            if (Mathf.Approximately(clamped, _currentHealth)) return;

            _currentHealth = clamped;
            OnHealthChanged?.Invoke(_currentHealth);

            if (_currentHealth <= 0f)
                OnDeath?.Invoke();
        }
    }

    private void Awake()
    {
        OnMaxHealthSet?.Invoke(_maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount <= 0f || _currentHealth <= 0f)
            return;

        CurrentHealth -= damageAmount;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0f || _currentHealth >= _maxHealth)
            return;

        CurrentHealth += healAmount;
    }


}
