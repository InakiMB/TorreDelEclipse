using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class TowerLevelBuilder
{
    [Serializable] private sealed class OrganizationReport
    {
        public bool complete;
        public int preservedObjects;
        public int preservedReferences;
        public int renderers;
        public int colliders;
        public string[] roots;
    }

    [MenuItem("Tools/Torre del Eclipse/Ordenar como prototipo")]
    public static void OrganizeCurrentScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Salir de Play antes de ordenar la escena.");
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath) throw new InvalidOperationException("Abrir TorreEclipse para ordenar sus objetos.");
        Directory.CreateDirectory("Library/LevelDesign");
        EditorSceneManager.SaveScene(scene);
        File.Copy(ScenePath, "Library/LevelDesign/TorreEclipse_antes_ordenar.unity", true);
        OrganizeHierarchy(scene);
        EditorSceneManager.SaveScene(scene);
        Validate();
        Selection.activeGameObject = SceneObjects(scene).First(item => item.name == "AudioSystems").gameObject;
        EditorApplication.delayCall += TowerLevelSmokeCheck.Begin;
    }

    private static Transform[] SceneObjects(Scene scene) => scene.GetRootGameObjects()
        .SelectMany(root => root.GetComponentsInChildren<Transform>(true)).ToArray();

    private static void OrganizeHierarchy(Scene scene)
    {
        Transform[] before = SceneObjects(scene);
        var matrices = before.ToDictionary(item => item, item => item.localToWorldMatrix);
        var componentIds = before.SelectMany(item => item.GetComponents<Component>())
            .Where(item => item != null).Select(item => item.GetInstanceID()).ToArray();
        var references = SceneReferences(before);
        int rendererCount = before.Sum(item => item.GetComponents<Renderer>().Length);
        int colliderCount = before.Sum(item => item.GetComponents<Collider>().Length);
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Organizar TorreEclipse");
        try
        {
            Transform groundFloor = before.FirstOrDefault(item => item.parent == null && item.name == "01_Arquitectura")
                ?? before.FirstOrDefault(item => item.parent == null && item.name == "GroundFloor");
            Transform terrace = before.FirstOrDefault(item => item.parent == null && item.name == "02_Ambientacion")
                ?? before.FirstOrDefault(item => item.parent == null && item.name == "Terrace");
            if (groundFloor == null || terrace == null) throw new InvalidOperationException("No se encuentran los grupos de arquitectura.");
            bool alreadyOrganized = groundFloor.name == "GroundFloor" && terrace.name == "Terrace";
            Transform[] originalArchitecture = Children(groundFloor);
            Transform[] originalProps = Children(terrace);
            Rename(groundFloor, "GroundFloor");
            Rename(terrace, "Terrace");

            var objective = before.Select(item => item.GetComponent<EncounterObjective>()).First(item => item != null);
            var director = before.Select(item => item.GetComponent<TowerAudioDirector>()).First(item => item != null);
            GameObject gate = Get<GameObject>(objective, "gateBlocker");
            Transform audio = director.transform;
            Collider exterior = Get<Collider>(director, "exteriorArea");
            Transform insidePoint = Get<Transform>(director, "interiorPoint");
            Transform outsidePoint = Get<Transform>(director, "exteriorPoint");
            Vector3 insidePosition = insidePoint.position;
            Vector3 outsidePosition = outsidePoint.position;
            Rename(audio, "AudioSystems");
            Move(insidePoint, audio); Rename(insidePoint, "AmbienteInterior");
            Move(outsidePoint, audio); Rename(outsidePoint, "AmbienteExterior");
            if (exterior != null) { Move(exterior.transform, audio); Rename(exterior.transform, "ZonaTerraza"); }
            Move(gate.transform, null); Rename(gate.transform, "GateBlocker");

            if (!alreadyOrganized)
            {
                var groundGroups = new Dictionary<string, Transform>();
                var terraceGroups = new Dictionary<string, Transform>();
                Transform originalSupplies = before.FirstOrDefault(item => item.name == "03_Suministros");
                foreach (string category in new[] { "Ground", "Walls", "Columns", "Props", "Supplies" })
                {
                    if (category == "Supplies" && originalSupplies != null)
                    {
                        Move(originalSupplies, groundFloor);
                        Rename(originalSupplies, "Supplies");
                        groundGroups[category] = originalSupplies;
                    }
                    else groundGroups[category] = EnsureGroup(groundFloor, category);
                    terraceGroups[category] = EnsureGroup(terrace, category);
                }
                foreach (Transform item in originalArchitecture.Concat(originalProps))
                {
                    if (item == gate.transform) continue;
                    bool upper = IsCloserToExterior(item.position, insidePosition, outsidePosition);
                    string category = Category(item.name);
                    Move(item, (upper ? terraceGroups : groundGroups)[category]);
                }
                foreach (DestructibleSupply supply in before.Select(item => item.GetComponent<DestructibleSupply>()).Where(item => item != null))
                {
                    bool upper = IsCloserToExterior(supply.transform.position, insidePosition, outsidePosition);
                    Move(supply.transform, (upper ? terraceGroups : groundGroups)["Supplies"]);
                }
                foreach (Transform category in groundGroups.Values.Concat(terraceGroups.Values)) RenameCategoryItems(category);
            }

            Transform[] enemies = before.Where(item => item.GetComponent<IEnemyCombatant>() != null)
                .OrderBy(item => EnemyNumber(item.name)).ToArray();
            Transform enemyGroup = enemies.Length > 0 ? enemies[0].transform.parent : null;
            if (enemyGroup == null) enemyGroup = EnsureGroup(null, "Enemies");
            Move(enemyGroup, null); Rename(enemyGroup, "Enemies");
            for (int i = 0; i < enemies.Length; i++)
            {
                Move(enemies[i].transform, enemyGroup);
                if (enemies[i].GetComponent<EnemyMage>() == null && enemies[i].GetComponent<EnemySupport>() == null
                    && enemies[i].GetComponent<WarriorAnimatorDriver>() == null)
                    Rename(enemies[i].transform, "Enemy" + (i + 1).ToString("00"));
                Undo.RecordObject(enemies[i].transform, "Ordenar enemigos");
                enemies[i].transform.SetSiblingIndex(i);
            }
            var hud = before.Select(item => item.GetComponent<TowerLevelHUD>()).First(item => item != null);
            Move(hud.transform, null); Rename(hud.transform, "CombatHUD");
            var surface = before.Select(item => item.GetComponent<NavMeshSurface>()).First(item => item != null);
            Move(surface.transform, null); Rename(surface.transform, "Navigation");

            Transform spawnPoints = EnsureGroup(null, "SpawnPoints");
            foreach (TowerStairway stair in before.Select(item => item.GetComponent<TowerStairway>()).Where(item => item != null))
            {
                bool ascending = Get<bool>(stair, "ascending");
                Move(stair.transform, null); Rename(stair.transform, ascending ? "ExitMarker" : "ReturnMarker");
                Transform destination = Get<Transform>(stair, "destination");
                Move(destination, spawnPoints);
                Rename(destination, ascending ? "TerraceSpawn" : "GroundFloorSpawn");
            }
            foreach (Transform item in before)
            {
                if (item.name == "Ajuste") Rename(item, "Visual");
                else if (item.name == "Modelo") Rename(item, "Model");
            }
            string[] rootOrder = { "Main Camera", "Directional Light", "Global Volume", "GroundFloor", "Terrace", "Player",
                "Navigation", "Enemies", "CombatHUD", "GateBlocker", "ExitMarker", "ReturnMarker", "SpawnPoints", "EncounterObjective", "AudioSystems" };
            for (int i = 0; i < rootOrder.Length; i++)
            {
                Transform item = scene.GetRootGameObjects().Select(root => root.transform).FirstOrDefault(root => root.name == rootOrder[i]);
                if (item != null) { Undo.RecordObject(item, "Ordenar jerarquia"); item.SetSiblingIndex(i); }
            }

            Transform[] after = SceneObjects(scene);
            foreach (KeyValuePair<Transform, Matrix4x4> entry in matrices)
            {
                if (entry.Key == null) throw new InvalidOperationException("Se perdio un objeto de la escena.");
                Matrix4x4 current = entry.Key.localToWorldMatrix;
                for (int i = 0; i < 16; i++)
                    if (Mathf.Abs(current[i] - entry.Value[i]) > .002f)
                        throw new InvalidOperationException("Cambio la transformacion de " + entry.Key.name);
            }
            HashSet<int> afterComponents = new HashSet<int>(after.SelectMany(item => item.GetComponents<Component>())
                .Where(item => item != null).Select(item => item.GetInstanceID()));
            if (componentIds.Any(id => !afterComponents.Contains(id))) throw new InvalidOperationException("Se perdio un componente.");
            Dictionary<string, int> afterReferences = SceneReferences(after);
            if (references.Any(pair => !afterReferences.TryGetValue(pair.Key, out int value) || pair.Value != value))
                throw new InvalidOperationException("Cambio una referencia de componentes.");
            var report = new OrganizationReport { complete = true, preservedObjects = before.Length,
                preservedReferences = references.Count, renderers = rendererCount, colliders = colliderCount,
                roots = scene.GetRootGameObjects().Select(root => root.name).ToArray() };
            Directory.CreateDirectory("Library/LevelDesign");
            File.WriteAllText("Library/LevelDesign/organization-result.json", JsonUtility.ToJson(report, true));
            EditorSceneManager.MarkSceneDirty(scene);
            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Jerarquia ordenada como el prototipo. Objetos, posiciones y referencias conservados.");
        }
        catch
        {
            Undo.RevertAllDownToGroup(undoGroup);
            throw;
        }
    }

    private static Transform[] Children(Transform parent) => parent.Cast<Transform>().ToArray();
    private static Transform EnsureGroup(Transform parent, string name)
    {
        Transform existing = parent != null ? Children(parent).FirstOrDefault(item => item.name == name)
            : SceneManager.GetActiveScene().GetRootGameObjects().Select(root => root.transform).FirstOrDefault(item => item.name == name);
        if (existing != null) return existing;
        var obj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(obj, "Crear grupo");
        Move(obj.transform, parent);
        return obj.transform;
    }
    private static void Move(Transform item, Transform parent)
    {
        if (item.parent != parent) Undo.SetTransformParent(item, parent, "Organizar jerarquia");
    }
    private static void Rename(Transform item, string name)
    {
        if (item.name == name) return;
        Undo.RecordObject(item.gameObject, "Renombrar objeto");
        item.name = name;
        if (PrefabUtility.IsPartOfPrefabInstance(item.gameObject)) PrefabUtility.RecordPrefabInstancePropertyModifications(item.gameObject);
    }
    private static bool IsCloserToExterior(Vector3 position, Vector3 inside, Vector3 outside)
    {
        position.y = inside.y = outside.y = 0;
        return (position - outside).sqrMagnitude < (position - inside).sqrMagnitude;
    }
    private static string Category(string name)
    {
        if (name == "Baldosa" || name == "SueloTerraza") return "Ground";
        if (name.StartsWith("Columna") || name == "ZocaloColumna") return "Columns";
        if (name == "Muro" || name == "Arco" || name == "Almena" || name == "PretilTerraza" || name == "AccesoTerraza") return "Walls";
        return "Props";
    }
    private static int EnemyNumber(string name)
    {
        string digits = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out int number) ? number : int.MaxValue;
    }
    private static void RenameCategoryItems(Transform parent)
    {
        var names = new Dictionary<string, string> {
            {"Baldosa","Floor"},{"SueloTerraza","GroundMesh"},{"Muro","Wall"},{"Arco","Arch"},
            {"Columna","Column"},{"ColumnaTerraza","Column"},{"ZocaloColumna","ColumnBase"},
            {"PretilTerraza","Parapet"},{"Almena","Battlement"},{"AccesoTerraza","DoorFrame"},
            {"Torreon","Tower"},{"CuerpoDeLaTorre","TowerBody"},{"AltarLateral","Altar"},{"AltarDelSantuario","Altar"},
            {"EscaleraDeAscenso","Stairs"},{"DescansoEscalera","Landing"},{"Estandarte","Banner"},
            {"Brasero","Brazier"},{"FuegoArcano","ArcaneBrazier"},{"CoberturaDeCajas","Cover"},
            {"EstanteriaOeste","Shelf"},{"EstanteriaEste","Shelf"},{"AlfombraCentral","Carpet"},
            {"BordeAlfombra","CarpetBorder"},{"CirculoDelEclipse","EclipseRune"},{"Reserva_Jefe_PorDefinir","BossArea"},
            {"CajaPocion","PotionCrate"},{"BarrilPocion","PotionBarrel"}};
        var counters = new Dictionary<string, int>();
        foreach (Transform child in Children(parent).OrderBy(item => item.position.z).ThenBy(item => item.position.x))
        {
            if (!names.TryGetValue(child.name, out string prefix)) continue;
            int index = counters.TryGetValue(prefix, out int previous) ? previous + 1 : 1;
            counters[prefix] = index;
            Rename(child, prefix + index.ToString("00"));
        }
        int sibling = 0;
        foreach (Transform child in Children(parent).OrderBy(item => item.name, StringComparer.Ordinal))
        { Undo.RecordObject(child, "Ordenar jerarquia"); child.SetSiblingIndex(sibling++); }
    }
    private static Dictionary<string, int> SceneReferences(Transform[] objects)
    {
        var result = new Dictionary<string, int>();
        foreach (MonoBehaviour component in objects.SelectMany(item => item.GetComponents<MonoBehaviour>()).Where(item => item != null))
        {
            var serialized = new SerializedObject(component);
            SerializedProperty property = serialized.GetIterator();
            while (property.Next(true))
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                    result[component.GetInstanceID() + ":" + property.propertyPath] = property.objectReferenceValue.GetInstanceID();
        }
        return result;
    }
}
