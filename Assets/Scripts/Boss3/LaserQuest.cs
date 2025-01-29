using UnityEngine;

public class LaserQuest : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to spawn
    public GameObject spawnArea; // The GameObject with a BoxCollider defining the spawn area
    public float spawnInterval = 2f; // Time interval between spawns

    private BoxCollider boxCollider; // Reference to the BoxCollider
    private float timer = 0f; // Timer to track time between spawns

    void Start()
    {
        // Get the BoxCollider component from the spawnArea
        if (spawnArea != null)
        {
            boxCollider = spawnArea.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.LogError("The spawnArea GameObject must have a BoxCollider component.");
            }
        }
        else
        {
            Debug.LogError("No spawnArea assigned.");
        }
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Spawn the prefab when the timer exceeds the interval
        if (timer >= spawnInterval)
        {
            SpawnPrefab();
            timer = 0f;
        }
    }

    void SpawnPrefab()
    {
        if (prefabToSpawn == null || boxCollider == null) return;

        // Get the center and size of the box collider, accounting for the transform's scale
        Vector3 boxCenter = spawnArea.transform.TransformPoint(boxCollider.center);
        Vector3 boxSize = Vector3.Scale(boxCollider.size, spawnArea.transform.localScale);

        // Calculate a random position within the scaled box bounds
        float randomX = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        float randomY = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        float randomZ = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);
        Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

        // Spawn the prefab at the random position with default rotation
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
