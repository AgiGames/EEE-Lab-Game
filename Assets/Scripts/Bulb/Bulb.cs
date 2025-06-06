using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Bulb : MonoBehaviour
{
    [SerializeField] private GameObject bulbGameObject;

    private Material bulbMaterial; // Reference to the Light component

    private WirePoint inputWirePoint; // WirePoint Class object of input wire point
    private WirePoint outputWirePoint; // WirePoint Class object of output wire point

    [SerializeField] private Transform inputWirePointTransform;
    [SerializeField] private Transform outputWirePointTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Material[] materials = bulbGameObject.GetComponentInChildren<Renderer>().sharedMaterials;

        for(int i = 0; i < materials.Length; i++)
        {
            Debug.Log($"{materials[i].name}, {i}");
        }

        bulbMaterial = materials[0];

        bulbMaterial.EnableKeyword("_EMISSION");

        /*        inputWirePointTransform = transform.Find("WirePoint");
                outputWirePointTransform = transform.Find("Wire").Find("WirePoint");*/

        // Instantiate both objects based on hierarchial order
        inputWirePoint = inputWirePointTransform.GetComponent<WirePoint>();
        outputWirePoint = outputWirePointTransform.GetComponent<WirePoint>();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(inputWirePoint.wirePointVoltage);

        // You can add logic here to toggle or reset the color if needed
        ///Debug.Log(inputWirePoint.wirePointVoltage);
        ///Debug.Log(outputWirePoint.wirePointVoltage);

        HashSet<Transform> visited = new HashSet<Transform>();
        ///Debug.Log("Input wire point transform id: ");
        ///Debug.Log(inputWirePointTransform.GetInstanceID());
        ///Debug.Log("START");
        bool circuitComplete = isCircuitCompleteDFS(outputWirePointTransform, inputWirePointTransform, visited);
        ///Debug.Log(circuitComplete);
        ///Debug.Log("END");

        if (inputWirePoint.wirePointVoltage != -1 && circuitComplete)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }

        outputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;
        outputWirePoint.wirePointCurrent = inputWirePoint.wirePointCurrent;
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

    void TurnOnLight()
    {
        bulbMaterial.EnableKeyword("_EMISSION");
        bulbMaterial.color = Color.yellow;
    }

    void TurnOffLight()
    {
        bulbMaterial.DisableKeyword("_EMISSION");
        bulbMaterial.color = Color.white;
    }
}
