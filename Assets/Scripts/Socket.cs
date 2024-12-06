using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{

    public bool preLinked = false; // will determine if the connection is hardwired, and not to be done by the user
    public Transform nextConnection = null; // next socket to which it is connected via a wire
    public bool voltageValueCannotBeChanged = false; // determine if the socket voltage cannot be changed or not
    public float socketVoltage = -1; // initial socket voltage, -1 means it is disconnected from any other voltage source, which is how the socket starts
    public float socketResistance = 0;
    public bool hasBeenConnectedTo = false;

    // Start is called before the first frame update1
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  
        if(hasBeenConnectedTo == false && voltageValueCannotBeChanged == false)
        {
            socketVoltage = -1;
        }

    }
}
