using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


// class to define the logic for moving if the end part of the wire, this extending the wire

public class ExtendableWire : MonoBehaviour
{

    public Socket connectedSocket = null;
    public bool connectionFound = false;
    private Vector3 startPoint; // starting point of the wire end, before moving
                                // will be used to change the direction of the wire end SpriteRenderer based on how the wire has been move
    private Vector3 startingDirection;

    // Start is called before the first frame update
    void Start()
    {
        
        startPoint = transform.position; // get the start point of the game object
        startingDirection = transform.right;

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

                    connectionFound = true;
                    connectedSocket.hasBeenConnectedTo = true;

                    updateWire(wirePoint.position); // update the wire's position and rotation using the collider's transfrom position
                    return;
                }
            }
        }

        connectionFound = false;
        if (connectedSocket != null)
        {
            connectedSocket.hasBeenConnectedTo = false;
        }
        connectedSocket = null;

        updateWire(newPosition); // update wire end's position and rotation

    }

    private void OnMouseUp()
    {

        if(!connectionFound)
        {
            updateWire(startPoint);
        }

    }

    private void updateWire(Vector3 newPosition)
    {

        transform.position = newPosition; // change the position to new position

        Vector3 direction;
        if (newPosition == startPoint)
        {
            direction = startingDirection;
        }
        else
        {
            direction = (newPosition - startPoint) * transform.lossyScale.x; // get the direction to which the wire end must face now
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // make the sprite renderer face that specific direction
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

}
