using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayWireSimulator : MonoBehaviour
{

    ExtendableWire extendableWire1;
    ExtendableWire extendableWire2;
    private Socket currentSocket; // socket component from child
    public Socket nextSocket1; // socket to which the wire will connect to
    public Socket nextSocket2;

    // Start is called before the first frame update
    void Start()
    {
        
        extendableWire1 = transform.Find("WireOne").GetComponentInChildren<ExtendableWire>();
        extendableWire2 = transform.Find("WireTwo").GetComponentInChildren<ExtendableWire>();
        currentSocket = transform.Find("WireSocket").GetComponent<Socket>();

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(currentWireSocket.socketVoltage);

        nextSocket1 = extendableWire1.connectedSocket;
        nextSocket2 = extendableWire2.connectedSocket;

        if(nextSocket1 != null && nextSocket1.voltageValueCannotBeChanged == false && nextSocket2 == null)
        {
            //Debug.Log("Connected");
            nextSocket1.socketVoltage = currentSocket.socketVoltage;
            nextSocket1.socketCurrent = currentSocket.socketCurrent;
        }

        if (nextSocket2 != null && nextSocket2.voltageValueCannotBeChanged == false && nextSocket1 == null)
        {
            nextSocket2.socketVoltage = currentSocket.socketVoltage;
            nextSocket2.socketCurrent = currentSocket.socketCurrent;
        }

        if(nextSocket1 != null && nextSocket1.voltageValueCannotBeChanged == false && nextSocket2 != null && nextSocket2.voltageValueCannotBeChanged == false)
        {
            Stack<Transform> nextSeriesConn = new Stack<Transform>();
            float pathOneResult = findEquivalentResistanceDFS(extendableWire1.connectedSocket.transform, Battery.inputWireSocket, Battery.cyclicalPath, 0, nextSeriesConn);
            float pathTwoResult = findEquivalentResistanceDFS(extendableWire2.connectedSocket.transform, Battery.inputWireSocket, Battery.cyclicalPath, 0, nextSeriesConn);

            float pathOneCurrentFactor = pathOneResult / (pathOneResult + pathTwoResult);
            float pathTwoCurrentFactor = 1 - pathOneCurrentFactor;

            nextSocket1.socketCurrent = pathOneCurrentFactor * currentSocket.socketCurrent;
            nextSocket2.socketCurrent = pathTwoCurrentFactor * currentSocket.socketCurrent;

            nextSocket1.socketVoltage = currentSocket.socketVoltage;
            nextSocket2.socketVoltage = currentSocket.socketVoltage;
        }

        //Debug.Log(currentWireSocket.socketVoltage);

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
