using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemySupport), typeof(Health), typeof(NavMeshAgent))]
public sealed class EnemySupportPresentation : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private Transform castOrb;
    [SerializeField] private Light castLight;
    [SerializeField] private LineRenderer healBeam;
    [SerializeField] private LineRenderer auraRing;
    [SerializeField] private ParticleSystem healBurst;
    private EnemySupport support;
    private Health health;
    private NavMeshAgent agent;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");
    private static readonly int Heal = Animator.StringToHash("Heal");
    private static readonly int Haste = Animator.StringToHash("Haste");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private void Awake()
    {
        support = GetComponent<EnemySupport>();
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualAnimator != null) visualAnimator.applyRootMotion = false;
        HideEffects();
    }

    private void OnEnable()
    {
        support.CastStarted += OnCast;
        support.AllyHealed += OnHealed;
        health.Died += OnDeath;
    }

    private void OnDisable()
    {
        support.CastStarted -= OnCast;
        support.AllyHealed -= OnHealed;
        health.Died -= OnDeath;
        HideEffects();
    }

    private void LateUpdate()
    {
        float speed = !health.IsDead && !support.IsCasting && agent.enabled && agent.isOnNavMesh && !agent.isStopped
            ? agent.velocity.magnitude : 0f;
        if (visualAnimator != null)
        {
            visualAnimator.SetFloat(Speed, speed);
            visualAnimator.SetFloat(RunSpeed, Mathf.Clamp(speed / 2.4f, 0.6f, 1.5f));
        }
        float charge = support.ChargeProgress;
        bool charging = !health.IsDead && charge > 0f;
        if (castOrb != null)
        {
            castOrb.gameObject.SetActive(charging);
            castOrb.localScale = Vector3.one * Mathf.Lerp(0.12f, 0.38f, charge);
        }
        if (castLight != null) castLight.intensity = charging ? charge * 2f : 0f;
        if (healBeam != null)
        {
            healBeam.enabled = charging && support.CastingSpell == SupportSpell.Heal && support.HealingTarget != null;
            if (healBeam.enabled)
            {
                healBeam.SetPosition(0, castOrb != null ? castOrb.position : transform.position);
                healBeam.SetPosition(1, support.HealingTarget.transform.position + Vector3.up * 0.2f);
            }
        }
        if (auraRing != null)
        {
            auraRing.enabled = support.IsAuraActive;
            float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.08f;
            auraRing.transform.localScale = Vector3.one * pulse;
        }
    }

    private void OnCast(SupportSpell spell)
    {
        if (visualAnimator != null) visualAnimator.SetTrigger(spell == SupportSpell.Heal ? Heal : Haste);
    }

    private void OnHealed(Health ally, int amount)
    {
        if (healBurst == null) return;
        healBurst.transform.position = ally.transform.position;
        healBurst.Play(true);
    }

    private void OnDeath()
    {
        if (visualAnimator != null)
        {
            visualAnimator.ResetTrigger(Heal);
            visualAnimator.ResetTrigger(Haste);
            visualAnimator.SetFloat(Speed, 0f);
            visualAnimator.SetBool(Dead, true);
        }
        HideEffects();
    }

    private void HideEffects()
    {
        if (castOrb != null) castOrb.gameObject.SetActive(false);
        if (castLight != null) castLight.intensity = 0f;
        if (healBeam != null) healBeam.enabled = false;
        if (auraRing != null) auraRing.enabled = false;
        if (healBurst != null) healBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
