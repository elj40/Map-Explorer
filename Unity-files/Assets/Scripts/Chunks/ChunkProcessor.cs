using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkProcessor : MonoBehaviour
{   
    public ChunkDrawer chunkDrawer;
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        chunkDrawer = this.GetComponent<ChunkDrawer>();
        parent = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Split() {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                GameObject chunk = Instantiate(this.transform.gameObject, parent.transform); //Instantiate a copy of myself
                ChunkDrawer cD = chunk.GetComponent<ChunkDrawer>();
                chunk.transform.position = this.transform.position + new Vector3(chunkDrawer.size.x/2 * j, 0, chunkDrawer.size.x/2 * i);


                cD.offset = chunkDrawer.offset;

                cD.size = chunkDrawer.size/2;
                cD.stepSize = chunkDrawer.stepSize/2;

                cD.UVData = chunkDrawer.UVData + new Vector2(chunkDrawer.UVStep.x/2 * j, chunkDrawer.UVStep.y/2 * i);
                cD.UVStep = chunkDrawer.UVStep/2f;

                cD.id = -1;

                cD.map = chunkDrawer.map;
            }
        }
    }
}
