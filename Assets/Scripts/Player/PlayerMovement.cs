using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -25f;

    private CharacterController characterController;
    private float verticalSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        verticalSpeed = 0f;
    }

    private void Update()
    {
        if (!characterController.enabled || Time.deltaTime <= 0f)
        {
            return;
        }

        Vector2 input = ReadMovement();
        Vector3 velocity =
    (transform.right * input.x + transform.forward * input.y) * moveSpeed;

        // Move no aplica gravedad por su cuenta.
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
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;

        // Evita que dos teclas diagonales aumenten la velocidad.
        return Vector2.ClampMagnitude(input, 1f);
    }
}
