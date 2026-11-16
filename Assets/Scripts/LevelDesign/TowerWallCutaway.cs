using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class TowerWallCutaway : MonoBehaviour
{
    [Serializable] public struct FadeMaterial { public Material original; public Material transparent; }
    [SerializeField] private bool fadeInsteadOfHide;
    [SerializeField, Range(0.1f, 1f)] private float occludedOpacity = 0.3f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.2f;
    [SerializeField] private FadeMaterial[] fadeMaterials = Array.Empty<FadeMaterial>();
    private Renderer[] surfaces;
    private Material[][] originals, faded;
    private readonly Dictionary<Material, Material> instances = new Dictionary<Material, Material>();
    private bool occluded, usingFadedMaterials;
    public bool UsesTransparency => fadeInsteadOfHide;
    public float CurrentOpacity { get; private set; } = 1f;

    public void SetHidden(bool hidden)
    {
        CacheSurfaces();
        if (fadeInsteadOfHide) { occluded = hidden; return; }
        foreach (Renderer surface in surfaces) if (surface != null) surface.forceRenderingOff = hidden;
    }
    public bool IntersectsViewRay(Ray ray, float distance)
    {
        if (!fadeInsteadOfHide) return false;
        CacheSurfaces();
        foreach (Renderer surface in surfaces)
            if (surface != null && surface.enabled && surface.bounds.IntersectRay(ray, out float entry) && entry < distance) return true;
        return false;
    }
    private void CacheSurfaces()
    {
        if (surfaces != null) return;
        surfaces = GetComponentsInChildren<Renderer>(); originals = new Material[surfaces.Length][];
        for (int i = 0; i < surfaces.Length; i++) originals[i] = surfaces[i].sharedMaterials;
    }
    private void Update()
    {
        if (!fadeInsteadOfHide || surfaces == null) return;
        float desired = occluded ? occludedOpacity : 1f;
        float opacity = Mathf.MoveTowards(CurrentOpacity, desired, Time.unscaledDeltaTime * (1f - occludedOpacity) / fadeDuration);
        if (Mathf.Approximately(CurrentOpacity, opacity)) return;
        CurrentOpacity = opacity;
        if (opacity >= 0.999f) { Restore(); return; }
        if (faded == null) CreateFadedMaterials();
        foreach (var pair in instances)
        {
            string colorProperty = pair.Key.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
            if (!pair.Key.HasProperty(colorProperty)) continue;
            Color color = pair.Key.GetColor(colorProperty); color.a *= opacity; pair.Value.SetColor(colorProperty, color);
        }
        if (usingFadedMaterials) return;
        for (int i = 0; i < surfaces.Length; i++) if (surfaces[i] != null && surfaces[i].enabled)
        { surfaces[i].forceRenderingOff = false; surfaces[i].sharedMaterials = faded[i]; }
        usingFadedMaterials = true;
    }
    private void CreateFadedMaterials()
    {
        faded = new Material[originals.Length][];
        for (int i = 0; i < originals.Length; i++)
        {
            faded[i] = new Material[originals[i].Length];
            for (int j = 0; j < originals[i].Length; j++)
            {
                Material source = originals[i][j]; if (source == null) continue;
                if (!instances.TryGetValue(source, out Material instance))
                {
                    Material template = source;
                    foreach (FadeMaterial pair in fadeMaterials) if (pair.original == source && pair.transparent != null) { template = pair.transparent; break; }
                    instance = new Material(template) { name = source.name + " (Tower fade)", hideFlags = HideFlags.DontSave };
                    ConfigureTransparentMaterial(instance); instances.Add(source, instance);
                }
                faded[i][j] = instance;
            }
        }
    }
    public static void ConfigureTransparentMaterial(Material material)
    {
        SetFloat(material, "_Surface", 1); SetFloat(material, "_Blend", 0); SetFloat(material, "_AlphaClip", 0);
        SetFloat(material, "_SrcBlend", (int)BlendMode.SrcAlpha); SetFloat(material, "_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        SetFloat(material, "_SrcBlendAlpha", (int)BlendMode.One); SetFloat(material, "_DstBlendAlpha", (int)BlendMode.OneMinusSrcAlpha);
        SetFloat(material, "_ZWrite", 0); SetFloat(material, "_ZWriteControl", 2); SetFloat(material, "_AlphaToMask", 0);
        SetFloat(material, "_BlendModePreserveSpecular", 0); SetFloat(material, "_CastShadows", 0);
        material.SetOverrideTag("RenderType", "Transparent"); material.renderQueue = (int)RenderQueue.Transparent;
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.DisableKeyword("_ALPHAMODULATE_ON"); material.DisableKeyword("_ALPHATEST_ON");
        material.SetShaderPassEnabled("ShadowCaster", false); material.SetShaderPassEnabled("DepthOnly", false); material.SetShaderPassEnabled("DepthNormals", false);
    }
    private static void SetFloat(Material material, string property, float value) { if (material.HasProperty(property)) material.SetFloat(property, value); }
    private void Restore()
    {
        if (surfaces == null) return;
        for (int i = 0; i < surfaces.Length; i++) if (surfaces[i] != null)
        { surfaces[i].forceRenderingOff = false; if (usingFadedMaterials) surfaces[i].sharedMaterials = originals[i]; }
        usingFadedMaterials = false;
    }
    private void OnDisable() { occluded = false; CurrentOpacity = 1f; Restore(); }
    private void OnDestroy() { foreach (Material material in instances.Values) if (material != null) Destroy(material); instances.Clear(); }
}
