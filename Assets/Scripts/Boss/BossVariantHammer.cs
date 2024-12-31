using UnityEngine;

public class BossVariantHammerEnemy : MonoBehaviour
{
    public float jumpInterval = 5f; // Time interval between jumps
    public float jumpForce = 10f; // Vertical force of the jump
    public float horizontalForce = 5f; // Horizontal force of the jump
    public float detectionRange = 10f; // Range within which the enemy detects the player

    public GameObject debrisPrefab; // Debris prefab to instantiate
    public Transform debrisSpawnPoint; // Point where debris is spawned
    public int minDebrisCount = 5; // Minimum number of debris
    public int maxDebrisCount = 10; // Maximum number of debris
    public float minDebrisForce = 5f; // Minimum force for debris
    public float maxDebrisForce = 10f; // Maximum force for debris

    private Rigidbody rb;
    private float jumpTimer;
    private Health health;
    private bool isGrounded;
    private bool hasLeaped; // Flag to track if the enemy just leaped
    private bool hasTouchedGround; // Flag to track if the enemy has touched the ground

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        jumpTimer = 0; // Initialize timer
        hasTouchedGround = false; // Initially, the enemy hasn't touched the ground
    }

    void Update()
    {
        // Stop all behavior if health is zero or below
        if (health != null && health.health <= 0)
        {
            return;
        }

        // Wait until PlayerController.player is not null
        if (PlayerController.player == null)
            return;

        // Only start acting if the enemy has touched the ground at least once
        if (!hasTouchedGround)
            return;

        Transform playerTransform = PlayerController.player.transform;

        // Check if the player is within range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If the player is within detection range, check if the enemy can see them
        if (distanceToPlayer <= detectionRange)
        {
            // Check line of sight (can the enemy see the player?)
            if (CanSeePlayer(playerTransform))
            {
                // If the enemy can see the player, start the jump action
                if (jumpTimer <= 0f)
                {
                    FacePlayer(playerTransform);
                    LeapAtPlayer(playerTransform);
                    jumpTimer = jumpInterval; // Reset the jump timer
                }
            }
        }
        else
        {
            // Reset jump timer if player is out of range
            jumpTimer = Mathf.Max(jumpTimer - Time.deltaTime, 0f);
        }

        // Decrement the jump timer even when the player isn't visible
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    bool CanSeePlayer(Transform playerTransform)
    {
        // Cast a ray from the enemy to the player
        RaycastHit hit;
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // Perform the raycast to check if there is a clear line of sight
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            // If the ray hits the player, it's a valid line of sight
            if (hit.transform == playerTransform)
            {
                return true;
            }
        }

        // If no valid line of sight, return false
        return false;
    }

    void FacePlayer(Transform playerTransform)
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0; // Ignore vertical difference for rotation

        // Rotate the enemy to face the player
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void LeapAtPlayer(Transform playerTransform)
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Calculate the jump force
        Vector3 jumpVector = new Vector3(
            directionToPlayer.x * horizontalForce,
            jumpForce,
            directionToPlayer.z * horizontalForce
        );

        // Apply the jump force
        rb.linearVelocity = Vector3.zero; // Reset velocity to avoid stacking forces
        rb.AddForce(jumpVector, ForceMode.Impulse);
        isGrounded = false; // The enemy is airborne
        hasLeaped = true; // Set the leap flag
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true; // The enemy has landed

            // The enemy has touched the ground for the first time
            if (!hasTouchedGround)
            {
                hasTouchedGround = true; // Set the flag to true

                // Perform the hammer slam upon first ground contact
                SpawnDebris();
                return; // Exit to avoid other behaviors during this frame
            }

            // Spawn debris only if the enemy has leaped
            if (hasLeaped)
            {
                SpawnDebris();
                hasLeaped = false; // Reset the leap flag after landing
            }
        }
    }

    void SpawnDebris()
    {
        if (debrisPrefab == null || debrisSpawnPoint == null)
        {
            Debug.LogWarning("Debris prefab or spawn point not set.");
            return;
        }

        int debrisCount = Random.Range(minDebrisCount, maxDebrisCount + 1);

        for (int i = 0; i < debrisCount; i++)
        {
            // Instantiate debris
            GameObject debris = Instantiate(debrisPrefab, debrisSpawnPoint.position, Quaternion.identity);

            // Get the Rigidbody of the debris
            Rigidbody debrisRb = debris.GetComponent<Rigidbody>();
            if (debrisRb != null)
            {
                // Calculate the horizontal direction forming a perfect circle
                float angle = i * (360f / debrisCount); // Evenly spaced angles
                Vector3 horizontalDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                // Randomize the vertical angle
                float verticalAngle = Random.Range(0f, 30f);
                float verticalMagnitude = Mathf.Sin(Mathf.Deg2Rad * verticalAngle);
                float horizontalMagnitude = Mathf.Cos(Mathf.Deg2Rad * verticalAngle);

                Vector3 direction = new Vector3(horizontalDirection.x * horizontalMagnitude, verticalMagnitude, horizontalDirection.z * horizontalMagnitude);

                // Apply random force
                float force = Random.Range(minDebrisForce, maxDebrisForce);
                debrisRb.AddForce(direction * force, ForceMode.Impulse);

                // Apply random spin
                Vector3 randomSpin = new Vector3(
                    Random.Range(-30f, 30f),
                    Random.Range(-30f, 30f),
                    Random.Range(-30f, 30f)
                );
                debrisRb.AddTorque(randomSpin, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize detection range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
