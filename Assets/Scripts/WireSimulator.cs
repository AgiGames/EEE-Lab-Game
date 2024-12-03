using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireSimulator : MonoBehaviour
{

    private ExtendableWire extendableWire; // 
    public Socket nextWireSocket;

    // Start is called before the first frame update
    void Start()
    {

        Transform wirePrefab = transform.Find("Wire");
        if (wirePrefab != null)
        {
            extendableWire = wirePrefab.GetComponentInChildren<ExtendableWire>(); // get the ExtendibleWire Class object
        }

    }

    // Update is called once per frame
    void Update()
    {

        nextWireSocket = extendableWire.connectedSocket;

    }

}
