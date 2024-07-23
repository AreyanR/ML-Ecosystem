using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    private List<CameraTransform> targetTransforms; // List of target positions and rotations for the camera
    private int currentIndex = 0; // Current index in the list

    private void Start()
    {
        // Initialize the list of target positions and rotations
        targetTransforms = new List<CameraTransform>
        {
            new CameraTransform(new Vector3(0, 18, 0), new Vector3(90, 0, 0)),     // Top View
            new CameraTransform(new Vector3(0, 1.5f, -10), new Vector3(0, 0, 0)),   // Side 1
            new CameraTransform(new Vector3(10, 1.5f, 0), new Vector3(0, -90, 0)),  // Side 2
            new CameraTransform(new Vector3(0, 1.5f, 10), new Vector3(0, -180, 0)), // Side 3
            new CameraTransform(new Vector3(-10, 1.5f, 0), new Vector3(0, 90, 0)),  // Side 4
            new CameraTransform(new Vector3(0, 12, -11.5f), new Vector3(55, 0, 0)) // Base View
        };
    }

    // Method to move the camera to the next target position and rotation in the list
    public void MoveCamera()
    {
        if (targetTransforms.Count == 0)
        {
            Debug.LogWarning("No target transforms available.");
            return;
        }

        mainCamera.transform.position = targetTransforms[currentIndex].Position;
        mainCamera.transform.rotation = Quaternion.Euler(targetTransforms[currentIndex].Rotation);

        // Increment the index and reset to 0 if it exceeds the list count
        currentIndex = (currentIndex + 1) % targetTransforms.Count;
    }
}
