using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inductor : MonoBehaviour
{

    private Transform outputWireContinuation; // WireContinuation transfrom whose has children with necessary components
    private Socket outputSocket; // wire socket that represents the output voltage
    Transform inputWireSocket; // WireSocket transform whose children has necessary components
    private Socket inputSocket; // wire socket that represents the input voltage


    // Start is called before the first frame update
    void Start()
    {

        outputWireContinuation = transform.Find("WireContinuation");

        outputSocket = outputWireContinuation.Find("WireSocket").GetComponent<Socket>();

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>(); // get input socket

    }

    // Update is called once per frame
    void Update()
    {
        
        outputSocket.socketCurrent = inputSocket.socketCurrent;
        outputSocket.socketVoltage = inputSocket.socketVoltage;

    }
}
