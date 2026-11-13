using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MagicProjectile))]
public sealed class ProjectileImpactAudio : MonoBehaviour
{
    [SerializeField] private EventReference impactEvent = default;
    private MagicProjectile projectile;

    private void Awake() => projectile = GetComponent<MagicProjectile>();
    private void OnEnable() => projectile.Impacted += OnImpact;
    private void OnDisable()
    {
        if (projectile != null) projectile.Impacted -= OnImpact;
    }
    private void OnImpact(Vector3 position) => FmodOneShot.Play(impactEvent, position);
}
