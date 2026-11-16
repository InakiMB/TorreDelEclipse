using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossProjectile : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float radius = 0.16f;
    [SerializeField, Min(0.1f)] private float lifetime = 5.5f;
    private BossController owner;
    private Vector3 direction;
    private float speed, age;
    private int damage;
    public void Initialize(BossController source, Vector3 travelDirection, float velocity, int hitDamage)
    { owner = source; direction = travelDirection.normalized; speed = velocity; damage = hitDamage; }
    private void Update()
    {
        if (owner == null || !owner.IsEngaged || owner.Target == null || owner.Target.IsDead) { Destroy(gameObject); return; }
        if (Time.deltaTime <= 0) return;
        age += Time.deltaTime; if (age >= lifetime) { Destroy(gameObject); return; }
        float distance = speed * Time.deltaTime;
        RaycastHit nearest = default; bool found = false; float closest = float.PositiveInfinity;
        foreach (RaycastHit hit in Physics.SphereCastAll(transform.position, radius, direction, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.IsChildOf(owner.transform) || hit.collider.GetComponentInParent<IEnemyCombatant>() != null) continue;
            if (hit.collider.GetComponentInParent<Health>() == owner.Target && owner.Target.IsDamageBlocked) continue;
            if (hit.distance < closest) { closest = hit.distance; nearest = hit; found = true; }
        }
        if (found)
        {
            if (nearest.collider.GetComponentInParent<Health>() == owner.Target) owner.TryDamagePlayer(damage);
            owner.ProjectileImpact(nearest.point); Destroy(gameObject); return;
        }
        transform.position += direction * distance;
    }
    private void OnDestroy() { if (owner != null) owner.ReleaseProjectile(this); }
}
