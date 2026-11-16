using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health), typeof(CharacterController))]
[DefaultExecutionOrder(350)]
public sealed class PlayerActionAudio : MonoBehaviour
{
    [SerializeField] private EventReference footstepEvent = default;
    [SerializeField] private EventReference spellEvent = default;
    [SerializeField] private EventReference staffSwingEvent = default;
    [SerializeField] private EventReference staffImpactEvent = default;
    [SerializeField] private EventReference pickupEvent = default;
    [SerializeField] private EventReference healEvent = default;
    [SerializeField, Min(0.1f)] private float stepInterval = 0.38f;

    private Health health;
    private CharacterController controller;
    private PlayerMovement movement;
    private PlayerSpellCaster caster;
    private PlayerStaffAttack staff;
    private PlayerInventory inventory;
    private float nextStepAt;
    private PlayerDodge dodge;

    private void Awake()
    {
        health = GetComponent<Health>();
        dodge = GetComponent<PlayerDodge>();
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        caster = GetComponent<PlayerSpellCaster>();
        staff = GetComponent<PlayerStaffAttack>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (caster != null) caster.ShotFired += OnSpell;
        if (staff != null)
        {
            staff.Swung += OnSwing;
            staff.Hit += OnStaffHit;
        }
        if (inventory != null)
        {
            inventory.PotionCollected += OnPickup;
            inventory.PotionUsed += OnHeal;
        }
    }

    private void OnDisable()
    {
        if (caster != null) caster.ShotFired -= OnSpell;
        if (staff != null)
        {
            staff.Swung -= OnSwing;
            staff.Hit -= OnStaffHit;
        }
        if (inventory != null)
        {
            inventory.PotionCollected -= OnPickup;
            inventory.PotionUsed -= OnHeal;
        }
    }

    private void LateUpdate()
    {
        if (Time.deltaTime <= 0f) return;
        Vector3 velocity = controller.velocity;
        velocity.y = 0f;
        bool walking = !health.IsDead && controller.enabled && controller.isGrounded
            && (dodge == null || !dodge.IsBusy)
            && movement != null && movement.isActiveAndEnabled && velocity.sqrMagnitude > 0.04f;
        if (!walking)
        {
            nextStepAt = Time.time;
            return;
        }
        if (Time.time < nextStepAt) return;
        FmodOneShot.Play(footstepEvent, transform.position);
        nextStepAt = Time.time + stepInterval;
    }

    private void OnSpell() => FmodOneShot.Play(spellEvent, transform.position);
    private void OnSwing() => FmodOneShot.Play(staffSwingEvent, transform.position);
    private void OnStaffHit() => FmodOneShot.Play(staffImpactEvent, transform.position);
    private void OnPickup() => FmodOneShot.Play(pickupEvent, transform.position);
    private void OnHeal(int restored) => FmodOneShot.Play(healEvent, transform.position);
}
