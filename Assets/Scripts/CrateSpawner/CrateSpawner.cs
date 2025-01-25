using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cratePrefab; // Assign your crate prefab here
    [SerializeField] private float launchForce = 10f; // Adjust the force of the launch
    [SerializeField] private float spawnInterval = 2f; // Time between spawns
    public int id = 1;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if the timer has exceeded the spawn interval
        if (timer >= spawnInterval)
        {
            SpawnAndLaunchCrate();
            timer = 0f; // Reset the timer
        }
    }

    private void SpawnAndLaunchCrate()
    {
        // Instantiate a new crate
        GameObject newCrate = Instantiate(cratePrefab, transform.position, transform.rotation);

        // Assign a unique ID to the crate
        Crate crateComponent = newCrate.GetComponent<Crate>();
        if (crateComponent != null)
        {
            crateComponent.id = id;
        }

        // Apply force to the crate
        Rigidbody crateRigidbody = newCrate.GetComponent<Rigidbody>();
        if (crateRigidbody != null)
        {
            crateRigidbody.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        }
    }
}
