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
    //[SerializeField]
    //private Transform batteryTransform;
    //private Battery battery;


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
        //battery = batteryTransform.GetComponent<Battery>();

    }

    // Update is called once per frame
    void Update()
    {
        outputSocket.socketCurrent = inputSocket.socketCurrent;
        if(inputSocket.socketVoltage != -1)
        {
            Stack<Transform> nextSeriesConn = new Stack<Transform>();
            float equivalentResistance = findEquivalentResistanceDFS(outputWireSocket, Battery.inputWireSocket, Battery.cyclicalPath, outputSocket.socketResistance, nextSeriesConn);
            float current = inputSocket.socketCurrent;
            //Debug.Log($"Current {current}, Voltage {Battery.voltage}, Er {Battery.equivalentResistance}");
            float voltageDrop = current * resistance;
            outputSocket.socketVoltage = inputSocket.socketVoltage - voltageDrop; // we choose the output socket to hold the resistance value
            //Debug.Log($"Current: {current}, Resistance: {resistance}");
            //Debug.Log($"Input Voltage: {inputSocket.socketVoltage}, Output Voltage: {outputSocket.socketVoltage}");
        }

    }

    private float findEquivalentResistanceDFS(Transform ithNode, Transform target, List<Transform> cyclicalPath, float currentResistance, Stack<Transform> nextSeriesConn)
    {

        if (ithNode == target)
        {
            return currentResistance + ithNode.GetComponent<Socket>().socketResistance;
        }

        Socket socket = ithNode.GetComponent<Socket>();
        if (socket != null && socket.numInputConnections > 1)
        {
            //Debug.Log(socket.numInputConnections);
            if (!nextSeriesConn.Contains(ithNode))
            {
                nextSeriesConn.Push(socket.nextConnection);
            }
            return currentResistance;
        }

        if (socket != null && socket.preLinked && cyclicalPath.Contains(ithNode))
        {
            //Debug.Log("PreLinked");
            currentResistance += socket.socketResistance;
            return findEquivalentResistanceDFS(socket.nextConnection, target, cyclicalPath, currentResistance, nextSeriesConn);
        }
        else
        {
            if (ithNode.transform.parent.GetComponent<WireSimulator>() != null && cyclicalPath.Contains(ithNode))
            {
                //Debug.Log("Not Prelinked");
                currentResistance += socket.socketResistance;
                WireSimulator jthWireSimulator = ithNode.transform.parent.GetComponent<WireSimulator>();
                if (jthWireSimulator != null && jthWireSimulator.nextWireSocket != null)
                {
                    return findEquivalentResistanceDFS(jthWireSimulator.nextWireSocket.transform, target, cyclicalPath, currentResistance, nextSeriesConn);
                }
            }
            else if (ithNode.transform.parent.GetComponent<TwoWayWireSimulator>() != null && cyclicalPath.Contains(ithNode))
            {
                currentResistance += socket.socketResistance;
                TwoWayWireSimulator kthTwoWayWireSimulator = ithNode.transform.parent.GetComponent<TwoWayWireSimulator>();
                float pathOneResult = 0;
                float pathTwoResult = 0;
                if (kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket1 != null && kthTwoWayWireSimulator.nextSocket1.transform != null)
                {
                    pathOneResult = findEquivalentResistanceDFS(kthTwoWayWireSimulator.nextSocket1.transform, target, cyclicalPath, 0, nextSeriesConn);
                }
                if (kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket2 != null && kthTwoWayWireSimulator.nextSocket2.transform != null)
                {
                    pathTwoResult = findEquivalentResistanceDFS(kthTwoWayWireSimulator.nextSocket2.transform, target, cyclicalPath, 0, nextSeriesConn);
                }

                if (pathOneResult == 0 && pathTwoResult != 0)
                {
                    currentResistance += pathTwoResult;
                }
                else if (pathOneResult != 0 && pathTwoResult == 0)
                {
                    currentResistance += pathOneResult;
                }
                else if (pathOneResult != 0 && pathTwoResult != 0)
                {
                    currentResistance += (pathOneResult * pathTwoResult) / (pathOneResult + pathTwoResult);
                }
                //Debug.Log($"POR {pathOneResult}, PTR {pathTwoResult}");
                //Debug.Log($"stack size {nextSeriesConn.Count}, current resistance {currentResistance}");
                if (nextSeriesConn.Count > 0)
                {
                    //Debug.Log(nextSeriesConn.Peek().GetComponent<Socket>().preLinked);
                    return findEquivalentResistanceDFS(nextSeriesConn.Pop(), target, cyclicalPath, currentResistance, nextSeriesConn);
                }
            }
        }

        return currentResistance;

    }

}
