using UnityEngine;

public class Switch : MonoBehaviour
{

    FlipSwitch flipSwitch; // FlipSwitch Class object attached to the switch 3D model

    WirePoint inputWirePoint;

    Transform outputWirePointTransform;

    WirePoint outputWirePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        flipSwitch = transform.GetComponent<FlipSwitch>();

        inputWirePoint = transform.Find("WirePoint").GetComponent<WirePoint>();

        outputWirePointTransform = transform.Find("Wire").Find("WirePoint");

        outputWirePoint = outputWirePointTransform.GetComponent<WirePoint>();

    }

    // Update is called once per frame
    void Update()
    {
        
        if(flipSwitch.on == false)
        {
            inputWirePoint.preLinked = false;
            inputWirePoint.nextPreLinkedConnection = null;
            outputWirePoint.numInputConnections = 0;
        }

        else
        {
            inputWirePoint.preLinked = true;
            inputWirePoint.nextPreLinkedConnection = outputWirePointTransform;
            outputWirePoint.numInputConnections = 1;
        }

        outputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;
        outputWirePoint.wirePointCurrent = inputWirePoint.wirePointCurrent;

    }
}
