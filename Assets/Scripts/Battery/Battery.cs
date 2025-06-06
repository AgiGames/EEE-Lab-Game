using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] Transform outputWirePointTransform;
    [SerializeField] Transform inputWirePointTransfrom;
    private WirePoint outputWirePoint;
    private WirePoint inputWirePoint;

    public static float voltage = 0;
    public static float equivalentResistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
        outputWirePoint = outputWirePointTransform.GetComponent<WirePoint>();
        inputWirePoint = inputWirePointTransfrom.GetComponent<WirePoint>();

        voltage = outputWirePoint.wirePointVoltage;

    }

    // Update is called once per frame
    void Update()
    {
        
        HashSet<Transform> visited = new HashSet<Transform>();

        if (isCircuitCompleteDFS(outputWirePointTransform, inputWirePointTransfrom, visited)) {
            HashSet<Transform> visited1 = new HashSet<Transform>();
            float resultantResistance = getEquivalentResistance(outputWirePointTransform, inputWirePointTransfrom, visited1, 0);
            equivalentResistance = resultantResistance;
        }
        else
        {
            Debug.Log("Fucking hell just kill me already");
            equivalentResistance = float.PositiveInfinity;
        }

        Debug.Log($"{equivalentResistance} is the equivalent resistance");
        outputWirePoint.wirePointCurrent = outputWirePoint.wirePointVoltage / equivalentResistance;

    }

    float getEquivalentResistance(Transform ithNode, Transform target, HashSet<Transform> visited, float resistanceSoFar)
    {
        WirePoint wirePoint = ithNode.GetComponent<WirePoint>();
 

        if (ithNode == target)
        {
            return resistanceSoFar + (wirePoint.wirePointResistance);
        }

        if (visited.Contains(ithNode))
        {
            return float.PositiveInfinity;
        }

        visited.Add(ithNode);

        if(wirePoint.preLinked)
        {
            return getEquivalentResistance(wirePoint.nextPreLinkedConnection, target, visited, resistanceSoFar + (wirePoint.wirePointResistance));
        }
        else
        {
            if (ithNode.transform.parent.name.StartsWith("TwoWayWire"))
            {

                Tuple<Transform, float> result = BFS(ithNode);
                Transform nextConnection = result.Item1;
                float resultingResistance = result.Item2;
                Debug.Log($"{resultingResistance}");
                Debug.Log($"{nextConnection.GetInstanceID()} {target.GetInstanceID()}");

                return getEquivalentResistance(nextConnection, target, visited, resistanceSoFar + resultingResistance);
            }
            else
            {
                Transform firstConnection = wirePoint.GetFirstConnection();
                return getEquivalentResistance(firstConnection, target, visited, resistanceSoFar + (wirePoint.wirePointResistance));
            }
        }

    }

    public static Tuple<Transform, float> BFS(Transform doubleChildedRoot)
    {

        float[] resistances = new float[2] { 0, 0 };
        var bfsQueue = new Queue<Tuple<Transform, int>>();

        WirePoint doubleChildedRootWirePoint = doubleChildedRoot.GetComponent<WirePoint>();
        List<Transform> nextConnections = doubleChildedRootWirePoint.GetAllConnections() ?? new List<Transform>();

        if (nextConnections.Count >= 1)
        {
            bfsQueue.Enqueue(new Tuple<Transform, int>(nextConnections[0], 0));
        }
        if (nextConnections.Count >= 2)
        {
            bfsQueue.Enqueue(new Tuple<Transform, int>(nextConnections[1], 1));
        }

        HashSet<Transform> visited = new HashSet<Transform>();
        visited.Add(doubleChildedRoot);
        List<Transform> visitedOrder = new List<Transform>();

        while (bfsQueue.Count > 0)
        {
            Tuple<Transform, int> state = bfsQueue.Dequeue();
            Transform node = state.Item1;
            int resistanceIdx = state.Item2;

            if (visited.Contains(node))
            {
                // Debug.Log("It Converges");
                Debug.Log($"{resistances[0]}, {resistances[1]}");
                if (resistances[0] == 0)
                {
                    Debug.Log("0 is 0");
                    return Tuple.Create(nextConnections[1], 0.0f);
                }
                if (resistances[1] == 0)
                {
                    Debug.Log("1 is 0");
                    return Tuple.Create(nextConnections[0], 0.0f);
                }
                float r0 = resistances[0], r1 = resistances[1];
                float equivalentResistance = (r0 * r1) / (r0 + r1);
                return Tuple.Create(node, equivalentResistance);
            }

            visited.Add(node);
            visitedOrder.Add(node);

            if (node.transform.parent.name.StartsWith("TwoWayWire"))
            {
                Tuple<Transform, float> result = BFS(node);
                Transform nextNode = result.Item1;
                float subResistance = result.Item2;
                resistances[resistanceIdx] += subResistance;
                bfsQueue.Enqueue(new Tuple<Transform, int>(nextNode, resistanceIdx));
            }

            else
            {
                // Debug.Log("What the fuck");
                WirePoint nodeWirePoint = node.GetComponent<WirePoint>();
                if (nodeWirePoint.wirePointResistance > 0)
                {   
                    resistances[resistanceIdx] += nodeWirePoint.wirePointResistance;
                    // Debug.Log($"{resistances[0]}, {resistances[1]}");
                }

                if (nodeWirePoint.preLinked)
                {
                    Transform nextNode = nodeWirePoint.nextPreLinkedConnection;
                    if (nextNode != null)
                    {
                        bfsQueue.Enqueue(new Tuple<Transform, int>(nextNode, resistanceIdx));
                    }
                }

                else
                {
                    Transform nextNode = nodeWirePoint.GetFirstConnection();
                    if (nextNode != null)
                    {
                        bfsQueue.Enqueue(new Tuple<Transform, int>(nextNode, resistanceIdx));
                    }
                }
            }
        }


        float finalResistance = 0;
        Transform lastVisited = null;
        if (resistances[1] > 0)
        {
            finalResistance = resistances[1];
            lastVisited = nextConnections[1];
        }

        else
        {
            finalResistance = resistances[0];
            lastVisited = nextConnections[0];
        }
        return Tuple.Create(lastVisited, finalResistance);

    }

    float CalculateParallelResistance(List<float> resistances)
    {
        if (resistances == null || resistances.Count == 0)
            return float.PositiveInfinity; // No resistors means infinite resistance (open circuit)

        float inverseSum = 0f;

        foreach (float resistance in resistances)
        {
            //Debug.Log(resistance);
            if (resistance != 0) // Avoid division by zero
                inverseSum += 1f / resistance;
        }

        return (inverseSum > 0) ? 1f / inverseSum : float.PositiveInfinity; // If no valid resistance, return infinity
    }

    bool isCircuitCompleteDFS(Transform ithNode, Transform target, HashSet<Transform> visited)
    {

        if (ithNode == target)
        {
            return true;
        }

        if (visited.Contains(ithNode))
        {
            return false;
        }

        visited.Add(ithNode);

        WirePoint wirePoint = ithNode.GetComponent<WirePoint>();

        if (wirePoint.preLinked)
        {
            return isCircuitCompleteDFS(wirePoint.nextPreLinkedConnection, target, visited);
        }
        else
        {
            if (ithNode.transform.parent.name.StartsWith("TwoWayWire"))
            {
                List<Transform> nextConnections = wirePoint.GetAllConnections();
                foreach (Transform nextConnection in nextConnections)
                {
                    if (isCircuitCompleteDFS(nextConnection, target, visited))
                    {
                        return true; // Return true only if a valid path is found
                    }
                }
            }
            else
            {
                Transform firstConnection = wirePoint.GetFirstConnection();
                if (firstConnection != null && isCircuitCompleteDFS(firstConnection, target, visited))
                {
                    return true;
                }
            }
        }

        return false;

    }

}
