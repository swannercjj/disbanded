using UnityEngine;
using System.Collections.Generic; // For using List

public class EnemyPhaseContinuous : MonoBehaviour
{
    public GameObject rocketPrefab; // Assign the rocket prefab in the Inspector
    public List<Transform> firePoints; // List of fire points for spawning rockets
    public float detectionRange; // Range to detect the player
    public float rotationSpeed = 2f; // Speed to rotate towards the player (though we won't use this for now)
    public float rocketFireDuration = 5f; // Duration for which the enemy will continuously shoot rockets
    public float rocketCooldown = 0.5f; // Time between rocket spawns while continuously firing
    public float rocketSpreadAngle = 5f; // Angle of spread for the rocket's firing direction

    private float rocketTimer = 0f;
    private bool isFiring = false; // Indicates if the enemy is firing rockets
    private float fireStartTime; // Time when the continuous rocket fire started
    private Health health; // Reference to the Health script
    private bool isDead = false; // Tracks whether the enemy is dead

    private Quaternion initialRotation; // To store the initial horizontal rotation

    void Start()
    {
        health = GetComponent<Health>();

        if (health == null)
        {
            Debug.LogError("Health component is missing from the enemy!");
        }

        if (firePoints.Count == 0)
        {
            Debug.LogError("No fire points assigned to the enemy!");
        }
    }

    void FixedUpdate()
    {
        // Check if the enemy is dead
        if (health != null && health.health <= 0)
        {
            if (!isDead)
            {
                Die();
            }
            return; // Stop all behaviors if dead
        }

        GameObject player = PlayerController.player; // Get the player GameObject

        // Fire rockets at regular intervals during the firing duration
        if (isFiring && rocketTimer >= rocketCooldown && rocketPrefab != null && firePoints.Count > 0)
        {
            FireRocketsAtPlayer();
            rocketTimer = 0f; // Reset the rocket timer
        }

        // Increment rocket timer
        rocketTimer += Time.fixedDeltaTime;
    }

    public void Initiate()
    {
        StartFiring();
    }

    private void StartFiring()
    {
        isFiring = true;
        fireStartTime = Time.time; // Record the start time of continuous firing
    }

    private void StopFiring()
    {
        isFiring = false;
    }

    private void FireRocketsAtPlayer()
    {
        foreach (Transform firePoint in firePoints)
        {
            // Instantiate the rocket and set the shooter reference
            GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

            // Use the firePoint's forward direction as the direction for the rocket
            Vector3 directionToTarget = firePoint.forward;

            // Apply random spread to the direction
            directionToTarget = ApplyRandomSpread(directionToTarget);

            // Adjust the rocket's rotation with the spread direction
            rocket.transform.rotation = Quaternion.LookRotation(directionToTarget);

            HomingRigidbody homingRigidbody = rocket.GetComponent<HomingRigidbody>();
            if (homingRigidbody != null)
            {
                homingRigidbody.shooter = this.gameObject; // Set the shooter as the enemy object
            }
        }
    }

    private Vector3 ApplyRandomSpread(Vector3 direction)
    {
        // Apply random spread to the firing direction within the specified angle
        float spreadX = Random.Range(-rocketSpreadAngle, rocketSpreadAngle);
        float spreadY = Random.Range(-rocketSpreadAngle, rocketSpreadAngle);

        // Create a quaternion for random rotation
        Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);

        // Apply the spread rotation to the direction
        return spreadRotation * direction;
    }

    private void Die()
    {
        isDead = true;

        // Optional: Add death effects, such as animations or particle systems
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the detection range in the Editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw fire point positions
        Gizmos.color = Color.red;
        foreach (Transform firePoint in firePoints)
        {
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }
    }
}
