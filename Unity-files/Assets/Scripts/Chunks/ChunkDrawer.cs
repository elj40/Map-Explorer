using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ChunkDrawer : MonoBehaviour
{
    public Vector2 position;
    public Vector2 size;
    public Vector2 offset;

    public int stepSize = 5;
    public float scalar = 10;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int id;
    // Start is called before the first frame update
     void Start()
    {
        //Intialising the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();

        this.GetComponent<MeshCollider>().sharedMesh = mesh;

        //offset = toUTM(offset.x, offset.y);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void CreateShape()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 p = position - offset;

        int xSize = 0;  //Keeps track of 2D size of array to convert... 
        int ySize = 0;  //into triangle array easily

        //Create grid of points
        int x = 0;
        int y = 0;
        while (y < size.y)
        {
            
            while (x < size.x)
            {   

                float yh = Mathf.PerlinNoise(x + position.x * 0.3f, y + position.y * 0.3f) * scalar;

                points.Add(new Vector3(x + p.x, yh, y + p.y));
                x += stepSize;

            }
            x = 0;
            float yha = Mathf.PerlinNoise(size.x + position.x * 0.3f, y + position.y * 0.3f) * scalar;
            points.Add(new Vector3(size.x + p.x, yha, y + p.y));
            y += stepSize;
            ySize++;
        }

        while (x < size.x)
        {   
            float yha = Mathf.PerlinNoise(x + position.x * 0.3f, size.y + position.y * 0.3f) * scalar;
            points.Add(new Vector3(x + p.x, yha, size.y + p.y));
            x += stepSize;
            xSize++;
        }
        ySize++;
        points.Add(new Vector3(size.x + p.x, 0, size.y + p.y));

        //Create triangles

        List<int> indices = createTriangles(xSize, ySize);

        triangles = indices.ToArray();
        vertices = points.ToArray();

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();

        mesh.RecalculateNormals();
    }

    List<int> createTriangles(int xSize, int ySize)
    {
        List<int> indices = new List<int>();

        //Create triangles array
        for (int j = 0; j < ySize; j++)
        {
            for (int i = 0; i < xSize; i++)
            {
                int w = xSize;
                indices.Add(j * w + i + 0);  // 0
                indices.Add((j + 1) * w + i + 0);  // 2
                indices.Add((j + 1) * w + i + 1);  // 3
                indices.Add(j * w + i + 1);  // 1
                indices.Add(j * w + i + 0);  // 0
                indices.Add((j + 1) * w + i + 1);  // 3
            }
        }

        return indices;
    }
}
