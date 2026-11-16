using UnityEngine;
using UnityEngine.UI;

// Formas de la interfaz: conservan bordes nítidos al cambiar la resolución.
[RequireComponent(typeof(CanvasRenderer))]
public sealed class EclipseGraphic : MaskableGraphic
{
    public enum Form { Panel, Plaque, Circle, Blood, Ring, Bar }
    public Form form;
    public Color fill = new Color(.055f,.05f,.052f,.97f);
    public Color border = new Color(.47f,.36f,.24f);
    [Range(0,1)] public float amount = 1;
    public float edge = 2;
    public void SetAmount(float value)
    {
        value=Mathf.Clamp01(value);
        if(Mathf.Abs(value-amount)<.0001f)return;
        amount=value;SetVerticesDirty();
    }
    public void SetFill(Color value) { if(fill==value)return;fill=value;SetVerticesDirty(); }
    void Update() { if(form==Form.Blood && Application.isPlaying && amount>0 && amount<1)SetVerticesDirty(); }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();Rect r=GetPixelAdjustedRect();
        if(form==Form.Blood) { Blood(vh,r);return; }
        if(form==Form.Ring) { Ring(vh,r);return; }
        if(form==Form.Circle)
        {
            Disc(vh,r.center,Mathf.Min(r.width,r.height)*.5f,border*color);
            Disc(vh,r.center,Mathf.Min(r.width,r.height)*.5f-edge,fill*color);return;
        }
        float cut=form==Form.Plaque?Mathf.Min(17,r.height*.45f):form==Form.Bar?3:6;
        Polygon(vh,r,cut,border*color);
        Rect inner=new Rect(r.x+edge,r.y+edge,Mathf.Max(0,r.width-edge*2),Mathf.Max(0,r.height-edge*2));
        Polygon(vh,inner,Mathf.Max(0,cut-edge*.5f),form==Form.Bar?new Color(.025f,.023f,.03f,1)*color:fill*color);
        if(form==Form.Bar && amount>0)
        {
            inner.width*=Mathf.Clamp01(amount);Polygon(vh,inner,Mathf.Min(2,inner.width*.5f),fill*color);
        }
    }
    void Blood(VertexHelper vh,Rect r)
    {
        if(amount<=0)return;float radius=Mathf.Min(r.width,r.height)*.5f;
        const int strips=80;float step=radius*2/strips;
        for(int i=0;i<strips;i++)
        {
            float x0=-radius+i*step,x1=x0+step;
            float c0=Mathf.Sqrt(Mathf.Max(0,radius*radius-x0*x0)),c1=Mathf.Sqrt(Mathf.Max(0,radius*radius-x1*x1));
            float wave0=amount>=1?0:Mathf.Sin(x0*.13f+Time.unscaledTime*1.8f)*.65f;
            float wave1=amount>=1?0:Mathf.Sin(x1*.13f+Time.unscaledTime*1.8f)*.65f;
            float h0=Mathf.Min(c0,-radius+radius*2*amount+wave0),h1=Mathf.Min(c1,-radius+radius*2*amount+wave1);
            if(h0<=-c0 && h1<=-c1)continue;
            h0=Mathf.Max(-c0,h0);h1=Mathf.Max(-c1,h1);
            Quad(vh,r.center+new Vector2(x0,-c0),r.center+new Vector2(x1,-c1),r.center+new Vector2(x1,h1),r.center+new Vector2(x0,h0),fill*color);
            if(amount<1)
                Quad(vh,r.center+new Vector2(x0,Mathf.Max(-c0,h0-1.5f)),r.center+new Vector2(x1,Mathf.Max(-c1,h1-1.5f)),r.center+new Vector2(x1,h1),r.center+new Vector2(x0,h0),new Color(.62f,.10f,.15f,1)*color);
        }
    }
    void Ring(VertexHelper vh,Rect r)
    {
        float radius=Mathf.Min(r.width,r.height)*.5f;const int segments=80;
        int count=Mathf.CeilToInt(segments*amount);
        for(int i=0;i<count;i++)
        {
            float a=Mathf.PI*.5f-i*Mathf.PI*2/segments,b=Mathf.PI*.5f-Mathf.Min(i+1,segments*amount)*Mathf.PI*2/segments;
            Vector2 d0=new Vector2(Mathf.Cos(a),Mathf.Sin(a)),d1=new Vector2(Mathf.Cos(b),Mathf.Sin(b));
            Quad(vh,r.center+d0*(radius-edge),r.center+d1*(radius-edge),r.center+d1*radius,r.center+d0*radius,fill*color);
        }
    }
    static void Disc(VertexHelper vh,Vector2 center,float radius,Color c)
    {
        if(radius<=0)return;int start=vh.currentVertCount;vh.AddVert(center,c,Vector2.zero);
        const int sides=80;
        for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;vh.AddVert(center+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,c,Vector2.zero);if(i>0)vh.AddTriangle(start,start+i,start+i+1);}
    }
    static void Polygon(VertexHelper vh,Rect r,float cut,Color c)
    {
        if(r.width<=0||r.height<=0)return;cut=Mathf.Min(cut,r.width*.5f,r.height*.5f);
        Vector2[] points={new Vector2(r.xMin+cut,r.yMin),new Vector2(r.xMax-cut,r.yMin),new Vector2(r.xMax,r.yMin+cut),new Vector2(r.xMax,r.yMax-cut),new Vector2(r.xMax-cut,r.yMax),new Vector2(r.xMin+cut,r.yMax),new Vector2(r.xMin,r.yMax-cut),new Vector2(r.xMin,r.yMin+cut)};
        int start=vh.currentVertCount;vh.AddVert(r.center,c,Vector2.zero);foreach(var p in points)vh.AddVert(p,c,Vector2.zero);
        for(int i=0;i<points.Length;i++)vh.AddTriangle(start,start+1+i,start+1+(i+1)%points.Length);
    }
    static void Quad(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color tint)
    {
        int start=vh.currentVertCount;vh.AddVert(a,tint,Vector2.zero);vh.AddVert(b,tint,Vector2.zero);vh.AddVert(c,tint,Vector2.zero);vh.AddVert(d,tint,Vector2.zero);
        vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);
    }
}
