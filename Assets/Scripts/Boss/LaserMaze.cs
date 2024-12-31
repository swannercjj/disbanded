using UnityEngine;
using System.Collections.Generic; // To use List

public class LaserMaze : MonoBehaviour
{
    public GameObject laserPrefab; // Prefab with the CylinderLaserGrow script attached
    public Transform emittingPoint; // Emitting point for the lasers to spawn from
    public int numberOfLasers = 5; // Number of lasers to spawn
    public float amplitude = 5f; // Maximum oscillation amplitude
    public float minSpeed;
    public float maxSpeed;

    private List<GameObject> lasers = new List<GameObject>(); // List to store spawned lasers
    private List<GameObject> laserTargets = new List<GameObject>(); // List to store target objects

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Optionally, you can call SpawnLasers here if you want to spawn lasers immediately on start
        // SpawnLasers();
    }

    public void Initiate()
    {
        // Ensure all the required references are assigned
        if (laserPrefab == null || emittingPoint == null)
        {
            Debug.LogError("LaserPrefab or EmittingPoint is not assigned!");
            return;
        }
        
        SpawnLasers();
    }

    // Spawns multiple lasers all moving to the same target point
    void SpawnLasers()
    {
        // Calculate the target position 1500 units forward from emittingPoint
        Vector3 targetPosition = emittingPoint.position + emittingPoint.forward * 1500f;

        // Spawn the specified number of lasers
        for (int i = 0; i < numberOfLasers; i++)
        {
            // Instantiate a new laser at the emitting point's position
            GameObject newLaser = Instantiate(laserPrefab, emittingPoint.position, Quaternion.identity);
            // Get the EnemyMovingLaser component attached to the prefab
            EnemyMovingLaser laserScript = newLaser.GetComponent<EnemyMovingLaser>();

            // Ensure the laserScript is assigned correctly
            if (laserScript != null)
            {
                // Create a temporary target transform to hold the target position
                GameObject targetObject = new GameObject("LaserTarget");
                targetObject.transform.position = targetPosition;  // Set the target position

                // Set the laser's target to the targetObject's transform
                laserScript.targetPoint = targetObject.transform; // Set target to the created transform
                laserScript.movementSpeedFactor = 1f; // Customize the movement speed

                // Assign a random oscillation amplitude between the given range
                laserScript.oscillationAmplitude = Random.Range(-amplitude, amplitude);
                laserScript.movementSpeedFactor = Random.Range(minSpeed, maxSpeed);

                // Add the laser to the list to track it
                lasers.Add(newLaser);
                // Add the target to the list to track it
                laserTargets.Add(targetObject);
            }
            else
            {
                Debug.LogError("EnemyMovingLaser component missing on laser prefab!");
            }
        }

        // Start coroutine to destroy lasers and targets after 15 seconds
        StartCoroutine(DestroyLasersAndTargetsAfterDelay(20f));
    }

    // Coroutine to destroy lasers and their targets after a delay
    private IEnumerator<WaitForSeconds> DestroyLasersAndTargetsAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy each laser in the list
        foreach (var laser in lasers)
        {
            Destroy(laser);
        }

        // Destroy each target in the list
        foreach (var target in laserTargets)
        {
            Destroy(target);
        }

        // Clear the lists after destroying the lasers and targets
        lasers.Clear();
        laserTargets.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // Additional behavior for the maze, if needed
    }
}
