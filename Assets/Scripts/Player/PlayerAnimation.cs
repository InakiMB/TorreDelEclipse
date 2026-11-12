using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
[DefaultExecutionOrder(50)]
public sealed class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator visualAnimator;

    private static readonly int SpeedParameter = Animator.StringToHash("Speed");
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (visualAnimator == null)
        {
            visualAnimator = GetComponentInChildren<Animator>();
        }

        if (visualAnimator == null)
        {
            Debug.LogError("PlayerAnimation necesita el Animator del mago.", this);
            enabled = false;
            return;
        }

        // La posicion del jugador la controla PlayerMovement.
        visualAnimator.applyRootMotion = false;
    }

    private void Update()
    {
        if (Time.deltaTime <= 0f || !visualAnimator.isActiveAndEnabled
            || visualAnimator.runtimeAnimatorController == null)
        {
            return;
        }

        Vector3 planarVelocity = characterController.velocity;
        planarVelocity.y = 0f;
        visualAnimator.SetFloat(SpeedParameter, planarVelocity.magnitude);
    }
}
