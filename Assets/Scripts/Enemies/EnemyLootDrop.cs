using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class EnemyLootDrop : MonoBehaviour
{
    [SerializeField] private PotionPickup potionPrefab = null;
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.25f;
    [SerializeField] private LayerMask groundLayers = Physics.DefaultRaycastLayers;
    [SerializeField, Min(0f)] private float groundOffset = 0.35f;
    private Health health;
    private bool dropped;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (potionPrefab == null)
        {
            Debug.LogError("EnemyLootDrop necesita un Potion Prefab.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        if (health != null) health.Died -= OnDied;
    }

    private void OnDied()
    {
        if (dropped || potionPrefab == null || dropChance <= 0f) return;
        dropped = true;
        if (dropChance < 1f && Random.value >= dropChance) return;

        Vector3 position = transform.position;
        if (Physics.Raycast(position + Vector3.up, Vector3.down,
            out RaycastHit hit, 6f, groundLayers, QueryTriggerInteraction.Ignore))
        {
            position = hit.point + Vector3.up * groundOffset;
        }
        else
        {
            Debug.LogWarning("No se encontró suelo para el loot; revisa Ground Layers.", this);
        }
        Instantiate(potionPrefab, position, Quaternion.identity);
    }
}
