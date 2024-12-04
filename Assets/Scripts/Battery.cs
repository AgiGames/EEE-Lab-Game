using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{

    public float equivalentResistance;
    private Transform outputWireContinuation;
    private WireSimulator outputWireSimulator; // wire and socket that represents the output voltage of the bulb
    private Transform outputWireSocket;
    private Transform inputWireSocket;
    private Socket outputSocket;
    public float voltage;

    // Start is called before the first frame update
    void Start()
    {

        outputWireContinuation = transform.Find("WireContinuation");
        outputWireSimulator = outputWireContinuation.GetComponent<WireSimulator>(); // get output wire simulator
        outputWireSocket = outputWireContinuation.Find("WireSocket");
        outputSocket = outputWireSocket.GetComponent<Socket>();

        inputWireSocket = transform.Find("WireSocket");

        voltage = outputSocket.socketVoltage;

    }

    // Update is called once per frame
    void Update()
    {

        if (isCircuitComplete())
        {
            equivalentResistance = findEquivalentResistance(outputWireSocket);
        }
        else
        {
            equivalentResistance = float.PositiveInfinity;
        }

    }

    bool isCircuitComplete()
    {
        Transform ithObject = inputWireSocket;
        HashSet<Transform> visited = new HashSet<Transform>();

        while (ithObject != null)
        {

            // prevent infinite loops caused by circular references
            if (visited.Contains(ithObject))
            {
                Debug.LogError("Detected a circular connection!");
                return false;
            }
            visited.Add(ithObject);

            // assuming ithObject has Socket.cs installed, since ithObject is always the "WireSocket" prefab
            Socket socket = ithObject.GetComponent<Socket>();
            if (socket != null && socket.preLinked) // if it is preLinked to some other socket
            {
                ithObject = socket.nextConnection; // then ithObject will be that next socket
            }
            // if the socket is not already pre linked to some other one, we have to find to what it is connected to via its wire
            else
            {
                Transform parent = ithObject.transform.parent; // get the parent object of the wire socket, from where we can access the wire's status
                WireSimulator jthWireSimulator = parent.GetComponent<WireSimulator>(); // get the WireSimulator component which has the wire's status
                if (jthWireSimulator != null)
                {
                    if (jthWireSimulator.nextWireSocket != null)
                    {
                        ithObject = jthWireSimulator.nextWireSocket.transform; // set ithObject to the next socket the wire is connected to
                    }
                    else
                    {
                        //Debug.LogWarning("jthWireSimulator.nextWireSocket is null.");
                        ithObject = null;
                    }
                }
                else
                {
                    //Debug.LogWarning("No wire prefab found! Parent: " + parent);
                    ithObject = null;
                }
            }

            if (ithObject == inputWireSocket) // if we have come a full circle, then the circuit is complete
            {
                return true;
            }

        }

        //Debug.Log("Circuit not complete.");
        return false;
    }

    private float findEquivalentResistance(Transform ithObject)
    {

        Socket socket = ithObject.GetComponent<Socket>();

        if (socket != null && socket.preLinked)
        {
            ithObject = socket.nextConnection; // then ithObject will be that next socket
        }
        else
        {
            Transform parent = ithObject.transform.parent; // get the parent object of the wire socket, from where we can access the wire's status
            WireSimulator jthWireSimulator = parent.GetComponent<WireSimulator>(); // get the WireSimulator component which has the wire's status
            if (jthWireSimulator != null)
            {
                if (jthWireSimulator.nextWireSocket != null)
                {
                    ithObject = jthWireSimulator.nextWireSocket.transform; // set ithObject to the next socket the wire is connected to
                }
                else
                {
                    //Debug.LogWarning("jthWireSimulator.nextWireSocket is null.");
                    ithObject = null;
                }
            }
            else
            {
                //Debug.LogWarning("No wire prefab found! Parent: " + parent);
                ithObject = null;
            }
        }

        //Debug.Log($"ithSocket EqRes: {socket.socketResistance}");

        if (ithObject == outputWireSocket)
        {
            return 0;
        }

        return socket.socketResistance + findEquivalentResistance(ithObject);


    }

}
