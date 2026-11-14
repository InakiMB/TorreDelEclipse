using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SupportSpell { None, Haste, Heal }

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemySupport : MonoBehaviour, IEnemyCombatant
{
    [SerializeField] private Transform target;
    [SerializeField, Min(1f)] private float detectionRange = 14f;
    [SerializeField, Min(1f)] private float supportRange = 8f;
    [SerializeField, Min(1f)] private float followRange = 18f;
    [SerializeField, Min(1f)] private float retreatRange = 4f;
    [SerializeField, Range(0, 100)] private float movementBonusPercent = 20f;
    [SerializeField, Range(0, 100)] private float attackSpeedBonusPercent = 25f;
    [SerializeField, Range(1, 100)] private float healingPercent = 15f;
    [SerializeField, Min(0.5f)] private float healInterval = 6f;
    [SerializeField, Min(0.1f)] private float castWindup = 0.9f;
    [SerializeField, Min(0f)] private float castRecovery = 0.4f;
    [SerializeField, Min(0f)] private float destroyDelay = 1.6f;

    private Health health;
    private Health targetHealth;
    private NavMeshAgent agent;
    private EnemyBuffReceiver[] allies = Array.Empty<EnemyBuffReceiver>();
    private readonly HashSet<EnemyBuffReceiver> affected = new HashSet<EnemyBuffReceiver>();
    private readonly List<EnemyBuffReceiver> leaving = new List<EnemyBuffReceiver>();
    private Health healingTarget;
    private bool provoked;
    private bool groupEngaged;
    private bool auraReady;
    private bool spellPending;
    private float spellAt;
    private float recoverAt;
    private float nextHealAt;
    private float nextScanAt;

    public SupportSpell CastingSpell { get; private set; }
    public bool IsCasting => CastingSpell != SupportSpell.None;
    public float ChargeProgress => IsCasting && spellPending ? Mathf.Clamp01(1f - (spellAt - Time.time) / castWindup) : 0f;
    public Health HealingTarget => healingTarget;
    public float SupportRange => supportRange;
    public bool IsEngaged => isActiveAndEnabled && health != null && !health.IsDead
        && agent != null && agent.enabled && agent.isOnNavMesh && target != null && targetHealth != null && !targetHealth.IsDead
        && (provoked || groupEngaged || (target.position - transform.position).sqrMagnitude <= detectionRange * detectionRange);
    public bool IsAuraActive => auraReady && IsEngaged;
    public event Action<SupportSpell> CastStarted;
    public event Action HasteActivated;
    public event Action<Health, int> AllyHealed;

    private void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
        CancelSpells();
        ClearAura();
        if (agent.enabled && agent.isOnNavMesh) Stop();
    }

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        if (target != null) targetHealth = target.GetComponent<Health>();
        if (targetHealth == null)
        {
            Debug.LogError("EnemySupport necesita un Player con Health.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (health.IsDead || !agent.enabled || !agent.isOnNavMesh || Time.deltaTime <= 0f) return;
        if (target == null || targetHealth == null || targetHealth.IsDead)
        {
            CancelSpells(); ClearAura(); Stop(); return;
        }

        bool scan = Time.time >= nextScanAt;
        if (scan)
        {
            nextScanAt = Time.time + 0.2f;
            allies = FindObjectsByType<EnemyBuffReceiver>(FindObjectsSortMode.None);
            groupEngaged = false;
            foreach (var ally in allies)
                if (IsNearby(ally, followRange) && ally.IsEngaged) { groupEngaged = true; break; }
        }
        if (!IsEngaged) { CancelSpells(); ClearAura(); Stop(); return; }
        if (scan) RefreshAura();

        if (IsCasting)
        {
            Stop();
            if (spellPending && Time.time >= spellAt) { spellPending = false; ResolveSpell(); }
            if (Time.time >= recoverAt) { CastingSpell = SupportSpell.None; healingTarget = null; }
            return;
        }

        float playerDistance = Vector3.Distance(transform.position, target.position);
        if (playerDistance < retreatRange && Retreat()) return;
        if (!auraReady && HasAllyInRange()) { BeginCast(SupportSpell.Haste, null); return; }

        if (Time.time >= nextHealAt)
        {
            Health wounded = SelectWoundedAlly();
            if (wounded != null) { BeginCast(SupportSpell.Heal, wounded); return; }
        }
        if (scan) FollowAllies();
    }

    private void BeginCast(SupportSpell spell, Health ally)
    {
        Stop();
        CastingSpell = spell;
        healingTarget = ally;
        spellPending = true;
        spellAt = Time.time + castWindup;
        recoverAt = spellAt + castRecovery;
        if (spell == SupportSpell.Heal) nextHealAt = Time.time + healInterval;
        Vector3 direction = Vector3.ProjectOnPlane((ally != null ? ally.transform.position : target.position) - transform.position, Vector3.up);
        if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(direction);
        CastStarted?.Invoke(spell);
    }

    private void ResolveSpell()
    {
        if (CastingSpell == SupportSpell.Haste)
        {
            auraReady = true;
            RefreshAura();
            HasteActivated?.Invoke();
        }
        else if (CastingSpell == SupportSpell.Heal && healingTarget != null)
        {
            var receiver = healingTarget.GetComponent<EnemyBuffReceiver>();
            if (!CanReach(receiver)) return;
            int amount = healingTarget.Heal(Mathf.CeilToInt(healingTarget.MaxHealth * healingPercent / 100f));
            if (amount > 0) AllyHealed?.Invoke(healingTarget, amount);
        }
    }

    private Health SelectWoundedAlly()
    {
        Health selected = null;
        float lowestFraction = 1f;
        foreach (var ally in allies)
        {
            if (!CanReach(ally)) continue;
            float fraction = (float)ally.Health.CurrentHealth / ally.Health.MaxHealth;
            if (fraction < lowestFraction) { lowestFraction = fraction; selected = ally.Health; }
        }
        return selected;
    }

    private bool HasAllyInRange()
    {
        foreach (var ally in allies) if (CanReach(ally)) return true;
        return false;
    }

    private bool IsNearby(EnemyBuffReceiver ally, float range) => ally != null && ally.IsAlive
        && Mathf.Abs(ally.transform.position.y - transform.position.y) < 2.5f
        && (ally.transform.position - transform.position).sqrMagnitude <= range * range;

    private bool CanReach(EnemyBuffReceiver ally)
    {
        if (!IsNearby(ally, supportRange)) return false;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Vector3 delta = ally.transform.position + Vector3.up * 0.1f - origin;
        foreach (var hit in Physics.RaycastAll(origin, delta.normalized, delta.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform.IsChildOf(ally.transform)) continue;
            if (hit.collider.GetComponentInParent<IEnemyCombatant>() != null) continue;
            if (hit.collider.GetComponentInParent<Health>() == targetHealth) continue;
            return false;
        }
        return true;
    }

    private void RefreshAura()
    {
        leaving.Clear();
        foreach (var ally in affected)
            if (!IsAuraActive || !CanReach(ally)) leaving.Add(ally);
        foreach (var ally in leaving)
        {
            if (ally != null) ally.Remove(this);
            affected.Remove(ally);
        }
        if (!IsAuraActive) return;
        foreach (var ally in allies)
            if (CanReach(ally))
            {
                ally.Apply(this, 1f + movementBonusPercent / 100f, 1f + attackSpeedBonusPercent / 100f);
                affected.Add(ally);
            }
    }

    private void ClearAura()
    {
        auraReady = false;
        foreach (var ally in affected) if (ally != null) ally.Remove(this);
        affected.Clear();
    }

    private void FollowAllies()
    {
        EnemyBuffReceiver anchor = null;
        float nearest = float.PositiveInfinity;
        foreach (var ally in allies)
        {
            if (!IsNearby(ally, followRange) || !ally.IsEngaged) continue;
            float distance = (ally.transform.position - transform.position).sqrMagnitude;
            if (distance < nearest) { anchor = ally; nearest = distance; }
        }
        if (anchor == null) { Stop(); return; }
        Vector3 away = Vector3.ProjectOnPlane(anchor.transform.position - target.position, Vector3.up).normalized;
        if (away.sqrMagnitude < 0.01f) away = -transform.forward;
        Vector3 desired = anchor.transform.position + away * 2.5f;
        if (Vector3.ProjectOnPlane(desired - transform.position, Vector3.up).sqrMagnitude < 1.5f * 1.5f) { Stop(); return; }
        if (!MoveTo(desired)) Stop();
    }

    private bool Retreat()
    {
        Vector3 away = Vector3.ProjectOnPlane(transform.position - target.position, Vector3.up).normalized;
        if (away.sqrMagnitude < 0.01f) away = -transform.forward;
        foreach (float angle in new[] { 0f, 45f, -45f, 90f, -90f })
        {
            Vector3 desired = transform.position + Quaternion.Euler(0, angle, 0) * away * 3f;
            if (Vector3.ProjectOnPlane(desired - target.position, Vector3.up).sqrMagnitude <= retreatRange * retreatRange) continue;
            if (MoveTo(desired)) return true;
        }
        return false;
    }

    private bool MoveTo(Vector3 desired)
    {
        if (!NavMesh.SamplePosition(desired - Vector3.up * agent.baseOffset, out var sample, 1.5f, agent.areaMask)) return false;
        var path = new NavMeshPath();
        if (!agent.CalculatePath(sample.position, path) || path.status != NavMeshPathStatus.PathComplete) return false;
        agent.stoppingDistance = 0.4f;
        agent.isStopped = false;
        agent.SetPath(path);
        return true;
    }

    private void Stop() { agent.isStopped = true; if (agent.hasPath) agent.ResetPath(); }
    private void CancelSpells() { CastingSpell = SupportSpell.None; spellPending = false; healingTarget = null; }
    private void OnDamaged(int amount) => provoked = true;
    private void OnDied()
    {
        CancelSpells(); ClearAura();
        if (agent.enabled && agent.isOnNavMesh) Stop();
        agent.enabled = false;
        foreach (var collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.35f, 1f, 0.4f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, supportRange);
    }
}
