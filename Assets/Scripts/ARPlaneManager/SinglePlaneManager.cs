using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SinglePlaneManager : MonoBehaviour
{
    public ARPlaneManager arPlaneManager; // Assign your ARPlaneManager in the Inspector
    private ARPlane detectedPlane;
    float planeSizeThreshold = 2f; // The size threshold for the plane to be considered "big enough"

    void OnEnable()
    {
        // Subscribe to planesChanged event
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // If a plane has already been detected and it's large enough, do nothing
        if (detectedPlane != null && detectedPlane.size.x >= planeSizeThreshold && detectedPlane.size.y >= planeSizeThreshold)
        {
            // Lock plane detection mode if the plane has become big enough
            arPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
            return;
        }

        // Check if any new planes were added
        if (args.added.Count > 0 && detectedPlane == null)
        {
            // Store the first detected plane
            detectedPlane = args.added[0];

            // Optionally, disable all other planes except the detected one
            foreach (var plane in arPlaneManager.trackables)
            {
                if (plane != detectedPlane)
                {
                    plane.gameObject.SetActive(false);
                }
            }

            Debug.Log("First plane detected and locked.");
        }

        // Monitor the detected plane's size and lock detection if it becomes big enough
        if (detectedPlane != null && detectedPlane.size.x >= planeSizeThreshold && detectedPlane.size.y >= planeSizeThreshold)
        {
            // Lock detection to prevent new planes
            arPlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
            Debug.Log("Plane is big enough, no more planes will be detected.");
        }
    }
}
