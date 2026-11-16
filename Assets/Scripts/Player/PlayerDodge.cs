using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Health), typeof(CharacterController), typeof(PlayerMovement))]
public sealed class PlayerDodge : MonoBehaviour, IDamageGuard
{
    [SerializeField, Min(0.1f)] private float rollDistance = 3.2f;
    [SerializeField, Min(0.1f)] private float rollDuration = 0.5f;
    [SerializeField, Min(0f)] private float recoveryTime = 0.15f;
    [SerializeField, Min(0f)] private float cooldown = 0.95f;
    [SerializeField, Min(0f)] private float invulnerabilityStart = 0.04f;
    [SerializeField, Min(0f)] private float invulnerabilityEnd = 0.3f;

    private Health health;
    private CharacterController controller;
    private PlayerMovement movement;
    private PlayerStaffAttack staff;
    private bool active;
    private bool motionBlocked;
    private bool landingNotified;
    private bool rollKeyHeld;
    private float startedAt;
    private float rollEndsAt;
    private float recoveryEndsAt;
    private float nextReadyAt;
    private float activeDistance;
    private float consumedDistance;
    private float activeInvulnerabilityStart;
    private float activeInvulnerabilityEnd;

    public Vector3 Direction { get; private set; }
    public float ActiveDuration { get; private set; }
    public bool IsRolling => active && Time.time < rollEndsAt;
    public bool IsBusy => active && Time.time < recoveryEndsAt;
    public float CooldownRemaining => Mathf.Max(0f, nextReadyAt - Time.time);
    public float CooldownProgress => 1f - Mathf.Clamp01(CooldownRemaining / Mathf.Max(cooldown, rollDuration + recoveryTime, 0.01f));
    public bool PreventsDamage => isActiveAndEnabled && IsRolling && health != null && !health.IsDead
        && Time.time >= activeInvulnerabilityStart && Time.time < activeInvulnerabilityEnd;
    public event Action Started;
    public event Action MotionFinished;
    public event Action Finished;

    private void Awake()
    {
        health = GetComponent<Health>();
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        staff = GetComponent<PlayerStaffAttack>();
    }

    private void OnEnable()
    {
        health.Died += Cancel;
        rollKeyHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
    }
    private void OnDisable()
    {
        if (health != null) health.Died -= Cancel;
        Cancel();
    }

    private void Update()
    {
        if (active && (health.IsDead || !controller.enabled || !movement.isActiveAndEnabled)) Cancel();
        if (active && !landingNotified && Time.time >= rollEndsAt)
        {
            landingNotified = true;
            MotionFinished?.Invoke();
        }
        if (active && Time.time >= recoveryEndsAt)
        {
            active = false;
            Finished?.Invoke();
        }
        bool held = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        bool pressed = held && !rollKeyHeld;
        rollKeyHeld = held;
        if (pressed) TryRoll(movement.ReadDirection());
    }

    public bool TryRoll(Vector3 direction)
    {
        if (EclipseGameUI.BlocksGameplay || !isActiveAndEnabled || health.IsDead || !controller.enabled || !controller.isGrounded
            || !movement.isActiveAndEnabled || Time.timeScale <= 0f || IsBusy || Time.time < nextReadyAt) return false;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
        if (direction.sqrMagnitude < 0.001f) direction = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (direction.sqrMagnitude < 0.001f) return false;
        Direction = direction.normalized;
        ActiveDuration = Mathf.Max(0.1f, rollDuration);
        activeDistance = Mathf.Max(0.1f, rollDistance);
        startedAt = Time.time;
        rollEndsAt = startedAt + ActiveDuration;
        recoveryEndsAt = rollEndsAt + Mathf.Max(0f, recoveryTime);
        nextReadyAt = startedAt + Mathf.Max(cooldown, ActiveDuration + recoveryTime);
        float safeStart = Mathf.Clamp(invulnerabilityStart, 0f, ActiveDuration);
        activeInvulnerabilityStart = startedAt + safeStart;
        activeInvulnerabilityEnd = startedAt + Mathf.Clamp(invulnerabilityEnd, safeStart, ActiveDuration);
        consumedDistance = 0f;
        motionBlocked = false;
        landingNotified = false;
        active = true;
        transform.rotation = Quaternion.LookRotation(Direction);
        if (staff != null) staff.CancelAttack();
        Started?.Invoke();
        return true;
    }

    // PlayerMovement sigue siendo el unico que llama a CharacterController.Move.
    public Vector3 ConsumeDisplacement(float deltaTime)
    {
        if (!active || deltaTime <= 0f || motionBlocked) return Vector3.zero;
        float progress = Mathf.Clamp01((Time.time - startedAt + deltaTime) / ActiveDuration);
        float total = activeDistance * (1f - (1f - progress) * (1f - progress));
        float step = Mathf.Max(0f, total - consumedDistance);
        consumedDistance = total;
        return Direction * step;
    }

    public void StopAtObstacle() => motionBlocked = true;

    public void RestoreCheckpoint() { Cancel(); nextReadyAt = 0f; }

    private void Cancel()
    {
        bool wasActive = active;
        active = false;
        motionBlocked = true;
        if (wasActive) Finished?.Invoke();
    }
}
