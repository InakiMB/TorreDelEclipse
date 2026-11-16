using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName="Torre del Eclipse/Estilo de interfaz")]
public sealed class EclipseUITheme : ScriptableObject
{
    public Font font;
    public Sprite menuBackground;
    public Sprite victoryBackground;
    public Sprite logo;
    public Sprite[] actionIcons=new Sprite[4];
    public TextAsset sources;
    public EventReference menuMusic;
    public EventReference hoverSound;
    public EventReference clickSound;
}
