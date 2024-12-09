using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluorescentLamp : MonoBehaviour
{
    private Color startingFluorescentLampColor;
    [SerializeField] private SpriteRenderer fluorescentLampSprite;
    private Transform outputWireContinuation;
    private Socket outputSocket;
    Transform inputWireSocket;
    private Socket inputSocket;
    [SerializeField] private Transform starterInputWireSocket;
    [SerializeField] private Transform inductorInputWireSocket;
    private bool starterConnected = false; // Track starter connection state
    private float starterConnectionTime = 1.0f; // Time the starter remains connected (in seconds)
    private float starterTimer = 0.0f; // Tracks elapsed time
    private bool needsStarter = true;

    void Start()
    {
        startingFluorescentLampColor = fluorescentLampSprite.color;
        outputWireContinuation = transform.Find("WireContinuation");
        outputSocket = outputWireContinuation.Find("WireSocket").GetComponent<Socket>();
        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();
    }

    void Update()
    {
        outputSocket.socketCurrent = inputSocket.socketCurrent;
        outputSocket.socketVoltage = inputSocket.socketVoltage;

        Debug.Log(inputSocket.socketVoltage);
        Debug.Log(Battery.cyclicalPath.Contains(starterInputWireSocket));
        Debug.Log(Battery.cyclicalPath.Contains(inductorInputWireSocket));

        if (inputSocket.socketVoltage == -1)
        {
            needsStarter = true;
            starterConnected = false;
        }

        if (needsStarter && Battery.cyclicalPath.Contains(inputWireSocket) && Battery.cyclicalPath.Contains(starterInputWireSocket))
        {
            // Start the starter timer
            starterConnected = true;
            starterTimer += Time.deltaTime;

            // Disconnect the starter after the specified time
            if (starterTimer >= starterConnectionTime)
            {
                starterConnected = false;
                needsStarter = false;
                starterTimer = 0.0f;
            }
        }

        if (starterConnected == false && needsStarter == false && Battery.cyclicalPath.Contains(inductorInputWireSocket))
        {
            // Starter and inductor are connected, and voltage is not -1
            fluorescentLampSprite.color = Color.white;
        }
        else
        {
            fluorescentLampSprite.color = startingFluorescentLampColor;
            needsStarter = true;
        }
    }
}
