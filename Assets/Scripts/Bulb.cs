using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering;
using UnityEditor.Rendering;
using UnityEngine;

/*
 * The GameObject to which this script is attached to is as follows:
 * Bulb
 *      Light (SpriteRenderer)
 *      WireConinuation (Transform) -> represents the wire connected to a scoket using which we connect to another electrical component
 *          Wire (Transform)
 *          WireSocket (Transform)
 *      WireSocket (Transform)
 *      BulbHolder(SpriteRenderer)
 */

public class Bulb : MonoBehaviour
{

    [SerializeField] SpriteRenderer lightSprite; // sprite that represents the light bulb
    private Transform outputWireContinuation; // WireContinuation transfrom whose has children with necessary components
    private Socket outputSocket; // wire socket that represents the output voltage
    Transform inputWireSocket; // WireSocket transform whose children has necessary components
    private Socket inputSocket; // wire socket that represents the input voltage
    Color startingLightColour; // starting colour of the light (colour for off state)
    
    // Start is called before the first frame update
    void Start()
    {

        startingLightColour = lightSprite.color; // save the starting colour

        outputWireContinuation = transform.Find("WireContinuation");
        
        outputSocket = outputWireContinuation.Find("WireSocket").GetComponent<Socket>();

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>(); // get input socket

    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log($"bulb {inputSocket.socketVoltage}");
        //Debug.Log(inputSocket.hasBeenConnectedTo);

        outputSocket.socketVoltage = inputSocket.socketVoltage; // output voltage and input voltage will be the same
        outputSocket.socketCurrent = inputSocket.socketCurrent;
        
        HashSet<Transform> visited = new HashSet<Transform>();

        // if the circuit is complete, then the light should glow
        if (inputSocket.socketVoltage != -1 && isCircuitCompleteDFS(outputWireContinuation.Find("WireSocket"), inputWireSocket, visited))
        {
            lightSprite.color = Color.yellow; // yellow = lit
        }
        else
        {
            lightSprite.color = startingLightColour;
        }

    }

    bool isCircuitCompleteDFS(Transform ithNode, Transform target, HashSet<Transform> visited)
    {

        //Debug.Log(ithNode.name);

        if (ithNode == target)
        {
            return true;
        }

        if(visited.Contains(ithNode))
        {
            return false;
        }

        visited.Add(ithNode);

        Socket socket = ithNode.GetComponent<Socket>();
        if (socket != null && socket.preLinked)
        {
            //Debug.Log("hello");
            return isCircuitCompleteDFS(socket.nextConnection, target, visited);
        }
        else
        {
            if (ithNode.transform.parent.GetComponent<WireSimulator>() != null)
            {
                WireSimulator jthWireSimulator = ithNode.transform.parent.GetComponent<WireSimulator>();
                if (jthWireSimulator != null && jthWireSimulator.nextWireSocket != null)
                {
                    return isCircuitCompleteDFS(jthWireSimulator.nextWireSocket.transform, target, visited);
                }
            }
            else if (ithNode.transform.parent.GetComponent<TwoWayWireSimulator>() != null)
            {
                TwoWayWireSimulator kthTwoWayWireSimulator = ithNode.transform.parent.GetComponent<TwoWayWireSimulator>();
                bool pathOneResult = false;
                if (kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket1 != null && kthTwoWayWireSimulator.nextSocket1.transform != null)
                {
                    pathOneResult =  isCircuitCompleteDFS(kthTwoWayWireSimulator.nextSocket1.transform, target, visited);
                }
                if (pathOneResult)
                {
                    return pathOneResult;
                }
                else if (!pathOneResult && kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket2 != null && kthTwoWayWireSimulator.nextSocket2.transform != null)
                {
                    return pathOneResult || isCircuitCompleteDFS(kthTwoWayWireSimulator.nextSocket2.transform, target, visited);
                }
            }
        }

        return false;

    }

}
