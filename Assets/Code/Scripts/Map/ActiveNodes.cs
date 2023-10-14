using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ActiveNodes : MonoBehaviour
{
    public List<GameObject> connectedPrevNodes;
    public List<GameObject> connectedNextNodes;
    public void ActiveAllNodes() {
        if (connectedPrevNodes.Count > 0)
        {
            foreach (GameObject prevNode in connectedPrevNodes)
            {
                prevNode.GetComponent<Button>().interactable = true;
            }
        }

    }
}
