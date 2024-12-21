using UnityEngine;

public class EnemyPhase : MonoBehaviour
{
    public GameObject rocketPrefab; // Assign the rocket prefab in the Inspector
    public Transform firePoint; // Assign a fire point for spawning rockets
    public float detectionRange; // Range to detect the player
    public float wanderSpeed = 3f; // Speed while wandering
    public float rotationSpeed = 2f; // Speed to rotate towards the player
    public float rocketCooldown = 3f; // Base time between rocket spawns
    public float rocketCooldownVariance = 1f; // Variance to the cooldown time (± 0.5)
    public float rocketSpreadAngle = 5f; // Angle of spread for the rocket's firing direction

    private Rigidbody rb;
    private float rocketTimer = 0f;
    private Vector3 wanderDirection;
    private float wanderChangeInterval = 5f; // Time to change wander direction
    private float wanderTimer = 0f;

    private Health health; // Reference to the Health script
    private bool isDead = false; // Tracks whether the enemy is dead

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the enemy!");
        }

        if (health == null)
        {
            Debug.LogError("Health component is missing from the enemy!");
        }

        // Set an initial random wander direction
        SetRandomWanderDirection();
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
        rocketTimer += Time.fixedDeltaTime;
        wanderTimer += Time.fixedDeltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Look at the player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // Shoot rockets at random intervals within the cooldown range
            if (rocketTimer >= rocketCooldown + Random.Range(-rocketCooldownVariance / 2f, rocketCooldownVariance / 2f) && rocketPrefab != null && firePoint != null)
            {
                // Instantiate the rocket and set the shooter reference
                GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

                // Calculate the random spread for the rocket's direction
                Vector3 directionToTarget = (player.transform.position - firePoint.position).normalized;

                // Apply random spread to the direction
                directionToTarget = ApplyRandomSpread(directionToTarget);

                // Adjust the rocket's rotation with the spread direction
                rocket.transform.rotation = Quaternion.LookRotation(directionToTarget);

                HomingRigidbody homingRigidbody = rocket.GetComponent<HomingRigidbody>();
                if (homingRigidbody != null)
                {
                    homingRigidbody.shooter = this.gameObject; // Set the shooter as the enemy object
                }

                rocketTimer = 0f; // Reset the rocket timer
            }

            // Stop movement (focused on attacking)
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            // Wander behavior
            if (wanderTimer >= wanderChangeInterval)
            {
                SetRandomWanderDirection();
                wanderTimer = 0f;
            }

            rb.linearVelocity = wanderDirection * wanderSpeed;

            // Gradually rotate to face wander direction
            if (wanderDirection != Vector3.zero)
            {
                Quaternion wanderRotation = Quaternion.LookRotation(wanderDirection);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, wanderRotation, rotationSpeed * Time.fixedDeltaTime));
            }
        }
    }

    private void SetRandomWanderDirection()
    {
        // Choose a random direction to wander
        float randomAngle = Random.Range(0f, 360f);
        wanderDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;
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

        // Disable movement and any other behaviors
        // rb.linearVelocity = Vector3.zero;

        // Optional: Add death effects, such as animations or particle systems
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the detection range in the Editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
