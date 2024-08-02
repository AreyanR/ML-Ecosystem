using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public List<GameObject> shrubPrefabs; // List of shrub prefabs to spawn
    public List<int> numberOfShrubs; // List of corresponding numbers of each shrub to spawn

    private Vector3 spawnAreaMin = new Vector3(-9, 0, -9); // Minimum spawn area coordinates
    private Vector3 spawnAreaMax = new Vector3(9, 0, 9); // Maximum spawn area coordinates

    public Transform shrubContainer; // Container to hold spawned shrubs

    // Initializes the environment by resetting it
    void Start()
    {
        ResetEnvironment();
    }

    // Resets the environment by clearing existing shrubs and spawning new ones
    public void ResetEnvironment()
    {
        if (shrubContainer != null)
        {
            foreach (Transform child in shrubContainer)
            {
                Destroy(child.gameObject); // Destroy all existing shrubs
            }
        }

        if (shrubPrefabs.Count != numberOfShrubs.Count)
        {
            Debug.LogError("Shrub Prefabs and Number Of Shrubs lists must have the same length.");
            return;
        }

        for (int i = 0; i < shrubPrefabs.Count; i++)
        {
            SpawnObjects(shrubPrefabs[i], numberOfShrubs[i]); // Spawn shrubs according to the defined lists
        }
    }

    // Spawns a specified number of objects within the defined spawn area
    // prefab: The prefab to instantiate
    // count: The number of instances to create
    void SpawnObjects(GameObject prefab, int count)
    {
        if (shrubContainer == null)
        {
            Debug.LogError("ShrubContainer is not assigned.");
            return;
        }

        Vector3 containerPosition = shrubContainer.position; // Position of the shrub container
        Vector3 containerScale = shrubContainer.localScale; // Scale of the shrub container

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Vector3 worldPosition = containerPosition + Vector3.Scale(randomPosition, containerScale);

            GameObject spawnedObject = Instantiate(prefab, worldPosition, Quaternion.identity); // Spawn the shrub

            spawnedObject.transform.SetParent(shrubContainer); // Set the shrub's parent to the container
        }
    }
}
