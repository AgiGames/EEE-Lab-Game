using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

// class to define how the wire should be drawn
// the wire is drawn using a Class object of the LineRenderer class

/*
 * The GameObject to which this script is attached to is as follows:
 * Wire
 *      WireStart (SpriteRenderer)
 *      Moving (Transform)
 */

public class Wire : MonoBehaviour
{

    private LineRenderer lineRenderer; // LineRenderer instance
    [SerializeField] private Transform[] points; // points between which the line will be drawn
    [SerializeField] private SpriteRenderer wireStart; // sprite renderer that illustrates the starting point of the wire (line)

    // Start is called before the first frame update
    void Start()
    {

        // the points will be given in the unity editor, i.e., two or more transforms

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length; // set the number of points

        float width = transform.localScale.y / 3.5f; // set the width of the drawn line, dividing by 3.5 made it look better
        lineRenderer.startWidth = width; // start width and end width should be the same
        lineRenderer.endWidth = width;

    }

    // Update is called once per frame
    void Update()
    {

        // draw the line each frame
        // for each transform's position, set it to draw line with the line renderer
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }

    }

}
