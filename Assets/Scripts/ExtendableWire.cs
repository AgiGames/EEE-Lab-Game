using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


// class to define the logic for moving of the end part of the wire (WireEnd), thus extending the wire
// the wire automatically extends,
// due to the fact that the line renderer keeps drawing the line to the sprite which we shall move using this script

/*
 * The GameObject to which this script is attached to is as follows:
 * Moving
 *      WireIn (SpriteRenderer)
 *      WireEnd (Transform)
 */

public class ExtendableWire : MonoBehaviour
{

    public Socket connectedSocket = null; // the socket to which the wire is connectef
    public bool connectionFound = false; // status of the wire's connection from moving it
    private Vector3 startPoint; // starting point of the WireEnd sprite, before moving
                                // will be used to change the direction of the WireEnd sprite based on how the WireEnd has been moved
    private Vector3 startingDirection;

    // Start is called before the first frame update
    void Start()
    {
        
        startPoint = transform.position; // get the start point of the game object
        startingDirection = transform.right; // get the starting direction of the wire
        // the above two variables will be used to reset the position of the wire

    }

    // Update is called once per frame
    void Update()
    {

    }

    // whenever the mouse is dragged (LMB down)
    private void OnMouseDrag()
    {
        
        // get new position, which is wherever the mouse is pointing to
        Vector3 newPosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        newPosition.z = 0; // 2D plane so z axis = 0

        // check for nearby connection points
        Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition, .2f);
        foreach(Collider2D collider in colliders)
        {
            // make sure the collider is not owned by this game object
            if(collider.gameObject != gameObject)
            {
                // update wire to the connection point position
                // find the "WirePoint" child of the collider's parent object
                Transform wirePoint = collider.transform.Find("WirePoint");

                Transform wireSocket = collider.transform; // get the transfrom to which the collider is attached

                // if the "WirePoint" exists, use its position
                if (wirePoint != null)
                {
                    if(wireSocket != null)
                    {
                        connectedSocket = wireSocket.GetComponent<Socket>();
                    }

                    if (connectionFound == false)
                    {
                        connectedSocket.numInputConnections++;
                    }
                    connectionFound = true; // connection is found

                    updateWire(wirePoint.position); // update the wire's position and rotation using the collider's transfrom position
                    return;
                }
            }
        }

        // if the function has not returned a value before this line, then it means a connection has not been found
        if(connectionFound == true && connectedSocket != null)
        {
            connectedSocket.numInputConnections--;
        }
        connectionFound = false; // thus set connectionFound to false
        if (connectedSocket != null && connectedSocket.voltageValueCannotBeChanged == false) // if a connection was previously found, but now the connection is removed
        {
            connectedSocket.socketVoltage = -1;
        }
        connectedSocket = null; // set the connectedSocket reference to null, since now the wire is not connected to anything

        updateWire(newPosition); // update wire end's position and rotation

    }

    private void OnMouseUp()
    {

        // if no connection is found, then we would like for the wire to return to its starting point (reset position)
        if(!connectionFound)
        {
            updateWire(startPoint); // thus sending the startPoint to update the wire's position
        }

    }

    private void updateWire(Vector3 newPosition)
    {

        transform.position = newPosition; // change the position to new position

        // if the argument given is same as the startPoint, then we must use the starting direction
        Vector3 direction;
        if (newPosition == startPoint)
        {
            direction = startingDirection;
        }
        // else we calculate the current direction
        else
        {
            direction = (newPosition - startPoint) * transform.lossyScale.x; // get the direction to which the wire end must face now
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // make the sprite renderer face that specific direction
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

}
