using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MagicProjectile : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float speed = 14f;
    [SerializeField, Min(0.01f)] private float radius = 0.12f;
    [SerializeField, Min(0.1f)] private float lifeTime = 2f;

    private Transform owner;
    private Vector3 direction;
    private int damage;
    private float age;
    private bool initialized;

    public event Action<Vector3> Impacted;

    public void Initialize(Transform source, Vector3 travelDirection, int hitDamage)
    {
        owner = source;
        direction = travelDirection.normalized;
        damage = hitDamage;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || Time.deltaTime <= 0f)
        {
            return;
        }

        age += Time.deltaTime;
        if (age >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        float distance = speed * Time.deltaTime;
        if (FindClosestHit(distance, out RaycastHit hit))
        {
            transform.position = hit.point;
            IDamageable receiver = hit.collider.GetComponentInParent<IDamageable>();
            if (receiver != null)
            {
                receiver.TakeDamage(damage);
            }

            Impacted?.Invoke(hit.point);
            Destroy(gameObject);
            return;
        }

        transform.position += direction * distance;
    }

    private bool FindClosestHit(float distance, out RaycastHit closest)
    {
        closest = default;
        float closestDistance = float.PositiveInfinity;
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position, radius, direction, distance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (owner != null && hit.collider.transform.IsChildOf(owner))
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closest = hit;
            }
        }

        return closestDistance < float.PositiveInfinity;
    }
}
