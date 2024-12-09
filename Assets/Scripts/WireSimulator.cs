using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireSimulator : MonoBehaviour
{

    private ExtendableWire extendableWire; // extendableWire component from child
    private Socket currentWireSocket; // socket component from child
    public Socket nextWireSocket; // socket to which the wire will connect to

    // Start is called before the first frame update
    void Start()
    {

        Transform wirePrefab = transform.Find("Wire");
        if (wirePrefab != null) 
        {
            extendableWire = wirePrefab.GetComponentInChildren<ExtendableWire>(); // get the ExtendibleWire Class object
        }

        Transform currentWireSocketPrefab = transform.Find("WireSocket");
        if (currentWireSocketPrefab != null)
        {
            currentWireSocket = currentWireSocketPrefab.GetComponent<Socket>();
        }

    }

    // Update is called once per frame
    void Update()
    {

        nextWireSocket = extendableWire.connectedSocket;
        // connected socket voltage will be same as current socket voltage
        if (nextWireSocket != null && nextWireSocket.voltageValueCannotBeChanged == false && nextWireSocket.numInputConnections <= 1)
        {
            nextWireSocket.socketVoltage = currentWireSocket.socketVoltage;
            nextWireSocket.socketCurrent = currentWireSocket.socketCurrent;
        }

    }

}
