using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class TowerLevelBuilder
{
    private const string Output = "Assets/LevelDesign/TorreEclipse";
    private const string Kenney = "Assets/Art/External/KenneyRetroFantasy/Models/";
    private const string Dungeon = "Assets/Art/External/BlenderVoyageDungeon/Models/ModularDungeonFreeEdition.fbx";
    private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();
    private static Transform architecture;
    private static Transform props;
    private static Transform supplies;
    private static Material stone;
    private static Material darkStone;
    private static Material purple;
    private static Material gold;
    private static Material glow;
    private static Material fire;
    private static Material wood;

    private static Transform Group(string name, Transform parent = null)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj.transform;
    }
    private static Material Mat(string name, string texture, Color color, bool emission = false)
    {
        if (Materials.TryGetValue(name, out Material known)) return known;
        string path = Output + "/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
        material.SetColor("_BaseColor", color);
        material.SetFloat("_Smoothness", 0.08f);
        if (!string.IsNullOrEmpty(texture)) material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texture));
        if (emission) { material.EnableKeyword("_EMISSION"); material.SetColor("_EmissionColor", color * 2.4f); }
        EditorUtility.SetDirty(material);
        Materials[name] = material;
        return material;
    }
    private static void PrepareMaterials()
    {
        Materials.Clear();
        stone = Mat("PiedraFria", "Assets/Art/External/BlenderVoyageDungeon/Textures/concrete_512_indexed.png", new Color(.59f,.65f,.7f));
        darkStone = Mat("PiedraOscura", "Assets/Art/External/BlenderVoyageDungeon/Textures/concrete_512_indexed_dark.png", new Color(.52f,.58f,.67f));
        purple = Mat("TelaCiruela", "Assets/Art/External/BlenderVoyageDungeon/Textures/512_fabric.png", new Color(.25f,.08f,.18f));
        gold = Mat("MetalAntiguo", "Assets/Art/External/BlenderVoyageDungeon/Textures/gold512.png", new Color(.66f,.42f,.15f));
        wood = Mat("Madera", Kenney + "Textures/barrel.png", new Color(.7f,.57f,.44f));
        glow = Mat("RunasVioletas", null, new Color(.4f,.16f,.72f), true);
        fire = Mat("LlamaAmbar", null, new Color(1f,.43f,.08f), true);
    }
    private static Material ConvertMaterial(Material source)
    {
        if (source == null) return stone;
        string name = source.name.Replace(" (Instance)", "");
        if (Materials.TryGetValue("Recurso_" + name, out Material known)) return known;
        Texture texture = source.mainTexture;
        string texturePath = texture != null ? AssetDatabase.GetAssetPath(texture) : "";
        if (string.IsNullOrEmpty(texturePath))
        {
            string key = name.ToLowerInvariant();
            if (key == "floor1") texturePath = "Assets/Art/External/BlenderVoyageDungeon/Textures/Floor512_indexed.png";
            else if (key == "walls") texturePath = "Assets/Art/External/BlenderVoyageDungeon/Textures/concrete_512_indexed_dark.png";
            else if (key == "pillars") texturePath = "Assets/Art/External/BlenderVoyageDungeon/Textures/concrete_512_indexed.png";
            else if (key.Contains("metal")) texturePath = "Assets/Art/External/BlenderVoyageDungeon/Textures/metal_512.png";
            else if (key.Contains("wood")) texturePath = "Assets/Art/External/BlenderVoyageDungeon/Textures/wood_512.png";
            else if (key.Contains("barrel")) texturePath = Kenney + "Textures/barrel.png";
            else if (key.Contains("plank")) texturePath = Kenney + "Textures/planks.png";
            else if (key.Contains("stone")) texturePath = Kenney + "Textures/cobblestoneAlternative.png";
            else texturePath = Kenney + "Textures/cobblestone.png";
        }
        return Mat("Recurso_" + name, texturePath, new Color(.72f,.76f,.8f));
    }
    private static GameObject Box(string name, Vector3 center, Vector3 size, Material material, Transform parent, bool collider = true)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.position = center;
        obj.transform.localScale = size;
        obj.layer = 3;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        if (!collider) UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
    private static Bounds BoundsOf(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }
    private static GameObject Piece(string name, string asset, string part, Vector3 bottom, Vector3 size,
        float yaw, Transform parent, bool collision = false, bool cutaway = false)
    {
        var wrapper = new GameObject(name);
        wrapper.layer = 3;
        GameObject visual;
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
        if (model == null) throw new InvalidOperationException("No se encuentra " + asset);
        if (string.IsNullOrEmpty(part)) visual = (GameObject)PrefabUtility.InstantiatePrefab(model);
        else
        {
            Transform source = model.GetComponentsInChildren<Transform>(true).First(t => t.name == part);
            visual = UnityEngine.Object.Instantiate(source.gameObject);
            visual.transform.rotation = source.rotation;
            visual.transform.localScale = source.lossyScale;
        }
        visual.name = "Modelo";
        visual.transform.SetParent(wrapper.transform, true);
        visual.transform.localPosition = Vector3.zero;
        foreach (Transform child in visual.GetComponentsInChildren<Transform>(true)) child.gameObject.layer = 3;
        foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>())
            renderer.sharedMaterials = renderer.sharedMaterials.Select(ConvertMaterial).ToArray();
        Bounds bounds = BoundsOf(visual);
        Vector3 factor = new Vector3(bounds.size.x > .001f ? size.x / bounds.size.x : 1f,
            bounds.size.y > .001f && size.y > 0f ? size.y / bounds.size.y : 1f,
            bounds.size.z > .001f ? size.z / bounds.size.z : 1f);
        wrapper.transform.localScale = factor;
        bounds = BoundsOf(visual);
        wrapper.transform.position = -new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        // Keep the fitting transform inside an unscaled placement root.
        var placement = new GameObject(name);
        placement.layer = 3;
        wrapper.name = "Ajuste";
        wrapper.transform.SetParent(placement.transform, true);
        placement.transform.SetParent(parent, false);
        placement.transform.SetPositionAndRotation(bottom, Quaternion.Euler(0f, yaw, 0f));
        if (collision)
        {
            foreach (MeshFilter filter in visual.GetComponentsInChildren<MeshFilter>())
                filter.gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
        }
        if (cutaway) placement.AddComponent<TowerWallCutaway>();
        return placement;
    }
    private static GameObject D(string name, string part, Vector3 bottom, Vector3 size, float yaw = 0f,
        bool collision = false, bool cutaway = false) => Piece(name, Dungeon, part, bottom, size, yaw, architecture, collision, cutaway);
    private static GameObject K(string name, string part, Vector3 bottom, Vector3 size, float yaw = 0f,
        Transform parent = null, bool collision = false) => Piece(name, Kenney + part + ".fbx", null, bottom, size, yaw, parent != null ? parent : architecture, collision);

    private static void Floors(float xMin, float xMax, float zMin, float zMax)
    {
        for (float x = xMin + 2; x < xMax; x += 4)
        for (float z = zMin + 2; z < zMax; z += 4)
        {
            GameObject tile = D("Baldosa", "Floor1", new Vector3(x, 0, z), new Vector3(4, 0, 4));
            BoxCollider floor = tile.AddComponent<BoxCollider>();
            floor.center = new Vector3(0, -.15f, 0);
            floor.size = new Vector3(4, .3f, 4);
        }
    }
    private static void Wall(Vector3 from, Vector3 to, float height = 3.8f)
    {
        Vector3 span = to - from;
        int count = Mathf.Max(1, Mathf.CeilToInt(span.magnitude / 3f));
        float length = span.magnitude / count;
        float yaw = -Mathf.Atan2(span.z, span.x) * Mathf.Rad2Deg;
        for (int i = 0; i < count; i++)
        {
            Vector3 bottom = Vector3.Lerp(from, to, (i + .5f) / count);
            D("Muro", "BaseWall", bottom, new Vector3(length + .02f, height, .55f), yaw, true, true);
        }
    }
    private static void Pillar(float x, float z, float height = 4.4f, float floor = 0f)
    {
        D("Columna", "BasePillar", new Vector3(x, floor, z), new Vector3(1.25f, height, 1.25f), 0, true, true);
        Box("ZocaloColumna", new Vector3(x, floor + .16f, z), new Vector3(1.65f,.32f,1.65f), darkStone, architecture);
    }
    private static void Arch(float x, float z, float yaw = 0f, float floor = 0f)
    {
        D("Arco", "Doorway", new Vector3(x, floor, z), new Vector3(6.4f,4.8f,1f), yaw, true, true);
    }
    private static void Banner(float x, float z, float yaw = 0f, float floor = 0f)
    {
        Transform group = Group("Estandarte", props);
        Box("Tela", new Vector3(0,2.8f,0), new Vector3(1.25f,2.6f,.06f), purple, group, false);
        Box("Travesano", new Vector3(0,4.15f,0), new Vector3(1.65f,.1f,.14f), gold, group, false);
        Box("Borde", new Vector3(0,1.55f,-.04f), new Vector3(1.25f,.09f,.05f), gold, group, false);
        for (int i = 0; i < 12; i++)
        {
            float angle = i * Mathf.PI * 2f / 12f;
            GameObject segment = Box("Eclipse", new Vector3(Mathf.Sin(angle)*.32f,3f+Mathf.Cos(angle)*.32f,-.055f),
                new Vector3(.09f,.18f,.03f), gold, group, false);
            segment.transform.localRotation = Quaternion.Euler(0,0,-angle*Mathf.Rad2Deg);
        }
        group.SetPositionAndRotation(new Vector3(x,floor,z), Quaternion.Euler(0,yaw,0));
    }
    private static void Brazier(float x, float z, float floor = 0f, bool violet = false)
    {
        Transform group = Group(violet ? "FuegoArcano" : "Brasero", props);
        K("Pedestal", "column", Vector3.zero, new Vector3(.45f,1.35f,.45f), 0, group);
        GameObject bowl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bowl.name = "Cuenco";
        bowl.transform.SetParent(group, false);
        bowl.transform.localPosition = new Vector3(0,1.35f,0);
        bowl.transform.localScale = new Vector3(.8f,.12f,.8f);
        bowl.GetComponent<Renderer>().sharedMaterial = gold;
        UnityEngine.Object.DestroyImmediate(bowl.GetComponent<Collider>());
        GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flame.name = "Llama";
        flame.transform.SetParent(group, false);
        flame.transform.localPosition = new Vector3(0,1.68f,0);
        flame.transform.localScale = new Vector3(.32f,.65f,.32f);
        flame.GetComponent<Renderer>().sharedMaterial = violet ? glow : fire;
        UnityEngine.Object.DestroyImmediate(flame.GetComponent<Collider>());
        Light light = flame.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = violet ? new Color(.5f,.25f,1f) : new Color(1f,.52f,.2f);
        light.intensity = violet ? 4f : 5f;
        light.range = 8f;
        light.shadows = LightShadows.None;
        group.position = new Vector3(x,floor,z);
    }
}
