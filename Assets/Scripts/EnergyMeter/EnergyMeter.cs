using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyMeter : MonoBehaviour
{

    [SerializeField] private TMP_Text energyText; // text that shows the energy used so far

    private WirePoint inputWirePoint; // WirePoint Class object of input wire point
    private WirePoint outputWirePoint; // WirePoint Class object of output wire point

    [SerializeField] private Transform inputWirePointTransform;
    [SerializeField] private Transform outputWirePointTransform;

    private float startTime;
    private float energy = 0;

    void Start()
    {
        
        inputWirePoint = inputWirePointTransform.GetComponent<WirePoint>();
        outputWirePoint = outputWirePointTransform.GetComponent<WirePoint>();

        startTime = Time.time;

    }

    // Update is called once per frame
    void Update()
    {

        outputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;
        outputWirePoint.wirePointCurrent = inputWirePoint.wirePointCurrent;

        HashSet<Transform> visited = new HashSet<Transform>();
        if (isCircuitCompleteDFS(outputWirePointTransform, inputWirePointTransform, visited) && inputWirePoint.wirePointVoltage != -1)
        {
            float elapsedTimeInHours = (Time.time - startTime) / 3600f;
            float power = (Battery.voltage * Battery.voltage) / Battery.equivalentResistance;
            energy += (power * elapsedTimeInHours) / 1000;
            energyText.SetText(energy.ToString());
        }

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
                    HashSet<Transform> visitedNew = new HashSet<Transform>(visited);
                    if (isCircuitCompleteDFS(nextConnection, target, visitedNew))
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
