using System.Collections;
using System.Collections.Generic;
using CoordinateSharp;
using UnityEngine;
using UnityEngine.UI;

public class ChunkManager : MonoBehaviour
{
    public Polygon_reader polygon_Reader;

    public Vector2 chunkSize = new Vector2(100, 100);
    public int stepSize;

    public Vector2 smallestPoint;
    public Vector2 largestPoint;

    public GameObject chunkPrefab;

    public bool singleChunk = false;
    public bool singleRow = true;

    public Texture2D map;

    List<GameObject> chunksToSplit = new List<GameObject>();


    List<Vector2> orbPoints = new List<Vector2>();
    // Start is called before the first frame update
    public void start()
    {

        smallestPoint = FindSmallestPoint(polygon_Reader.polygonData.features);
        largestPoint = FindLargestPoint(polygon_Reader.polygonData.features);

        Setup.print(smallestPoint,"Smallest point degrees: ");
        Setup.print(largestPoint,"Largest point degrees: ");

        //Convert to UTM coordinates
        smallestPoint = toUTM(smallestPoint.x, smallestPoint.y);
        largestPoint = toUTM(largestPoint.x, largestPoint.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("Space key was pressed (ChunkManager)");
            clearChunks();
            createChunks();
            //StartCoroutine(createChunks2());
           // FilterChunks();

        }
        if (Input.GetKeyDown("f")) {
            Debug.Log("'F' key was pressed");
            FilterChunks();

        }
        if (Input.GetKeyDown("s")) {
            Debug.Log("'S' key was pressed");
            SplitChunks(chunksToSplit);
            chunksToSplit = new List<GameObject>();

        }
    }

    Vector2 FindSmallestPoint(List<Feature> features)
    {
        Vector2 small = new Vector2(features[0].geometry.coordinates[0][0][0][0], features[0].geometry.coordinates[0][0][0][1]);
        foreach (Feature f in features)
        {
            foreach (float[] p in f.geometry.coordinates[0][0])
            {

                small.x = (p[0] < Mathf.Abs(small.x)) ? p[0] : small.x;
                small.y = (p[1] < Mathf.Abs(small.y)) ? p[1] : small.y;
                
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
                big.x = (p[0] > Mathf.Abs(big.x)) ? p[0] : big.x;
                big.y = (p[1] > Mathf.Abs(big.y)) ? p[1] : big.y;
            }
        }
        return big;
    }

    Vector2 toUTM(float x, float y)
    {
        Coordinate c = new Coordinate(x, y);

        string u = c.MGRS.ToRoundedString();
        string[] utm = u.Split(' ');
        return new Vector2(float.Parse(utm[2]), float.Parse(utm[3]));
    }

    Vector2 appliedChunkSize(Vector2 chunkSize, Vector2 largeP, Vector2 pos) {
        Vector2 newData = chunkSize;

        float xS;
        if (chunkSize.x > 0)    xS = Mathf.Min(largeP.x - pos.x, chunkSize.x);
        else                    xS = Mathf.Max((largeP.x - pos.x), chunkSize.x);
        newData.x = xS;  

        float yS;
        if (chunkSize.y > 0)    yS = Mathf.Min(largeP.y - pos.y, chunkSize.y);
        else                    yS = Mathf.Max((largeP.y - pos.y), chunkSize.y);
        newData.y = yS;

        return newData;
    }

    void clearChunks()
    {
        Debug.Log("Clearing chunks...");
        for (int i = 1; i <  this.transform.childCount; i++) {
            GameObject chunk = this.transform.GetChild(i).gameObject;
            Destroy(chunk);
        }
    }

    void FilterChunks() {
        Debug.Log("Filtering chunks...");

        for (int i = 1; i <  this.transform.childCount; i++) {
            GameObject chunk = this.transform.GetChild(i).gameObject;
            bool useful = chunk.GetComponent<ChunkDrawer>().amIUseful;
            if (!useful) {
                Debug.Log("Cleared chunk: " + chunk.name);
                Destroy(chunk);
            }else {
                chunksToSplit.Add(chunk);
            }
        }

        //(chunksToSplit);
    }

    void SplitChunks(List<GameObject> chunksToSplit) {
        Debug.Log("Splitting each chunk");
        foreach (GameObject c in chunksToSplit) {
            c.GetComponent<ChunkProcessor>().Split();
            Destroy(c);
        }
    }

    void createChunks()
    {

        Vector2 mapDifference = largestPoint - smallestPoint;
        Vector2 adjustedChunkSize = new Vector2(Mathf.Abs(mapDifference.x) / mapDifference.x, Mathf.Abs(mapDifference.y) / mapDifference.y);
        adjustedChunkSize *= chunkSize;

        int ID = 0;

        float UVStepX = Mathf.Abs(chunkSize.x/(largestPoint.x - smallestPoint.x));
        float UVStepY = Mathf.Abs(chunkSize.y/(largestPoint.y - smallestPoint.y));        
        Vector2 UVStep = new Vector2(UVStepX, UVStepY);

        Setup.print(UVStep, "UVStep: ");
        float y = smallestPoint.y;
        while (true)
        {
            float x = smallestPoint.x;
            while (true)
            {

                GameObject chunk = Instantiate(chunkPrefab, this.transform);


                float xUV = Mathf.Abs((x - smallestPoint.x)/(largestPoint.x - smallestPoint.x));
                float yUV = Mathf.Abs((y - smallestPoint.y)/(largestPoint.y - smallestPoint.y));

                Vector2 UVData = new Vector2(xUV, yUV);

                ChunkDrawer chunkDrawer = chunk.GetComponent<ChunkDrawer>();
                chunk.transform.position = new Vector3(x - smallestPoint.x, 0, (y - smallestPoint.y));


                chunkDrawer.offset = smallestPoint;

                chunkDrawer.size = appliedChunkSize(adjustedChunkSize, largestPoint, new Vector2(x, y));
                chunkDrawer.stepSize = stepSize;

                chunkDrawer.UVData = UVData;
                chunkDrawer.UVStep = UVStep;

                chunkDrawer.id = ID;

                chunkDrawer.map = map;

                ID++;

                x += adjustedChunkSize.x;
                if (adjustedChunkSize.x < 0 && x < largestPoint.x) break;
                if (adjustedChunkSize.x > 0 && x > largestPoint.x) break;

                if (singleChunk) break;
            }
            y += adjustedChunkSize.y;
            if (adjustedChunkSize.y < 0 && y < largestPoint.y) break;
            if (adjustedChunkSize.y > 0 && y > largestPoint.y) break;

            if (singleRow) break;
        }

        Debug.Log("Created all the chunks");
    }

    IEnumerator createChunks2()
    {

        Vector2 mapDifference = largestPoint - smallestPoint;
        Vector2 adjustedChunkSize = new Vector2(Mathf.Abs(mapDifference.x) / mapDifference.x, Mathf.Abs(mapDifference.y) / mapDifference.y);
        adjustedChunkSize *= chunkSize;

        int ID = 0;

        float UVStepX = Mathf.Abs(stepSize/(largestPoint.x - smallestPoint.x));
        float UVStepY = Mathf.Abs(stepSize/(largestPoint.y - smallestPoint.y));        
        Vector2 UVStep = new Vector2(UVStepX, UVStepY);


        float y = smallestPoint.y;
        while (true)
        {
            float x = smallestPoint.x;
            while (true)
            {

                GameObject chunk = Instantiate(chunkPrefab, this.transform);


                float xUV = Mathf.Abs((x - smallestPoint.x)/(largestPoint.x - smallestPoint.x));
                float yUV = Mathf.Abs((y - smallestPoint.y)/(largestPoint.y - smallestPoint.y));

                Vector2 UVData = new Vector2(xUV, yUV);

                orbPoints.Add(UVData);

                ChunkDrawer chunkDrawer = chunk.GetComponent<ChunkDrawer>();
                chunk.transform.position = new Vector3(x - smallestPoint.x, 0, (y - smallestPoint.y));


                chunkDrawer.offset = smallestPoint;

                chunkDrawer.size = appliedChunkSize(adjustedChunkSize, largestPoint, new Vector2(x, y));
                chunkDrawer.stepSize = stepSize;

                chunkDrawer.UVData = UVData;
                chunkDrawer.UVStep = UVStep;

                chunkDrawer.id = ID;

                chunkDrawer.map = map;

                ID++;

                x += adjustedChunkSize.x;
                if (adjustedChunkSize.x < 0 && x < largestPoint.x) break;
                if (adjustedChunkSize.x > 0 && x > largestPoint.x) break;

                if (singleChunk) break;

            }
            //FilterChunks();

            y += adjustedChunkSize.y;
            if (adjustedChunkSize.y < 0 && y < largestPoint.y) break;
            if (adjustedChunkSize.y > 0 && y > largestPoint.y) break;

            yield return new WaitForSeconds(0.1f);


            if (singleRow) break;
        }

        //FilterChunks();
        Debug.Log("Created all the chunks");
    }
}
