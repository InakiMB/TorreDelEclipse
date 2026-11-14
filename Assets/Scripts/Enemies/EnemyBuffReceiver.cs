using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Conserva la velocidad original y combina apoyos sin multiplicar sus efectos.
[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemyBuffReceiver : MonoBehaviour
{
    [SerializeField] private GameObject hasteMarker;
    private readonly Dictionary<EnemySupport, Vector2> sources = new Dictionary<EnemySupport, Vector2>();
    private readonly List<EnemySupport> expired = new List<EnemySupport>();
    private NavMeshAgent agent;
    private Health health;
    private float originalSpeed;

    public float MovementMultiplier { get; private set; } = 1f;
    public float AttackSpeedMultiplier { get; private set; } = 1f;
    public bool IsBuffed => MovementMultiplier > 1f || AttackSpeedMultiplier > 1f;
    public bool IsAlive => isActiveAndEnabled && health != null && !health.IsDead;
    public Health Health => health;
    public bool IsEngaged => GetComponent<IEnemyCombatant>()?.IsEngaged == true;

    private void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        if (hasteMarker != null) hasteMarker.SetActive(false);
    }

    private void OnEnable() => health.Died += Clear;
    private void OnDisable()
    {
        if (health != null) health.Died -= Clear;
        Clear();
    }

    public void Apply(EnemySupport source, float movement, float attack)
    {
        if (source == null || !IsAlive) return;
        sources[source] = new Vector2(Mathf.Max(1f, movement), Mathf.Max(1f, attack));
        Recalculate();
    }

    public void Remove(EnemySupport source)
    {
        if (sources.Remove(source)) Recalculate();
    }

    private void Update()
    {
        expired.Clear();
        foreach (var source in sources.Keys)
            if (source == null || !source.IsAuraActive) expired.Add(source);
        foreach (var source in expired) sources.Remove(source);
        if (expired.Count > 0) Recalculate();
    }

    private void Clear()
    {
        sources.Clear();
        Recalculate();
    }

    private void Recalculate()
    {
        float movement = 1f, attack = 1f;
        foreach (var value in sources.Values)
        {
            movement = Mathf.Max(movement, value.x);
            attack = Mathf.Max(attack, value.y);
        }
        MovementMultiplier = movement;
        AttackSpeedMultiplier = attack;
        if (agent != null) agent.speed = originalSpeed * movement;
        if (hasteMarker != null) hasteMarker.SetActive(IsAlive && IsBuffed);
    }
}
