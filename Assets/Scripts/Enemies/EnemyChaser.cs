using System;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemyChaser : MonoBehaviour, IEnemyCombatant
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float detectionRange = 10f;
    [SerializeField, Min(0.1f)] private float attackRange = 1.4f;
    [SerializeField, Min(1)] private int attackDamage = 10;
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;
    [SerializeField] private EventReference attackEvent = default;
    [SerializeField, Min(0f)] private float attackWindup = 0f;
    [SerializeField, Min(0f)] private float attackRecovery = 0f;
    [SerializeField, Min(0f)] private float destroyDelay = 0f;

    public event Action AttackStarted;
    public bool IsAttacking { get; private set; }
    public float AttackPlaybackSpeed { get; private set; } = 1f;

    private Health health;
    private Health targetHealth;
    private CharacterController targetController;
    private NavMeshAgent agent;
    private EnemyBuffReceiver buffs;
    private float nextAttackTime;
    private bool provoked;
    private float strikeTime;
    private float recoveryEnd;
    private bool strikePending;
    public bool IsEngaged => isActiveAndEnabled && health != null && !health.IsDead
        && agent != null && agent.enabled && agent.isOnNavMesh
        && target != null && targetHealth != null && !targetHealth.IsDead
        && (provoked || (target.position - transform.position).sqrMagnitude <= detectionRange * detectionRange);

    private void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        buffs = GetComponent<EnemyBuffReceiver>();
        if (buffs == null) buffs = gameObject.AddComponent<EnemyBuffReceiver>();
        agent.stoppingDistance = Mathf.Max(0.1f, attackRange - 0.15f);
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
        IsAttacking = false;
        strikePending = false;
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            targetHealth = target.GetComponent<Health>();
            targetController = target.GetComponent<CharacterController>();
        }
        if (targetHealth == null)
        {
            Debug.LogError("EnemyChaser necesita como Target al Player con Health.", this);
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("El enemigo debe estar sobre un NavMesh horneado.", this);
        }
    }

    private void Update()
    {
        if (health.IsDead || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        if (IsAttacking)
        {
            StopAgent();
            if (strikePending && Time.time >= strikeTime)
            {
                strikePending = false;
                Strike();
            }
            if (Time.time >= recoveryEnd) IsAttacking = false;
            return;
        }

        if (target == null || targetHealth == null || targetHealth.IsDead)
        {
            StopAgent();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (!provoked && distance > detectionRange)
        {
            StopAgent();
            return;
        }

        if (distance <= attackRange && CanStrikeTarget())
        {
            StopAgent();
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, Quaternion.LookRotation(direction), 540f * Time.deltaTime);
            }

            if (Time.time >= nextAttackTime)
            {
                // Fijar el ritmo de este golpe mantiene sincronizados impacto y animacion.
                AttackPlaybackSpeed = buffs.AttackSpeedMultiplier;
                IsAttacking = attackWindup + attackRecovery > 0f;
                strikePending = attackWindup > 0f;
                strikeTime = Time.time + attackWindup / AttackPlaybackSpeed;
                recoveryEnd = strikeTime + attackRecovery / AttackPlaybackSpeed;
                nextAttackTime = Time.time + Mathf.Max(attackInterval, attackWindup + attackRecovery) / AttackPlaybackSpeed;
                AttackStarted?.Invoke();
                if (!strikePending) Strike();
            }
        }
        else
        {
            agent.isStopped = false;
            Vector3 destination = target.position;
            if (targetController != null)
            {
                destination = target.TransformPoint(targetController.center)
                    - Vector3.up * targetController.height * 0.5f * Mathf.Abs(target.lossyScale.y);
            }
            agent.SetDestination(destination);
        }
    }

    private void Strike()
    {
        // El tajo suena incluso si el jugador esquiva durante la preparacion.
        FmodOneShot.Play(attackEvent, transform.position);
        if (target == null || targetHealth == null || targetHealth.IsDead) return;
        Vector3 delta = target.position - transform.position;
        if (delta.sqrMagnitude > attackRange * attackRange) return;
        delta.y = 0f;
        if (attackWindup > 0f && delta.sqrMagnitude > 0.01f
            && Vector3.Angle(transform.forward, delta) > 75f) return;
        if (CanStrikeTarget()) targetHealth.TakeDamage(attackDamage);
    }

    private bool CanStrikeTarget()
    {
        Vector3 delta = target.position - transform.position;
        if (delta.sqrMagnitude < 0.01f) return true;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, delta.normalized, delta.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        float nearestDistance = float.PositiveInfinity;
        Health nearestHealth = null;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                nearestHealth = hit.collider.GetComponentInParent<Health>();
            }
        }

        return nearestHealth == targetHealth;
    }

    private void StopAgent()
    {
        agent.isStopped = true;
        if (agent.hasPath) agent.ResetPath();
    }

    private void OnDamaged(int amount)
    {
        provoked = true;
    }

    private void OnDied()
    {
        IsAttacking = false;
        strikePending = false;
        if (agent.enabled && agent.isOnNavMesh) StopAgent();
        agent.enabled = false;
        if (destroyDelay > 0f)
            foreach (Collider collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        Destroy(gameObject, destroyDelay);
    }
}
