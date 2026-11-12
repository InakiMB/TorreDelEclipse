using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(200)]
public sealed class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;
    [SerializeField, Min(1f)] private float turnSpeed = 1080f;

    private void Awake()
    {
        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }

        if (viewCamera == null)
        {
            Debug.LogError("PlayerAim necesita una View Camera en el Inspector.", this);
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || viewCamera == null || Time.deltaTime <= 0f)
        {
            return;
        }

        Vector2 screenPosition = mouse.position.ReadValue();
        if (!viewCamera.pixelRect.Contains(screenPosition))
        {
            return;
        }

        // El mouse apunta sobre un plano horizontal a la altura del jugador.
        Ray ray = viewCamera.ScreenPointToRay(screenPosition);
        Plane aimingPlane = new Plane(Vector3.up, transform.position);
        if (!aimingPlane.Raycast(ray, out float distance))
        {
            return;
        }

        Vector3 direction = ray.GetPoint(distance) - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion desiredRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            desiredRotation,
            turnSpeed * Time.deltaTime);
    }
}
