using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossAction { Dormant, Summon, Hunt, Combo, Slam, Charge, Transform, Fan, Rings, Spiral, Leap, Dying, Dead, Defeated }
public enum BossCue { Spawn, Windup, Swing, Slam, Charge, Cast, Wings, Transform, Hurt, Death, OrbImpact, Step }

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class BossController : MonoBehaviour, IEnemyCombatant, IDamageGuard
{
    [SerializeField] private Health target;
    [SerializeField] private BossProjectile projectilePrefab;
    [SerializeField, Min(0.1f)] private float moveSpeed = 3.8f;
    [SerializeField, Min(1)] private int normalDamage = 24;
    [SerializeField, Min(1)] private int heavyDamage = 35;
    [SerializeField, Min(1)] private int chargeDamage = 30;
    [SerializeField, Min(1)] private int projectileDamage = 11;
    [SerializeField, Min(0.1f)] private float meleeRange = 4f;
    [SerializeField, Min(0.1f)] private float slamRadius = 2.8f;
    [SerializeField, Min(0.1f)] private float chargeSpeed = 12f;
    [SerializeField, Min(0.1f)] private float targetHitInterval = 0.2f;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 6f;
    [SerializeField, Min(0.1f)] private float summonDuration = 3.2f;
    [SerializeField, Min(0.1f)] private float transformDuration = 2.8f;
    private Health health;
    private NavMeshAgent agent;
    private float since, duration, nextAttackAt, nextSpecialAt, nextPlayerHitAt, nextStepAt;
    private int hits, volleys, sequence;
    private bool locked;
    private Vector3 attackDirection, attackPoint, leapStart, leapEnd;
    private readonly HashSet<BossProjectile> projectiles = new HashSet<BossProjectile>();
    public BossAction Action { get; private set; } = BossAction.Dormant;
    public int Phase { get; private set; } = 1;
    public Health Health => health;
    public Health Target => target;
    public Vector3 AttackPoint => attackPoint;
    public Vector3 AttackDirection => attackDirection;
    public float ActionDuration => duration;
    public float ActionElapsed => Mathf.Max(0, Time.time - since);
    public float ActionProgress => duration > 0 ? Mathf.Clamp01(ActionElapsed / duration) : 0;
    public float SlamRadius => slamRadius;
    public float MoveSpeed => agent != null && agent.enabled ? agent.velocity.magnitude : 0f;
    public int ActiveProjectiles => projectiles.Count;
    public bool IsEngaged => isActiveAndEnabled && !health.IsDead && Action != BossAction.Dormant
        && Action != BossAction.Dead && Action != BossAction.Defeated && target != null && !target.IsDead;
    public bool PreventsDamage => Action == BossAction.Dormant || Action == BossAction.Summon
        || Action == BossAction.Transform || Action == BossAction.Dying || Action == BossAction.Dead || Action == BossAction.Defeated;
    public event System.Action<BossAction> ActionChanged;
    public event System.Action<BossCue, Vector3> Cue;
    public event System.Action DeathFinished;

    private void Awake() { health = GetComponent<Health>(); agent = GetComponent<NavMeshAgent>(); }
    private void OnEnable() { health.Died += OnDied; health.Damaged += OnHurt; }
    private void OnDisable() { health.Died -= OnDied; health.Damaged -= OnHurt; ClearProjectiles(); }
    public void Begin(Health player, bool retry)
    {
        target = player; Phase = 1; sequence = 0; nextPlayerHitAt = 0;
        SetAction(BossAction.Summon, retry ? 1.2f : summonDuration); Emit(BossCue.Spawn);
    }
    private void Update()
    {
        if (Action == BossAction.Dormant || Action == BossAction.Dead || Action == BossAction.Defeated) return;
        if (Action == BossAction.Dying)
        {
            if (ActionElapsed >= duration) { SetAction(BossAction.Dead); DeathFinished?.Invoke(); }
            return;
        }
        if (target == null || target.IsDead) { Abort(); return; }
        if (Phase == 1 && !health.IsDead && health.CurrentHealth <= health.MaxHealth * 0.5f && Action != BossAction.Summon)
        {
            EndLeap(); Phase = 2; ClearProjectiles(); SetAction(BossAction.Transform, transformDuration); Emit(BossCue.Transform); return;
        }
        if (Action == BossAction.Summon || Action == BossAction.Transform)
        {
            if (ActionElapsed >= duration)
            {
                EnableAgent(); SetAction(BossAction.Hunt); nextAttackAt = Time.time + 0.65f;
                if (Phase == 2) Emit(BossCue.Wings);
            }
            return;
        }
        if (Action == BossAction.Hunt) { Hunt(); return; }
        TickAttack();
        if (ActionElapsed >= duration && Action != BossAction.Hunt)
        {
            EndLeap(); SetAction(BossAction.Hunt); nextAttackAt = Time.time + 0.55f;
        }
    }
    private void Hunt()
    {
        if (!agent.enabled || !agent.isOnNavMesh) { EnableAgent(); return; }
        Vector3 delta = Flat(target.transform.position - transform.position); float distance = delta.magnitude;
        agent.speed = moveSpeed; agent.stoppingDistance = meleeRange - 0.4f;
        agent.isStopped = false; agent.SetDestination(target.transform.position);
        if (agent.velocity.sqrMagnitude > 0.2f && Time.time >= nextStepAt) { Emit(BossCue.Step); nextStepAt = Time.time + 0.6f; }
        if (Time.time < nextAttackAt) return;
        if (Phase == 2 && sequence % 2 == 1)
        {
            BossAction pattern = (sequence / 2) % 3 == 0 ? BossAction.Fan : (sequence / 2) % 3 == 1 ? BossAction.Rings : BossAction.Spiral;
            StartAttack(pattern); return;
        }
        if (distance > 6f && Time.time >= nextSpecialAt) { StartAttack(Phase == 2 && sequence % 3 == 0 ? BossAction.Leap : BossAction.Charge); nextSpecialAt = Time.time + 5f; }
        else if (distance <= meleeRange + 0.3f) StartAttack(sequence % 3 == 1 ? BossAction.Slam : BossAction.Combo);
    }
    public bool StartAttack(BossAction attack)
    {
        if (health.IsDead || target == null || target.IsDead || (Action != BossAction.Hunt && Action != BossAction.Dormant)) return false;
        if (Phase == 1 && (attack == BossAction.Fan || attack == BossAction.Rings || attack == BossAction.Spiral || attack == BossAction.Leap)) return false;
        StopAgent(); attackPoint = target.transform.position; attackDirection = DirectionToTarget(); Face(attackDirection);
        float length = attack == BossAction.Combo ? 2.4f : attack == BossAction.Slam ? 2.4f
            : attack == BossAction.Charge ? 2.0f : attack == BossAction.Spiral ? 3.5f : attack == BossAction.Leap ? 2.8f : 2.5f;
        SetAction(attack, length); sequence++; Emit(BossCue.Windup); return true;
    }
    private void TickAttack()
    {
        float elapsed = ActionElapsed;
        float lockAt = Action == BossAction.Charge ? 0.55f : Action == BossAction.Leap ? 0.65f : 0.7f;
        if (!locked && elapsed < lockAt)
        {
            attackPoint = target.transform.position; attackDirection = DirectionToTarget(); Face(attackDirection);
        }
        else locked = true;
        if (Action == BossAction.Combo)
        {
            if (hits == 0 && elapsed >= 0.65f) { hits++; Melee(normalDamage, 145f); }
            if (hits == 1 && elapsed >= 1.65f) { hits++; Melee(normalDamage, 165f); }
        }
        else if (Action == BossAction.Slam && hits == 0 && elapsed >= 1.4f)
        {
            hits++; Emit(BossCue.Slam, attackPoint); AreaDamage(attackPoint, slamRadius, heavyDamage);
        }
        else if (Action == BossAction.Charge)
        {
            if (elapsed >= 0.85f && elapsed < 1.4f)
            {
                if (volleys++ == 0) Emit(BossCue.Charge);
                MoveCharge(attackDirection * chargeSpeed * Time.deltaTime);
                if (hits == 0 && Flat(target.transform.position - transform.position).magnitude <= 2.2f)
                { hits++; TryDamagePlayer(chargeDamage); Emit(BossCue.Swing); }
            }
        }
        else if (Action == BossAction.Fan)
        {
            if (volleys < 3 && elapsed >= 0.7f + volleys * 0.4f)
            { volleys++; Fan(DirectionToTarget(), 7, 100f); Emit(BossCue.Cast); }
        }
        else if (Action == BossAction.Rings)
        {
            if (volleys < 2 && elapsed >= 0.8f + volleys * 0.55f)
            { Ring(16, volleys * 11.25f); volleys++; Emit(BossCue.Cast); }
        }
        else if (Action == BossAction.Spiral)
        {
            if (volleys < 18 && elapsed >= 0.7f + volleys * 0.12f)
            {
                float yaw = volleys * 19f; SpawnOrb(Quaternion.Euler(0, yaw, 0) * Vector3.forward, 0.85f);
                SpawnOrb(Quaternion.Euler(0, yaw + 180, 0) * Vector3.forward, 0.85f);
                if (volleys % 4 == 0) Emit(BossCue.Cast); volleys++;
            }
        }
        else if (Action == BossAction.Leap)
        {
            if (elapsed >= 0.95f && hits == 0)
            {
                hits = 1; leapStart = transform.position; leapEnd = attackPoint;
                if (NavMesh.SamplePosition(attackPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas)) leapEnd = hit.position + Vector3.up * agent.baseOffset;
                else leapEnd = leapStart;
                attackPoint = leapEnd;
                agent.enabled = false; Emit(BossCue.Wings);
            }
            if (hits == 1)
            {
                float t = Mathf.Clamp01((elapsed - 0.95f) / 0.65f);
                transform.position = Vector3.Lerp(leapStart, leapEnd, t) + Vector3.up * (Mathf.Sin(t * Mathf.PI) * 2.7f);
                if (t >= 1f) { hits = 2; EndLeap(); AreaDamage(transform.position, slamRadius, heavyDamage); Ring(16, 0); Emit(BossCue.Slam); }
            }
        }
    }
    private void Melee(int damage, float arc)
    {
        Emit(BossCue.Swing);
        Vector3 delta = Flat(target.transform.position - transform.position);
        if (delta.magnitude <= meleeRange && Vector3.Angle(attackDirection, delta) <= arc * 0.5f && ClearPath(target.transform.position)) TryDamagePlayer(damage);
    }
    private void AreaDamage(Vector3 center, float radius, int damage)
    {
        if (Flat(target.transform.position - center).magnitude <= radius && ClearPath(target.transform.position)) TryDamagePlayer(damage);
    }
    public bool TryDamagePlayer(int damage)
    {
        if (!IsEngaged || PreventsDamage || Time.time < nextPlayerHitAt || target.IsDamageBlocked) return false;
        int before = target.CurrentHealth; target.TakeDamage(damage);
        if (target.CurrentHealth == before) return false;
        nextPlayerHitAt = Time.time + targetHitInterval; return true;
    }
    private bool ClearPath(Vector3 destination)
    {
        Vector3 origin = transform.position - Vector3.up * Mathf.Max(0, agent.baseOffset - 1.1f);
        Vector3 delta = destination - origin;
        foreach (RaycastHit hit in Physics.RaycastAll(origin, delta.normalized, delta.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            if (!hit.transform.IsChildOf(transform) && !hit.transform.IsChildOf(target.transform)) return false;
        return true;
    }
    private void MoveCharge(Vector3 delta)
    {
        if (!agent.enabled || !agent.isOnNavMesh) return;
        Vector3 start = agent.nextPosition - Vector3.up * agent.baseOffset;
        if (NavMesh.Raycast(start, start + delta + delta.normalized * 0.8f, out _, agent.areaMask)) return;
        agent.Move(delta);
    }
    private void EnableAgent()
    {
        if (!NavMesh.SamplePosition(transform.position - Vector3.up * agent.baseOffset, out NavMeshHit hit, 3f, NavMesh.AllAreas)) return;
        agent.enabled = true; agent.Warp(hit.position); agent.speed = moveSpeed; agent.updateRotation = true;
    }
    private void EndLeap()
    {
        if (Action == BossAction.Leap && hits > 0 && !agent.enabled) { transform.position = leapEnd; EnableAgent(); }
    }
    private void StopAgent() { if (agent.enabled && agent.isOnNavMesh) { agent.isStopped = true; agent.ResetPath(); } }
    private void SetAction(BossAction action, float seconds = 0)
    {
        Action = action; since = Time.time; duration = seconds; hits = 0; volleys = 0; locked = false;
        if (action != BossAction.Hunt) StopAgent();
        ActionChanged?.Invoke(action);
    }
    private Vector3 DirectionToTarget() { Vector3 delta = Flat(target.transform.position - transform.position); return delta.sqrMagnitude > 0.001f ? delta.normalized : transform.forward; }
    private static Vector3 Flat(Vector3 value) => Vector3.ProjectOnPlane(value, Vector3.up);
    private void Face(Vector3 direction) { if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(direction); }
    private void Fan(Vector3 direction, int count, float arc)
    { for (int i = 0; i < count; i++) SpawnOrb(Quaternion.Euler(0, -arc * 0.5f + arc * i / (count - 1), 0) * direction, 1f); }
    private void Ring(int count, float offset)
    { for (int i = 0; i < count; i++) SpawnOrb(Quaternion.Euler(0, offset + 360f * i / count, 0) * Vector3.forward, 0.85f); }
    private void SpawnOrb(Vector3 direction, float speedFactor)
    {
        if (projectilePrefab == null || projectiles.Count >= 100) return;
        Vector3 origin = transform.position - Vector3.up * Mathf.Max(0, agent.baseOffset - 1.1f) + direction * 1.2f;
        BossProjectile orb = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectiles.Add(orb); orb.Initialize(this, direction, projectileSpeed * speedFactor, projectileDamage);
    }
    public void ReleaseProjectile(BossProjectile projectile) => projectiles.Remove(projectile);
    public void ProjectileImpact(Vector3 point) => Emit(BossCue.OrbImpact, point);
    public void ClearProjectiles()
    {
        foreach (BossProjectile orb in new List<BossProjectile>(projectiles)) if (orb != null) Destroy(orb.gameObject);
        projectiles.Clear();
    }
    public void Abort() { EndLeap(); StopAgent(); ClearProjectiles(); SetAction(BossAction.Defeated); }
    private void OnHurt(int amount) => Emit(BossCue.Hurt);
    private void OnDied()
    {
        EndLeap(); StopAgent(); agent.enabled = false; ClearProjectiles();
        foreach (Collider collider in GetComponents<Collider>()) collider.enabled = false;
        SetAction(BossAction.Dying, 2.6f); Emit(BossCue.Death);
    }
    private void Emit(BossCue cue) => Emit(cue, transform.position);
    private void Emit(BossCue cue, Vector3 point) => Cue?.Invoke(cue, point);
}
