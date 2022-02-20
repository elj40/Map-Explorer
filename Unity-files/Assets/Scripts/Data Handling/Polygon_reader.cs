using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TriangleNet;


public class Polygon_reader : MonoBehaviour
{   
    [SerializeField] string fileName = @"Places_polygon.geojson";

    public GameObject polygonPrefab;

    public PolygonData polygonData;
    // Start is called before the first frame update
    void Start()
    {
        string fullPath = Application.dataPath + @"/Map data/" + fileName;

        string jsonInfo = File.ReadAllText(fullPath);

        //PolygonData pd = JsonUtility.FromJson<PolygonData>(jsonInfo);

        polygonData = JsonConvert.DeserializeObject<PolygonData>(jsonInfo);


        Debug.Log(polygonData.type);
        Debug.Log(polygonData.name);
        Debug.Log(polygonData.features[0].geometry.type);

        polygonData.countCoordinates();
        Debug.Log(polygonData.totalCoordinates);

        createPolygons();
    }

    void createPolygons() {
        int id = 0;

        float offsetX = polygonData.features[0].geometry.coordinates[0][0][0][0];
        float offsetZ = polygonData.features[0].geometry.coordinates[0][0][0][1];

        foreach (Feature f in polygonData.features) {
            GameObject p = Instantiate(polygonPrefab, this.transform);
            Polygon_drawer drawer = p.GetComponent<Polygon_drawer>();
            drawer.id = id;
            drawer.offsetX = offsetX;
            drawer.offsetZ = offsetZ;
            drawer.polygonReader = this;

            id++;
        }
    }

}
