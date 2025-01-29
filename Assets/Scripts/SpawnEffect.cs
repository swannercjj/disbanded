using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public GameObject prefabToSpawn; // Assign the prefab in the Inspector

    void Start()
    {
        // Check if the prefab is assigned
        if (prefabToSpawn != null)
        {
            // Get the position and rotation of the parent GameObject

            // Instantiate the prefab at the parent's position and rotation
            GameObject spawnedObject = Instantiate(prefabToSpawn, gameObject.transform.position, gameObject.transform.rotation);

            // Destroy the spawned object after 5 seconds
            Destroy(spawnedObject, 5f);
        }
        else
        {
            Debug.LogWarning("Prefab not assigned to SpawnEffect script.");
        }
    }
}
