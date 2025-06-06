using UnityEngine;

public class Resistor : MonoBehaviour
{

    [SerializeField] Transform inputWirePointTransfrom;
    [SerializeField] Transform outputWirePointTransfrom;
    private WirePoint inputWirePoint;
    private WirePoint outputWirePoint;
    public float resistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        inputWirePoint = inputWirePointTransfrom.GetComponent<WirePoint>();
        outputWirePoint = outputWirePointTransfrom.GetComponent<WirePoint>();
        outputWirePoint.wirePointResistance = resistance;

    }

    // Update is called once per frame
    void Update()
    {
        if(inputWirePoint.wirePointVoltage != -1)
        {
            float current = inputWirePoint.wirePointCurrent;
            float voltageDrop = current * resistance;
            //Debug.Log($"{voltageDrop} is the voltage drop");
            outputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage - voltageDrop;
        }
        outputWirePoint.wirePointCurrent = inputWirePoint.wirePointCurrent;
    }
}
