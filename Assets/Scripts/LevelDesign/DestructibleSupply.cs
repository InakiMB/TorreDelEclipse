using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class DestructibleSupply : MonoBehaviour
{
    [SerializeField] private PotionPickup potionPrefab = null;
    [SerializeField] private ParticleSystem breakEffect = null;
    [SerializeField] private EventReference hitSound = default;
    [SerializeField] private EventReference breakSound = default;
    private Health health;
    private bool broken;

    private void Awake() => health = GetComponent<Health>();
    private void OnEnable() { health.Damaged += OnDamaged; health.Died += OnBroken; }
    private void OnDisable()
    {
        if (health == null) return;
        health.Damaged -= OnDamaged;
        health.Died -= OnBroken;
    }
    private void OnDamaged(int amount)
    {
        if (!health.IsDead) FmodOneShot.Play(hitSound, transform.position + Vector3.up * 0.6f);
    }
    private void OnBroken()
    {
        if (broken) return;
        broken = true;
        foreach (Collider item in GetComponentsInChildren<Collider>()) item.enabled = false;
        Vector3 position = transform.position + Vector3.up * 0.35f;
        if (potionPrefab != null) Instantiate(potionPrefab, position, Quaternion.identity);
        if (breakEffect != null) Instantiate(breakEffect, position, Quaternion.identity).Play();
        FmodOneShot.Play(breakSound, position);
        Destroy(gameObject);
    }
}
