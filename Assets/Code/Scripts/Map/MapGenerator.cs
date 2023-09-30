using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GD.MinMaxSlider;
using Unity.VisualScripting;
using static UnityEngine.Rendering.HableCurve;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int initialBranchRooms; //x=min y=max
    int initialBranch = 0;
    [SerializeField] Vector2Int roomQuantity;
    int segments = 0;

    [SerializeField] GameObject room;
    [SerializeField] Sprite[] roomsIcon = new Sprite[4];

    [SerializeField] GameObject initialRoom;
    [SerializeField] GameObject finalRoom;

    [SerializeField] Material lineRenderMaterial;
    [SerializeField] Gradient lineRenderColor;
    [SerializeField] Scrollbar scrollbar;

    [SerializeField] List<GameObject> nodesContainers = new List<GameObject>();

    [SerializeField, Range(0,100)] int deleteRoomPercentage4, deleteRoomPercentage3, deleteRoomPercentage2;


    // Start is called before the first frame update
    void Start()
    {   
        segments = Random.Range(roomQuantity.x, roomQuantity.y+1);
        Debug.Log("Segments: "+ segments);

        GenerateNodes();
        MakeConnections();
    }

    private void GenerateNodes()
    {
        float initRoomWidthOffsetY = Mathf.Abs(initialRoom.transform.localScale.y/2);
        float finalRoomWidthOffsetX = Mathf.Abs(finalRoom.transform.localScale.x/2);

        float totalDistanceIF = Mathf.Abs((finalRoom.transform.position.x) - (initialRoom.transform.position.x));
        Debug.Log("Dist: " + totalDistanceIF);


        //float segmentDist = Mathf.Lerp(initialRoom.transform.position.x, finalRoom.transform.position.x, 1/ segments);
        // Calculate segment distance including padding
        float segmentDist = (totalDistanceIF - 2 * finalRoomWidthOffsetX) / segments;
        float counter = 0; // Start with padding

        Vector2 rectPos = gameObject.GetComponent<RectTransform>().position;

        

        for (int k = 0; k< segments; k++) {

            initialBranch = Random.Range(initialBranchRooms.x, initialBranchRooms.y+1);
            counter += segmentDist;
            GameObject roomContainer = new GameObject(k + " RoomContainer");
            roomContainer.transform.SetParent(gameObject.transform.GetChild(0));
            roomContainer.transform.localPosition = Vector2.zero+ Vector2.right*-(rectPos+(Vector2.right*1.5f)); //1.5 is half of the widht of the circles
            roomContainer.tag = "NodeContainer";

            for (int i = 1; i < initialBranch+1; i++) {
                
                float mapOffsetX = (totalDistanceIF) / 2;
                float roomHeightHalved = (gameObject.GetComponent<Renderer>().bounds.size.y / 2f) - initRoomWidthOffsetY;
                float roomHeight = (gameObject.GetComponent<Renderer>().bounds.size.y/2) - initRoomWidthOffsetY;
                float yOffset = initRoomWidthOffsetY * 2;//*2 porque se puede dar que se toquen los nodos                
                GameObject generatedNode;

                if (initialBranch % 2 == 0)
                {
                    float randomPosY = 0;
                    float randomPosX = Random.Range(-1,1);
                    int deleteRandomPercentage=0;

                    if (initialBranch == 2)
                    {

                        switch (i)
                        {
                            case 1: { randomPosY = Random.Range(yOffset, roomHeight); } break;
                            case 2: { randomPosY = Random.Range(-yOffset, -roomHeight); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage2;
                        //room.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: { randomPosY = Random.Range(yOffset, roomHeight / 2); } break;
                            case 2: { randomPosY = Random.Range(-yOffset, -roomHeight / 2); } break;
                            case 3: { randomPosY = Random.Range(yOffset + (roomHeight / 2), roomHeight); } break;
                            case 4: { randomPosY = Random.Range(-yOffset - (roomHeight / 2), -roomHeight); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage4;
                        //room.GetComponent<SpriteRenderer>().color = Color.blue;
                    }

                    if (i > 1)
                    {
                        int deadZone = Random.Range(0, 100);
                        if (deadZone > deleteRandomPercentage)
                        {
                            Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                            Debug.Log("Par");
                            generatedNode=Instantiate(room, spawnPositionPar, Quaternion.identity, roomContainer.transform);
                        }
                        else { break; }
                    }
                    else { 
                        Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                        Debug.Log("Par");
                        generatedNode=Instantiate(room, spawnPositionPar, Quaternion.identity, roomContainer.transform);
                    }
                }
                else { 

                    float randomPosY = 0;
                    float randomPosX = Random.Range(-1, 1);
                    int deleteRandomPercentage = 0;

                    if (initialBranch == 1)
                    {

                        switch (i)
                        {
                            case 1: { randomPosY = Random.Range(-roomHeight, roomHeight); } break;
                        }
                        //room.GetComponent<SpriteRenderer>().color = Color.black;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: { randomPosY = Random.Range(-(roomHeight / 3)+ yOffset, (roomHeight / 3)- yOffset); } break;
                            case 2: { randomPosY = Random.Range(-(roomHeight / 3)- yOffset, -roomHeight+ yOffset); } break;
                            case 3: { randomPosY = Random.Range((roomHeight / 3) + yOffset, roomHeight- yOffset); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage3;
                        //room.GetComponent<SpriteRenderer>().color = Color.red;
                    }

                    if (i > 1)
                    {
                        int deadZone = Random.Range(0, 100);
                        if (deadZone > deleteRandomPercentage)
                        {
                            Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                            Debug.Log("Impar");
                            generatedNode=Instantiate(room, spawnPositionPar, Quaternion.identity, roomContainer.transform);
                        }
                        else { break; }
                    }
                    else
                    {
                        Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                        Debug.Log("Par");
                        generatedNode=Instantiate(room, spawnPositionPar, Quaternion.identity, roomContainer.transform);
                    }
                }
                 
                generatedNode.tag = "Node";
            }
        }
    }

    private void MakeConnections() {        

        for (int i = 0; i < gameObject.transform.GetChild(0).transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(0).transform.GetChild(i).tag == "NodeContainer") {
                nodesContainers.Add(gameObject.transform.GetChild(0).transform.GetChild(i).gameObject);
            }
        }

        for (int i = nodesContainers.Count-1; i > 0; i--)
        {
            GameObject nextContainer;

            if (i - 1 >= 0)
            {
                nextContainer = nodesContainers[i - 1];
            }
            else { return; }

            for (int j = 0; j < nodesContainers[i].transform.childCount; j++)
            {
                GameObject node = nodesContainers[i].transform.GetChild(j).gameObject;
                GameObject nextNode=null;//null default
                float distanceBetweenNodes=10000;//Default high value

                for (int k = 0; k < nextContainer.transform.childCount; k++)
                {
                    GameObject nextNodeAUX = nextContainer.transform.GetChild(k).gameObject;
                    float distanceBetweenNodesAUX;

                    Vector2 nodesV2 = nextNodeAUX.transform.position - node.transform.position;
                    distanceBetweenNodesAUX = Mathf.Abs( nodesV2.magnitude);

                    if (distanceBetweenNodesAUX< distanceBetweenNodes) {
                        distanceBetweenNodes = distanceBetweenNodesAUX;
                        nextNode= nextContainer.transform.GetChild(k).gameObject;
                    }
                }                

                if (nodesContainers[i].transform.childCount>= nextContainer.transform.childCount) {
                    DrawPaths(node, nextNode);
                }
                else
                {
                    switch (nodesContainers[i].transform.childCount)
                    {
                        case 1:
                            {
                                for (int a = 0; a < nextContainer.transform.childCount; a++)
                                {
                                    nextNode = nextContainer.transform.GetChild(a).gameObject;
                                    DrawPaths(node, nextNode);
                                }
                            }
                            break;
                        case 2: { } break;
                        case 3: { } break;
                    }
                }
            }
        }
    }

    private void DrawPaths(GameObject node, GameObject nextNode) {
        GameObject lineRendererContainer = new GameObject();
        lineRendererContainer.transform.parent = node.transform;

        LineRenderer lineRenderer = lineRendererContainer.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineRenderMaterial;

        lineRenderer.widthMultiplier = 0.25f;
        lineRenderer.colorGradient = lineRenderColor;

        lineRenderer.SetPosition(0, node.transform.localPosition);
        lineRenderer.SetPosition(1, nextNode.transform.localPosition);
        node.AddComponent<MaintainLineRenderer>().AssignProperties(lineRenderer, node.transform, nextNode.transform);
    }
    private void OnDrawGizmos()    {

        float initRoomWidthOffsetX = Mathf.Abs(initialRoom.transform.localScale.x / 2);
        float roomHeightHalved = (gameObject.GetComponent<Renderer>().bounds.size.y / 2f) - initRoomWidthOffsetX;
        float roomHeight = (gameObject.GetComponent<Renderer>().bounds.size.y) - initRoomWidthOffsetX;
        float yOffset = initRoomWidthOffsetX * 2;//*2 porque se puede dar que se toquen los nodos


        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, yOffset),Color.green);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -yOffset), Color.green);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, roomHeight / 2), Color.green);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -roomHeight / 2), Color.green);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, yOffset + (roomHeight / 2)), Color.red);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -yOffset - (roomHeight / 2)), Color.red);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, roomHeight), Color.red);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -(roomHeight)), Color.red);
        
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -(roomHeight / 3) + yOffset), Color.yellow);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, (roomHeight / 3) - yOffset), Color.yellow);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -(roomHeight / 3) - yOffset), Color.yellow);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -roomHeight + yOffset), Color.yellow);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, (roomHeight / 3) + yOffset), Color.yellow);
        Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, roomHeight - yOffset), Color.yellow);



        //Debug.DrawLine(new Vector2(-1000, roomHeightHalved / 2), new Vector2(1000, (roomHeightHalved / 2)));
        //Debug.DrawLine(new Vector2(-1000,-roomHeightHalved / 2), new Vector2(1000, (-roomHeightHalved / 2)));
        //Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, -roomHeight));
        //Debug.DrawLine(new Vector2(-1000,0), new Vector2(1000, roomHeight));
    }
}
