using UnityEngine;

public sealed class EnemyHealthBar : MonoBehaviour
{
    public Health target;
    public EclipseGraphic bar;
    public RectTransform canvasRect;
    IEnemyCombatant combatant;
    Camera view;
    RectTransform position;
    Renderer[] renderers;
    float shown=1,noticeUntil;
    int lastHealth;
    void Start()
    {
        position=(RectTransform)transform;view=Camera.main;
        if(target==null)return;
        foreach(var behaviour in target.GetComponents<MonoBehaviour>())if(behaviour is IEnemyCombatant enemy)combatant=enemy;
        renderers=target.GetComponentsInChildren<Renderer>();lastHealth=target.CurrentHealth;
    }
    void LateUpdate()
    {
        if(target==null||view==null||target.IsDead||EclipseGameUI.BlocksGameplay){bar.enabled=false;return;}
        if(lastHealth!=target.CurrentHealth){lastHealth=target.CurrentHealth;noticeUntil=Time.unscaledTime+4;}
        if(!(combatant?.IsEngaged??false)&&Time.unscaledTime>noticeUntil){bar.enabled=false;return;}
        Vector3 head=target.transform.position+Vector3.up*.5f;
        if(renderers!=null)foreach(var renderer in renderers)if(renderer!=null&&renderer.enabled&&(renderer is MeshRenderer||renderer is SkinnedMeshRenderer))head.y=Mathf.Max(head.y,renderer.bounds.max.y+.22f);
        Vector3 point=view.WorldToScreenPoint(head);
        bool visible=point.z>0&&point.x>0&&point.y>0&&point.x<view.pixelWidth&&point.y<view.pixelHeight;
        bar.enabled=visible;if(!visible)return;
        var canvas=canvasRect.GetComponent<Canvas>();var uiCamera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,point,uiCamera,out Vector2 local);
        position.anchoredPosition=local;
        shown=Mathf.MoveTowards(shown,(float)target.CurrentHealth/target.MaxHealth,Time.unscaledDeltaTime*3);
        bar.SetAmount(shown);
    }
}
