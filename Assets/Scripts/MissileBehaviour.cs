using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.VFX;


public class HomingRigidbody : MonoBehaviour
{
    public GameObject shooter; // Assign the shooter GameObject dynamically
    public float speed = 5f;  // Initial movement speed
    public float rotationSpeed = 3f;  // Speed of rotation to face the player
    public float initialStraightDuration = 2f; // Time to travel straight at spawn
    public float homingDuration = 3f; // Time to home in on the player
    public float finalSpeed = 8f; // Speed after homing phase
    public GameObject explosionEffect; // Explosion visual effect prefab
    public float explosionRadius = 5f; // Radius of the explosion
    public float explosionForce = 500f; // Knockback force of the explosion
    public int damageAmount = 50; // Damage to apply within the explosion radius
    public VisualEffect trailEffect; // The trail effect Visual Effect Graph
    public float trailFadeDuration = 2f; // Duration for the trail effect to fade out
    public float startDelay = 0f;
    private Rigidbody rb;
    private float elapsedTime = 0f;
    private bool hasExploded = false; // Flag to track if the missile has exploded
    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>(); // To track which GameObjects have been affected

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the object!");
        }

        // Start the delayed movement behavior
        StartCoroutine(DelayedStart());
    }

    // Coroutine to handle the delay before starting missile movement
    IEnumerator DelayedStart()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(startDelay);

        // Initial movement
        if (!rb.isKinematic) {
            rb.linearVelocity = transform.forward * speed;
        }
        
    }

    void FixedUpdate()
    {
        // Check if Rigidbody is null or kinematic before performing physics updates
        if (rb == null || rb.isKinematic) return;

        elapsedTime += Time.fixedDeltaTime;
        GameObject player = PlayerController.player;

        if (elapsedTime < initialStraightDuration)
        {
            // Travel straight
            rb.linearVelocity = transform.forward * speed;
        }
        else if (elapsedTime < initialStraightDuration + homingDuration)
        {
            // Homing phase
            if (player != null)
            {
                Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
                rb.linearVelocity = transform.forward * speed;
            }
        }
        else
        {
            // Post-homing phase with final speed
            rb.linearVelocity = transform.forward * finalSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ensure the missile doesn't explode more than once
        if (hasExploded || collision.gameObject == shooter)
        {
            return; // Skip the explosion if already exploded or the collision is with the shooter
        }

        Explode();

        // Set the Rigidbody to null or make it kinematic instead of destroying it
        if (rb != null)
        {
            rb.isKinematic = true;  // Disable physics interactions
        }

        // Start fade-out effect and destroy the missile after fading out
        StartCoroutine(FadeOutAndDestroy());
    }

    void Explode()
    {
        // Mark the missile as exploded to prevent re-explosion
        hasExploded = true;

        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f); // Destroy the explosion effect after 2 seconds
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // HashSet to track affected objects (avoid multiple triggers)
        foreach (Collider nearbyObject in colliders)
        {
            // Skip if the object has already been affected
            if (affectedObjects.Contains(nearbyObject.gameObject))
            {
                continue;
            }

            affectedObjects.Add(nearbyObject.gameObject); // Add the object to the affected set

            // Check if the object has a Health component
            Health health = nearbyObject.GetComponent<Health>();
            if (health != null)
            {
                // Only apply damage if the object has a Health component
                health.TakeDamage(damageAmount, false);
            }

            // Apply explosion force if the object has a Rigidbody
            Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
            if (nearbyRb != null)
            {
                Vector3 direction = nearbyRb.transform.position - transform.position;
                nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // Loop through all direct children of the current object
        foreach (Transform child in transform)
{
    if (child.name == "Projectile") // Check if the child's name is "Projectile"
    {
        // Disable the Renderer
        Renderer childRenderer = child.GetComponent<Renderer>();
        if (childRenderer != null)
        {
            childRenderer.enabled = false; // This hides the child's renderer
        }

        // Disable the Collider
        Collider childCollider = child.GetComponent<Collider>();
        if (childCollider != null)
        {
            childCollider.enabled = false; // This disables the child's collider
        }
    }
}



        // Gradually fade out the trail effect
        if (trailEffect != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < trailFadeDuration)
            {
                elapsedTime += Time.deltaTime;

                // Calculate alpha value (0 to 1)
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / trailFadeDuration);

                // Update the alpha of the trail effect
                trailEffect.SetFloat("Alpha", alpha);

                yield return null;
            }

            // Turn off the trail effect after fading out
            trailEffect.Stop();
        }

        // Destroy the missile after the fade-out
        if (transform.parent != null) // Check if the GameObject has a parent
        {
            if (transform.parent.name == "VoidBlast") // Replace with the actual parent's name
            {
                Destroy(transform.parent.gameObject); // Destroy the parent GameObject
            } else {
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
