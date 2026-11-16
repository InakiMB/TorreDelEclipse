using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossEncounter : MonoBehaviour, IDamageGuard
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private EncounterObjective objective;
    [SerializeField] private BossController bossPrefab;
    [SerializeField] private Transform bossSpawn;
    [SerializeField] private Transform retrySpawn;
    [SerializeField] private GameObject entranceSeal;
    [SerializeField] private TopDownCamera viewCamera;
    [SerializeField, Min(0.1f)] private float activationRadius = 5f;
    private int savedHealth, savedPotions;
    private bool checkpoint, defeated, completed, retrying;
    public BossController Boss { get; private set; }
    public bool ArenaSealed => checkpoint && !completed;
    public bool CanRetry => checkpoint && defeated && playerHealth.IsDead && !retrying;
    public bool Completed => completed;
    public bool PreventsDamage => Boss != null && (Boss.Action == BossAction.Summon || Boss.Action == BossAction.Transform);

    private void Start()
    {
        if (entranceSeal != null) entranceSeal.SetActive(false);
        // El protector vive en Player, donde Health consulta sus componentes.
        if (playerHealth != null)
        {
            var guard = playerHealth.GetComponent<BossPlayerProtection>() ?? playerHealth.gameObject.AddComponent<BossPlayerProtection>();
            guard.Bind(this); playerHealth.Died += OnPlayerDied;
        }
    }
    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
        if (Boss != null) Boss.DeathFinished -= OnBossDead;
    }
    private void Update()
    {
        if (checkpoint || playerHealth == null || playerHealth.IsDead || objective == null
            || objective.Stage != EncounterStage.ExitUnlocked) return;
        Vector3 delta = playerHealth.transform.position - transform.position;
        if (Mathf.Abs(delta.y) < 3f && Vector3.ProjectOnPlane(delta, Vector3.up).magnitude <= activationRadius) Activate();
    }
    public bool Activate()
    {
        if (checkpoint || playerHealth == null || playerHealth.IsDead || objective.Stage != EncounterStage.ExitUnlocked
            || bossPrefab == null || bossSpawn == null) return false;
        checkpoint = true; savedHealth = playerHealth.CurrentHealth; savedPotions = inventory != null ? inventory.PotionCount : 0;
        if (entranceSeal != null) entranceSeal.SetActive(true);
        SpawnBoss(false); return true;
    }
    private void SpawnBoss(bool retry)
    {
        Boss = Instantiate(bossPrefab, bossSpawn.position, bossSpawn.rotation, transform);
        Boss.name = "CustodioDelEclipse"; Boss.DeathFinished += OnBossDead; Boss.Begin(playerHealth, retry);
        if (viewCamera != null) viewCamera.SetCombatFocus(Boss.transform);
    }
    private void OnPlayerDied()
    {
        if (!checkpoint || completed) return;
        defeated = true;
        if (Boss != null) Boss.Abort();
        if (viewCamera != null) viewCamera.SetCombatFocus(null);
    }
    public bool Retry()
    {
        if (!CanRetry || retrySpawn == null) return false;
        retrying = true;
        if (Boss != null) { Boss.DeathFinished -= OnBossDead; Boss.Abort(); Destroy(Boss.gameObject); Boss = null; }
        var controller = playerHealth.GetComponent<CharacterController>(); controller.enabled = false;
        playerHealth.transform.SetPositionAndRotation(retrySpawn.position, retrySpawn.rotation); controller.enabled = true;
        playerHealth.RestoreCheckpoint(savedHealth);
        if (inventory != null) inventory.RestoreCheckpoint(savedPotions);
        PlayerControlLock.Restore(playerHealth.gameObject);
        var dodge = playerHealth.GetComponent<PlayerDodge>(); if (dodge != null) dodge.RestoreCheckpoint();
        var staff = playerHealth.GetComponent<PlayerStaffAttack>(); if (staff != null) staff.CancelAttack();
        objective.RestoreBossCheckpoint();
        Physics.SyncTransforms(); if (viewCamera != null) viewCamera.SnapToTarget();
        defeated = false; retrying = false; SpawnBoss(true); return true;
    }
    private void OnBossDead()
    {
        if (playerHealth.IsDead || completed) return;
        completed = true;
        if (viewCamera != null) viewCamera.SetCombatFocus(null);
        if (entranceSeal != null) entranceSeal.SetActive(false);
        objective.TryComplete();
    }
}
