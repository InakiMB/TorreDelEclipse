using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private Camera viewCamera = null;

    private CharacterController characterController;
    private float verticalSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (viewCamera == null) viewCamera = Camera.main;

        if (viewCamera == null)
        {
            Debug.LogError("PlayerMovement necesita una View Camera en el Inspector.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        verticalSpeed = 0f;
    }

    private void Update()
    {
        if (!characterController.enabled || viewCamera == null || Time.deltaTime <= 0f)
        {
            return;
        }

        Vector2 input = ReadMovement();

        // Los ejes de movimiento pertenecen a la vista, independientemente del apuntado.
        Vector3 forward = Vector3.ProjectOnPlane(viewCamera.transform.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.ProjectOnPlane(viewCamera.transform.up, Vector3.up);
        }
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 velocity = (right * input.x + forward * input.y) * moveSpeed;

        if (characterController.isGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }

        verticalSpeed += gravity * Time.deltaTime;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    private static Vector2 ReadMovement()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;

        return Vector2.ClampMagnitude(input, 1f);
    }
}
