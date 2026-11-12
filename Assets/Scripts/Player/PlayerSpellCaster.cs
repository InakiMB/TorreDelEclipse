using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
[DefaultExecutionOrder(250)]
public sealed class PlayerSpellCaster : MonoBehaviour
{
    [SerializeField] private MagicProjectile projectilePrefab = null;
    [SerializeField] private Transform castOrigin = null;
    [SerializeField, Min(1)] private int damage = 25;
    [SerializeField, Min(0.05f)] private float fireInterval = 0.35f;
    private Health health;
    private PlayerStaffAttack staffAttack;
    private Camera viewCamera;
    private float nextShotTime;

    public event Action ShotFired;

    private void Awake()
    {
        health = GetComponent<Health>();
        staffAttack = GetComponent<PlayerStaffAttack>();
        viewCamera = Camera.main;
        if (projectilePrefab == null || castOrigin == null)
        {
            Debug.LogError("PlayerSpellCaster necesita Projectile Prefab y Cast Origin.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        Mouse mouse = Mouse.current;
        if (health.IsDead || mouse == null || Time.deltaTime <= 0f
            || !mouse.leftButton.isPressed || Time.time < nextShotTime
            || (staffAttack != null && staffAttack.IsAttacking)) return;
        if (viewCamera != null && !viewCamera.pixelRect.Contains(mouse.position.ReadValue())) return;

        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();
        MagicProjectile projectile = Instantiate(
            projectilePrefab, castOrigin.position, Quaternion.LookRotation(direction));
        projectile.Initialize(transform, direction, damage);
        nextShotTime = Time.time + fireInterval;
        ShotFired?.Invoke();
    }
}
