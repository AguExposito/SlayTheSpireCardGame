using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintainLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform startPoint;
    private Transform endPoint;

    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, startPoint.position); // Set the start position of the LineRenderer
        lineRenderer.SetPosition(1, endPoint.position); // Set the end position of the LineRenderer
    }

    public void AssignProperties(LineRenderer lineRenderer, Transform startPoint, Transform endPoint)
    {
        this.lineRenderer = lineRenderer;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
}
