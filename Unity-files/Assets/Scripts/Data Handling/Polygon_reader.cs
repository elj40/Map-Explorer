using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


public class Polygon_reader : MonoBehaviour
{   
    [SerializeField] string fileName = @"Places_polygon.geojson";

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
    }

}
