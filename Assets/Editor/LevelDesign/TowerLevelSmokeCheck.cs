using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class TowerLevelSmokeCheck
{
    [Serializable] private class Report { public List<string> passed=new List<string>();public List<string> failed=new List<string>();public bool complete; }
    private static Report report;
    private static double since;
    private static int phase;
    private static int potionsBefore;

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
        if(SessionState.GetBool("TowerSmoke.Active",false))
        {
            report=new Report();phase=0;since=EditorApplication.timeSinceStartup;
        }
    }
    [MenuItem("Tools/Torre del Eclipse/Verificar jugabilidad")]
    public static void Begin()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().path!=TowerLevelBuilder.ScenePath)
            throw new InvalidOperationException("Abrir TorreEclipse fuera de Play.");
        Directory.CreateDirectory("Library/LevelDesign");report=new Report();phase=0;since=EditorApplication.timeSinceStartup;
        SessionState.SetBool("TowerSmoke.Active",true);EditorApplication.isPlaying=true;
    }
    [MenuItem("Tools/Torre del Eclipse/Reconstruir y verificar")]
    public static void BuildAndCheck()
    {
        TowerLevelBuilder.Build();
        EditorApplication.delayCall+=Begin;
    }
    private static bool CanReach(GameObject actor,GameObject item)
    {
        Collider area=item.GetComponent<Collider>();
        Vector3 point=area.ClosestPoint(actor.transform.position);
        bool line=(bool)typeof(PlayerInteractor).GetMethod("CanReach",BindingFlags.Instance|BindingFlags.NonPublic)
            .Invoke(actor.GetComponent<PlayerInteractor>(),new object[]{area.bounds.center,item.transform});
        return line && Vector3.Distance(point,actor.transform.position)<=1.8f;
    }
    private static void Check(bool condition,string name)
    {
        (condition?report.passed:report.failed).Add(name);
    }
    private static void Next(int next){phase=next;since=EditorApplication.timeSinceStartup;Save();}
    private static void Save()=>File.WriteAllText("Library/LevelDesign/smoke-result.json",JsonUtility.ToJson(report,true));
    private static T[] All<T>() where T:UnityEngine.Object => UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
    private static void Tick()
    {
        if(!SessionState.GetBool("TowerSmoke.Active",false)||!EditorApplication.isPlaying||EditorApplication.isPaused)return;
        if(report==null)report=new Report();
        double elapsed=EditorApplication.timeSinceStartup-since;
        try
        {
            GameObject player=GameObject.FindGameObjectWithTag("Player");
            if(player==null){if(elapsed>20)throw new InvalidOperationException("No aparece jugador.");return;}
            EncounterObjective objective=UnityEngine.Object.FindFirstObjectByType<EncounterObjective>();
            if(phase==0 && elapsed>1.5)
            {
                Check(All<EnemyChaser>().Length==10 && All<NavMeshAgent>().All(a=>a.isOnNavMesh),"Diez enemigos sobre NavMesh");
                Check(objective.Stage==EncounterStage.Fighting && objective.RemainingEnemies==10,"Inicio con ascenso bloqueado");
                foreach(EnemyChaser enemy in All<EnemyChaser>())enemy.enabled=false;
                Vector3 before=player.transform.position;
                GameObject.Find("ExitMarker").GetComponent<TowerStairway>().Interact(player);
                Check(Vector3.Distance(player.transform.position,before)<.1f,"No se puede ascender antes de limpiar");
                potionsBefore=All<PotionPickup>().Length;
                GameObject projectile=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/MagicBolt.prefab");
                for(int i=0;i<2;i++)
                {
                    GameObject shot=UnityEngine.Object.Instantiate(projectile,new Vector3(-8,1.2f,-21),Quaternion.identity);
                    shot.GetComponent<MagicProjectile>().Initialize(player.transform,Vector3.forward,25);
                }
                Next(1);
            }
            else if(phase==1 && elapsed>.8)
            {
                Check(All<DestructibleSupply>().Length==5,"Los proyectiles rompen una caja");
                Check(All<PotionPickup>().Length==potionsBefore+1,"Dos impactos producen exactamente una pocion");
                PotionPickup potion=All<PotionPickup>().FirstOrDefault();
                if(potion!=null)
                {
                    potion.Interact(player);
                    PlayerInventory inventory=player.GetComponent<PlayerInventory>();
                    Check(inventory.PotionCount==1,"La pocion del suministro se puede recoger");
                    player.GetComponent<Health>().TakeDamage(20);
                    Check(inventory.TryUsePotion()&&inventory.PotionCount==0&&player.GetComponent<Health>().CurrentHealth==100,"La pocion recupera vida sin superar el maximo");
                }
                foreach(EnemyChaser enemy in All<EnemyChaser>())enemy.GetComponent<Health>().TakeDamage(999);
                Next(2);
            }
            else if(phase==2 && elapsed>.5)
            {
                Check(objective.Stage==EncounterStage.ExitUnlocked && objective.RemainingEnemies==0,"Limpiar la planta baja desbloquea ascenso");
                Check(!GameObject.FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t=>t.name=="GateBlocker"&&t.gameObject.activeInHierarchy),"El sello deja de bloquear la escalera");
                CharacterController controller=player.GetComponent<CharacterController>();controller.enabled=false;
                player.transform.position=new Vector3(0,1.1f,25.5f);controller.enabled=true;Physics.SyncTransforms();
                Check(CanReach(player,GameObject.Find("ExitMarker")),"La interaccion de ascenso tiene alcance y linea de vision");
                GameObject.Find("ExitMarker").GetComponent<TowerStairway>().Interact(player);
                Check(Vector3.Distance(player.transform.position,new Vector3(100,13.1f,-10.5f))<.2f,"Ascenso transporta al punto seguro de terraza");
                Check(Vector3.Distance(Camera.main.transform.position,player.transform.position+new Vector3(0,17,-9.8f))<.2f,"La camara acompana el ascenso sin barrer el vacio");
                Next(3);
            }
            else if(phase==3 && elapsed>2)
            {
                TowerLevelFlow flow=UnityEngine.Object.FindFirstObjectByType<TowerLevelFlow>();
                Check(flow.IsOnTerrace&&flow.ReachedTerrace,"La terraza se reconoce como segunda zona");
                Check(objective.Stage==EncounterStage.ExitUnlocked&&player.GetComponent<PlayerMovement>().enabled,"Llegar a terraza mantiene el control y no declara victoria");
                Check(UnityEngine.Object.FindFirstObjectByType<TowerAudioDirector>().Zone>.95f,"El ambiente pasa al exterior");
                Check(CanReach(player,GameObject.Find("ReturnMarker")),"La interaccion de regreso tiene alcance y linea de vision");
                GameObject.Find("ReturnMarker").GetComponent<TowerStairway>().Interact(player);
                Check(Vector3.Distance(player.transform.position,new Vector3(0,1.1f,26))<.2f,"Se puede regresar a planta baja");
                player.GetComponent<Health>().TakeDamage(999);Next(4);
            }
            else if(phase==4 && elapsed>.4)
            {
                Check(objective.Stage==EncounterStage.Defeated&&!player.GetComponent<PlayerMovement>().enabled,"La derrota detiene el jugador");
                TowerLevelHUD hud=UnityEngine.Object.FindFirstObjectByType<TowerLevelHUD>();
                typeof(TowerLevelHUD).GetMethod("Restart",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(hud,null);Next(5);
            }
            else if(phase==5 && elapsed>1.5)
            {
                Check(All<EnemyChaser>().Length==10&&All<DestructibleSupply>().Length==6,"Reiniciar restaura enemigos y suministros");
                Check(All<PotionPickup>().Length==0&&player.GetComponent<PlayerInventory>().PotionCount==0,"Reiniciar limpia el loot y el inventario");
                Check(player.GetComponent<Health>().CurrentHealth==100&&objective.Stage==EncounterStage.Fighting,"Reiniciar restaura salud y objetivo");
                Check(!UnityEngine.Object.FindFirstObjectByType<TowerLevelFlow>().ReachedTerrace,"Reiniciar restablece la progresion");
                RuntimeManager.GetEventDescription("event:/AMB/Torre").getInstanceCount(out int ambience);
                RuntimeManager.GetEventDescription("event:/MUS/Torre").getInstanceCount(out int music);
                Check(ambience==1&&music==1,"Reiniciar conserva un solo loop de ambiente y musica");
                Check(UnityEngine.Object.FindFirstObjectByType<TowerAudioDirector>().Zone<.05f,"El ambiente vuelve al interior");
                report.complete=true;Save();SessionState.SetBool("TowerSmoke.Active",false);
                Debug.Log("Verificacion del nivel: "+report.passed.Count+" controles correctos, "+report.failed.Count+" fallos.");
                EditorApplication.isPlaying=false;
            }
        }
        catch(Exception exception)
        {
            report.failed.Add(exception.ToString());report.complete=true;Save();SessionState.SetBool("TowerSmoke.Active",false);
            Debug.LogException(exception);EditorApplication.isPlaying=false;
        }
    }
}
