using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    [SerializeField] private SpriteRenderer offSprite;
    [SerializeField] private SpriteRenderer onSprite;
    private bool on = false;
    private Transform inputWireSocket;
    private Socket inputSocket;

    // Start is called before the first frame update
    void Start()
    {

        inputWireSocket = transform.Find("WireSocket");
        inputSocket = inputWireSocket.GetComponent<Socket>();
        inputSocket.preLinked = false;

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

        on = !on;
        inputSocket.preLinked = on;

        if(on)
        {
            Color color = offSprite.color;
            color.a = Mathf.Clamp01(0);
            offSprite.color = color;

            color = onSprite.color;
            color.a = Mathf.Clamp01(1);
            onSprite.color = color;
        }
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
