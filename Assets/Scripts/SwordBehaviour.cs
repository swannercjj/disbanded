using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

public class HomingSwordRigidbody : MonoBehaviour
{
    public GameObject shooter; // Assign the shooter GameObject dynamically
    public float speed = 5f; // Initial movement speed
    public float rotationSpeed = 3f; // Speed of rotation to face the player
    public float initialStraightDuration = 2f; // Time to travel straight at spawn
    public float homingDuration = 3f; // Time to home in on the player
    public float finalSpeed = 8f; // Speed after homing phase
    public int damageAmount = 50; // Damage to apply on impact
    public VisualEffect trailEffect; // The trail effect Visual Effect Graph
    public float trailFadeDuration = 2f; // Duration for the trail effect to fade out
    public float destructionDelay = 3f; // Time after collision to destroy the sword

    private Rigidbody rb;
    private float elapsedTime = 0f;
    private bool hasImpacted = false; // Flag to track if the sword has already impacted

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing from the object!");
        }

        // Initial movement
        rb.linearVelocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        if (rb == null || rb.isKinematic) return;

        elapsedTime += Time.fixedDeltaTime;
        GameObject player = PlayerController.player;

        if (elapsedTime < initialStraightDuration)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else if (elapsedTime < initialStraightDuration + homingDuration)
        {
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
            rb.linearVelocity = transform.forward * finalSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted || collision.gameObject == shooter)
        {
            return;
        }

        hasImpacted = true;

        // Enable gravity and stop any active forces
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
        }

        // Check if the collided object has a Health component and deal damage
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damageAmount, false);
        }

        // Start destruction sequence
        StartCoroutine(FadeOutAndDestroy());
    }

    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // Fade out trail effect
        if (trailEffect != null)
        {
            float fadeElapsed = 0f;
            while (fadeElapsed < trailFadeDuration)
            {
                fadeElapsed += Time.deltaTime;

                float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / trailFadeDuration);
                trailEffect.SetFloat("Alpha", alpha);

                yield return null;
            }

            trailEffect.Stop();
        }

        // Wait for destruction delay before destroying the sword
        yield return new WaitForSeconds(destructionDelay);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Visual debug for collision size
    }
}
