using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Script will define how to extend the wire when it is dragged:
/// - Cast a ray from the main camera.
/// - Determine if the hit object has the wire 3D model.
/// - Move the wireEnd to the new raycast hit position during drag.
/// - Use the LineRenderer component to visualize the wire.
/// </summary>

[RequireComponent(typeof(LineRenderer))]
public class ExtendibleWire : PressInputBase
{
    private Camera mainCamera; // For raycast

    [SerializeField] private Transform moving; // The part that will be moved
    /// <summary>
    /// Moving's position will be used to make sure that the wire when dragged does not move in on its start point causing it to look ugly
    /// </summary>
    private Vector3 movingStartingPosition;

    private bool isDragging; // To check if wireEnd is being dragged

    /// <summary>
    /// We fix the y position of the model when moving, 
    /// So this storing the starting y cooridnate is useful
    /// </summary>
    private float fixedYPosition;

    /// <summary>
    /// Starting point of the line that will be drawn using the line renderer component which represents the wire,
    /// Will be the position of a prefab called WirePoint
    /// Will be used to change the facing direction of the wireEnd transform based on how the wireEnd has been moved
    /// </summary>
    [SerializeField] private Transform wirePointTransform;
    private Vector3 startingPoint;

    // LineRenderer component to visualize the line
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] points; // Points between which the line will be drawn

    float positionClampThreshold; // We will need this to stop the movement of wireEnd if it's position gets below some threshold which we calculate later

    bool connectionFound = false; // If a connection is found to some other wirePoint, false at start

    private WirePoint wirePoint; // Wire point Class object of the wire point from which this wire originates
    private WirePoint connectedWirePoint; // Wire point Class object of the wire point to which this wire will connect to
    private Transform connectedWirePointTransform;

    [SerializeField] private Transform blockedWirePoint; // Wire point transform that this wire is not allowed to connect to

    [SerializeField] private Color disconnectedWireColor = Color.red;
    [SerializeField] private Color connectedWireColor = Color.green;

    [SerializeField] private GameObject wireGameObject;
    private Renderer wireRenderer;
    Material[] wireMaterials;
    Material wireBodyMaterial;

    [SerializeField] private AudioClip wireSpark;
    private AudioSource sparkAudioSource;

    private bool ignoreInput = false;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    public void Start()
    {
        OnScaleEnable();

        startingPoint = wirePointTransform.position;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length;

        movingStartingPosition = moving.position;
        // positionClampThreshold = (movingStartingPosition - wirePointTransform.position).magnitude;

        wirePoint = wirePointTransform.GetComponent<WirePoint>();

        wireRenderer = wireGameObject.GetComponentInChildren<Renderer>();

        wireMaterials = wireRenderer.materials;
        wireBodyMaterial = wireMaterials[1];

        if (disconnectedWireColor == default)
            disconnectedWireColor = Color.red;

        if (connectedWireColor == default)
            connectedWireColor = Color.green;

        if(sparkAudioSource == null)
        {
            sparkAudioSource = transform.AddComponent<AudioSource>();
        }
    }

    protected override void OnPressBegan(Vector3 position)
    {
        Ray ray = mainCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform == moving)
            {
                isDragging = true; // Start dragging
                fixedYPosition = moving.position.y;
            }
        }
    }

    private void Update()
    {
        UpdateLineWidth();
        DrawLine();
        if (moving != null && isDragging && activeContext.HasValue && !ignoreInput)
        {
            // Use activeContext to get the current pointer position
            Vector3 currentPosition = activeContext.Value.control.device is Pointer device ? device.position.ReadValue() : Vector3.zero;

            Ray ray = mainCamera.ScreenPointToRay(currentPosition);

            // Perform raycast to find the new position
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 newPosition = new Vector3(hit.point.x, fixedYPosition, hit.point.z);

                Collider[] colliders = Physics.OverlapSphere(newPosition, .05f * (transform.lossyScale.y));
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.name.StartsWith("WirePoint") && collider.transform != wirePointTransform && collider.transform != blockedWirePoint)
                    {
                        connectedWirePointTransform = collider.transform;
                        ///Debug.Log(connectedWirePointTransform.name);
                        ///Debug.Log(connectedWirePointTransform.position);

                        if (connectedWirePointTransform != null)
                        {
                            connectedWirePoint = connectedWirePointTransform.GetComponent<WirePoint>();
                            if (connectedWirePoint != null)
                            {
                                if (connectionFound == false)
                                {
                                    connectedWirePoint.numInputConnections++;
                                    if (connectedWirePoint.voltageValueCannotBeChanged == false)
                                    {
                                        connectedWirePoint.wirePointVoltage = wirePoint.wirePointVoltage;
                                    }
                                    connectedWirePoint.wirePointCurrent = wirePoint.wirePointCurrent;

                                    // Play the wireSpark sound ONCE when the connection is found
                                    if (!sparkAudioSource.isPlaying)
                                    {
                                        sparkAudioSource.clip = wireSpark;
                                        sparkAudioSource.Play();
                                    }
                                    StartCoroutine(IgnoreTouchInput(0.5f));
                                }
                                if (connectionFound == false)
                                {
                                    wirePoint.AddConnection(connectedWirePointTransform);
                                }
                                connectionFound = true;
                            }
                            lineRenderer.material.color = connectedWireColor;
                            wireBodyMaterial.color = connectedWireColor;
                            UpdateWire(connectedWirePointTransform.position);
                            return;
                        }
                    }
                }

                // If the function has not returned a value before this line, then it means a connection has not been found
                if (connectionFound == true && connectedWirePoint != null)
                {
                    connectedWirePoint.numInputConnections--;
                }
                connectionFound = false; // Thus set connectionFound to false

                if (connectedWirePoint != null && connectedWirePoint.voltageValueCannotBeChanged == false) // if a connection was previously found, but now the connection is removed
                {
                    connectedWirePoint.wirePointVoltage = -1;
                }
                if (connectedWirePoint != null)
                {
                    connectedWirePoint.wirePointCurrent = 0;
                }
                if(connectedWirePointTransform != null)
                    wirePoint.RemoveConnection(connectedWirePointTransform);

                wireBodyMaterial.color = disconnectedWireColor;
                lineRenderer.material.color = disconnectedWireColor;
                connectedWirePointTransform = null;

                UpdateWire(newPosition);
            }
        }

        if(connectedWirePoint != null && connectedWirePoint.voltageValueCannotBeChanged == false)
        {
            connectedWirePoint.wirePointVoltage = wirePoint.wirePointVoltage;
        }

        if(connectedWirePoint != null)
        {
            connectedWirePoint.wirePointCurrent = wirePoint.wirePointCurrent;
        }

        //Debug.Log(connectedWirePoint == null);
    }

    private IEnumerator IgnoreTouchInput(float delay)
    {
        ignoreInput = true;
        yield return new WaitForSeconds(delay);
        ignoreInput = false;
    }

    protected override void OnPressCancel()
    {
        isDragging = false; // Stop dragging
        if(!connectionFound)
        {
            UpdateWire(movingStartingPosition);
        }
    }

    public void DrawLine()
    {
        // Draw line between each point in the points array
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    private void UpdateLineWidth()
    {
        // Determine the axis responsible for the wire's thickness
        float wireThickness = transform.lossyScale.y / 50; // Assuming the wire's thickness corresponds to the y-axis scale

        // Update the LineRenderer width to exactly match the wire's scale
        lineRenderer.startWidth = wireThickness;
        lineRenderer.endWidth = wireThickness;
    }

    public void UpdateWire(Vector3 newPosition)
    {

        /// <summary>
        /// If the newPoisiton that is sent as argument to the function is the same as starting point,
        /// It means that the drag has been stopped and no connection has been found, therefore
        /// We must reset the position to how it was at the start
        /// </summary>

        if (newPosition == movingStartingPosition)
        {
            moving.position = newPosition;
        }

        /// <summary>
        /// Only if the new position is not below the threshold, then we go ahead with the movement to the new position
        /// This was done to maintain immersion when moving the wire, by stopping clipping with its WirePoint 3D Model
        /// This was purely done for visual purposes
        /// </ summary >

        //else if (!((startingPoint - newPosition).magnitude <= positionClampThreshold))
        else
        {
            moving.position = newPosition; // Update wireEnd position
        }

/*        else
        {
            moving.position = movingStartingPosition;
        }*/

        // Calculate direction from the starting point to the new position
        Vector3 direction = newPosition - startingPoint;

        // Apply rotation using Quaternion.LookRotation to face the direction
        if (direction.sqrMagnitude > 0.0001f) // Avoid a zero-length vector
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    private void OnScaleEnable()
    {
        // Subscribe to the event when the script is enabled
        ScaleAndRotateSlider.OnScaleChanged += OnScaleChanged;
    }

    private void OnScaleDisable()
    {
        // Unsubscribe from the event when the script is disabled
        ScaleAndRotateSlider.OnScaleChanged -= OnScaleChanged;
    }

    private void OnScaleChanged()
    {
        // Update the starting position when scaling occurs
        movingStartingPosition = moving.position;
        startingPoint = wirePointTransform.position;
        positionClampThreshold = (movingStartingPosition - wirePointTransform.position).magnitude;
        UpdateLineWidth();
    }
}