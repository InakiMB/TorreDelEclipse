using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TowerAssetAudit
{
    [Serializable] private class Catalog { public List<Model> models = new List<Model>(); public List<string> roots = new List<string>(); }
    [Serializable] private class Model { public string path; public List<Piece> pieces = new List<Piece>(); }
    [Serializable] private class Piece { public string name; public string mesh; public Vector3 size; public Vector3 center; public Vector3 position; public Vector3 scale; public List<string> materials = new List<string>(); }

    [InitializeOnLoadMethod]
    private static void Schedule()
    {
        if (!File.Exists("Library/LevelDesign/asset-catalog.json")) EditorApplication.delayCall += Export;
    }

    [MenuItem("Tools/Torre del Eclipse/Inspeccionar recursos")]
    public static void Export()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode) return;
        var catalog = new Catalog();
        foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { "Assets/Art/External" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (root == null) continue;
            var model = new Model { path = path };
            foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh == null) continue;
                var piece = new Piece { name = filter.name, mesh = filter.sharedMesh.name,
                    size = filter.sharedMesh.bounds.size, center = filter.sharedMesh.bounds.center,
                    position = filter.transform.localPosition, scale = filter.transform.localScale };
                var renderer = filter.GetComponent<Renderer>();
                if (renderer != null) foreach (Material material in renderer.sharedMaterials)
                    piece.materials.Add(material != null ? material.name + ":" + AssetDatabase.GetAssetPath(material) : "NULL");
                model.pieces.Add(piece);
            }
            catalog.models.Add(model);
        }
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            catalog.roots.Add(root.name + " " + root.transform.position);
        Directory.CreateDirectory("Library/LevelDesign");
        File.WriteAllText("Library/LevelDesign/asset-catalog.json", JsonUtility.ToJson(catalog, true));
        Debug.Log("Catalogo de recursos disponible en Library/LevelDesign/asset-catalog.json");
    }
}
