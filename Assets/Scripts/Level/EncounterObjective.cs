using System;
using UnityEngine;

public enum EncounterStage { Fighting, ExitUnlocked, Completed, Defeated }

[DisallowMultipleComponent]
public sealed class EncounterObjective : MonoBehaviour
{
    [SerializeField] private Health playerHealth = null;
    [SerializeField] private Health[] requiredEnemies = new Health[0];
    [SerializeField] private GameObject gateBlocker = null;
    private bool ready;

    public EncounterStage Stage { get; private set; } = EncounterStage.Fighting;
    public int RemainingEnemies
    {
        get
        {
            int alive = 0;
            foreach (Health enemy in requiredEnemies)
                if (enemy != null && !enemy.IsDead) alive++;
            return alive;
        }
    }
    public event Action<EncounterStage> StageChanged;

    private void Start()
    {
        if (playerHealth == null || gateBlocker == null || requiredEnemies.Length == 0)
        {
            Debug.LogError("EncounterObjective necesita jugador, enemigos y Gate Blocker.", this);
            enabled = false;
            return;
        }
        foreach (Health enemy in requiredEnemies)
        {
            if (enemy == null)
            {
                Debug.LogError("Required Enemies contiene una referencia vacía.", this);
                enabled = false;
                return;
            }
        }
        gateBlocker.SetActive(true);
        ready = true;
    }

    private void Update()
    {
        if (!ready || Stage == EncounterStage.Completed || Stage == EncounterStage.Defeated) return;
        if (playerHealth.IsDead)
        {
            SetStage(EncounterStage.Defeated);
            return;
        }
        if (Stage == EncounterStage.Fighting && RemainingEnemies == 0)
        {
            gateBlocker.SetActive(false);
            SetStage(EncounterStage.ExitUnlocked);
        }
    }

    public bool TryComplete()
    {
        if (!ready || !isActiveAndEnabled || Stage != EncounterStage.ExitUnlocked || playerHealth.IsDead)
            return false;
        PlayerControlLock.Freeze(playerHealth.gameObject);
        SetStage(EncounterStage.Completed);
        return true;
    }

    private void SetStage(EncounterStage stage)
    {
        Stage = stage;
        StageChanged?.Invoke(stage);
    }
}
