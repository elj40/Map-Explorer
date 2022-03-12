using System.Collections;
using System.Collections.Generic;
using CoordinateSharp;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{   
    public Polygon_reader polygon_Reader;

    List<Vector3> terrainPoints = new List<Vector3>();

    public int stepSize;
    public Vector2 chunkSize = new Vector2(100, 100);

    public Vector2 smallestPoint;
    public Vector2 largestPoint;
    // Start is called before the first frame update
    public void start()
    {

        Debug.Log("From the chunkManager: " + polygon_Reader.polygonData.name);
        chunkSize = new Vector2(100, 100);
        stepSize = 50;

        smallestPoint = FindSmallestPoint(polygon_Reader.polygonData.features);
        largestPoint = FindLargestPoint(polygon_Reader.polygonData.features);

        Setup.print(largestPoint - smallestPoint, "Difference in degrees: ");

        //Convert to UTM coordinates
        smallestPoint = toUTM(smallestPoint.x, smallestPoint.y);
        largestPoint = toUTM(largestPoint.x, largestPoint.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createFlatMesh() {

        //Creating chunks
        Vector2 s = largestPoint - smallestPoint;
        Vector2 cS = new Vector2(Mathf.Abs(s.x) / s.x, Mathf.Abs(s.y) / s.y);
        cS *= stepSize;

        int ID = 0;

        float y = smallestPoint.y;
        while (true)
        {
            float x = smallestPoint.x;
            while (true)
            {
                //points inside chunk
                

                x += cS.x;
                if (cS.x < 0 && x < largestPoint.x) break;
                if (cS.x > 0 && x > largestPoint.x) break;

            }
            y += cS.y;
            if (cS.y < 0 && y < largestPoint.y) break;
            if (cS.y > 0 && y > largestPoint.y) break;

            break;
        }
    }



    Vector2 FindSmallestPoint(List<Feature> features)
    {
        Vector2 small = new Vector2(features[0].geometry.coordinates[0][0][0][0], features[0].geometry.coordinates[0][0][0][1]);
        foreach (Feature f in features)
        {
            foreach (float[] p in f.geometry.coordinates[0][0])
            {
                if (p[0] < small.x && p[1] < small.y)
                {
                    small.x = (p[0] < small.x) ? p[0] : small.x;
                    small.y = (p[1] < small.y) ? p[1] : small.y;
                }
            }
        }
        return small;
    }

    Vector2 FindLargestPoint(List<Feature> features)
    {
        Vector2 big = new Vector2(features[0].geometry.coordinates[0][0][0][0], features[0].geometry.coordinates[0][0][0][1]);
        foreach (Feature f in features)
        {
            foreach (float[] p in f.geometry.coordinates[0][0])
            {
                big.x = (p[0] > big.x) ? p[0] : big.x;
                big.y = (p[1] > big.y) ? p[1] : big.y;
            }
        }
        return big;
    }

    Vector2 toUTM(float x, float y)
    {
        Coordinate c = new Coordinate(x, y);

        string u = c.MGRS.ToRoundedString();
        print(u);
        string[] utm = u.Split(' ');
        return new Vector2(float.Parse(utm[2]), float.Parse(utm[3]));
    }
}
