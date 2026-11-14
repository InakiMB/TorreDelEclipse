using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyChaser), typeof(NavMeshAgent), typeof(Health))]
public sealed class WarriorAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;
    [SerializeField, Min(0.1f)] private float referenceRunSpeed = 2.8f;

    private EnemyChaser chaser;
    private NavMeshAgent agent;
    private Health health;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackSpeed = Animator.StringToHash("AttackSpeed");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private void Awake()
    {
        chaser = GetComponent<EnemyChaser>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualAnimator == null || visualAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("El guerrero necesita KnightVisual con su Animator Controller.", this);
            enabled = false;
            return;
        }
        visualAnimator.applyRootMotion = false;
    }

    private void OnEnable()
    {
        chaser.AttackStarted += OnAttack;
        health.Died += OnDeath;
    }

    private void OnDisable()
    {
        if (chaser != null) chaser.AttackStarted -= OnAttack;
        if (health != null) health.Died -= OnDeath;
    }

    private void LateUpdate()
    {
        if (visualAnimator == null || !visualAnimator.isActiveAndEnabled) return;
        float speed = !health.IsDead && !chaser.IsAttacking && agent.enabled && agent.isOnNavMesh
            && !agent.isStopped ? Vector3.ProjectOnPlane(agent.velocity, Vector3.up).magnitude : 0f;
        visualAnimator.SetFloat(Speed, speed);
        visualAnimator.SetFloat(RunSpeed, Mathf.Clamp(speed / referenceRunSpeed, 0.6f, 1.4f));
        visualAnimator.SetFloat(AttackSpeed, chaser.AttackPlaybackSpeed);
    }

    private void OnAttack()
    {
        if (visualAnimator != null && !health.IsDead)
        {
            visualAnimator.SetFloat(AttackSpeed, chaser.AttackPlaybackSpeed);
            visualAnimator.SetTrigger(Attack);
        }
    }

    private void OnDeath()
    {
        if (visualAnimator == null) return;
        visualAnimator.ResetTrigger(Attack);
        visualAnimator.SetFloat(Speed, 0f);
        visualAnimator.SetBool(Dead, true);
    }
}
