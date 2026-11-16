using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-900)]
public sealed class EclipseGameUI : MonoBehaviour
{
    public enum ScreenState { Playing, Main, Pause, Audio, Defeat, Victory, Credits, Sources, Confirm, Loading }
    public static EclipseGameUI Active { get; private set; }
    public static bool BlocksGameplay => Active!=null && (Active.State!=ScreenState.Playing || Time.unscaledTime<Active.inputReadyAt);
    public ScreenState State { get; private set; }
    public bool mainMenuScene;
    public EclipseUITheme theme;
    public Health playerHealth;
    public EncounterObjective objective;
    public BossEncounter encounter;
    public AudioVolumeSettings audioSettings;
    public GameObject hud,main,pause,audio,defeat,victory,credits,sources,confirm,loading;
    public Text defeatHint,confirmTitle,confirmHint;
    public Text[] volumeLabels=new Text[5];
    public Slider[] volumeSliders=new Slider[5];
    public Button[] focusButtons=new Button[9];
    ScreenState audioOrigin,confirmOrigin;
    Action confirmedAction;
    float inputReadyAt,endedAt=-1,hoverReadyAt;
    bool loadingScene,audioPaused;
    bool effectsWerePaused,ambienceWasPaused;
    Bus effectsBus,ambienceBus;
    EventInstance menuMusic;
    const string LevelPath="Assets/Scenes/TorreEclipse.unity";
    const string MainPath="Assets/Scenes/MenuPrincipal.unity";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics(){Active=null;Time.timeScale=1;}
    void Awake(){Active=this;}
    void Start()
    {
        SetScreen(mainMenuScene?ScreenState.Main:ScreenState.Playing);
        if(mainMenuScene)StartCoroutine(StartMenuMusic());
    }
    IEnumerator StartMenuMusic()
    {
        yield return null;
        if(theme==null||theme.menuMusic.IsNull)yield break;
        menuMusic=RuntimeManager.CreateInstance(theme.menuMusic);
        menuMusic.setParameterByName("Combate",0,true);menuMusic.start();
    }
    void Update()
    {
        if(loadingScene)return;
        if(State==ScreenState.Playing)
        {
            bool dead=playerHealth!=null&&playerHealth.IsDead;
            bool won=objective!=null&&objective.Stage==EncounterStage.Completed;
            if(dead||won)
            {
                if(endedAt<0)endedAt=Time.unscaledTime;
                if(Time.unscaledTime-endedAt >= (dead ? .85f : 1f))
                {
                    if(dead && defeatHint!=null)defeatHint.text=encounter!=null&&encounter.CanRetry
                        ?"Reintentá desde la terraza.\nRecuperás la vida y las pociones del punto de control."
                        :"Volvés a la entrada de la torre para intentarlo de nuevo.";
                    SetScreen(dead?ScreenState.Defeat:ScreenState.Victory);
                }
                return;
            }
            endedAt=-1;
        }
        var keyboard=Keyboard.current;if(keyboard==null)return;
        if(keyboard.escapeKey.wasPressedThisFrame)
        {
            switch(State)
            {
                case ScreenState.Playing: OpenPause();break;
                case ScreenState.Pause: ResumeGame();break;
                case ScreenState.Audio: CloseAudio();break;
                case ScreenState.Sources: SetScreen(ScreenState.Credits);break;
                case ScreenState.Credits: GoMainMenu();break;
                case ScreenState.Confirm: CancelConfirm();break;
            }
        }
        if(keyboard.rKey.wasPressedThisFrame)
        {
            if(State==ScreenState.Defeat)Retry();
            else if(State==ScreenState.Playing)AskRestart();
        }
    }
    void OnDestroy()
    {
        if(menuMusic.isValid()){menuMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);menuMusic.release();}
        SetAudioPaused(false);
        if(Active==this){Active=null;Time.timeScale=1;}
    }
    public void StartGame(){Load(LevelPath);}
    public void OpenPause(){if(State==ScreenState.Playing&&endedAt<0)SetScreen(ScreenState.Pause);}
    public void ResumeGame(){inputReadyAt=Time.unscaledTime+.15f;SetScreen(ScreenState.Playing);}
    public void OpenAudio(){audioOrigin=State;RefreshAudio();SetScreen(ScreenState.Audio);}
    public void CloseAudio(){audioSettings?.Save();SetScreen(audioOrigin);}
    public void ShowCredits(){SetScreen(ScreenState.Credits);}
    public void ContinueToCredits(){SetScreen(ScreenState.Credits);}
    public void OpenSources(){SetScreen(ScreenState.Sources);}
    public void CloseSources(){SetScreen(ScreenState.Credits);}
    public void OpenSourceRegister(){Application.OpenURL("https://github.com/InakiMB/TorreDelEclipse/blob/main/Docs/Fuentes.md");}
    public void OpenDungeonSource(){Application.OpenURL("https://blendervoyage.itch.io/psx-style-modular-low-poly-dungeon");}
    public void OpenDungeonLicense(){Application.OpenURL("https://creativecommons.org/licenses/by/4.0/");}
    public void OpenMusicSource(){Application.OpenURL("https://opengameart.org/content/colossal-boss-battle-theme");}
    public void OpenMusicLicense(){Application.OpenURL("https://creativecommons.org/licenses/by/3.0/");}
    public void GoMainMenu(){if(mainMenuScene)SetScreen(ScreenState.Main);else Load(MainPath);}
    public void AskMainMenu()
    {
        if(State==ScreenState.Pause)Ask("¿Volver al menú principal?","Se perderá el progreso de esta partida.",GoMainMenu);
        else GoMainMenu();
    }
    public void AskRestart(){Ask("¿Reiniciar el nivel?","Volverás al comienzo de la planta baja.",()=>Load(LevelPath));}
    public void AskQuit()
    {
        if(mainMenuScene)Quit();else Ask("¿Salir del juego?","Se perderá el progreso de esta partida.",Quit);
    }
    static void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
        #else
        Application.Quit();
        #endif
    }
    void Ask(string title,string hint,Action action)
    {
        confirmOrigin=State;confirmedAction=action;confirmTitle.text=title;confirmHint.text=hint;SetScreen(ScreenState.Confirm);
    }
    public void AcceptConfirm(){var action=confirmedAction;confirmedAction=null;action?.Invoke();}
    public void CancelConfirm(){confirmedAction=null;inputReadyAt=Time.unscaledTime+.15f;SetScreen(confirmOrigin);}
    public void Retry()
    {
        if(State!=ScreenState.Defeat)return;
        Time.timeScale=1;SetAudioPaused(false);
        if(encounter!=null&&encounter.Retry())
        {
            endedAt=-1;inputReadyAt=Time.unscaledTime+.15f;SetScreen(ScreenState.Playing);return;
        }
        Load(LevelPath);
    }
    void Load(string path)
    {
        if(loadingScene)return;
        if(!Application.CanStreamedLevelBeLoaded(path)){Debug.LogError("Falta la escena "+path+" en la lista de escenas.",this);return;}
        loadingScene=true;Time.timeScale=1;SetAudioPaused(false);SetScreen(ScreenState.Loading);
        SceneManager.LoadSceneAsync(path);
    }
    public void RestoreAudio(){audioSettings?.RestoreDefaults();RefreshAudio();}
    public void SetMaster(float value){audioSettings?.SetMasterVolume(value);Percent(0,value);}
    public void SetMusic(float value){audioSettings?.SetMusicVolume(value);Percent(1,value);}
    public void SetAmbience(float value){audioSettings?.SetAmbienceVolume(value);Percent(2,value);}
    public void SetEffects(float value){audioSettings?.SetEffectsVolume(value);Percent(3,value);}
    public void SetInterface(float value){audioSettings?.SetInterfaceVolume(value);Percent(4,value);}
    void Percent(int index,float value){if(volumeLabels[index]!=null)volumeLabels[index].text=Mathf.RoundToInt(value*100)+"%";}
    void RefreshAudio()
    {
        if(audioSettings==null)return;
        float[] values={audioSettings.MasterVolume,audioSettings.MusicVolume,audioSettings.AmbienceVolume,audioSettings.EffectsVolume,audioSettings.InterfaceVolume};
        for(int i=0;i<values.Length;i++){volumeSliders[i].SetValueWithoutNotify(values[i]);Percent(i,values[i]);}
    }
    public void PlayHoverSound()
    {
        if(Time.unscaledTime<hoverReadyAt||theme==null||theme.hoverSound.IsNull)return;
        hoverReadyAt=Time.unscaledTime+.10f;RuntimeManager.PlayOneShot(theme.hoverSound);
    }
    public void PlayClickSound(){if(theme!=null&&!theme.clickSound.IsNull)RuntimeManager.PlayOneShot(theme.clickSound);}
    void SetScreen(ScreenState next)
    {
        State=next;
        Set(hud,next==ScreenState.Playing);Set(main,next==ScreenState.Main||(mainMenuScene&&next==ScreenState.Audio));Set(pause,next==ScreenState.Pause);
        if(main!=null)main.transform.Find("Contenido").gameObject.SetActive(next==ScreenState.Main);
        Set(audio,next==ScreenState.Audio);Set(defeat,next==ScreenState.Defeat);Set(victory,next==ScreenState.Victory);
        Set(credits,next==ScreenState.Credits);Set(sources,next==ScreenState.Sources);Set(confirm,next==ScreenState.Confirm);Set(loading,next==ScreenState.Loading);
        bool frozen=!mainMenuScene&&next!=ScreenState.Playing&&next!=ScreenState.Loading;
        Time.timeScale=frozen?0:1;SetAudioPaused(frozen);
        Cursor.visible=true;Cursor.lockState=CursorLockMode.None;
        if(EventSystem.current!=null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            int index=next==ScreenState.Main?0:next==ScreenState.Pause?1:next==ScreenState.Audio?2:next==ScreenState.Defeat?3:next==ScreenState.Victory?4:next==ScreenState.Credits?5:next==ScreenState.Sources?6:next==ScreenState.Confirm?7:-1;
            if(index>=0&&focusButtons[index]!=null)focusButtons[index].Select();
        }
    }
    static void Set(GameObject obj,bool active){if(obj!=null)obj.SetActive(active);}
    void SetAudioPaused(bool paused)
    {
        if(audioPaused==paused)return;
        if(paused)
        {
            effectsBus=RuntimeManager.GetBus("bus:/Efectos");ambienceBus=RuntimeManager.GetBus("bus:/Ambiente");
            effectsBus.getPaused(out effectsWerePaused);ambienceBus.getPaused(out ambienceWasPaused);
            effectsBus.setPaused(true);ambienceBus.setPaused(true);audioPaused=true;
        }
        else
        {
            if(effectsBus.isValid())effectsBus.setPaused(effectsWerePaused);
            if(ambienceBus.isValid())ambienceBus.setPaused(ambienceWasPaused);
            audioPaused=false;
        }
    }
}
