using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WirePoint : MonoBehaviour
{
    public bool preLinked = false; // Determines if the connection is hardwired and not user-controlled
    public List<Transform> nextConnections = new List<Transform>(); // List allows duplicate connections
    public bool voltageValueCannotBeChanged = false; // Determines if the wire point voltage can change
    public float wirePointVoltage = -1; // Initial wire point voltage (-1 means disconnected)
    public float wirePointResistance = 0; // Resistance of the wire point
    public float numInputConnections; // Number of wires connected to the wire point
    public float wirePointCurrent = 0; // Current flowing through the wire point
    [SerializeField] public Transform nextPreLinkedConnection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.name.StartsWith("TwoWayWire"))
        {
/*            Debug.Log("Start");
            Debug.Log(nextConnections.Count);
            foreach (Transform connection in nextConnections)
            {
                Debug.Log(connection);
            }
            Debug.Log("End");*/
        }

        //Debug.Log(wirePointVoltage);
        //Debug.Log(numInputConnections);
        //Debug.Log(wirePointCurrent);
    }

    // Method to add a connection (allows duplicates)
    public void AddConnection(Transform connection)
    {
        if (connection != null)
        {
            nextConnections.Add(connection);
        }
    }

    // Method to remove a connection (removes only the first occurrence)
    public void RemoveConnection(Transform connection)
    {
        if (connection != null)
        {
            nextConnections.Remove(connection);
        }
    }

    // Method to check if a connection exists
    public bool HasConnection(Transform connection)
    {
        return nextConnections.Contains(connection);
    }

    // Method to get the first connection in the list
    public Transform GetFirstConnection()
    {
        return nextConnections.FirstOrDefault();
    }

    // Method to get all connections
    public List<Transform> GetAllConnections()
    {
        return new List<Transform>(nextConnections);
    }

    // Method to clear all connections
    public void ClearConnections()
    {
        nextConnections.Clear();
    }
}
