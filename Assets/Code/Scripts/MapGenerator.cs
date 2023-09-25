using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GD.MinMaxSlider;
using Unity.VisualScripting;
using static UnityEngine.Rendering.HableCurve;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int initialBranchRooms; //x=min y=max
    int initialBranch = 0;
    [SerializeField] Vector2Int roomQuantity;
    int segments = 0;

    [SerializeField] GameObject room;
    [SerializeField] Sprite[] roomsIcon = new Sprite[4];
    [SerializeField] Vector2[] parVector2;
    [SerializeField] Vector2[] imparVector2;

    [SerializeField] GameObject initialRoom;
    [SerializeField] GameObject finalRoom;


    // Start is called before the first frame update
    void Start()
    {   
        segments = Random.Range(roomQuantity.x, roomQuantity.y+1);
        Debug.Log("Segments: "+ segments);


        GenerateNodes();
    }

    private void GenerateNodes()
    {
        float initRoomWidthOffsetX = Mathf.Abs(initialRoom.transform.localScale.x/2);
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
            Debug.Log("Segm: " + counter);

            for (int i = 0; i < initialBranch; i++) {

                //float roomOffsetX = room.GetComponent<Renderer>().bounds.size.x / 2f; // Adjust for room width
                float mapOffsetX = (totalDistanceIF) / 2;             

                if (initialBranch % 2 == 0)
                {
                    Vector2 spawnPositionPar = rectPos + new Vector2(counter - mapOffsetX /* -roomOffsetX */, parVector2[i].y);
                    Debug.Log("Par");
                    Instantiate(room, spawnPositionPar, Quaternion.identity, gameObject.transform.GetChild(0));
                }
                else {
                    Vector2 spawnPositionImpar = rectPos + new Vector2(counter - mapOffsetX /* -roomOffsetX */, imparVector2[i].y);
                    Debug.Log("Impar");
                    Instantiate(room, spawnPositionImpar, Quaternion.identity, gameObject.transform.GetChild(0));
                }
            }
        }
    } 
}
