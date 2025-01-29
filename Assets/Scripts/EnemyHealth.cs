using UnityEngine;

public class EnemyHealth : Health
{
    private Rigidbody rb; // Reference to the Rigidbody component
    private EnemyMovingLaser laserScript; // Reference to the EnemyMovingLaser component
    private int enemyID;  // Unique ID for each enemy
    private static float idFactor = 1000f; // Factor to generate unique ID from position

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Get the EnemyMovingLaser component
        laserScript = GetComponent<EnemyMovingLaser>();

        // Assign a unique ID based on the enemy's position
        if (enemyID == 0) // Only assign if it's not already set (use case for scene reloads)
        {
            enemyID = GenerateUniqueID(transform.position);
        }
    }

    // Generate a unique ID based on the position
    private int GenerateUniqueID(Vector3 position)
    {
        // Use a simple hash code to generate a unique ID from the position
        return Mathf.Abs((int)(position.x * idFactor + position.z * idFactor));
    }

    public override void TakeDamage(int damage, bool cause)
    {
        // Reduce health without calling base method to avoid destroying the object
        health -= damage;

        if (health <= 0)
        {
            HandleDeath();

            if (cause)
            {
                GameObject player = PlayerController.player;
                player.GetComponent<Health>().TakeDamage(-5, false);
            }
        }
    }

    private void HandleDeath()
    {
        // Stop the laser
        if (laserScript != null)
        {
            laserScript.enabled = false;
            Debug.Log("Laser script disabled for enemy ID: " + enemyID);
        }

        // Unlock rotation on the Rigidbody and enable gravity
        if (rb == null)
        {
            // Add Rigidbody if not already present
            rb = gameObject.AddComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None; // Unlock all rotation constraints
            rb.useGravity = true; // Enable gravity
        }
    }

    // Getter for enemy ID
    public int GetEnemyID()
    {
        return enemyID;
    }

    // Setter for enemy ID
    public void SetEnemyID(int id)
    {
        this.enemyID = id;
    }
}
