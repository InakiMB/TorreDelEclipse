using UnityEngine;
using UnityEngine.UI;

public sealed class EclipseHUD : MonoBehaviour
{
    public Health player;
    public PlayerInventory inventory;
    public PlayerDodge dodge;
    public PlayerInteractor interactor;
    public EncounterObjective objective;
    public TowerLevelFlow level;
    public BossEncounter encounter;
    public EclipseGraphic blood,rollRing,bossFill;
    public Text life,potions,location,goal,bossPhase,prompt;
    public GameObject bossBar,promptPanel;
    public CanvasGroup hotbar;
    public Image potionIcon,rollIcon;
    public Health[] commonEnemies;
    Camera view;
    RectTransform hotbarRect;
    float shownHealth=1;
    void Start(){view=Camera.main;hotbarRect=hotbar.GetComponent<RectTransform>();}
    void LateUpdate()
    {
        if(player==null)return;
        shownHealth=Mathf.MoveTowards(shownHealth,(float)player.CurrentHealth/player.MaxHealth,Time.unscaledDeltaTime*2);
        blood.SetAmount(shownHealth);life.text=player.CurrentHealth+"/"+player.MaxHealth;
        potions.text=inventory!=null?inventory.PotionCount+"/"+inventory.MaxPotions:"0/5";
        potionIcon.color=inventory!=null&&inventory.PotionCount>0?Color.white:new Color(.5f,.45f,.45f);
        if(dodge!=null){rollRing.SetAmount(dodge.CooldownProgress);rollIcon.color=dodge.IsBusy||dodge.CooldownRemaining>0?new Color(.52f,.52f,.58f):Color.white;}
        bool terrace=level!=null&&level.IsOnTerrace;
        location.text=terrace?"TERRAZA DEL ECLIPSE":"PLANTA BAJA";
        goal.text=objective.Stage==EncounterStage.Fighting?"Despejá la torre · "+objective.RemainingEnemies+" restantes":terrace?"Entrá en el círculo":"Ascendé por el santuario";
        var boss=encounter!=null?encounter.Boss:null;
        bool bossActive=boss!=null&&!encounter.Completed&&!player.IsDead;
        bossBar.SetActive(bossActive);
        if(bossActive){bossFill.SetAmount((float)boss.Health.CurrentHealth/boss.Health.MaxHealth);bossPhase.text=boss.Phase==2?"FASE II":"FASE I";goal.text="Derrotá al Custodio";}
        string message=interactor!=null&&interactor.enabled?interactor.CurrentPrompt:"";
        if(inventory!=null&&Time.time<inventory.MessageUntil)message=inventory.Message;
        promptPanel.SetActive(!string.IsNullOrEmpty(message)&&!player.IsDead);prompt.text=message;
        bool overlaps=IsBehind(player.transform.position+Vector3.up);
        if(bossActive)overlaps|=IsBehind(boss.transform.position+Vector3.up*2);
        if(!overlaps&&commonEnemies!=null)foreach(var enemy in commonEnemies)if(enemy!=null&&!enemy.IsDead&&IsBehind(enemy.transform.position+Vector3.up)){overlaps=true;break;}
        hotbar.alpha=Mathf.MoveTowards(hotbar.alpha,overlaps ? .28f : 1f,Time.unscaledDeltaTime*5);
    }
    bool IsBehind(Vector3 point)
    {
        if(view==null||hotbarRect==null)return false;Vector3 screen=view.WorldToScreenPoint(point);
        var canvas=hotbarRect.GetComponentInParent<Canvas>();var uiCamera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
        return screen.z>0&&RectTransformUtility.RectangleContainsScreenPoint(hotbarRect,screen,uiCamera);
    }
}
