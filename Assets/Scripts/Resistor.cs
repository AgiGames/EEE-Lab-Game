using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resistor : MonoBehaviour
{

    // prerequsisites for calculating the voltage drops, refer the prefab in the prefab view to understand more
    private Transform inputWireSocket;
    private Transform outputWireContinuation;
    private Transform outputWireSocket;
    private Socket inputSocket;
    private Socket outputSocket;
    [SerializeField]
    private Transform batteryTransform;
    private Battery battery;


    private float resistance; // resistance value of resistor

    void Start()
    {

        // prerequsisites for calculating the voltage drops, refer the prefab in the prefab view to understand more
        inputWireSocket = transform.Find("WireSocket");
        outputWireContinuation = transform.Find("WireContinuation");
        outputWireSocket = outputWireContinuation.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();   
        outputSocket = outputWireSocket.GetComponent<Socket>();
        resistance = outputSocket.socketResistance;
        battery = batteryTransform.GetComponent<Battery>();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(inputSocket.socketVoltage != -1)
        {
            float current = battery.voltage / battery.equivalentResistance;
            float voltageDrop = current * resistance;
            outputSocket.socketVoltage = inputSocket.socketVoltage - voltageDrop; // we choose the output socket to hold the resistance value
            Debug.Log($"Input Voltage: {inputSocket.socketVoltage}, Output Voltage: {outputSocket.socketVoltage}");
        }

    }
}
