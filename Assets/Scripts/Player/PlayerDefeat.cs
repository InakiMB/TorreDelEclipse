using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public sealed class PlayerDefeat : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator = null;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
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
        PlayerControlLock.Freeze(gameObject);
        if (visualAnimator == null) return;
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
