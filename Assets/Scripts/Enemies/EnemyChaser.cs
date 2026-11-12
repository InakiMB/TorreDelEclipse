using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemyChaser : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float detectionRange = 10f;
    [SerializeField, Min(0.1f)] private float attackRange = 1.4f;
    [SerializeField, Min(1)] private int attackDamage = 10;
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;

    private Health health;
    private Health targetHealth;
    private CharacterController targetController;
    private NavMeshAgent agent;
    private float nextAttackTime;
    private bool provoked;

    private void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
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
                targetHealth.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackInterval;
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
        if (agent.enabled && agent.isOnNavMesh) StopAgent();
        agent.enabled = false;
        Destroy(gameObject);
    }
}
