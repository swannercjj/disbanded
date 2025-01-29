using UnityEngine;
using System.Collections.Generic;

public class BoxOfEnemies : MonoBehaviour
{
    public List<GameObject> cubes; // List of cube GameObjects
    public Transform referenceTransform; // Transform used for the force direction
    public float cubeForceMagnitude = 100f; // Magnitude of the force for cubes

    public List<GameObject> prefabOptions; // List of prefab options
    public Transform spawnPoint; // Transform where the prefabs will be spawned
    public int numberOfPrefabsToSpawn = 5; // Number of prefabs to spawn
    public float prefabForceMagnitude = 50f; // Magnitude of the random force for prefabs
    public float tumbleMagnitude = 10f; // Magnitude of the tumble torque
    private bool activated = false; // To prevent multiple activations

    void Start()
    {
        // Validation for cube functionality
        if (cubes == null || cubes.Count == 0)
        {
            Debug.LogWarning("Cubes list is empty. Add GameObjects to the list.");
        }

        if (referenceTransform == null)
        {
            Debug.LogError("Reference Transform is not assigned.");
        }

        // Validation for prefab spawning
        if (prefabOptions == null || prefabOptions.Count == 0)
        {
            Debug.LogError("Prefab options list is empty. Add prefabs to the list.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned.");
        }

        // Apply initial tumble
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * tumbleMagnitude;

            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("No Rigidbody attached to the BoxOfEnemies for tumbling.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Activate();
    }

    // Activates the cubes and initiates their destruction
    public void Activate()
    {
        if (activated) return; // Prevent multiple activations
        activated = true;

        if (cubes == null || referenceTransform == null) return;

        foreach (GameObject cube in cubes)
        {
            if (cube == null) continue;

            // Ensure the cube has a Rigidbody
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = cube.AddComponent<Rigidbody>();
            }

            // Unparent the cube (set it as a root object in the hierarchy)
            cube.transform.parent = null;

            // Calculate the direction of the force (opposite to referenceTransform.forward)
            Vector3 forceDirection = -(referenceTransform.position - cube.transform.position).normalized;

            // Apply force to the cube
            rb.AddForce(forceDirection * cubeForceMagnitude, ForceMode.Impulse);

            // Destroy the cube after 5 seconds
            Destroy(cube, 5f);
        }

        // Spawn and propel random prefabs
        SpawnAndPropelPrefabs();

        // Destroy this GameObject after 5 seconds
        Destroy(gameObject, 5f);
    }

    // Spawns a specified number of random prefabs and propels them in random upward directions
    public void SpawnAndPropelPrefabs()
    {
        if (prefabOptions == null || prefabOptions.Count == 0 || spawnPoint == null) return;

        for (int i = 0; i < numberOfPrefabsToSpawn; i++)
        {
            // Select a random prefab from the list
            GameObject randomPrefab = prefabOptions[Random.Range(0, prefabOptions.Count)];

            // Instantiate the prefab at the spawn point's position and rotation
            GameObject spawnedObject = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);

            // Get the Rigidbody component of the spawned object
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Spawned prefab does not have a Rigidbody component.");
                continue;
            }

            // Generate a random upward direction (x and z random, y positive)
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f), // Random x direction
                Random.Range(0.5f, 1f), // Positive y direction
                Random.Range(-1f, 1f)  // Random z direction
            ).normalized;

            // Apply force to the Rigidbody
            rb.AddForce(randomDirection * prefabForceMagnitude, ForceMode.Impulse);
        }
    }
}
