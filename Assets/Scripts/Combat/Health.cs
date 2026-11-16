using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Health : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maxHealth = 100;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0;
    private readonly List<MonoBehaviour> guards = new List<MonoBehaviour>();
    public bool IsDamageBlocked
    {
        get
        {
            guards.Clear();
            GetComponents(guards);
            foreach (var component in guards)
                if (component != null && component.isActiveAndEnabled && component is IDamageGuard guard && guard.PreventsDamage)
                    return true;
            return false;
        }
    }

    public event Action<int, int> Changed;
    public event Action<int> Damaged;
    public event Action<int> Healed;
    public event Action Died;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead || IsDamageBlocked) return;

        int actualDamage = Mathf.Min(amount, CurrentHealth);
        CurrentHealth -= actualDamage;
        Changed?.Invoke(CurrentHealth, maxHealth);
        Damaged?.Invoke(actualDamage);
        if (IsDead) Died?.Invoke();
    }

    public int Heal(int amount)
    {
        if (amount <= 0 || IsDead) return 0;

        int actualHealing = Mathf.Min(amount, maxHealth - CurrentHealth);
        if (actualHealing <= 0) return 0;
        CurrentHealth += actualHealing;
        Changed?.Invoke(CurrentHealth, maxHealth);
        Healed?.Invoke(actualHealing);
        return actualHealing;
    }

    public void RestoreCheckpoint(int amount)
    {
        CurrentHealth = Mathf.Clamp(amount, 1, maxHealth);
        Changed?.Invoke(CurrentHealth, maxHealth);
    }
}
