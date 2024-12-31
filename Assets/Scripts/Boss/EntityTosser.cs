using UnityEngine;
using System.Collections; // Required for IEnumerator

public class EntityTosser : MonoBehaviour
{
    public GameObject entityPrefab; // The prefab to spawn
    public Transform spawnLocation; // The location where entities are spawned
    public Transform towardsLocation; // The location towards which entities are tossed
    public float tossForce = 500f; // The base force to apply when tossing
    public float randomSpread = 1f; // The maximum spread (now in decimal form)
    public float spawnInterval = 2f; // Time between spawns

    private bool isSpawning = false; // Flag to manage spawning

    public void Initiate()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnEntities());
        }
    }

    private IEnumerator SpawnEntities()
    {
        yield return new WaitForSeconds(1f);
        while (isSpawning)
        {
            SpawnEntity();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEntity()
    {
        if (entityPrefab == null || spawnLocation == null || towardsLocation == null)
        {
            Debug.LogWarning("EntityTosser: Ensure all required references are assigned.");
            return;
        }

        // Instantiate the entity at the spawn location
        GameObject spawnedEntity = Instantiate(entityPrefab, spawnLocation.position, Quaternion.identity);

        // Ensure the entity has a Rigidbody
        Rigidbody rb = spawnedEntity.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("EntityTosser: The spawned entity does not have a Rigidbody.");
            return;
        }

        // Calculate direction with random spread in the XZ plane
        Vector3 directionXZ = (towardsLocation.position - spawnLocation.position).normalized;

        // Apply random spread with decimal precision to the tenth place
        directionXZ.x += Mathf.Round(Random.Range(-randomSpread, randomSpread) * 10f) / 10f;
        directionXZ.z += Mathf.Round(Random.Range(-randomSpread, randomSpread) * 10f) / 10f;

        // Set Y direction to 45 degrees up
        directionXZ.y = 1f; // Adjust Y to 1 to give the entity a 45-degree angle upwards

        // Normalize the direction vector to maintain consistent force application
        directionXZ.Normalize();

        // Apply force to the entity with the adjusted direction
        rb.AddForce(directionXZ * tossForce);
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    // Stop spawning when the script or game object is disabled
    private void OnDisable()
    {
        isSpawning = false; // This ensures spawning stops when the script is disabled
    }
}
