using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoordinateSharp;


[RequireComponent(typeof(MeshFilter))]
public class Polygon_drawer : MonoBehaviour
{

    public Polygon_reader polygonReader;

    public int resolution = 1;
    public float width = 10f;
    public float height = 10f;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    [SerializeField] float scalar = 1f;

    public Vector2 offset;

    public int id;

    // Start is called before the first frame update
    void Start()
    {   
        //Intialising the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        resolution = 1;
        Setup.print(offset, "Offsets: ");

        offset = toUTM(offset.x, offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
            CreateShape();
            UpdateMesh();

            this.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

    }

    void CreateShape() {
        //Putting the coordinates into vertices;
        Feature f = polygonReader.polygonData.features[id];

        //Find the bounds of the area
        Vector2 small = FindSmallestPoint(f.geometry.coordinates);
        Vector2 big = FindLargestPoint(f.geometry.coordinates);

        small = toUTM(small.x, small.y);
        big = toUTM(big.x, big.y);

        offset = offset - small;

        List<Vector3> points = new List<Vector3>();

        float wi = big.x - small.x;
        float hi = big.y - small.y;

        // float xStep = width/resolution;
        // float zStep = height/resolution;        
        float xStep = wi/resolution;
        float zStep = hi/resolution;


        Debug.Log("xStep: " + xStep.ToString() + ", zStep: " + zStep.ToString());

        for (int i = 0; i <= resolution; i++) {
            float z = i * zStep;
            for (int j = 0; j <= resolution; j++) {
                float x = j * xStep;

                points.Add(new Vector3(x + offset.x,0, z + offset.y));
            }
        }

        Setup.printArray(points.ToArray());

        List<int> indices = createTriangles(resolution);

        triangles = indices.ToArray();
        vertices = points.ToArray();
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();

        mesh.RecalculateNormals();
    }

    Vector2 toUTM(float x, float y) {
        Coordinate c = new Coordinate(x, y);

        string[] utm = c.MGRS.ToRoundedString().Split(' ');

        return new Vector2(float.Parse(utm[2]), float.Parse(utm[3]));
    }

    Vector2 FindSmallestPoint(float[][][][] points) {
        Vector2 small = new Vector2(points[0][0][0][0], points[0][0][0][1]);

        foreach (float[] p in points[0][0]) {
            if (p[0] < small.x && p[1] < small.y)
            {
                small.x = p[0];
                small.y = p[1];
            }
        }

        return small;
    }

    Vector2 FindLargestPoint(float[][][][] points) {
        Vector2 big = new Vector2(points[0][0][0][0], points[0][0][0][1]);

        foreach (float[] p in points[0][0]) {
            if (p[0] > big.x && p[1] > big.y)
            {
                big.x = p[0];
                big.y = p[1];
            }
        }

        return big;
    }

    List<int> createTriangles(int resolution) {
        List<int> indices = new List<int>();

        //Create triangles array
        for (int j = 0; j < resolution; j++) {
          for (int i = 0; i < resolution; i++) {
             int w = resolution+1;
             indices.Add(j * w   + i + 0);  // 0
             indices.Add((j+1)*w + i + 0);  // 2
             indices.Add((j+1)*w + i + 1);  // 3
             indices.Add(j * w   + i + 1);  // 1
             indices.Add(j * w   + i + 0);  // 0
             indices.Add((j+1)*w + i + 1);  // 3
          }
        }

        return indices; 
    }


}
