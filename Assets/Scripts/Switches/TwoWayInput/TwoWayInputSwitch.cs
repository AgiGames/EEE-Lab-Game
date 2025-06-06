using UnityEngine;

public class TwoWayinputSwitch : MonoBehaviour
{
    FlipSwitch flipSwitch; // FlipSwitch Class object attached to the switch 3D model

    WirePoint topInputWirePoint;

    WirePoint bottomInputWirePoint;

    Transform outputWirePointTransform;

    WirePoint outputWirePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flipSwitch = transform.GetComponent<FlipSwitch>();

        topInputWirePoint = transform.Find("WirePointTop").GetComponent<WirePoint>();

        bottomInputWirePoint = transform.Find("WirePointBottom").GetComponent<WirePoint>();

        outputWirePointTransform = transform.Find("Wire").Find("WirePoint");

        outputWirePoint = outputWirePointTransform.GetComponent<WirePoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if(flipSwitch.on == false)
        {
            topInputWirePoint.preLinked = true;
            topInputWirePoint.nextPreLinkedConnection = outputWirePointTransform;
            bottomInputWirePoint.preLinked = false;
            bottomInputWirePoint.nextPreLinkedConnection = null;

            outputWirePoint.wirePointVoltage = topInputWirePoint.wirePointVoltage;
        }
        else
        {
            topInputWirePoint.preLinked = false;
            topInputWirePoint.nextPreLinkedConnection = null;
            bottomInputWirePoint.preLinked = true;
            bottomInputWirePoint.nextPreLinkedConnection = outputWirePointTransform;

            outputWirePoint.wirePointVoltage = bottomInputWirePoint.wirePointVoltage;
        }

        ///Debug.Log($"TWO WAY INPUT SWITCH:\nTOP INPUT VOLTAGE: {topInputWirePoint.wirePointVoltage}\nBOTTOM INPUT VOLTAGE: {bottomInputWirePoint.wirePointVoltage}\nOUTPUT VOLTAGE {outputWirePoint.wirePointVoltage}");
    }
}
