using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ActiveNodes : MonoBehaviour
{
    public List<GameObject> connectedPrevNodes;
    public List<GameObject> connectedNextNodes;
    public void ActivateNodes() {
        if (connectedPrevNodes.Count > 0)
        {
            foreach (GameObject prevNode in connectedPrevNodes)
            {
                prevNode.GetComponent<Button>().interactable = true;
            }
        }
        if (connectedNextNodes.Count > 0)
        {
            foreach (GameObject nextNode in connectedNextNodes)
            {
                nextNode.GetComponent<Button>().interactable = true;
            }
        }

    }

    public void DeactivateNodes() {
        if (connectedPrevNodes.Count > 0)
        {
            foreach (GameObject prevNode in connectedPrevNodes) // accedo a cada GO de cada Nodo previo
            {
                List<GameObject> prevNodePrevNodes = prevNode.GetComponent<ActiveNodes>().connectedPrevNodes;

                foreach (GameObject prevPrevNode in prevNodePrevNodes)  // accedo a cada GO de cada Nodo previo de cada Nodo previo
                {
                    prevPrevNode.GetComponent<Button>().interactable = false; //desactivo el botón de cada Nodo previo de cada Nodo previo
                }
            }
        }
        if (connectedNextNodes.Count > 0)
        {
            foreach (GameObject nextNode in connectedNextNodes) // accedo a cada GO de cada Nodo siguiente
            {
                List<GameObject> nextNodeNextNodes = nextNode.GetComponent<ActiveNodes>().connectedNextNodes;

                foreach (GameObject nextNextNode in nextNodeNextNodes)  // accedo a cada GO de cada Nodo siguiente de cada Nodo siguiente
                {
                    nextNextNode.GetComponent<Button>().interactable = false; //desactivo el botón de cada Nodo siguiente de cada Nodo siguiente
                }
            }
        }

        for (int i = 0; i < gameObject.transform.parent.childCount; i++)
        {
            if (gameObject.transform.parent.GetChild(i).gameObject != gameObject) {
                if (gameObject.transform.parent.GetChild(i).gameObject.GetComponent<Button>() != null) {
                    gameObject.transform.parent.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
                }

            }

        }

    }
}
