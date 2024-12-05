using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayInputSwitch : MonoBehaviour
{

    [SerializeField] private SpriteRenderer offSprite; // image for when the switch is off
    [SerializeField] private SpriteRenderer onSprite; // image for when the switch is on

    private bool on = false; // boolean for switch's on or active status

    private Transform outputWireSocket;
    private Socket outputSocket; // the one output socket

    private Transform inputWireSocketUp;
    private Socket inputSocketUp; // the upper input socket

    private Transform inputWireSocketDown;
    private Socket inputSocketDown; // the lower input socket

    // Start is called before the first frame update
    void Start()
    {

        inputWireSocketUp = transform.Find("InputWireSocketUp");
        inputSocketUp = inputWireSocketUp.GetComponent<Socket>();

        if (inputSocketUp != null)
        {
            Debug.Log("Input Up Socket Found");
        }

        inputWireSocketDown = transform.Find("InputWireSocketDown");
        inputSocketDown = inputWireSocketDown.GetComponent<Socket>();

        if (inputSocketDown != null)
        {
            Debug.Log("Input Down Socket Found");
        }

        // assuming the switch to be off
        // which means the upper input socket will be connected to the output socket

        outputWireSocket = transform.Find("WireContinuation").Find("WireSocket");
        outputSocket = outputWireSocket.GetComponent<Socket>();

        inputSocketUp.preLinked = true; // upper link will be active at start
        inputSocketDown.preLinked = false; // lower link will be inactive at start
        inputSocketUp.nextConnection = outputWireSocket; // set connection to output socket prefab
        inputSocketDown.nextConnection = outputWireSocket;

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

        // if not on, socket voltage of the upper input socket will be the voltage of the output socket
        if(!on)
        {
            outputSocket.socketVoltage = inputSocketUp.socketVoltage;
        }
        // if on, socket volrage of the lower input socket will be the voltage of the output socket
        else
        {
            outputSocket.socketVoltage = inputSocketDown.socketVoltage;
        }

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

            inputSocketDown.preLinked = true; // down link is active
            inputSocketUp.preLinked = false;
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

            inputSocketDown.preLinked = false;
            inputSocketUp.preLinked = true; // up link is active
        }

    }

}
