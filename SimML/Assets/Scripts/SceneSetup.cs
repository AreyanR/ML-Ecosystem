using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public List<GameObject> shrubPrefabs;
    public List<int> numberOfShrubs;

    private Vector3 spawnAreaMin = new Vector3(-8, 0, -8);
    private Vector3 spawnAreaMax = new Vector3(8, 0, 8);

    public Transform shrubContainer;

    void Start()
    {
        ResetEnvironment();
    }

    public void ResetEnvironment()
    {
        if (shrubContainer != null)
        {
            foreach (Transform child in shrubContainer)
            {
                Destroy(child.gameObject);
            }
        }

        if (shrubPrefabs.Count != numberOfShrubs.Count)
        {
            Debug.LogError("Shrub Prefabs and Number Of Shrubs lists must have the same length.");
            return;
        }

        for (int i = 0; i < shrubPrefabs.Count; i++)
        {
            SpawnObjects(shrubPrefabs[i], numberOfShrubs[i]);
        }
    }

    void SpawnObjects(GameObject prefab, int count)
    {
        if (shrubContainer == null)
        {
            Debug.LogError("ShrubContainer is not assigned.");
            return;
        }

        Vector3 containerPosition = shrubContainer.position;
        Vector3 containerScale = shrubContainer.localScale;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0,
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            Vector3 worldPosition = containerPosition + Vector3.Scale(randomPosition, containerScale);

            GameObject spawnedObject = Instantiate(prefab, worldPosition, Quaternion.identity);

            spawnedObject.transform.SetParent(shrubContainer);
        }
    }
}
