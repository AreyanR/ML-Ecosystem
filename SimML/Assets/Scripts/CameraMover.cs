using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    private List<CameraTransform> targetTransforms; // List of target positions and rotations for the camera
    [SerializeField] private List<Transform> agents; // List of agents (prey and hunter)
    private CameraTransform baseView; // Base View position and rotation
    private int currentIndex = 0; // Current index in the list
    private const float yOffset = 1.5f; // Y offset to keep the camera above the ground for agent POV
    private const float zOffset = 1.5f; // Z offset for agent POV

    // Initializes the camera positions and rotations
    private void Start()
    {
        targetTransforms = new List<CameraTransform>
        {
            new CameraTransform(new Vector3(0, 18, 0), new Vector3(90, 0, 0)),     // Top View
            new CameraTransform(new Vector3(0, 1.5f, -10), new Vector3(0, 0, 0)),   // Side 1
            new CameraTransform(new Vector3(10, 1.5f, 0), new Vector3(0, -90, 0)),  // Side 2
            new CameraTransform(new Vector3(0, 1.5f, 10), new Vector3(0, -180, 0)), // Side 3
            new CameraTransform(new Vector3(-10, 1.5f, 0), new Vector3(0, 90, 0))  // Side 4
        };

        // Define the Base View separately
        baseView = new CameraTransform(new Vector3(0, 12, -11.5f), new Vector3(55, 0, 0)); // Base View
    }

    // Moves the camera to the next target position and rotation in the list
    public void MoveCamera()
    {
        int totalViews = targetTransforms.Count + agents.Count + 1; // Total views including Base View

        if (totalViews == 0)
        {
            Debug.LogWarning("No target transforms or agents available.");
            return;
        }

        // Check if the current index corresponds to a predefined position and rotation
        if (currentIndex < targetTransforms.Count)
        {
            // Move to predefined position and rotation
            mainCamera.transform.SetParent(null); // Detach from any parent
            mainCamera.transform.position = targetTransforms[currentIndex].Position;
            mainCamera.transform.rotation = Quaternion.Euler(targetTransforms[currentIndex].Rotation);
        }
        else if (currentIndex < targetTransforms.Count + agents.Count)
        {
            // Switch to agent's POV
            int agentIndex = currentIndex - targetTransforms.Count;
            mainCamera.transform.SetParent(agents[agentIndex]);
            Vector3 localPosition = new Vector3(0, yOffset, zOffset); // Adjust the Y and Z positions for agent POV
            mainCamera.transform.localPosition = localPosition;
            mainCamera.transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Move to Base View
            mainCamera.transform.SetParent(null); // Detach from any parent
            mainCamera.transform.position = baseView.Position;
            mainCamera.transform.rotation = Quaternion.Euler(baseView.Rotation);
        }

        // Increment the index and reset to 0 if it exceeds the total views count
        currentIndex = (currentIndex + 1) % totalViews;
    }
}
