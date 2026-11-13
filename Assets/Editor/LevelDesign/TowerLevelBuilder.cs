using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FMODUnity;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static partial class TowerLevelBuilder
{
    public const string ScenePath = "Assets/Scenes/TorreEclipse.unity";
    private static readonly Vector3[] EnemyPositions = {
        new Vector3(-5,1,-10),new Vector3(5,1,-8),new Vector3(-6,1,3),new Vector3(6,1,6),
        new Vector3(-18,1,-1),new Vector3(-19,1,13),new Vector3(18,1,-1),new Vector3(19,1,13),
        new Vector3(-6,1,21),new Vector3(6,1,21)};

    [MenuItem("Tools/Torre del Eclipse/Construir nivel")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Salir de Play antes de construir.");
        try
        {
            Directory.CreateDirectory(Output+"/Materials");Directory.CreateDirectory(Output+"/Meshes");Directory.CreateDirectory(Output+"/Prefabs");
            AssetDatabase.Refresh();
            Scene current=SceneManager.GetActiveScene();
            if(current.isDirty && !string.IsNullOrEmpty(current.path)) EditorSceneManager.SaveScene(current);
            Scene source=EditorSceneManager.OpenScene("Assets/Scenes/PrototipoTorre.unity",OpenSceneMode.Single);
            Directory.CreateDirectory("Library/LevelDesign");
            if(File.Exists(ScenePath)) File.Copy(ScenePath,"Library/LevelDesign/TorreEclipse_anterior.unity",true);
            EditorSceneManager.SaveScene(source,ScenePath,true);
            Scene scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            GameObject player=GameObject.Find("Player");
            GameObject camera=GameObject.Find("Main Camera");
            GameObject template=GameObject.Find("Enemy01");
            EncounterObjective objective=UnityEngine.Object.FindFirstObjectByType<EncounterObjective>();
            TowerAudioDirector director=UnityEngine.Object.FindFirstObjectByType<TowerAudioDirector>();
            EventReference impact=Get<EventReference>(player.GetComponent<PlayerActionAudio>(),"staffImpactEvent");
            if(player==null||camera==null||template==null||objective==null||director==null) throw new InvalidOperationException("Faltan componentes del prototipo.");
            PrepareMaterials();
            architecture=Group("01_Arquitectura");props=Group("02_Ambientacion");supplies=Group("03_Suministros");
            foreach(GameObject root in scene.GetRootGameObjects())
            {
                if(new[]{"Main Camera","Directional Light","Global Volume","Player","EncounterObjective","AudioSystems","Enemy01",
                    "01_Arquitectura","02_Ambientacion","03_Suministros"}.Contains(root.name))continue;
                UnityEngine.Object.DestroyImmediate(root);
            }
            BuildGroundFloor();BuildTerrace();
            player.transform.SetPositionAndRotation(new Vector3(0,1.1f,-19),Quaternion.identity);
            TopDownCamera follow=camera.GetComponent<TopDownCamera>();
            Set(follow,"offset",new Vector3(0,17,-9.8f));
            Camera view=camera.GetComponent<Camera>();view.orthographicSize=9.5f;view.farClipPlane=150;view.backgroundColor=new Color(.035f,.045f,.075f);
            view.clearFlags=CameraClearFlags.SolidColor;follow.SnapToTarget();
            TowerCameraOcclusion occlusion=camera.AddComponent<TowerCameraOcclusion>();Set(occlusion,"target",player.transform);
            var enemies=new List<EnemyChaser>();
            Transform enemyGroup=Group("04_Encuentros");
            for(int i=0;i<EnemyPositions.Length;i++)
            {
                GameObject enemy=UnityEngine.Object.Instantiate(template,EnemyPositions[i],Quaternion.identity,enemyGroup);
                enemy.name="Centinela_"+(i+1).ToString("00");
                EnemyChaser chaser=enemy.GetComponent<EnemyChaser>();Set(chaser,"target",player.transform);Set(chaser,"detectionRange",6.5f);Set(chaser,"attackDamage",8);
                enemy.GetComponent<NavMeshAgent>().speed=2.8f;
                enemies.Add(chaser);
            }
            UnityEngine.Object.DestroyImmediate(template);
            GameObject gate=D("SelloDelAscenso","IronGate",new Vector3(0,0,27.9f),new Vector3(5.8f,3.6f,.3f),0,true);
            NavMeshObstacle gateObstacle=gate.AddComponent<NavMeshObstacle>();gateObstacle.shape=NavMeshObstacleShape.Box;
            gateObstacle.center=new Vector3(0,1.8f,0);gateObstacle.size=new Vector3(5.8f,3.6f,.6f);gateObstacle.carving=true;
            Set(objective,"playerHealth",player.GetComponent<Health>());Set(objective,"requiredEnemies",enemies.Select(e=>e.GetComponent<Health>()).ToArray());Set(objective,"gateBlocker",gate);
            Set(director,"enemies",enemies.ToArray());
            Collider terraceArea=Area("ZonaTerraza",new Vector3(100,15,0),new Vector3(38,12,38));
            Set(director,"exteriorArea",terraceArea);
            Get<Transform>(director,"interiorPoint").position=new Vector3(0,0,0);
            Get<Transform>(director,"exteriorPoint").position=new Vector3(100,12,0);
            TowerLevelFlow flow=objective.gameObject.AddComponent<TowerLevelFlow>();Set(flow,"player",player.transform);Set(flow,"terraceArea",terraceArea);
            Transform topSpawn=Group("LlegadaTerraza");topSpawn.position=new Vector3(100,13.1f,-10.5f);
            Transform bottomSpawn=Group("LlegadaPlantaBaja");bottomSpawn.position=new Vector3(0,1.1f,26);
            bottomSpawn.rotation=Quaternion.Euler(0,180,0);
            Stair("SubirTerraza",new Vector3(0,1.2f,26.9f),topSpawn,objective,follow,true);
            Stair("BajarPlantaBaja",new Vector3(100,13,-13),bottomSpawn,objective,follow,false);
            ParticleSystem particles=CreateBreakParticles();
            Vector3[] supplyPositions={new Vector3(-8,0,-17),new Vector3(-21,0,0),new Vector3(21,0,13),
                new Vector3(8,0,19),new Vector3(88,12,2),new Vector3(112,12,2)};
            for(int i=0;i<supplyPositions.Length;i++) Supply(supplyPositions[i],i%2==0,particles,impact);
            var hud=new GameObject("05_Interfaz",typeof(TowerLevelHUD));
            Set(hud.GetComponent<TowerLevelHUD>(),"playerHealth",player.GetComponent<Health>());
            Set(hud.GetComponent<TowerLevelHUD>(),"inventory",player.GetComponent<PlayerInventory>());
            Set(hud.GetComponent<TowerLevelHUD>(),"interactor",player.GetComponent<PlayerInteractor>());
            Set(hud.GetComponent<TowerLevelHUD>(),"objective",objective);Set(hud.GetComponent<TowerLevelHUD>(),"level",flow);
            Light sun=GameObject.Find("Directional Light").GetComponent<Light>();sun.transform.rotation=Quaternion.Euler(48,-30,0);
            sun.color=new Color(.69f,.78f,1f);sun.intensity=1.05f;sun.shadowStrength=.68f;
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.29f,.33f,.43f);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=35;RenderSettings.fogEndDistance=100;
            RenderSettings.fogColor=view.backgroundColor;
            var navigation=new GameObject("06_Navegacion",typeof(NavMeshSurface));
            NavMeshSurface surface=navigation.GetComponent<NavMeshSurface>();surface.collectObjects=CollectObjects.All;
            surface.layerMask=1<<3;surface.useGeometry=NavMeshCollectGeometry.PhysicsColliders;
            surface.ignoreNavMeshAgent=true;surface.ignoreNavMeshObstacle=true;surface.overrideVoxelSize=true;surface.voxelSize=.12f;
            Physics.SyncTransforms();surface.BuildNavMesh();
            string navPath=AssetDatabase.GenerateUniqueAssetPath(Output+"/Rutas.asset");AssetDatabase.CreateAsset(surface.navMeshData,navPath);
            var buildScenes=EditorBuildSettings.scenes.Where(s=>s.path!=ScenePath).ToList();buildScenes.Insert(0,new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=buildScenes.ToArray();
            OrganizeHierarchy(scene);
            AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(scene);
            SceneView.lastActiveSceneView?.LookAt(new Vector3(0,0,3),Quaternion.Euler(60,0,0),40);
            Selection.activeGameObject=player;
            Validate();
            CaptureViews();
            File.WriteAllText("Library/LevelDesign/build-result.json","{\"scene\":\""+ScenePath+"\",\"enemies\":10,\"supplies\":6,\"status\":\"built\"}");
            Debug.Log("TorreEclipse construida: planta baja, terraza, ascenso y suministros.");
        }
        catch(Exception exception) {File.WriteAllText("Library/LevelDesign/build-error.txt",exception.ToString());Debug.LogException(exception);}
    }
    private static Collider Area(string name,Vector3 position,Vector3 size)
    {
        var obj=new GameObject(name,typeof(BoxCollider));obj.transform.position=position;
        var box=obj.GetComponent<BoxCollider>();box.isTrigger=true;box.size=size;return box;
    }
    private static void Stair(string name,Vector3 position,Transform destination,EncounterObjective objective,TopDownCamera camera,bool ascending)
    {
        Collider area=Area(name,position,new Vector3(3.5f,3f,2f));
        TowerStairway stair=area.gameObject.AddComponent<TowerStairway>();Set(stair,"objective",objective);Set(stair,"destination",destination);Set(stair,"view",camera);Set(stair,"ascending",ascending);
    }
    private static void Supply(Vector3 position,bool crate,ParticleSystem particles,EventReference impact)
    {
        GameObject obj=K(crate?"CajaPocion":"BarrilPocion",crate?"detail-crate":"detail-barrel",position,new Vector3(1.25f,1.6f,1.25f),0,supplies);
        BoxCollider box=obj.AddComponent<BoxCollider>();box.center=new Vector3(0,.8f,0);box.size=new Vector3(1.25f,1.6f,1.25f);
        Health health=obj.AddComponent<Health>();Set(health,"maxHealth",25);
        DestructibleSupply supply=obj.AddComponent<DestructibleSupply>();
        Set(supply,"potionPrefab",AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Items/PotionPickup.prefab").GetComponent<PotionPickup>());
        Set(supply,"breakEffect",particles);Set(supply,"hitSound",impact);Set(supply,"breakSound",impact);
        NavMeshObstacle obstacle=obj.AddComponent<NavMeshObstacle>();obstacle.shape=NavMeshObstacleShape.Box;obstacle.center=box.center;
        obstacle.size=box.size;obstacle.carving=true;
    }
    private static ParticleSystem CreateBreakParticles()
    {
        var obj=new GameObject("Astillas",typeof(ParticleSystem));ParticleSystem system=obj.GetComponent<ParticleSystem>();
        system.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=system.main;main.loop=false;main.duration=.3f;main.startLifetime=.65f;main.startSpeed=2.8f;
        main.startSize=new ParticleSystem.MinMaxCurve(.08f,.16f);main.gravityModifier=1.2f;main.startColor=new Color(.5f,.3f,.16f);
        main.playOnAwake=true;main.stopAction=ParticleSystemStopAction.Destroy;
        var emission=system.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,18)});
        var shape=system.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=.3f;
        system.GetComponent<ParticleSystemRenderer>().sharedMaterial=Mat("Astillas",null,new Color(.55f,.35f,.18f));
        GameObject prefab=PrefabUtility.SaveAsPrefabAsset(obj,Output+"/Prefabs/Astillas.prefab");UnityEngine.Object.DestroyImmediate(obj);
        return prefab.GetComponent<ParticleSystem>();
    }
    private static void Set(UnityEngine.Object component,string name,object value)
    {
        FieldInfo field=component.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        if(field==null) throw new MissingFieldException(component.GetType().Name,name);
        field.SetValue(component,value);EditorUtility.SetDirty(component);
    }
    private static T Get<T>(UnityEngine.Object component,string name) => (T)component.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public).GetValue(component);
}
