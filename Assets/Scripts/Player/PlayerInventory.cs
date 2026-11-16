using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class PlayerInventory : MonoBehaviour
{
    [SerializeField, Min(0)] private int startingPotions = 0;
    [SerializeField, Min(1)] private int maxPotions = 5;
    [SerializeField, Min(1)] private int healingPerPotion = 35;

    private Health health;
    private PlayerDodge dodge;
    public int PotionCount { get; private set; }
    public int MaxPotions => maxPotions;
    public string Message { get; private set; } = string.Empty;
    public float MessageUntil { get; private set; }
    public event Action<int, int> Changed;
    public event Action PotionCollected;
    public event Action<int> PotionUsed;

    private void Awake()
    {
        health = GetComponent<Health>();
        dodge = GetComponent<PlayerDodge>();
        PotionCount = Mathf.Clamp(startingPotions, 0, maxPotions);
    }

    private void Update()
    {
        if (EclipseGameUI.BlocksGameplay || health.IsDead || Time.deltaTime <= 0f) return;
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame) TryUsePotion();
    }

    public bool TryAddPotion(int amount)
    {
        if (amount <= 0 || health.IsDead) return false;
        if (PotionCount >= maxPotions)
        {
            Notify("Inventario de pociones lleno.");
            return false;
        }
        int added = Mathf.Min(amount, maxPotions - PotionCount);
        PotionCount += added;
        Changed?.Invoke(PotionCount, maxPotions);
        Notify("Poción recogida.");
        PotionCollected?.Invoke();
        return true;
    }

    public bool TryUsePotion()
    {
        if (EclipseGameUI.BlocksGameplay || health.IsDead || (dodge != null && dodge.IsBusy)) return false;
        if (PotionCount <= 0)
        {
            Notify("No tenés pociones.");
            return false;
        }
        if (health.CurrentHealth >= health.MaxHealth)
        {
            Notify("La vida ya está completa.");
            return false;
        }
        int restored = health.Heal(healingPerPotion);
        if (restored <= 0) return false;
        PotionCount--;
        Changed?.Invoke(PotionCount, maxPotions);
        Notify("Recuperaste " + restored + " de vida.");
        PotionUsed?.Invoke(restored);
        return true;
    }

    private void Notify(string message)
    {
        Message = message;
        MessageUntil = Time.time + 2.5f;
    }

    public void RestoreCheckpoint(int potions)
    {
        PotionCount = Mathf.Clamp(potions, 0, maxPotions);
        Message = string.Empty;
        MessageUntil = 0f;
        Changed?.Invoke(PotionCount, maxPotions);
    }
}
