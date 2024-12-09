using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{

    public static float equivalentResistance;
    private Transform outputWireContinuation;
    private WireSimulator outputWireSimulator; // wire and socket that represents the output voltage of the bulb
    private Transform outputWireSocket;
    public static Transform inputWireSocket;
    private Socket outputSocket; // Socket Class object of socket of output
    public static float voltage;
    public static List<Transform> cyclicalPath;

    // Start is called before the first frame update
    void Start()
    {

        outputWireContinuation = transform.Find("WireContinuation");
        outputWireSimulator = outputWireContinuation.GetComponent<WireSimulator>(); // get output wire simulator
        outputWireSocket = outputWireContinuation.Find("WireSocket");
        outputSocket = outputWireSocket.GetComponent<Socket>(); // get output socket

        inputWireSocket = transform.Find("WireSocket");

        voltage = outputSocket.socketVoltage; // voltage of the battery is stored in the output socket
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log($"In Battery {outputSocket.socketVoltage}, {voltage}");

        HashSet<Transform> visited = new HashSet<Transform>();
        cyclicalPath = new List<Transform>();
        Stack<Transform> nextSeriesConn = new Stack<Transform>();

        bool result = isCircuitCompleteDFS(outputWireSocket, inputWireSocket, visited, cyclicalPath);
        if(result == true)
        {
            cyclicalPath.Add(outputWireSocket);
            float resultantResitance = findEquivalentResistanceDFS(outputWireSocket, inputWireSocket, cyclicalPath, outputSocket.socketResistance, nextSeriesConn);
            if(resultantResitance == outputSocket.socketResistance)
            {
                equivalentResistance = float.PositiveInfinity;
            }
            else
            {
                equivalentResistance = resultantResitance;
            }
        }
        else
        {
            equivalentResistance = float.PositiveInfinity;
        }

        outputSocket.socketCurrent = voltage / equivalentResistance;
        //Debug.Log(equivalentResistance);

    }

    bool isCircuitCompleteDFS(Transform ithNode, Transform target, HashSet<Transform> visited, List<Transform> cyclicalPath)
    {

        //Debug.Log(ithNode.name);

        if (ithNode == target)
        {
            cyclicalPath.Add(target);
            return true;
        }

        if (visited.Contains(ithNode))
        {
            return false;
        }

        visited.Add(ithNode);

        Socket socket = ithNode.GetComponent<Socket>();
        if (socket != null && socket.preLinked)
        {
            //Debug.Log("hello");
            bool result = isCircuitCompleteDFS(socket.nextConnection, target, visited, cyclicalPath);
            if(result == true)
            {
                cyclicalPath.Add(socket.nextConnection);
            }
            return result;
        }
        else
        {
            if (ithNode.transform.parent.GetComponent<WireSimulator>() != null)
            {
                WireSimulator jthWireSimulator = ithNode.transform.parent.GetComponent<WireSimulator>();
                if (jthWireSimulator != null && jthWireSimulator.nextWireSocket != null)
                {
                    bool result = isCircuitCompleteDFS(jthWireSimulator.nextWireSocket.transform, target, visited, cyclicalPath);
                    if(result == true)
                    {
                        cyclicalPath.Add(jthWireSimulator.nextWireSocket.transform);
                    }
                    return result;
                }
            }
            else if (ithNode.transform.parent.GetComponent<TwoWayWireSimulator>() != null)
            {
                TwoWayWireSimulator kthTwoWayWireSimulator = ithNode.transform.parent.GetComponent<TwoWayWireSimulator>();
                bool pathOneResult = false;
                HashSet<Transform> visitedPathOne = new HashSet<Transform>(visited);
                HashSet<Transform> visitedPathTwo = new HashSet<Transform>(visited);
                if (kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket1 != null && kthTwoWayWireSimulator.nextSocket1.transform != null)
                {
                    pathOneResult = isCircuitCompleteDFS(kthTwoWayWireSimulator.nextSocket1.transform, target, visitedPathOne, cyclicalPath);
                }
                if (pathOneResult == true)
                {
                    cyclicalPath.Add(kthTwoWayWireSimulator.nextSocket1.transform);
                }
                bool pathTwoResult = false;
                if (kthTwoWayWireSimulator != null && kthTwoWayWireSimulator.nextSocket2 != null && kthTwoWayWireSimulator.nextSocket2.transform != null)
                {
                    pathTwoResult = isCircuitCompleteDFS(kthTwoWayWireSimulator.nextSocket2.transform, target, visitedPathTwo, cyclicalPath);
                }
                if (pathTwoResult == true)
                {
                    cyclicalPath.Add(kthTwoWayWireSimulator.nextSocket2.transform);
                }
                return pathOneResult || pathTwoResult;
            }
        }

        return false;

    }

    private float findEquivalentResistanceDFS(Transform ithNode, Transform target, List<Transform> cyclicalPath, float currentResistance, Stack<Transform> nextSeriesConn)
    {

        if (ithNode == target)
        {
            return currentResistance + ithNode.GetComponent<Socket>().socketResistance;
        }

        Socket socket = ithNode.GetComponent<Socket>();
        if(socket != null && socket.numInputConnections > 1)
        {
            //Debug.Log(socket.numInputConnections);
            if(!nextSeriesConn.Contains(ithNode))
            {
                if (socket.nextConnection != null)
                {
                    nextSeriesConn.Push(socket.nextConnection);
                }
                else if(ithNode.transform.parent.GetComponent<WireSimulator>() != null)
                {
                    nextSeriesConn.Push(ithNode.transform.parent.GetComponent<WireSimulator>().nextWireSocket.transform);
                }
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

                if(pathOneResult == 0 && pathTwoResult != 0)
                {
                    currentResistance += pathTwoResult;
                }
                else if(pathOneResult != 0 && pathTwoResult == 0)
                {
                    currentResistance += pathOneResult;
                }
                else if(pathOneResult != 0 && pathTwoResult != 0)
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
