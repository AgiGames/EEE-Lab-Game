using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

// class to define how the wire should behave.
// the wire is drawn using a Class object of the LineRenderer class

public class Wire : MonoBehaviour
{

    private LineRenderer lineRenderer; // LineRenderer instance
    [SerializeField] private Transform[] points; // points between which the line will be drawn
    [SerializeField] private SpriteRenderer wireStart; // sprite renderer that illustrates the starting point of the wire (line)

    // Start is called before the first frame update
    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length; // set the number of points

        float width = transform.localScale.y / 3.5f; // set the width of the drawn line
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

    }

    // Update is called once per frame
    void Update()
    {

        // draw the line each frame

        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }

    }

}
