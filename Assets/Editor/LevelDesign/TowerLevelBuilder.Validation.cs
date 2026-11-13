using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static partial class TowerLevelBuilder
{
    [Serializable] private class ValidationReport
    {
        public string scene;
        public int enemies;
        public int supplies;
        public int renderers;
        public int reachableEnemies;
        public bool stairsReachable;
        public bool terraceReachable;
        public List<string> issues=new List<string>();
    }
    [MenuItem("Tools/Torre del Eclipse/Validar nivel")]
    public static void Validate()
    {
        var report=new ValidationReport {scene=SceneManager.GetActiveScene().path};
        GameObject player=GameObject.FindGameObjectWithTag("Player");
        EnemyChaser[] enemies=UnityEngine.Object.FindObjectsByType<EnemyChaser>(FindObjectsSortMode.None);
        report.enemies=enemies.Length;report.supplies=UnityEngine.Object.FindObjectsByType<DestructibleSupply>(FindObjectsSortMode.None).Length;
        Renderer[] renderers=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);report.renderers=renderers.Length;
        foreach(Renderer renderer in renderers)
        foreach(Material material in renderer.sharedMaterials)
            if(material==null || material.shader==null || material.shader.name=="Hidden/InternalErrorShader") report.issues.Add("Material: "+renderer.name);
        if(player!=null)
        {
            foreach(EnemyChaser enemy in enemies)
            {
                if(Path(player.transform.position,enemy.transform.position)) report.reachableEnemies++;
                else report.issues.Add("Sin ruta al enemigo: "+enemy.name);
            }
            report.stairsReachable=Path(player.transform.position,new Vector3(0,0,27));
            report.terraceReachable=Path(new Vector3(100,12,-10.5f),new Vector3(100,12,6));
            if(!report.stairsReachable)report.issues.Add("Sin ruta al acceso de escalera.");
            if(!report.terraceReachable)report.issues.Add("Sin ruta en la terraza.");
            if(Physics.CheckCapsule(player.transform.position-Vector3.up*.5f,player.transform.position+Vector3.up*.5f,.44f,1<<3,QueryTriggerInteraction.Ignore))
                report.issues.Add("El punto de inicio se solapa con arquitectura.");
        }
        else report.issues.Add("Falta jugador.");
        if(report.enemies!=10)report.issues.Add("Se esperaban diez perseguidores de prueba.");
        if(report.supplies!=6)report.issues.Add("Se esperaban seis suministros.");
        foreach(MonoBehaviour component in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            if(component==null)report.issues.Add("Componente ausente.");
        Directory.CreateDirectory("Library/LevelDesign");
        File.WriteAllText("Library/LevelDesign/validation.json",JsonUtility.ToJson(report,true));
        Debug.Log("Revision del nivel: "+report.reachableEnemies+"/"+report.enemies+" enemigos accesibles; "+report.issues.Count+" observaciones.");
    }
    private static bool Path(Vector3 from,Vector3 to)
    {
        if(!NavMesh.SamplePosition(from,out NavMeshHit start,3f,NavMesh.AllAreas)
            || !NavMesh.SamplePosition(to,out NavMeshHit end,3f,NavMesh.AllAreas))return false;
        var path=new NavMeshPath();return NavMesh.CalculatePath(start.position,end.position,NavMesh.AllAreas,path)&&path.status==NavMeshPathStatus.PathComplete;
    }

    [MenuItem("Tools/Torre del Eclipse/Capturar vistas")]
    public static void CaptureViews()
    {
        Camera camera=Camera.main;
        Vector3 oldPosition=camera.transform.position;Quaternion oldRotation=camera.transform.rotation;
        float oldSize=camera.orthographicSize;
        var shots=new[]{
            new {name="01_Entrada",point=new Vector3(0,0,-17),size=12f,angle=60f},
            new {name="02_GranSalon",point=new Vector3(0,0,1),size=16f,angle=60f},
            new {name="03_Santuario",point=new Vector3(0,0,22),size=12f,angle=60f},
            new {name="04_Terraza",point=new Vector3(100,12,0),size=21f,angle=60f},
            new {name="05_PlantaGeneral",point=new Vector3(0,0,0),size=38f,angle=82f}};
        Directory.CreateDirectory("Library/LevelDesign/Previews");
        try
        {
            foreach(var shot in shots)
            {
                camera.transform.rotation=Quaternion.Euler(shot.angle,0,0);
                camera.transform.position=shot.point-camera.transform.forward*55f;
                camera.orthographicSize=shot.size;
                camera.ResetWorldToCameraMatrix();camera.ResetProjectionMatrix();
                var target=new RenderTexture(1600,1100,24,RenderTextureFormat.ARGB32);
                camera.targetTexture=target;camera.Render();
                RenderTexture previous=RenderTexture.active;RenderTexture.active=target;
                var texture=new Texture2D(1600,1100,TextureFormat.RGB24,false);
                texture.ReadPixels(new Rect(0,0,1600,1100),0,0);texture.Apply();
                File.WriteAllBytes("Library/LevelDesign/Previews/"+shot.name+".png",texture.EncodeToPNG());
                RenderTexture.active=previous;camera.targetTexture=null;target.Release();
                UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(texture);
            }
        }
        finally{camera.targetTexture=null;camera.transform.SetPositionAndRotation(oldPosition,oldRotation);camera.orthographicSize=oldSize;}
        Debug.Log("Vistas guardadas en Library/LevelDesign/Previews.");
    }
}
