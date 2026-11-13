using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class HealthAudio : MonoBehaviour
{
    [SerializeField] private EventReference damagedEvent = default;
    [SerializeField] private EventReference deathEvent = default;
    private Health health;

    private void Awake() => health = GetComponent<Health>();

    private void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        if (health == null) return;
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
    }

    private void OnDamaged(int damage)
    {
        if (!health.IsDead) FmodOneShot.Play(damagedEvent, transform.position);
    }

    private void OnDied() => FmodOneShot.Play(deathEvent, transform.position);
}
