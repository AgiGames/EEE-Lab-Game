using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    [SerializeField] private SpriteRenderer offSprite; // image for when the switch is off
    [SerializeField] private SpriteRenderer onSprite; // image for when the switch is on
    private bool on = false; // boolean for switch's on or active status

    private Transform inputWireSocket;
    private Socket inputSocket;

    private Transform outputWireSocket;
    private Socket outputSocket;

    // Start is called before the first frame update
    void Start()
    {

        // assuming the switch to be off
        // which means the input socket will not be connected to the output socket

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();
        inputSocket.preLinked = false; // so put preLinked to false

        outputWireSocket = transform.Find("WireContinuation").Find("WireSocket");
        outputSocket = outputWireSocket.GetComponent<Socket>();

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

    }

    private void OnMouseDown()
    {

        // flip the boolean variable (complement)
        on = !on;
        inputSocket.preLinked = on; // if on is true, preLinked should be true, and vice versa

        // if on == true then make onSprite visible and offSprite transparent
        if(on)
        {
            Color color = offSprite.color;
            color.a = Mathf.Clamp01(0);
            offSprite.color = color;

            color = onSprite.color;
            color.a = Mathf.Clamp01(1);
            onSprite.color = color;

            if(inputSocket.socketVoltage != -1)
            {
                outputSocket.socketVoltage = inputSocket.socketVoltage;
            }

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
        }

    }

}
