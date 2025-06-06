using UnityEngine;

public class TransferVoltageValues : MonoBehaviour
{
    private WirePoint inputWirePoint; // WirePoint Class object of input wire point
    private WirePoint outputWirePoint; // WirePoint Class object of output wire point

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Instantiate both objects based on hierarchial order
        inputWirePoint = transform.Find("WirePoint").GetComponent<WirePoint>();
        outputWirePoint = transform.Find("Wire").Find("WirePoint").GetComponent<WirePoint>();
    }

    // Update is called once per frame
    void Update()
    {
        outputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;
    }
}
