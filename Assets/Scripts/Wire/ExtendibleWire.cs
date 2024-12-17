using UnityEngine;
using UnityEngine.InputSystem;

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
    /// The 3D model, who has diferent pivot location when compared to moving transform
    /// moving is parent of moving3DModel
    /// moving3DModel's position will be used to make sure that the wire when dragged does not move in on its start point
    /// causing it to look ugly
    /// </summary>
    [SerializeField] private Transform moving3DModel;

    private bool isDragging; // To check if wireEnd is being dragged

    /// <summary>
    /// We fix the y position of the model when moving, 
    /// So this storing the starting y cooridnate is useful
    /// </summary>
    private float fixedYPosition;

    /// <summary>
    /// Starting point of the wireEnd transform, before moving, will be the position of a prefab called wirePoint
    /// Will be used to change the facing direction of the wireEnd transform based on how the wireEnd has been moved
    /// </summary>
    [SerializeField] private Transform wirePoint;
    private Vector3 startingPoint;

    // LineRenderer component to visualize the line
    private LineRenderer lineRenderer;
    [SerializeField] private Transform[] points; // Points between which the line will be drawn

    float positionClampThreshold; // we will need this to stop the movement of wireEnd if it gets below some threshold which we calculate later

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    public void Start()
    {
        startingPoint = wirePoint.position;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Length;
        DrawLine();

        positionClampThreshold = Mathf.Abs((moving3DModel.position.z - startingPoint.z)) + 0.15f; // this value was found experimentally
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
        if (moving != null && isDragging && activeContext.HasValue)
        {
            // Use activeContext to get the current pointer position
            Vector3 currentPosition = activeContext.Value.control.device is Pointer device ? device.position.ReadValue() : Vector3.zero;

            Ray ray = mainCamera.ScreenPointToRay(currentPosition);

            // Perform raycast to find the new position
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 newPosition = new Vector3(hit.point.x, fixedYPosition, hit.point.z);
                if (!(Mathf.Abs(newPosition.x - startingPoint.x) < positionClampThreshold && Mathf.Abs(newPosition.z - startingPoint.z) < positionClampThreshold))
                {
                    moving.position = newPosition; // Update wireEnd position
                }

                // Calculate direction from the starting point to the new position
                Vector3 direction = newPosition - startingPoint;

                // Apply rotation using Quaternion.LookRotation to face the direction
                if (direction.sqrMagnitude > 0.0001f) // Avoid a zero-length vector
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                }
            }

            DrawLine();
        }

    }

    protected override void OnPressCancel()
    {
        isDragging = false; // Stop dragging
    }

    public void DrawLine()
    {
        // Draw line between each point in the points array
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }
}
