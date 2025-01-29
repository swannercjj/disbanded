using UnityEngine;
using System.Collections.Generic;

public class HiddenPillarScript : MonoBehaviour
{
    public List<Rigidbody> cubes; // List of cube rigidbodies
    public Transform referenceTransform; // Transform used for the force direction
    public float cubeForceMagnitude = 100f; // Magnitude of the force for cubes

    public GameObject prefab; // Prefab to spawn
    public Transform spawnPoint; // Transform where the prefab will be spawned
    public float prefabForceMagnitude = 50f; // Magnitude of the forward force for the prefab

    // Called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Validation for cube functionality
        if (cubes == null || cubes.Count == 0)
        {
            Debug.LogWarning("Cubes list is empty. Add Rigidbody components to the list.");
        }

        if (referenceTransform == null)
        {
            Debug.LogError("Reference Transform is not assigned.");
        }

        // Validation for prefab spawning
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // For debugging or testing, you could call `Activate()` or `SpawnAndShove()` here
        // if needed during gameplay.
    }

    // Activates the cubes
    public void Activate()
    {
        if (cubes == null || referenceTransform == null) return;

        foreach (Rigidbody cube in cubes)
        {
            if (cube == null) continue;

            // Set isKinematic to false to enable physics interaction
            cube.isKinematic = false;

            // Calculate the direction of the force (opposite to referenceTransform.forward)
            Vector3 forceDirection = -(referenceTransform.position - cube.position).normalized;

            // Apply force to the cube
            cube.AddForce(forceDirection * cubeForceMagnitude, ForceMode.Impulse);
        }
        SpawnAndShove();
    }

    // Spawns a prefab and applies a forward shove
    public void SpawnAndShove()
    {
        if (prefab == null || spawnPoint == null) return;

        // Instantiate the prefab at the spawn point's position and rotation
        GameObject spawnedObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Spawned");
        // Get the Rigidbody component of the spawned object
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Spawned prefab does not have a Rigidbody component.");
            return;
        }

        // Apply a forward force to the Rigidbody
        Vector3 forwardForce = spawnPoint.forward * prefabForceMagnitude;
        rb.AddForce(forwardForce, ForceMode.Impulse);
    }
}
