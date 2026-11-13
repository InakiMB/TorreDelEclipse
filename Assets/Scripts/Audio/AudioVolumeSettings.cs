using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-100)]
public sealed class AudioVolumeSettings : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float ambienceVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float interfaceVolume = 1f;

    private const string Prefix = "TorreDelEclipse.Audio.";
    private Bus master;
    private Bus music;
    private Bus ambience;
    private Bus effects;
    private Bus userInterface;
    private bool ready;
    private bool preferencesDirty;
    private float lastMaster;
    private float lastMusic;
    private float lastAmbience;
    private float lastEffects;
    private float lastInterface;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float AmbienceVolume => ambienceVolume;
    public float EffectsVolume => effectsVolume;
    public float InterfaceVolume => interfaceVolume;

    private void Awake()
    {
        masterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Master", masterVolume));
        musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Music", musicVolume));
        ambienceVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Ambience", ambienceVolume));
        effectsVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Effects", effectsVolume));
        interfaceVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(Prefix + "Interface", interfaceVolume));
    }

    private void Start()
    {
        try
        {
            master = RuntimeManager.GetBus("bus:/");
            music = RuntimeManager.GetBus("bus:/Musica");
            ambience = RuntimeManager.GetBus("bus:/Ambiente");
            effects = RuntimeManager.GetBus("bus:/Efectos");
            userInterface = RuntimeManager.GetBus("bus:/Interfaz");
            ready = true;
            ApplyAndStore();
        }
        catch (Exception exception)
        {
            ready = false;
            Debug.LogError("Revisa los grupos Musica, Ambiente, Efectos e Interfaz en el Mixer de FMOD y reconstruye los bancos. "
                + exception.Message, this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!ready) return;
        if (masterVolume != lastMaster || musicVolume != lastMusic || ambienceVolume != lastAmbience
            || effectsVolume != lastEffects || interfaceVolume != lastInterface) ApplyAndStore();
    }

    public void SetMasterVolume(float value) { masterVolume = Mathf.Clamp01(value); ApplyAndStore(); }
    public void SetMusicVolume(float value) { musicVolume = Mathf.Clamp01(value); ApplyAndStore(); }
    public void SetAmbienceVolume(float value) { ambienceVolume = Mathf.Clamp01(value); ApplyAndStore(); }
    public void SetEffectsVolume(float value) { effectsVolume = Mathf.Clamp01(value); ApplyAndStore(); }
    public void SetInterfaceVolume(float value) { interfaceVolume = Mathf.Clamp01(value); ApplyAndStore(); }

    private void ApplyAndStore()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        musicVolume = Mathf.Clamp01(musicVolume);
        ambienceVolume = Mathf.Clamp01(ambienceVolume);
        effectsVolume = Mathf.Clamp01(effectsVolume);
        interfaceVolume = Mathf.Clamp01(interfaceVolume);
        if (ready)
        {
            master.setVolume(masterVolume);
            music.setVolume(musicVolume);
            ambience.setVolume(ambienceVolume);
            effects.setVolume(effectsVolume);
            userInterface.setVolume(interfaceVolume);
        }
        lastMaster = masterVolume;
        lastMusic = musicVolume;
        lastAmbience = ambienceVolume;
        lastEffects = effectsVolume;
        lastInterface = interfaceVolume;
        PlayerPrefs.SetFloat(Prefix + "Master", masterVolume);
        PlayerPrefs.SetFloat(Prefix + "Music", musicVolume);
        PlayerPrefs.SetFloat(Prefix + "Ambience", ambienceVolume);
        PlayerPrefs.SetFloat(Prefix + "Effects", effectsVolume);
        PlayerPrefs.SetFloat(Prefix + "Interface", interfaceVolume);
        preferencesDirty = true;
    }

    public void Save()
    {
        if (!preferencesDirty) return;
        PlayerPrefs.Save();
        preferencesDirty = false;
    }

    [ContextMenu("Restaurar volumenes al 100%")]
    public void RestoreDefaults()
    {
        masterVolume = musicVolume = ambienceVolume = effectsVolume = interfaceVolume = 1f;
        ApplyAndStore();
        Save();
    }

    private void OnDisable() => Save();
    private void OnApplicationQuit() => Save();
    private void OnApplicationPause(bool paused)
    {
        if (paused) Save();
    }
}
