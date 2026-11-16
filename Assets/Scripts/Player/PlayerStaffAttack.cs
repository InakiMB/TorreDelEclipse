using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
[DefaultExecutionOrder(240)]
public sealed class PlayerStaffAttack : MonoBehaviour
{
    [SerializeField] private Transform strikeOrigin = null;
    [SerializeField] private Animator visualAnimator = null;
    [SerializeField, Min(1)] private int damage = 50;
    [SerializeField, Min(0.1f)] private float reach = 1.8f;
    [SerializeField, Range(1f, 180f)] private float arcAngle = 100f;
    [SerializeField, Min(0f)] private float windUp = 0.15f;
    [SerializeField, Min(0.1f)] private float attackInterval = 0.8f;

    private static readonly int AttackParameter = Animator.StringToHash("StaffAttack");
    private readonly HashSet<IDamageable> struckTargets = new HashSet<IDamageable>();
    private Health health;
    private Camera viewCamera;
    private float impactAt;
    private float readyAt;
    private bool pendingImpact;
    private bool hasAttackTrigger;
    private PlayerDodge dodge;

    public bool IsAttacking => isActiveAndEnabled && Time.time < readyAt;
    public event Action Swung;
    public event Action Hit;

    private void Awake()
    {
        health = GetComponent<Health>();
        dodge = GetComponent<PlayerDodge>();
        viewCamera = Camera.main;
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualAnimator != null && visualAnimator.runtimeAnimatorController != null)
        {
            foreach (AnimatorControllerParameter parameter in visualAnimator.parameters)
            {
                if (parameter.nameHash == AttackParameter
                    && parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    hasAttackTrigger = true;
                    break;
                }
            }
        }
        if (!hasAttackTrigger)
        {
            Debug.LogWarning("Configura el Trigger StaffAttack en el Animator del mago.", this);
        }
    }

    private void OnDisable()
    {
        CancelAttack();
    }

    public void CancelAttack()
    {
        pendingImpact = false;
        if (hasAttackTrigger && visualAnimator != null) visualAnimator.ResetTrigger(AttackParameter);
    }

    private void LateUpdate()
    {
        if (EclipseGameUI.BlocksGameplay || health.IsDead || Time.deltaTime <= 0f || (dodge != null && dodge.IsBusy)) return;
        if (pendingImpact && Time.time >= impactAt)
        {
            pendingImpact = false;
            Strike();
        }

        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.rightButton.isPressed || Time.time < readyAt) return;
        if (viewCamera != null && !viewCamera.pixelRect.Contains(mouse.position.ReadValue())) return;

        readyAt = Time.time + Mathf.Max(attackInterval, windUp + 0.05f);
        impactAt = Time.time + windUp;
        pendingImpact = true;
        if (hasAttackTrigger) visualAnimator.SetTrigger(AttackParameter);
        Swung?.Invoke();
    }

    private void Strike()
    {
        Vector3 origin = strikeOrigin != null ? strikeOrigin.position : transform.position;
        Collider[] candidates = Physics.OverlapSphere(
            origin, reach, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        struckTargets.Clear();
        foreach (Collider candidate in candidates)
        {
            if (candidate.transform.IsChildOf(transform)) continue;
            IDamageable target = candidate.GetComponentInParent<IDamageable>();
            if (target == null || target.IsDead || struckTargets.Contains(target)) continue;

            Vector3 direction = candidate.bounds.center - origin;
            Vector3 planarDirection = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (planarDirection.sqrMagnitude > 0.0001f
                && Vector3.Angle(transform.forward, planarDirection) > arcAngle * 0.5f) continue;
            if (!HasClearPath(origin, candidate.bounds.center, target)) continue;

            struckTargets.Add(target);
            target.TakeDamage(damage);
            Hit?.Invoke();
        }
    }

    private bool HasClearPath(Vector3 origin, Vector3 destination, IDamageable target)
    {
        Vector3 delta = destination - origin;
        if (delta.sqrMagnitude < 0.0001f) return true;
        RaycastHit[] hits = Physics.RaycastAll(
            origin, delta.normalized, delta.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        Collider nearest = null;
        float distance = float.PositiveInfinity;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.distance >= distance) continue;
            nearest = hit.collider;
            distance = hit.distance;
        }
        return nearest == null || nearest.GetComponentInParent<IDamageable>() == target;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = strikeOrigin != null ? strikeOrigin.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, reach);
        Gizmos.DrawLine(origin, origin + Quaternion.AngleAxis(-arcAngle * 0.5f, Vector3.up) * transform.forward * reach);
        Gizmos.DrawLine(origin, origin + Quaternion.AngleAxis(arcAngle * 0.5f, Vector3.up) * transform.forward * reach);
    }
}
