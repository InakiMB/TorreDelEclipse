using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(250)]
public sealed class TowerCameraOcclusion : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private LayerMask walls = 8;
    private readonly HashSet<TowerWallCutaway> hidden = new HashSet<TowerWallCutaway>();
    private readonly HashSet<TowerWallCutaway> next = new HashSet<TowerWallCutaway>();

    private void LateUpdate()
    {
        if (target == null) return;
        next.Clear();
        for (int i = -1; i <= 1; i++)
        {
            Vector3 point = target.position + Vector3.right * (i * 0.55f);
            Vector3 direction = point - transform.position;
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction.normalized,
                direction.magnitude, walls, QueryTriggerInteraction.Ignore))
            {
                TowerWallCutaway wall = hit.collider.GetComponentInParent<TowerWallCutaway>();
                if (wall != null) next.Add(wall);
            }
        }
        foreach (TowerWallCutaway wall in hidden) if (wall != null && !next.Contains(wall)) wall.SetHidden(false);
        hidden.Clear();
        foreach (TowerWallCutaway wall in next) { wall.SetHidden(true); hidden.Add(wall); }
    }
    private void OnDisable()
    {
        foreach (TowerWallCutaway wall in hidden) if (wall != null) wall.SetHidden(false);
        hidden.Clear();
    }
}
