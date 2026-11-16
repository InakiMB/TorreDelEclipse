using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BossController))]
public sealed class BossAudio : MonoBehaviour
{
    [Serializable] public struct Sound { public BossCue cue; public EventReference sound; }
    [SerializeField] private EventReference musicEvent;
    [SerializeField] private Sound[] sounds = new Sound[0];
    private BossController boss;
    private EventInstance music;
    private float nextImpact, nextHurt, phaseMix, desiredPhase, fadeAt = -1f;
    private void Awake() => boss = GetComponent<BossController>();
    private void OnEnable() { boss.Cue += OnCue; boss.ActionChanged += OnAction; }
    private void OnDisable() { boss.Cue -= OnCue; boss.ActionChanged -= OnAction; StopMusic(); }
    private void OnAction(BossAction action)
    {
        if (action == BossAction.Summon && !musicEvent.IsNull)
        {
            StopMusic(); phaseMix = desiredPhase = 0; fadeAt = -1;
            music = RuntimeManager.CreateInstance(musicEvent); music.setParameterByName("Fase", 0, true); music.start();
        }
        if (action == BossAction.Transform) desiredPhase = 1;
        if (action == BossAction.Dying) fadeAt = Time.unscaledTime;
        if (action == BossAction.Dead || action == BossAction.Defeated) StopMusic();
    }
    private void Update()
    {
        if (!music.isValid()) return;
        phaseMix = Mathf.MoveTowards(phaseMix, desiredPhase, Time.unscaledDeltaTime / .8f);
        music.setParameterByName("Fase", phaseMix);
        if (fadeAt < 0) return;
        float volume = 1 - Mathf.Clamp01((Time.unscaledTime - fadeAt) / .8f);
        music.setVolume(volume);
        if (volume <= 0) StopMusic();
    }
    private void OnCue(BossCue cue, Vector3 point)
    {
        if (cue == BossCue.OrbImpact && Time.time < nextImpact || cue == BossCue.Hurt && Time.time < nextHurt) return;
        if (cue == BossCue.OrbImpact) nextImpact = Time.time + 0.12f;
        if (cue == BossCue.Hurt) nextHurt = Time.time + 0.15f;
        foreach (Sound entry in sounds) if (entry.cue == cue) { FmodOneShot.Play(entry.sound, point); return; }
    }
    private void StopMusic()
    {
        if (!music.isValid()) return;
        music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); music.release(); music.clearHandle();
    }
}
