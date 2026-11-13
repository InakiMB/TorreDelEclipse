using UnityEngine;

[DisallowMultipleComponent]
public sealed class TowerLevelFlow : MonoBehaviour
{
    [SerializeField] private Transform player = null;
    [SerializeField] private Collider terraceArea = null;
    public bool IsOnTerrace => player != null && terraceArea != null
        && (terraceArea.ClosestPoint(player.position) - player.position).sqrMagnitude < 0.001f;
    public bool ReachedTerrace { get; private set; }
    private void LateUpdate() { if (IsOnTerrace) ReachedTerrace = true; }
}
