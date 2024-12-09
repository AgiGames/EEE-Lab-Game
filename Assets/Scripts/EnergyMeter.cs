using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyMeter : MonoBehaviour
{

    [SerializeField] private TMP_Text energyText;
    [SerializeField] private int energyTextSortingOrder;
    private Transform inputWireSocket;
    private Socket inputSocket;
    private Transform outputWireSocket;
    private Socket outputSocket;

    private float startTime;
    private float energy = 0;

    // Start is called before the first frame update
    void Start()
    {

        energyText.GetComponent<Renderer>().sortingOrder = energyTextSortingOrder;

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();

        outputWireSocket = transform.Find("WireContinuation").Find("WireSocket");
        outputSocket = outputWireSocket.GetComponent<Socket>();

        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {

        outputSocket.socketVoltage = inputSocket.socketVoltage;
        outputSocket.socketCurrent = inputSocket.socketCurrent;
        
        HashSet<Transform> visited = new HashSet<Transform>();
        List<Transform> cyclicalPath = new List<Transform>();
        if (isCircuitCompleteDFS(outputWireSocket, inputWireSocket, visited, cyclicalPath) && inputSocket.socketVoltage != -1 && Battery.equivalentResistance != float.PositiveInfinity)
        {
            float elapsedTimeInHours = (Time.time - startTime) / 3600f;
            float power = (Battery.voltage * Battery.voltage) / Battery.equivalentResistance;
            energy += (power * elapsedTimeInHours) / 1000;
            energyText.SetText(energy.ToString());
        }
        
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
            if (result == true)
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
                    if (result == true)
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
}
