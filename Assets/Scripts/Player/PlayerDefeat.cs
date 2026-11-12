using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class PlayerDefeat : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;

    private Health health;
    private PlayerMovement movement;
    private PlayerAim aim;
    private PlayerSpellCaster spellCaster;
    private PlayerAnimation animationController;

    private void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<PlayerMovement>();
        aim = GetComponent<PlayerAim>();
        spellCaster = GetComponent<PlayerSpellCaster>();
        animationController = GetComponent<PlayerAnimation>();
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void OnDied()
    {
        if (movement != null) movement.enabled = false;
        if (aim != null) aim.enabled = false;
        if (spellCaster != null) spellCaster.enabled = false;
        if (animationController != null) animationController.enabled = false;

        if (visualAnimator == null) return;
        visualAnimator.SetFloat("Speed", 0f);
        foreach (AnimatorControllerParameter parameter in visualAnimator.parameters)
        {
            if (parameter.name == "Die" && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                visualAnimator.SetTrigger("Die");
                return;
            }
        }

        visualAnimator.enabled = false;
    }
}
