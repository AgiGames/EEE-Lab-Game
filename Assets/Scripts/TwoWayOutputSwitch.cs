using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayOutputSwitch : MonoBehaviour
{

    [SerializeField] private SpriteRenderer offSprite; // image for when the switch is off
    [SerializeField] private SpriteRenderer onSprite; // image for when the switch is on

    private bool on = false; // boolean for switch's on or active status

    private Transform inputWireSocket;
    private Socket inputSocket;

    private Transform outputWireSocketUp;
    private Socket outputSocketUp;

    private Transform outputWireSocketDown;
    private Socket outputSocketDown;

    // Start is called before the first frame update
    void Start()
    {

        Transform wireContinuationUp = transform.Find("WireContinuationUp");
        outputWireSocketUp = wireContinuationUp.Find("WireSocket");
        outputSocketUp = outputWireSocketUp.GetComponent<Socket>();

        if(outputSocketUp != null)
        {
            Debug.Log("Output Up Socket Found");
        }

        Transform wireContinuationDown = transform.Find("WireContinuationDown");
        outputWireSocketDown = wireContinuationDown.Find("WireSocket");
        outputSocketDown = outputWireSocketDown.GetComponent<Socket>();

        if (outputSocketDown != null)
        {
            Debug.Log("Output Down Socket Found");
        }

        // assuming the switch to be off
        // which means the input socket will be connected to the upper output socket

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();
        inputSocket.preLinked = true; // so put preLinked to true
        inputSocket.nextConnection = outputWireSocketUp; // set connection to upper socket prefab

        // make the offSprite visible, and onSprite transparent
        Color color = offSprite.color;
        color.a = Mathf.Clamp01(1);
        offSprite.color = color;

        color = onSprite.color;
        color.a = Mathf.Clamp01(0);
        onSprite.color = color;

    }

    // Update is called once per frame
    void Update()
    {

        outputSocketUp.socketVoltage = outputSocketDown.socketVoltage = inputSocket.socketVoltage;

    }

    private void OnMouseDown()
    {

        // flip the boolean variable (complement)
        on = !on;

        // if on == true then make onSprite visible and offSprite transparent
        if (on)
        {
            Color color = offSprite.color;
            color.a = Mathf.Clamp01(0);
            offSprite.color = color;

            color = onSprite.color;
            color.a = Mathf.Clamp01(1);
            onSprite.color = color;

            inputSocket.nextConnection = outputWireSocketDown;
        }
        // else make onsSprite transparent and offSprite visible
        else
        {
            Color color = offSprite.color;
            color.a = Mathf.Clamp01(1);
            offSprite.color = color;

            color = onSprite.color;
            color.a = Mathf.Clamp01(0);
            onSprite.color = color;

            inputSocket.nextConnection = outputWireSocketUp;
        }

    }

}
