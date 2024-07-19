using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    // List of shrub prefabs
    public List<GameObject> shrubPrefabs;

    // Number of each type of shrub to spawn
    public List<int> numberOfShrubs;

    // Bounds for spawning positions (set in code)
    private Vector3 spawnAreaMin = new Vector3(-8, 0, -8);
    private Vector3 spawnAreaMax = new Vector3(8, 0, 8);

    // Reference to the parent object (ShrubContainer)
    public Transform shrubContainer;

    // Start is called before the first frame update
    void Start()
    {
        // Clear existing shrubs if they are children of the container
        if (shrubContainer != null)
        {
            foreach (Transform child in shrubContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Check if the lists are populated correctly
        if (shrubPrefabs.Count != numberOfShrubs.Count)
        {
            Debug.LogError("Shrub Prefabs and Number Of Shrubs lists must have the same length.");
            return;
        }

        // Initialize spawn areas here if needed
        InitializeSpawnAreas();

        for (int i = 0; i < shrubPrefabs.Count; i++)
        {
            SpawnObjects(shrubPrefabs[i], numberOfShrubs[i]);
        }
    }

    // Method to initialize spawn areas
    void InitializeSpawnAreas()
    {
        // Set spawn areas programmatically
        spawnAreaMin = new Vector3(-8, 0, -8);
        spawnAreaMax = new Vector3(8, 0, 8);
    }

    // Method to spawn objects
    void SpawnObjects(GameObject prefab, int count)
    {
        if (shrubContainer == null)
        {
            Debug.LogError("ShrubContainer is not assigned.");
            return;
        }

        // Calculate the container's position and scale
        Vector3 containerPosition = shrubContainer.position;
        Vector3 containerScale = shrubContainer.localScale;

        for (int i = 0; i < count; i++)
        {
            // Generate random position relative to the container
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0,  // Assuming the ground is at y = 0
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            // Convert the position to world space
            Vector3 worldPosition = containerPosition + Vector3.Scale(randomPosition, containerScale);

            GameObject spawnedObject = Instantiate(prefab, worldPosition, Quaternion.identity);

            // Set the parent of the spawned object
            spawnedObject.transform.SetParent(shrubContainer);
        }
    }
}
