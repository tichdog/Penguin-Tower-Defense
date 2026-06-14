using System;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance;

    [SerializeField] private int startHealth = 100;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => startHealth;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentHealth = startHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    public void ResetHealth()
    {
        CurrentHealth = startHealth;
        NotifyHealthChanged();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        int healthDamage = Mathf.CeilToInt(damage);

        if (healthDamage < 1)
            healthDamage = 1;

        CurrentHealth = Mathf.Max(0, CurrentHealth - healthDamage);

        NotifyHealthChanged();

        if (IsDead)
            OnPlayerDied?.Invoke();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
