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
    private PlayerDodge dodge;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        dodge = GetComponent<PlayerDodge>();
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

        bool dodging = dodge != null && dodge.IsBusy;
        Vector3 displacement = dodging ? dodge.ConsumeDisplacement(Time.deltaTime) : ReadDirection() * moveSpeed * Time.deltaTime;

        if (characterController.isGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }

        verticalSpeed += gravity * Time.deltaTime;
        displacement.y = verticalSpeed * Time.deltaTime;
        CollisionFlags collisions = characterController.Move(displacement);
        if (dodging && (collisions & CollisionFlags.Sides) != 0) dodge.StopAtObstacle();
    }

    public Vector3 ReadDirection()
    {
        if (viewCamera == null) return Vector3.zero;
        Vector2 input = ReadMovement();
        // Los ejes pertenecen a la camara y no a la direccion del apuntado.
        Vector3 forward = Vector3.ProjectOnPlane(viewCamera.transform.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.ProjectOnPlane(viewCamera.transform.up, Vector3.up);
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        return right * input.x + forward * input.y;
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
