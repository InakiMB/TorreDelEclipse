using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(400)]
public sealed class TowerAudioDirector : MonoBehaviour
{
    [SerializeField] private Health playerHealth = null;
    [SerializeField] private EnemyChaser[] enemies = new EnemyChaser[0];
    [SerializeField] private EnemyMage[] mages = new EnemyMage[0];
    [SerializeField] private EnemySupport[] supports = new EnemySupport[0];
    [SerializeField] private EncounterObjective objective = null;
    [SerializeField] private Transform interiorPoint = null;
    [SerializeField] private Transform exteriorPoint = null;
    [SerializeField] private Collider exteriorArea = null;
    [SerializeField] private EventReference ambienceEvent = default;
    [SerializeField] private EventReference musicEvent = default;
    [SerializeField] private EventReference accessEvent = default;
    [SerializeField] private EventReference victoryEvent = default;
    [SerializeField] private EventReference defeatEvent = default;
    [SerializeField, Min(0.1f)] private float zoneTransitionTime = 1.2f;
    [SerializeField, Min(0.1f)] private float combatTransitionTime = 0.8f;

    private EventInstance ambience;
    private EventInstance music;
    private bool started;
    private bool ready;
    private float zone;
    private float combat;
    private float musicGain = 1f;

    public float Zone => zone;
    public float Combat => combat;

    private void OnEnable()
    {
        if (objective != null) objective.StageChanged += OnStageChanged;
        if (started) StartAudio();
    }

    private void Start()
    {
        started = true;
        StartAudio();
    }

    private void OnDisable()
    {
        if (objective != null) objective.StageChanged -= OnStageChanged;
        StopAudio();
    }

    private void StartAudio()
    {
        StopAudio();
        if (playerHealth == null || objective == null || interiorPoint == null || exteriorPoint == null
            || ambienceEvent.IsNull || musicEvent.IsNull)
        {
            Debug.LogError("TowerAudioDirector necesita jugador, objetivo, puntos de ambiente y los dos eventos de loop.", this);
            return;
        }
        Vector3 span = Vector3.ProjectOnPlane(exteriorPoint.position - interiorPoint.position, Vector3.up);
        if (span.sqrMagnitude < 0.01f)
        {
            Debug.LogError("Los puntos Interior y Exterior deben estar separados sobre el piso.", this);
            return;
        }
        if (enemies.Length + mages.Length + supports.Length == 0) Debug.LogWarning("Asigna los enemigos para controlar la música de combate.", this);
        try
        {
            ambience = RuntimeManager.CreateInstance(ambienceEvent);
            music = RuntimeManager.CreateInstance(musicEvent);
            RequireParameter(ambience, "Zona");
            RequireParameter(music, "Combate");
            zone = CalculateZone();
            combat = HasEngagedEnemy() && !HasEnded() ? 1f : 0f;
            musicGain = HasEnded() ? 0.2f : 1f;
            ambience.setParameterByName("Zona", zone, true);
            music.setParameterByName("Combate", combat, true);
            music.setVolume(musicGain);
            if (ambience.start() != FMOD.RESULT.OK || music.start() != FMOD.RESULT.OK)
                throw new InvalidOperationException("No se pudieron iniciar los eventos de ambiente y música.");
            ready = true;
        }
        catch (Exception exception)
        {
            StopAudio();
            Debug.LogError("Revisa los bancos de FMOD y sus parámetros. " + exception.Message, this);
        }
    }

    private static void RequireParameter(EventInstance instance, string name)
    {
        if (instance.getDescription(out EventDescription description) != FMOD.RESULT.OK
            || description.getParameterDescriptionByName(name, out _) != FMOD.RESULT.OK)
            throw new InvalidOperationException("Falta el parámetro local " + name + " en su evento.");
    }

    private void LateUpdate()
    {
        if (!ready || Time.deltaTime <= 0f) return;
        bool ended = HasEnded();
        zone = Mathf.MoveTowards(zone, CalculateZone(), Time.deltaTime / zoneTransitionTime);
        float targetCombat = !ended && HasEngagedEnemy() ? 1f : 0f;
        combat = Mathf.MoveTowards(combat, targetCombat, Time.deltaTime / combatTransitionTime);
        musicGain = Mathf.MoveTowards(musicGain, ended ? 0.2f : 1f, Time.deltaTime);
        ambience.setParameterByName("Zona", zone);
        music.setParameterByName("Combate", combat);
        music.setVolume(musicGain);
    }

    private bool HasEnded()
    {
        return playerHealth.IsDead || objective.Stage == EncounterStage.Completed
            || objective.Stage == EncounterStage.Defeated;
    }

    private bool HasEngagedEnemy()
    {
        foreach (EnemyChaser enemy in enemies)
            if (enemy != null && enemy.IsEngaged) return true;
        foreach (EnemyMage mage in mages)
            if (mage != null && mage.IsEngaged) return true;
        foreach (EnemySupport support in supports)
            if (support != null && support.IsEngaged) return true;
        return false;
    }

    private float CalculateZone()
    {
        if (exteriorArea != null) return (exteriorArea.ClosestPoint(playerHealth.transform.position) - playerHealth.transform.position).sqrMagnitude < 0.001f ? 1f : 0f;
        Vector3 span = Vector3.ProjectOnPlane(exteriorPoint.position - interiorPoint.position, Vector3.up);
        Vector3 position = Vector3.ProjectOnPlane(playerHealth.transform.position - interiorPoint.position, Vector3.up);
        return Mathf.Clamp01(Vector3.Dot(position, span) / span.sqrMagnitude);
    }

    private void OnStageChanged(EncounterStage stage)
    {
        Vector3 position = playerHealth != null ? playerHealth.transform.position : transform.position;
        switch (stage)
        {
            case EncounterStage.ExitUnlocked: FmodOneShot.Play(accessEvent, position); break;
            case EncounterStage.Completed: FmodOneShot.Play(victoryEvent, position); break;
            case EncounterStage.Defeated: FmodOneShot.Play(defeatEvent, position); break;
        }
    }

    private void StopAudio()
    {
        ready = false;
        StopAndRelease(ref ambience);
        StopAndRelease(ref music);
    }

    private static void StopAndRelease(ref EventInstance instance)
    {
        if (!instance.isValid()) return;
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
        instance.clearHandle();
    }

    private void OnDrawGizmosSelected()
    {
        if (interiorPoint == null || exteriorPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(interiorPoint.position, exteriorPoint.position);
        Gizmos.DrawWireSphere(interiorPoint.position, 0.4f);
        Gizmos.DrawWireSphere(exteriorPoint.position, 0.4f);
    }
}
