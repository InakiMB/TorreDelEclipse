using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(100)]
public sealed class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset = new Vector3(0f, 14f, -8f);
    [SerializeField, Min(0.01f)] private float smoothTime = 0.12f;
    [SerializeField, Range(0f, 0.5f)] private float combatFocusWeight = 0.5f;
    [SerializeField, Min(1f)] private float combatZoom = 1.35f;

    private Vector3 followVelocity;
    private Transform combatFocus;
    private Camera view;
    private float normalOrthographicSize;

    public void SetCombatFocus(Transform focus) => combatFocus = focus;

    private void Start()
    {
        view = GetComponent<Camera>();
        normalOrthographicSize = view.orthographicSize;
        if (target == null)
        {
            Debug.LogError("TopDownCamera necesita un Target en el Inspector.", this);
            enabled = false;
            return;
        }

        transform.SetPositionAndRotation(
            target.position + offset,
            Quaternion.Euler(60f, 0f, 0f));
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        followVelocity = Vector3.zero;
        transform.SetPositionAndRotation(target.position + offset, Quaternion.Euler(60f, 0f, 0f));
    }

    private void LateUpdate()
    {
        if (target == null || Time.deltaTime <= 0f)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        float desiredSize = normalOrthographicSize;
        if (combatFocus != null)
        {
            Vector3 focus = combatFocus.position; focus.y = target.position.y;
            Vector3 center = Vector3.Lerp(target.position, focus, combatFocusWeight);
            float zoom = combatZoom;
            desiredPosition = center + offset * zoom;
            // Reservar espacio para la hotbar y la barra del jefe en la interfaz final.
            if (EclipseGameUI.Active != null && view != null && view.orthographic)
            {
                Quaternion inverse = Quaternion.Inverse(Quaternion.Euler(60f, 0f, 0f));
                Vector3 playerLocal = inverse * (target.position - desiredPosition);
                Vector3 bossLocal = inverse * (combatFocus.position + Vector3.up * 3.5f - desiredPosition);
                float low = Mathf.Min(playerLocal.y, bossLocal.y), high = Mathf.Max(playerLocal.y, bossLocal.y);
                desiredSize = Mathf.Max(normalOrthographicSize, (high - low + 1f) / 1.24f,
                    Mathf.Abs(playerLocal.x - bossLocal.x) / (view.aspect * 1.68f));
                float shift = (low + high) * .5f - .06f * desiredSize;
                desiredPosition += Quaternion.Euler(60f, 0f, 0f) * Vector3.up * shift;
            }
            else if (EclipseGameUI.Active != null && view != null)
            {
                for (int attempt = 0; attempt < 18; attempt++)
                {
                    if (FitsInterface(target.position, desiredPosition)
                        && FitsInterface(combatFocus.position + Vector3.up * 3.5f, desiredPosition)) break;
                    zoom += .06f;
                    desiredPosition = center + offset * zoom;
                }
            }
        }
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime);
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
        if (view != null && view.orthographic)
            view.orthographicSize = Mathf.Lerp(view.orthographicSize, desiredSize,
                1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(.01f, smoothTime)));
    }

    private bool FitsInterface(Vector3 point, Vector3 cameraPosition)
    {
        Vector3 local = Quaternion.Inverse(Quaternion.Euler(60f, 0f, 0f)) * (point - cameraPosition);
        if (local.z <= .1f) return false;
        float halfHeight = local.z * Mathf.Tan(view.fieldOfView * .5f * Mathf.Deg2Rad);
        float x = .5f + local.x / (2f * halfHeight * view.aspect);
        float y = .5f + local.y / (2f * halfHeight);
        return x > .08f && x < .92f && y > .22f && y < .84f;
    }
}
