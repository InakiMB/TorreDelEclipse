using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
[DefaultExecutionOrder(300)]
public sealed class PlayerInteractor : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float interactionRange = 1.8f;
    private Health health;
    public string CurrentPrompt { get; private set; } = string.Empty;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnDisable()
    {
        CurrentPrompt = string.Empty;
    }

    private void LateUpdate()
    {
        CurrentPrompt = string.Empty;
        if (health.IsDead || Time.deltaTime <= 0f) return;

        IInteractable closest = null;
        float closestDistance = float.PositiveInfinity;
        Collider[] candidates = Physics.OverlapSphere(
            transform.position, interactionRange,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        foreach (Collider candidate in candidates)
        {
            if (candidate.transform.IsChildOf(transform)) continue;
            IInteractable interactable = candidate.GetComponentInParent<IInteractable>();
            if (interactable == null || !interactable.CanInteract(gameObject)) continue;
            Component component = interactable as Component;
            if (component == null) continue;
            float distance = (candidate.ClosestPoint(transform.position) - transform.position).sqrMagnitude;
            if (distance >= closestDistance || !CanReach(candidate.bounds.center, component.transform)) continue;
            closest = interactable;
            closestDistance = distance;
        }

        if (closest == null) return;
        CurrentPrompt = "E: " + closest.Prompt;
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            closest.Interact(gameObject);
            CurrentPrompt = string.Empty;
        }
    }

    private bool CanReach(Vector3 destination, Transform item)
    {
        Vector3 delta = destination - transform.position;
        if (delta.sqrMagnitude < 0.0001f) return true;
        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, delta.normalized, delta.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform.IsChildOf(item)) continue;
            return false;
        }
        return true;
    }
}
