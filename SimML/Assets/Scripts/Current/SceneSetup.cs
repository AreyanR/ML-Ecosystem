using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    // Prefabs for the tree and bush objects
    public GameObject treePrefab;
    public GameObject bushPrefab;

    // Number of trees and bushes to spawn
    public int numberOfTrees = 10;
    public int numberOfBushes = 20;

    // Bounds for spawning positions
    private Vector3 spawnAreaMin = new Vector3(-19, 0, -19);
    private Vector3 spawnAreaMax = new Vector3(19, 0, 19);

    // Start is called before the first frame update
    void Start()
    {
        SpawnObjects(treePrefab, numberOfTrees);
        SpawnObjects(bushPrefab, numberOfBushes);
    }

    // Method to spawn objects
    void SpawnObjects(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0,  // Assuming the ground is at y = 0
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }
}
