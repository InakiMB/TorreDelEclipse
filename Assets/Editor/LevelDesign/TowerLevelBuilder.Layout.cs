using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static partial class TowerLevelBuilder
{
    private static void BuildGroundFloor()
    {
        Floors(-12,12,-24,28);
        Floors(-24,-12,-8,20);
        Floors(12,24,-8,20);
        Floors(-4,4,-36,-24);
        Floors(-4,4,28,36);
        Wall(new Vector3(-12,0,-24), new Vector3(-4,0,-24), 1.25f);
        Wall(new Vector3(4,0,-24), new Vector3(12,0,-24), 1.25f);
        Wall(new Vector3(-12,0,-24), new Vector3(-12,0,-8));
        Wall(new Vector3(12,0,-24), new Vector3(12,0,-8));
        Wall(new Vector3(-24,0,-8), new Vector3(-12,0,-8), 1.4f);
        Wall(new Vector3(12,0,-8), new Vector3(24,0,-8), 1.4f);
        Wall(new Vector3(-24,0,-8), new Vector3(-24,0,20));
        Wall(new Vector3(24,0,-8), new Vector3(24,0,20));
        Wall(new Vector3(-24,0,20), new Vector3(-12,0,20));
        Wall(new Vector3(12,0,20), new Vector3(24,0,20));
        foreach (float x in new[] {-12f,12f})
        {
            Wall(new Vector3(x,0,-8),new Vector3(x,0,-6));
            Wall(new Vector3(x,0,0),new Vector3(x,0,12));
            Wall(new Vector3(x,0,18),new Vector3(x,0,28));
            Arch(x,-3,90);
            Arch(x,15,90);
        }
        Wall(new Vector3(-12,0,28),new Vector3(-3,0,28));
        Wall(new Vector3(3,0,28),new Vector3(12,0,28));
        Arch(0,28);
        Wall(new Vector3(-4,0,28),new Vector3(-4,0,36));
        Wall(new Vector3(4,0,28),new Vector3(4,0,36));
        Wall(new Vector3(-4,0,36),new Vector3(4,0,36));
        Wall(new Vector3(-12,0,12),new Vector3(-3,0,12),2.2f);
        Wall(new Vector3(3,0,12),new Vector3(12,0,12),2.2f);
        Arch(0,12);
        Arch(0,-24);
        foreach (float x in new[] {-6f,6f}) foreach(float z in new[] {-10f,0f,9f}) Pillar(x,z);
        foreach (float x in new[] {-10f,10f}) Pillar(x,-19,3.7f);
        foreach (float x in new[] {-9f,9f}) Pillar(x,23,4.7f);
        // The side wings connect to the central hall at both ends.
        foreach (float x in new[] {-18f,18f})
        {
            Pillar(x,7,3.6f);
            D("AltarLateral", "Altar", new Vector3(x,0,16), new Vector3(3.5f,1.15f,3.5f),0,true);
        }
        D("AltarDelSantuario", "Altar", new Vector3(0,0,22), new Vector3(4.8f,1.3f,4.8f),0,true);
        D("EscaleraDeAscenso", "Stairs", new Vector3(0,0,31.3f), new Vector3(4.5f,2.3f,6.2f),0,true);
        Box("DescansoEscalera",new Vector3(0,2.1f,35),new Vector3(5,.4f,2),darkStone,architecture);
        foreach(float x in new[] {-4.5f,4.5f}) Banner(x,11.6f);
        foreach(float x in new[] {-7f,7f}) Banner(x,27.55f);
        Banner(-23.6f,13,90);
        Banner(23.6f,13,-90);
        foreach(Vector2 position in new[] {new Vector2(-9,-20),new Vector2(9,-20),new Vector2(-9,-7),new Vector2(9,-7),
            new Vector2(-9,8),new Vector2(9,8),new Vector2(-8,25),new Vector2(8,25),new Vector2(-21,4),new Vector2(21,4)})
            Brazier(position.x,position.y,0,position.y>=20);
        // Low cover leaves the centre aisle and the wing entrances clear.
        foreach(Vector2 position in new[] {new Vector2(-8,4),new Vector2(8,4),new Vector2(-20,-5),new Vector2(20,-5)})
            K("CoberturaDeCajas","barrels",new Vector3(position.x,0,position.y),new Vector3(2.4f,1.7f,1.5f),0,props,true);
        K("EstanteriaOeste","structure",new Vector3(-22,0,10),new Vector3(1.4f,2.5f,3.4f),0,props);
        K("EstanteriaEste","structure",new Vector3(22,0,10),new Vector3(1.4f,2.5f,3.4f),0,props);
        Box("AlfombraCentral",new Vector3(0,.025f,-1),new Vector3(3.6f,.025f,25),purple,props,false);
        foreach(float x in new[]{-1.85f,1.85f}) Box("BordeAlfombra",new Vector3(x,.043f,-1),new Vector3(.07f,.02f,25),gold,props,false);
        // Low bridge parapets frame the approach without covering the player.
        foreach(float x in new[]{-4f,4f}) Wall(new Vector3(x,0,-36),new Vector3(x,0,-24),1.1f);
        Wall(new Vector3(-4,0,-36),new Vector3(4,0,-36),1.1f);
        Brazier(-3,-31); Brazier(3,-31);
    }

    private static void BuildTerrace()
    {
        Vector3 origin = new Vector3(100,12,0);
        Vector3[] outline = {new Vector3(-10,0,-16),new Vector3(10,0,-16),new Vector3(16,0,-10),new Vector3(16,0,10),
            new Vector3(10,0,16),new Vector3(-10,0,16),new Vector3(-16,0,10),new Vector3(-16,0,-10)};
        var vertices = new List<Vector3> {Vector3.zero};
        vertices.AddRange(outline);
        var triangles = new List<int>();
        for(int i=0;i<outline.Length;i++) {triangles.Add(0);triangles.Add((i+1)%outline.Length+1);triangles.Add(i+1);}
        var mesh = new Mesh {name="SueloOctogonalTerraza"};
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles,0);
        var uv = new List<Vector2>(); foreach(Vector3 vertex in vertices) uv.Add(new Vector2(vertex.x/4f,vertex.z/4f));
        mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh,Output+"/Meshes/TerrazaOctogonal.asset");
        var floor=new GameObject("SueloTerraza",typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider));
        floor.layer=3;floor.transform.SetParent(architecture,false);floor.transform.position=origin;
        floor.GetComponent<MeshFilter>().sharedMesh=mesh;
        floor.GetComponent<MeshCollider>().sharedMesh=mesh;
        floor.GetComponent<Renderer>().sharedMaterial=Mat("SueloTerraza",Kenney+"Textures/cobblestoneAlternative.png",new Color(.62f,.68f,.76f));
        K("CuerpoDeLaTorre","tower-base",new Vector3(100,-8,0),new Vector3(31,19.8f,31));
        for(int i=0;i<outline.Length;i++)
        {
            Vector3 a=outline[i]+origin;Vector3 b=outline[(i+1)%outline.Length]+origin;Vector3 span=b-a;
            float yaw=-Mathf.Atan2(span.z,span.x)*Mathf.Rad2Deg;
            GameObject wall=Box("PretilTerraza",(a+b)*.5f+Vector3.up*.55f,new Vector3(span.magnitude,1.1f,.55f),darkStone,architecture);
            wall.transform.rotation=Quaternion.Euler(0,yaw,0);
            int count=Mathf.CeilToInt(span.magnitude/2f);
            for(int j=0;j<count;j++) K("Almena","battlement",Vector3.Lerp(a,b,(j+.5f)/count)+Vector3.up*1.1f,new Vector3(span.magnitude/count,.8f,.6f),yaw);
        }
        foreach(Vector2 offset in new[]{new Vector2(-10,-10),new Vector2(10,-10),new Vector2(-10,10),new Vector2(10,10)})
        {
            GameObject turret=K("Torreon","tower",origin+new Vector3(offset.x,-3,offset.y),new Vector3(4.8f,6.2f,4.8f));
            // The turret footprint is cover; its tall top is hidden when it covers the mage.
            GameObject blocker=Box("BaseTorreon",origin+new Vector3(offset.x,1.4f,offset.y),new Vector3(4.8f,2.8f,4.8f),darkStone,architecture);
            blocker.transform.SetParent(turret.transform,true);
            blocker.GetComponent<Renderer>().enabled=false;
            turret.AddComponent<TowerWallCutaway>();
        }
        foreach(float x in new[]{93f,107f})
        {
            K("ColumnaTerraza","column-damaged",new Vector3(x,12,5),new Vector3(1.4f,3.2f,1.4f),0,null,true);
            Brazier(x,7,12,true);
        }
        // A broad, mostly empty centre keeps the future arena adaptable.
        for(int i=0;i<20;i++)
        {
            float angle=i*Mathf.PI*2/20;
            GameObject segment=Box("CirculoDelEclipse",origin+new Vector3(Mathf.Sin(angle)*5,.025f,Mathf.Cos(angle)*5),
                new Vector3(.08f,.025f,1.3f),gold,props,false);
            segment.transform.rotation=Quaternion.Euler(0,angle*Mathf.Rad2Deg+90,0);
        }
        K("AccesoTerraza","wall-fortified-door",new Vector3(100,12,-13),new Vector3(5.5f,4.8f,1.4f));
        Brazier(96,-12,12);Brazier(104,-12,12);
        Banner(100,15.5f,0,12);
        Transform boss=Group("Reserva_Jefe_PorDefinir",props);boss.position=new Vector3(100,12,6);
    }
}
