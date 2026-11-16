using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(250)]
public sealed class TowerCameraOcclusion : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private LayerMask walls = 8;
    [SerializeField] private BossEncounter bossEncounter;
    private TowerWallCutaway[] transparentObjects;
    private readonly HashSet<TowerWallCutaway> hidden = new HashSet<TowerWallCutaway>();
    private readonly HashSet<TowerWallCutaway> next = new HashSet<TowerWallCutaway>();
    private void Start() => transparentObjects = FindObjectsByType<TowerWallCutaway>(FindObjectsSortMode.None);

    private void LateUpdate()
    {
        if (target == null) return;
        next.Clear();
        Observe(target.position, 0.55f, false);
        if (bossEncounter != null && bossEncounter.Boss != null && bossEncounter.Boss.Action != BossAction.Dead && bossEncounter.Boss.Action != BossAction.Defeated)
            Observe(bossEncounter.Boss.transform.position, 1.05f, true);
        foreach (TowerWallCutaway wall in hidden) if (wall != null && !next.Contains(wall)) wall.SetHidden(false);
        hidden.Clear();
        foreach (TowerWallCutaway wall in next) { wall.SetHidden(true); hidden.Add(wall); }
    }
    private void Observe(Vector3 position, float halfWidth, bool onlyTransparent)
    {
        for (int i = -1; i <= 1; i++)
        {
            Vector3 point = position + Vector3.right * (i * halfWidth);
            Vector3 direction = point - transform.position;
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction.normalized,
                direction.magnitude, walls, QueryTriggerInteraction.Ignore))
            {
                TowerWallCutaway wall = hit.collider.GetComponentInParent<TowerWallCutaway>();
                if (wall != null && (!onlyTransparent || wall.UsesTransparency)) next.Add(wall);
            }
            if (transparentObjects == null) continue;
            Ray ray = new Ray(transform.position, direction.normalized);
            foreach (TowerWallCutaway wall in transparentObjects)
                if (wall != null && wall.UsesTransparency && wall.IntersectsViewRay(ray, direction.magnitude)) next.Add(wall);
        }
    }
    private void OnDisable()
    {
        foreach (TowerWallCutaway wall in hidden) if (wall != null) wall.SetHidden(false);
        hidden.Clear();
    }
}
