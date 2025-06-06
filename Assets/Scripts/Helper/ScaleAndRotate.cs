using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleAndRotateSlider : MonoBehaviour
{
    private Slider scaleSlider;
    private Slider rotateSlider;
    public float scaleMinValue;
    public float scaleMaxValue;
    public float rotMinValue;
    public float rotMaxValue;

    // Define an event to notify when scaling is done
    public delegate void ScaleChangedAction();
    public static event ScaleChangedAction OnScaleChanged;

    void Start()
    {
        scaleMinValue = PlaceOnPlane.initialScale.x;

        // Initialize sliders
        scaleSlider = GameObject.Find("Scale").GetComponent<Slider>();
        scaleSlider.minValue = scaleMinValue;
        scaleSlider.maxValue = scaleMaxValue;
        scaleSlider.onValueChanged.AddListener(ScaleSliderUpdate);

        rotateSlider = GameObject.Find("Rotate").GetComponent<Slider>();
        rotateSlider.minValue = rotMinValue;
        rotateSlider.maxValue = rotMaxValue;
        rotateSlider.onValueChanged.AddListener(RotateSliderUpdate);
    }

    void ScaleSliderUpdate(float value)
    {
        transform.localScale = new Vector3(value, value, value);

        // Trigger the event when scaling is updated
        OnScaleChanged?.Invoke();
    }

    void RotateSliderUpdate(float value)
    {
        Vector3 currentRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(transform.rotation.x + PlaceOnPlane.xRotation, transform.rotation.y, value);

        OnScaleChanged?.Invoke();
    }
}
