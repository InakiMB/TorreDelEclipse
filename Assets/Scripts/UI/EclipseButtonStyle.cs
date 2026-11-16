using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class EclipseButtonStyle : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,ISelectHandler,IDeselectHandler,IPointerClickHandler
{
    public EclipseGraphic frame;
    bool hovered,selected;
    void OnEnable(){hovered=false;selected=false;Refresh();}
    public void OnPointerEnter(PointerEventData data){hovered=true;Refresh();EclipseGameUI.Active?.PlayHoverSound();}
    public void OnPointerExit(PointerEventData data){hovered=false;Refresh();}
    public void OnSelect(BaseEventData data){selected=true;Refresh();}
    public void OnDeselect(BaseEventData data){selected=false;Refresh();}
    public void OnPointerClick(PointerEventData data){if(data.button==PointerEventData.InputButton.Left&&GetComponent<Button>().IsInteractable())EclipseGameUI.Active?.PlayClickSound();}
    void Refresh()
    {
        if(frame==null)return;
        frame.SetFill(hovered||selected?new Color(.025f,.16f,.19f,.98f):new Color(.065f,.052f,.044f,.97f));
        frame.border=hovered||selected?new Color(.66f,.48f,.27f):new Color(.47f,.36f,.24f);frame.SetVerticesDirty();
    }
}
