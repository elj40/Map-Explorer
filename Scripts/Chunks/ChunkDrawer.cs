using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class ChunkDrawer : MonoBehaviour
{
    public Vector2 position;
    public Vector2 size;
    public Vector2 offset;

    public Vector2 UVData;
    public Vector2 UVStep;

    public Texture2D map;

    public int stepSize = 5;

    public float maxHeight = 100;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int id;

    public bool constantUpdate = false;
    public bool variableHeight = true;
    public bool amIUseful = true;
    // Start is called before the first frame update
     void Start()
    {   

        maxHeight = 100f;
        //Intialising the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();

        this.GetComponent<MeshCollider>().sharedMesh = mesh;

        //Setup.print(UVData, "UV start point: ");

        amIUseful = AmIUseful();

        //offset = toUTM(offset.x, offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (constantUpdate) {
            CreateShape();
            UpdateMesh();

            this.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }


    void CreateShape()
    {   
        int xSize = (int) Mathf.Abs(Mathf.Ceil(size.x/stepSize));
        int zSize = (int) Mathf.Abs(Mathf.Ceil(size.y/stepSize));

        Vector2 p = new Vector2(this.transform.position.x, this.transform.position.z);

        vertices = new Vector3[(xSize+1)*(zSize+1)];    //The actual points of the mesh
        uvs = new Vector2[vertices.Length];             //Used to map the image to the chunks

        Vector2 UVCoordinates = UVData;
        for (int i = 0, j = 0; j <= zSize; j++) {

            float z = Mathf.Min(size.y * j/zSize, Mathf.Abs(size.y));

            UVCoordinates.x = UVData.x;

            for (int k = 0; k <= xSize; k++) {

                float x = Mathf.Min(size.x * k/xSize, Mathf.Abs(size.x));

                int pixelX = Mathf.FloorToInt(UVCoordinates.x * map.width);
                int pixelY = Mathf.FloorToInt(UVCoordinates.y * map.height);

                float y;
                //print(map.GetPixel(pixelX, pixelY).grayscale);
                if (variableHeight)
                    y = (1-map.GetPixel(pixelX, pixelY).grayscale) * maxHeight;
                else
                    y = (1-map.GetPixel(pixelX, pixelY).grayscale > 0.1f) ? maxHeight : 0;

                vertices[i] = new Vector3(x, y, z);
                uvs[i] = new Vector2(UVCoordinates.x, UVCoordinates.y);

                i++;
                UVCoordinates.x += UVStep.x*(stepSize/Mathf.Abs(size.x));
            }
            UVCoordinates.y += UVStep.y*(stepSize/Mathf.Abs(size.y));
        }


        int vert = 0;
        int tris=0;

        triangles = new int[xSize * zSize * 6];

        for (int z = 0; z < zSize; z++) {

            for (int x = 0; x < xSize; x++) {
                
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        //Setup.printArray(triangles);

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        //mesh.Optimize();

        mesh.RecalculateNormals();
    }

    float getLayeredNoise(int layers, float scalar, Vector2 v, Vector2 offset) {
        float pn = 0;
        float sc = scalar;
        for (int i = 0; i < layers; i++) {
            pn += Mathf.PerlinNoise(offset.x + v.x*sc, offset.y + v.y*sc) * scalar;
            sc = sc / 2;
        }
        return pn;
    }

    bool AmIUseful() {
        foreach (Vector3 v in vertices) {
            if (v.y != 0) return true;
        }
        return false;
    }
}
