using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public float offsetX;
    public float offsetZ;

    public int id;

    // Start is called before the first frame update
    void Start()
    {   
        //Intialising the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        resolution = 1;
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

        List<Vector3> points = new List<Vector3>();
        List<List<Vector3>> points2d = new List<List<Vector3>>(); 

        float xStep = width/resolution;
        float zStep = height/resolution;

        Debug.Log("xStep: " + xStep.ToString() + ", zStep: " + zStep.ToString());

        for (int i = 0; i <= resolution; i++) {
            points2d.Add(new List<Vector3>());
            float z = i * zStep;
            for (int j = 0; j <= resolution; j++) {
                float x = j * xStep;

                points2d[i].Add(new Vector3(x,0, z));
                points.Add(new Vector3(x,0, z));
            }
        }

        Setup.printArray(points.ToArray());

        List<int> indices = new List<int>();

        //Create triangles array
        for (int j = 0; j < points2d.Count-1; j++) {
          for (int i = 0; i < points2d[j].Count-1; i++) {
             int w = points2d[j].Count;
             indices.Add(j * w   + i + 0);  // 0
             indices.Add((j+1)*w + i + 0);  // 2
             indices.Add((j+1)*w + i + 1);  // 3
             indices.Add(j * w   + i + 1);  // 1
             indices.Add(j * w   + i + 0);  // 0
             indices.Add((j+1)*w + i + 1);  // 3
          }
        } 

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

}
