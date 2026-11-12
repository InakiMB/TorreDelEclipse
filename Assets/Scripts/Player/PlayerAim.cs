using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[DefaultExecutionOrder(200)]
public sealed class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera viewCamera = null;
    [SerializeField] private Transform aimOrigin = null;
    [SerializeField, Min(0f)] private float aimDeadZone = 0.05f;

    private void Awake()
    {
        if (viewCamera == null) viewCamera = Camera.main;

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
        if (!viewCamera.pixelRect.Contains(screenPosition)) return;

        // Usar el mismo origen y altura que el disparo alinea su recorrido con el cursor.
        Vector3 origin = aimOrigin != null ? aimOrigin.position : transform.position;
        Plane aimingPlane = new Plane(Vector3.up, origin);
        Ray ray = viewCamera.ScreenPointToRay(screenPosition);
        if (!aimingPlane.Raycast(ray, out float distance)) return;

        Vector3 direction = ray.GetPoint(distance) - origin;
        direction.y = 0f;
        if (direction.sqrMagnitude <= aimDeadZone * aimDeadZone) return;

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
