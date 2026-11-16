using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemyMage : MonoBehaviour, IEnemyCombatant
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform castOrigin;
    [SerializeField] private MagicProjectile projectilePrefab;
    [SerializeField, Min(1f)] private float detectionRange = 14f;
    [SerializeField, Min(1f)] private float castRange = 10f;
    [SerializeField, Min(1f)] private float preferredRange = 7f;
    [SerializeField, Min(0.5f)] private float retreatRange = 3.5f;
    [SerializeField, Min(1)] private int damage = 12;
    [SerializeField, Min(0.1f)] private float castWindup = 0.7f;
    [SerializeField, Min(0f)] private float castRecovery = 0.5f;
    [SerializeField, Min(0.1f)] private float castInterval = 2.4f;
    [SerializeField, Min(0f)] private float destroyDelay = 1.6f;
    [SerializeField] private EventReference castEvent;
    [SerializeField, Range(0.5f, 2f)] private float castPitch = 0.75f;

    private Health health;
    private Health targetHealth;
    private NavMeshAgent agent;
    private EnemyBuffReceiver buffs;
    private float activeCastWindup;
    private bool provoked;
    private bool pendingShot;
    private Vector3 lockedDirection;
    private float shotAt;
    private float recoverAt;
    private float nextCastAt;
    public bool IsCasting { get; private set; }
    public float CastPlaybackSpeed { get; private set; } = 1f;
    public float ChargeProgress => IsCasting && pendingShot ? Mathf.Clamp01(1f - (shotAt - Time.time) / activeCastWindup) : 0f;
    public bool IsEngaged => isActiveAndEnabled && health != null && !health.IsDead
        && agent != null && agent.enabled && agent.isOnNavMesh && targetHealth != null && !targetHealth.IsDead
        && (provoked || (target.position - transform.position).sqrMagnitude <= detectionRange * detectionRange);
    public event Action CastStarted;
    public void AlertToPlayer() { if (health != null && !health.IsDead) provoked = true; }
    public event Action ShotFired;

    private void Awake()
    {
        health = GetComponent<Health>(); agent = GetComponent<NavMeshAgent>();
        buffs = GetComponent<EnemyBuffReceiver>();
        if (buffs == null) buffs = gameObject.AddComponent<EnemyBuffReceiver>();
        agent.stoppingDistance = 0.3f;
    }
    private void OnEnable() { health.Damaged += OnDamaged; health.Died += OnDied; }
    private void OnDisable()
    {
        health.Damaged -= OnDamaged; health.Died -= OnDied;
        IsCasting = false; pendingShot = false;
        if (agent != null && agent.enabled && agent.isOnNavMesh) Stop();
    }
    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        if (target != null) targetHealth = target.GetComponent<Health>();
        if (targetHealth == null || castOrigin == null || projectilePrefab == null)
        {
            Debug.LogError("EnemyMage necesita Player, CastOrigin y su proyectil.", this); enabled = false;
        }
    }
    private void Update()
    {
        if (health.IsDead || !agent.enabled || !agent.isOnNavMesh || Time.deltaTime <= 0f) return;
        if (target == null || targetHealth == null || targetHealth.IsDead)
        {
            IsCasting = false; pendingShot = false; Stop(); return;
        }
        if (IsCasting)
        {
            Stop();
            if (pendingShot && Time.time >= shotAt) { pendingShot = false; Fire(); }
            if (Time.time >= recoverAt) IsCasting = false;
            return;
        }
        if (!IsEngaged) { Stop(); return; }
        float distance = Vector3.Distance(transform.position, target.position);
        bool clear = HasLineOfSight();
        if (distance < retreatRange && Retreat()) return;
        if (distance > castRange || !clear) { Approach(); return; }
        Stop(); Face(target.position);
        if (Time.time < nextCastAt) return;
        lockedDirection = target.position - castOrigin.position;
        lockedDirection.y = 0f;
        if (lockedDirection.sqrMagnitude < 0.001f) lockedDirection = transform.forward;
        lockedDirection.Normalize();
        transform.rotation = Quaternion.LookRotation(lockedDirection);
        CastPlaybackSpeed = buffs.AttackSpeedMultiplier;
        activeCastWindup = castWindup / CastPlaybackSpeed;
        IsCasting = true; pendingShot = true;
        shotAt = Time.time + activeCastWindup; recoverAt = shotAt + castRecovery / CastPlaybackSpeed;
        nextCastAt = Time.time + Mathf.Max(castInterval, castWindup + castRecovery) / CastPlaybackSpeed;
        CastStarted?.Invoke();
    }
    private void Fire()
    {
        MagicProjectile projectile = Instantiate(projectilePrefab, castOrigin.position, Quaternion.LookRotation(lockedDirection));
        projectile.Initialize(transform, lockedDirection, damage, targetHealth);
        if (!castEvent.IsNull)
        {
            var sound = RuntimeManager.CreateInstance(castEvent);
            sound.set3DAttributes(RuntimeUtils.To3DAttributes(castOrigin.position));
            sound.setPitch(castPitch); sound.start(); sound.release();
        }
        ShotFired?.Invoke();
    }
    private void Approach()
    {
        Vector3 destination = target.position;
        var controller = target.GetComponent<CharacterController>();
        if (controller != null) destination = target.TransformPoint(controller.center) - Vector3.up * controller.height * 0.5f;
        agent.stoppingDistance = HasLineOfSight() ? preferredRange : 0.3f;
        agent.isStopped = false; agent.SetDestination(destination);
    }
    private bool Retreat()
    {
        Vector3 away = Vector3.ProjectOnPlane(transform.position - target.position, Vector3.up).normalized;
        if (away.sqrMagnitude < 0.01f) away = -transform.forward;
        foreach (float angle in new[] { 0f, 45f, -45f, 90f, -90f })
        {
            Vector3 desired = transform.position + Quaternion.Euler(0, angle, 0) * away * (preferredRange - retreatRange + 1f);
            if (!NavMesh.SamplePosition(desired - Vector3.up * agent.baseOffset, out var sample, 1.5f, agent.areaMask)) continue;
            if (Vector3.ProjectOnPlane(sample.position - target.position, Vector3.up).magnitude < retreatRange + 0.5f) continue;
            var path = new NavMeshPath();
            if (!agent.CalculatePath(sample.position, path) || path.status != NavMeshPathStatus.PathComplete) continue;
            agent.stoppingDistance = 0.2f; agent.isStopped = false; agent.SetPath(path); return true;
        }
        return false;
    }
    private bool HasLineOfSight()
    {
        Vector3 delta = target.position - castOrigin.position;
        foreach (var hit in Physics.RaycastAll(castOrigin.position, delta.normalized, delta.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform.IsChildOf(target)) continue;
            if (hit.collider.GetComponentInParent<IEnemyCombatant>() != null) continue;
            return false;
        }
        return true;
    }
    private void Face(Vector3 position)
    {
        Vector3 direction = Vector3.ProjectOnPlane(position - transform.position, Vector3.up);
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 540f * Time.deltaTime);
    }
    private void Stop() { agent.isStopped = true; if (agent.hasPath) agent.ResetPath(); }
    private void OnDamaged(int amount) => provoked = true;
    private void OnDied()
    {
        IsCasting = false; pendingShot = false;
        if (agent.enabled && agent.isOnNavMesh) Stop();
        agent.enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        Destroy(gameObject, destroyDelay);
    }
}
