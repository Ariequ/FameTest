using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MyFameObstacle : FameObstacle
{
    GameObject obstacle;

    protected override void Start()
    {
        base.Start();
        Vector3 pos = gameObject.transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(pos);
        gameObject.transform.position = pos;
    
    }
    const float obstacleHeight = 10f;

    public override void InitObstacle(Vector3 position, float radius)
    {
        base.InitObstacle(position, radius);
        obstacle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        FallingObstacle fallingObstacle = obstacle.AddComponent<FallingObstacle>();
        fallingObstacle.SetYDest(obstacleHeight / 4);
        float diameter = radius * 2 - 5f;
        obstacle.transform.localScale = new Vector3(diameter, obstacleHeight / 2, diameter);
        obstacle.transform.parent = transform;
        obstacle.transform.localPosition = Vector3.zero;
        obstacle.renderer.material.color = new Color(0.8f, 0.8f, 0.8f);
        obstacle.renderer.material = AssetManager.Singleton.rock;
        obstacle.layer = 11;
    }

    public override void InitObstacle(Vector3[] points)
    {
        base.InitObstacle(points);

        obstacle = new GameObject("Fame_PolyObstacle");
        obstacle.AddComponent<FallingObstacle>();
        Vector3 centroid = FameUnityUtil.CalculateCentroid(points);
        centroid.y = 0;

        //create the polymesh
        MeshFilter meshFilter = obstacle.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.Clear();
        Vector3[] vertexList1 = new Vector3[points.Length * 2]; // inner

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertexList1.Length; i++)
        {
            vertexList1[i] = points[i % points.Length] - centroid;
            float uv_x = (i % points.Length) % 2;
            float uv_y = 0;
            if (i >= points.Length)
            {
                vertexList1[i].y = obstacleHeight;
                uv_y = 1;
            }
            else
            {
                vertexList1[i].y = 0;
            }
            uvs.Add(new Vector2(uv_x, uv_y));
        }

        mesh.vertices = vertexList1;
        mesh.uv = uvs.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < points.Length; i++)
        {
            int index0 = i;
            int index1 = (i + 1) % points.Length;
            int index2 = index0 + points.Length;
            int index3 = index1 + points.Length;
            triList.Add(index0);
            triList.Add(index1);
            triList.Add(index2);

            triList.Add(index2);
            triList.Add(index1);
            triList.Add(index0);

            triList.Add(index3);
            triList.Add(index2);
            triList.Add(index1);

            triList.Add(index1);
            triList.Add(index2);
            triList.Add(index3);
        }
        Vector2[] vertices2D = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices2D[i] = (new Vector2(points[i].x, points[i].z));
        }
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();
        for (int i = 0; i < indices.Length; i++)
        {
            triList.Add(indices[i] + points.Length);
        }
        mesh.triangles = triList.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        MeshRenderer r = obstacle.AddComponent<MeshRenderer>();
        r.material.color = new Color(0.8f, 0.8f, 0.8f);
        r.material = AssetManager.Singleton.rock;
        //r.material.mainTexture = Resources.Load("brick") as Texture2D;


        obstacle.transform.parent = transform;
        obstacle.transform.localPosition = Vector3.zero;
    }

}
