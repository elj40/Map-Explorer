using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class Polygon_drawer : MonoBehaviour
{

    public Polygon_reader polygonReader;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    [SerializeField] float scalar = 10f;

    public float offsetX;
    public float offsetZ;

    public int id;

    // Start is called before the first frame update
    void Start()
    {   
        //Intialising the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        
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

        vertices = new Vector3[f.geometry.coordinates[0][0].Length];

        int i = 0;
        foreach (System.Single[] coords in f.geometry.coordinates[0][0]) {
            vertices[i] = new Vector3((coords[0] - offsetX) * scalar, 0, (coords[1] - offsetZ) * scalar);
            Debug.Log("Coordinates = " + vertices[i].x.ToString() + " " + vertices[i].x.ToString());
            i++;
        }


        //Sorting out the triangles array
        triangles = new int[vertices.Length*3];
        int t = 0;

        for (i = 0; i < vertices.Length-2; i += 1) {
            triangles[t + 0] = 0;
            triangles[t + 1] = i+1;
            triangles[t + 2] = i+2;

            t += 3;
        } 
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    // private void OnDrawGizmos() {
       

    //     for (int i = 0; i < vertices.Length; i++) {
    //         Gizmos.DrawSphere(vertices[i], 0.1f);
    //     }

    // }
}
