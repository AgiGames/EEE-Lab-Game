using UnityEngine;

public class TwoWayOutputSwitch : MonoBehaviour
{
    FlipSwitch flipSwitch; // FlipSwitch Class object attached to the switch 3D model

    WirePoint inputWirePoint;

    Transform topOutputWirePointTransfrom;

    Transform bottomOutputWirePointTransfrom;

    WirePoint topOutputWirePoint;

    WirePoint bottomOutputWirePoint;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flipSwitch = transform.GetComponent<FlipSwitch>();

        inputWirePoint = transform.Find("WirePoint").GetComponent<WirePoint>();

        topOutputWirePointTransfrom = transform.Find("WireTop").Find("WirePoint");

        topOutputWirePoint = topOutputWirePointTransfrom.GetComponent<WirePoint>();

        bottomOutputWirePointTransfrom = transform.Find("WireBottom").Find("WirePoint");

        bottomOutputWirePoint = bottomOutputWirePointTransfrom.GetComponent<WirePoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flipSwitch.on == false)
        {
            inputWirePoint.nextPreLinkedConnection = topOutputWirePointTransfrom;
            topOutputWirePoint.numInputConnections = 1;
            bottomOutputWirePoint.numInputConnections = 0;

            topOutputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;

            bottomOutputWirePoint.wirePointVoltage = -1;
        }

        else
        {
            inputWirePoint.nextPreLinkedConnection = bottomOutputWirePointTransfrom;
            topOutputWirePoint.numInputConnections = 0;
            bottomOutputWirePoint.numInputConnections = 1;

            bottomOutputWirePoint.wirePointVoltage = inputWirePoint.wirePointVoltage;

            topOutputWirePoint.wirePointVoltage = -1;
        }

        ///Debug.Log($"TWO WAY OUTPUT SWITCH:\nINPUT VOLTAGE: {inputWirePoint.wirePointVoltage}\nTOP OUTPUT VOLTAGE: {topOutputWirePoint.wirePointVoltage}\nBOTTOM OUTPUT VOLTAGE: {bottomOutputWirePoint.wirePointVoltage}");
    }
}
