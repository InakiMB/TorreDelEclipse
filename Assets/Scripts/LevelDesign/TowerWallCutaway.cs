using UnityEngine;

[DisallowMultipleComponent]
public sealed class TowerWallCutaway : MonoBehaviour
{
    private Renderer[] surfaces;
    public void SetHidden(bool hidden)
    {
        if (surfaces == null) surfaces = GetComponentsInChildren<Renderer>();
        foreach (Renderer surface in surfaces) if (surface != null) surface.forceRenderingOff = hidden;
    }
    private void OnDisable() => SetHidden(false);
}
