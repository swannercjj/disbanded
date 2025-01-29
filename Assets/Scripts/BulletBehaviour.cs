using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public class StraightProjectile : MonoBehaviour
{
    public GameObject shooter; // Assign the shooter GameObject dynamically
    public float speed = 5f; // Movement speed
    public GameObject explosionEffect; // Explosion visual effect prefab
    public float explosionRadius = 5f; // Radius of the explosion
    public float explosionForce = 500f; // Knockback force of the explosion
    public int damageAmount = 50; // Damage to apply within the explosion radius
    public VisualEffect trailEffect; // The trail effect Visual Effect Graph
    public float trailFadeDuration = 2f; // Duration for the trail effect to fade out

    private Rigidbody rb;
    private bool hasExploded = false; // Flag to track if the projectile has exploded
    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>(); // To track which GameObjects have been affected

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the object!");
            return;
        }
        trailEffect.SetFloat("Alpha", 1);
        // Set the initial velocity for the projectile
        rb.linearVelocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Avoid exploding multiple times or interacting with the shooter
        if (hasExploded || collision.gameObject == shooter)
        {
            return;
        }

        Explode();

        // Stop physics interactions to avoid further collisions
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Start the fade-out process and destroy the projectile afterward
        StartCoroutine(FadeOutAndDestroy());
    }

    void Explode()
{
    hasExploded = true;

    // Disable the collider to stop further collisions
    Collider collider = GetComponent<Collider>();
    if (collider != null)
    {
        collider.enabled = false; // Disables the collision hitbox
    }

    // Hide the object (make it invisible)
    Renderer objectRenderer = GetComponent<Renderer>();
    if (objectRenderer != null)
    {
        objectRenderer.enabled = false; // Disables rendering of the object
    }

    // Create explosion visual effect
    if (explosionEffect != null)
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f); // Automatically clean up the effect after 2 seconds
    }

    // Apply effects to objects within the explosion radius
    Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

    foreach (Collider nearbyObject in colliders)
    {
        // Apply explosion force if the object has a Rigidbody
        Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
        if (nearbyRb != null)
        {
            Vector3 direction = nearbyRb.transform.position - transform.position;
            nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Skip the shooter and any of its children
        if (nearbyObject.gameObject == shooter || IsChildOfShooter(nearbyObject.transform))
        {
            continue;
        }

        // Find the closest Health component in the object or its ancestors
        Transform currentTransform = nearbyObject.transform;
        Health health = null;

        while (currentTransform != null)
        {
            health = currentTransform.GetComponent<Health>();
            if (health != null)
            {
                break; // Stop searching once a Health component is found
            }
            currentTransform = currentTransform.parent;
        }

        // Skip if no Health component is found
        if (health == null)
        {
            continue;
        }

        // Skip if the health object has already been processed
        if (affectedObjects.Contains(health.gameObject))
        {
            continue;
        }

        // Add the health object to the affected objects list
        affectedObjects.Add(health.gameObject);
        // Apply damage to the health component
        health.TakeDamage(damageAmount, true);
    }


}


// Helper function to check if the object is a child of the shooter
bool IsChildOfShooter(Transform objTransform)
{
    Transform parent = objTransform.parent;
    while (parent != null)
    {
        if (parent.gameObject == shooter)
        {
            return true;
        }
        parent = parent.parent;
    }
    return false;
}


    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // Fade out the projectile's trail effect
        if (trailEffect != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < trailFadeDuration)
            {
                elapsedTime += Time.deltaTime;

                // Update the alpha value of the trail effect
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / trailFadeDuration);
                trailEffect.SetFloat("Alpha", alpha);

                yield return null;
            }

            // Stop the trail effect completely
            trailEffect.Stop();
        }

        // Destroy the projectile after the fade-out
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the explosion radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
