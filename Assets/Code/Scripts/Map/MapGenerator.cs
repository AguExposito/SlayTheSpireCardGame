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
using System.Linq;
using System;
using Unity.Mathematics;

public class MapGenerator : MonoBehaviour
{
    private static MapGenerator mapGen;
    Vector2Int initialBranchRooms = new Vector2Int(1,4); //x=min y=max
    int initialBranch = 0;
    int currentStep = 0;

    [SerializeField] GameObject room;
    [SerializeField] Sprite[] roomsIcon = new Sprite[4];

    [SerializeField] GameObject initialRoomGO;
    [SerializeField] GameObject finalRoomGO;
    [SerializeField] Node finalRoom = new Node();
    [SerializeField] Node initialRoom = new Node();

    [SerializeField] Material lineRenderMaterial;
    [SerializeField] Gradient lineRenderColor;
    [SerializeField] Scrollbar scrollbar;

    [SerializeField] List<GameObject> nodesContainers = new List<GameObject>();
    [SerializeField] List<Node> nodesList = new List<Node>();


    [SerializeField, Header("Room Spawn Settings")] Vector2Int roomQuantity;
    public int segments = 0;
    [SerializeField, Range(0,100)] int deleteRoomPercentage4, deleteRoomPercentage3, deleteRoomPercentage2;
    [Header("Room Type Settings")]
    [SerializeField, Range(0,100), Tooltip("They must sum 100 to work")] int fightPercentage, eventPercentage, relaxPercentage, subBossRooms;

    
    class Node {
        public MapGenerator mapGenerator;
        public List<Node> conectedToNext = new List<Node>();
        public List<Node> conectedToPrev = new List<Node>();
        public GameObject nodeGO;
        public Sprite sprite;
        public enum roomType {fight, events, subBoss, relax};
        roomType rType = new roomType();

        public Node()
        {
            
        }
        public void AssignGOandSprite(GameObject room, Vector3 spawnPosition,Transform parent) {
            mapGenerator = mapGen;
            int i=0;

            if (mapGen.currentStep >= Mathf.Round(mapGen.segments / 2)) //es <= porque el mapa se arma de derecha a izquierda
            {
                int random = UnityEngine.Random.Range(0, 101);

                if (random <= mapGen.fightPercentage) { i = 1; }
                else if (random > mapGen.fightPercentage && random <= mapGen.fightPercentage + mapGen.eventPercentage) { i = 2; }
                else if (random > mapGen.fightPercentage + mapGen.eventPercentage && random <= mapGen.fightPercentage + mapGen.eventPercentage+mapGen.relaxPercentage) { i = 3; }
                else { i = 4; }

            }
            else {
                int random = UnityEngine.Random.Range(0, 101);
                if (random <= mapGen.fightPercentage) { i = 1; }
                else if (random > mapGen.fightPercentage && random <= mapGen.fightPercentage + mapGen.eventPercentage) { i = 2; }
                else if (random > mapGen.fightPercentage + mapGen.eventPercentage && random <= mapGen.fightPercentage + mapGen.eventPercentage + mapGen.relaxPercentage) { i = 3; }
                else { i = 2; } // sino cae en ninguna opción el resultado es events en lugar de subBoss
            }

            if (mapGen.segments-1 == mapGen.currentStep) { i = 3; }
            if (mapGen.currentStep == Mathf.Round(mapGen.segments / 2)-1) { i = 4;  }

            
            switch (i)
            {
                case 1:
                    {
                        rType = roomType.fight;
                        sprite = mapGenerator.roomsIcon[0];
                    }
                    break;
                case 2:
                    {
                        rType = roomType.events;
                        sprite = mapGenerator.roomsIcon[1];
                    }
                    break;
                case 3:
                    {
                        rType = roomType.relax;
                        sprite = mapGenerator.roomsIcon[2];
                    }
                    break;
                case 4:
                    {

                        rType = roomType.relax;
                        sprite = mapGenerator.roomsIcon[3];
                    }
                    break;
            }

            

            nodeGO = Instantiate(room, spawnPosition, Quaternion.identity, parent);
            nodeGO.GetComponent<Image>().sprite = sprite;
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        mapGen = this;
        segments = UnityEngine.Random.Range(roomQuantity.x, roomQuantity.y+1);
        Debug.Log("Segments: "+ segments);

        GenerateNodes();
        MakeConnections();
    }

    private void GenerateNodes()
    {
        float initRoomWidthOffsetY = Mathf.Abs(initialRoomGO.transform.localScale.y/2);
        float finalRoomWidthOffsetX = Mathf.Abs(finalRoomGO.transform.localScale.x/2);

        float totalDistanceIF = Mathf.Abs((finalRoomGO.transform.position.x) - (initialRoomGO.transform.position.x));
        Debug.Log("Dist: " + totalDistanceIF);


        //float segmentDist = Mathf.Lerp(initialRoom.transform.position.x, finalRoom.transform.position.x, 1/ segments);
        // Calculate segment distance including padding
        float segmentDist = (totalDistanceIF - 2 * finalRoomWidthOffsetX) / segments;
        float counter = 0; // Start with padding

        Vector2 rectPos = gameObject.GetComponent<RectTransform>().position;

        

        for (int k = 0; k< segments; k++) {
            currentStep = k;
            initialBranch = UnityEngine.Random.Range(initialBranchRooms.x, initialBranchRooms.y+1);
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
                Node node = new Node();

                if (initialBranch % 2 == 0)
                {
                    float randomPosY = 0;
                    float randomPosX = UnityEngine.Random.Range(-1,1);
                    int deleteRandomPercentage=0;

                    if (initialBranch == 2)
                    {

                        switch (i)
                        {
                            case 1: { randomPosY = UnityEngine.Random.Range(yOffset, roomHeight); } break;
                            case 2: { randomPosY = UnityEngine.Random.Range(-yOffset, -roomHeight); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage2;
                        //room.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: { randomPosY = UnityEngine.Random.Range(yOffset, roomHeight / 2); } break;
                            case 2: { randomPosY = UnityEngine.Random.Range(-yOffset, -roomHeight / 2); } break;
                            case 3: { randomPosY = UnityEngine.Random.Range(yOffset + (roomHeight / 2), roomHeight); } break;
                            case 4: { randomPosY = UnityEngine.Random.Range(-yOffset - (roomHeight / 2), -roomHeight); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage4;
                        //room.GetComponent<SpriteRenderer>().color = Color.blue;
                    }

                    if (i > 1)
                    {
                        int deadZone = UnityEngine.Random.Range(0, 100);
                        if (deadZone > deleteRandomPercentage)
                        {
                            Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                            Debug.Log("Par");
                            node.AssignGOandSprite(room, spawnPositionPar, roomContainer.transform);
                        }
                        else { break; }
                    }
                    else { 
                        Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                        Debug.Log("Par");
                        node.AssignGOandSprite(room, spawnPositionPar, roomContainer.transform);
                    }
                }
                else { 

                    float randomPosY = 0;
                    float randomPosX = UnityEngine.Random.Range(-1, 1);
                    int deleteRandomPercentage = 0;

                    if (initialBranch == 1)
                    {

                        switch (i)
                        {
                            case 1: { randomPosY = UnityEngine.Random.Range(-roomHeight, roomHeight); } break;
                        }
                        //room.GetComponent<SpriteRenderer>().color = Color.black;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: { randomPosY = UnityEngine.Random.Range(-(roomHeight / 3)+ yOffset, (roomHeight / 3)- yOffset); } break;
                            case 2: { randomPosY = UnityEngine.Random.Range(-(roomHeight / 3)- yOffset, -roomHeight+ yOffset); } break;
                            case 3: { randomPosY = UnityEngine.Random.Range((roomHeight / 3) + yOffset, roomHeight- yOffset); } break;
                        }
                        deleteRandomPercentage = deleteRoomPercentage3;
                        //room.GetComponent<SpriteRenderer>().color = Color.red;
                    }

                    if (i > 1)
                    {
                        int deadZone = UnityEngine.Random.Range(0, 100);
                        if (deadZone > deleteRandomPercentage)
                        {
                            Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                            Debug.Log("Impar");
                            node.AssignGOandSprite(room, spawnPositionPar, roomContainer.transform);
                        }
                        else { break; }
                    }
                    else
                    {
                        Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX + randomPosX, randomPosY);
                        Debug.Log("Par");
                        node.AssignGOandSprite(room, spawnPositionPar, roomContainer.transform);
                    }
                }

                node.nodeGO.tag = "Node";
                
                if (!nodesContainers.Contains(node.nodeGO.transform.parent.gameObject))
                {
                    nodesContainers.Add(node.nodeGO.transform.parent.gameObject);
                }
            }
        }
    }

    private void MakeConnections() {      

        for (int i = nodesContainers.Count-1; i >= 0; i--)
        {
            GameObject nextContainer;
            bool traba=false;

            if (i - 1 >= 0)
            {
                nextContainer = nodesContainers[i - 1];
            }
            else { return; }

            for (int j = 0; j < nodesContainers[i].transform.childCount; j++)
            {
                Node node = new Node();
                
                nodesList.Add(node);
                Node nextNode = new Node();
                node.nodeGO = nodesContainers[i].transform.GetChild(j).gameObject;
                nextNode.nodeGO =null;//null default                
                float distanceBetweenNodes=10000;//Default high value

                if (i == nodesContainers.Count - 1)
                {
                    finalRoom.nodeGO = finalRoomGO.gameObject;
                    DrawPaths(finalRoom, node);
                }
                if (i == 1 && !traba) {
                    Node nodeAUX = new Node();
                    for (int z = 0; z < nodesContainers[i-1].transform.childCount; z++)
                    {
                        nodeAUX.nodeGO = nodesContainers[i - 1].transform.GetChild(z).gameObject;
                        initialRoom.nodeGO = initialRoomGO.gameObject;
                        DrawPaths(nodeAUX, initialRoom);
                        traba= true;
                    }                   
                }

                for (int k = 0; k < nextContainer.transform.childCount; k++)
                {
                    GameObject nextNodeAUX = nextContainer.transform.GetChild(k).gameObject;
                    float distanceBetweenNodesAUX;

                    Vector2 nodesV2 = nextNodeAUX.transform.position - node.nodeGO.transform.position;
                    distanceBetweenNodesAUX = Mathf.Abs( nodesV2.magnitude);

                    if (distanceBetweenNodesAUX< distanceBetweenNodes) {
                        distanceBetweenNodes = distanceBetweenNodesAUX;
                        nextNode.nodeGO = nextContainer.transform.GetChild(k).gameObject;
                    }
                }

                List<GameObject> connectedNodes = new List<GameObject>();

                DrawPaths(node, nextNode);
                connectedNodes.Add(nextNode.nodeGO);

                for (int a = 0; a < nextContainer.transform.childCount; a++)
                {
                    if (!connectedNodes.Contains(nextContainer.transform.GetChild(a).gameObject))
                    {
                        float[] distances = new float[4];

                        for (int b = 0; b < nodesContainers[i].transform.childCount; b++)
                        {
                            distances[b] = (nodesContainers[i].transform.GetChild(b).transform.position - nextContainer.transform.GetChild(a).transform.position).magnitude;

                        }

                        for (int b = 0; b < distances.Length; b++)
                        {
                            if (distances[b] == 0) { distances[b] = 10000; }
                        }

                        nextNode.nodeGO = nextContainer.transform.GetChild(a).gameObject;


                        if (distances[0] <= distances[1] && distances[0] <= distances[2] && distances[0] <= distances[3])
                        {
                            node.nodeGO = nodesContainers[i].transform.GetChild(0).gameObject;
                            DrawPaths(node, nextNode);
                        }
                        else
                        {
                            if (distances[1] < distances[0] && distances[1] < distances[2] && distances[1] < distances[3])
                            {
                                node.nodeGO = nodesContainers[i].transform.GetChild(1).gameObject;
                                DrawPaths(node, nextNode);
                            }
                            else {

                                if (distances[2] < distances[1] && distances[2] < distances[0] && distances[2] < distances[3])
                                {
                                    node.nodeGO = nodesContainers[i].transform.GetChild(2).gameObject;
                                    DrawPaths(node, nextNode);
                                }
                                else {

                                    if (distances[3] < distances[1] && distances[3] < distances[2] && distances[3] < distances[0])
                                    {
                                        node.nodeGO = nodesContainers[i].transform.GetChild(3).gameObject;
                                        DrawPaths(node, nextNode);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }



    }

    private void DrawPaths(Node node, Node nextNode) {
        GameObject lineRendererContainer = new GameObject();
        lineRendererContainer.transform.parent = node.nodeGO.transform;

        LineRenderer lineRenderer = lineRendererContainer.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineRenderMaterial;

        lineRenderer.widthMultiplier = 0.25f;
        lineRenderer.colorGradient = lineRenderColor;

        lineRenderer.SetPosition(0, node.nodeGO.transform.localPosition);
        lineRenderer.SetPosition(1, nextNode.nodeGO.transform.localPosition);
        node.nodeGO.AddComponent<MaintainLineRenderer>().AssignProperties(lineRenderer, node.nodeGO.transform, nextNode.nodeGO.transform);
        node.conectedToNext.Add(nextNode);
        nextNode.conectedToPrev.Add(node);
    }

    private void OnDrawGizmos()    {

        float initRoomWidthOffsetX = Mathf.Abs(initialRoomGO.transform.localScale.x / 2);
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
    }
}
