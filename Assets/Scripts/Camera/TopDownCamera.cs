using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(100)]
public sealed class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset = new Vector3(0f, 14f, -8f);
    [SerializeField, Min(0.01f)] private float smoothTime = 0.12f;

    private Vector3 followVelocity;

    private void Start()
    {
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
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime);
        transform.rotation = Quaternion.Euler(60f, 0f, 0f);
    }
}
