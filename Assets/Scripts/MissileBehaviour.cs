using UnityEngine;
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

    private Rigidbody rb;
    private float elapsedTime = 0f;

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
    if (collision.gameObject == shooter)
    {
        return; // Do nothing if it's the shooter
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
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Health health = nearbyObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }

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

        Transform capsuleChild = transform.Find("Capsule"); // Replace "Capsule" with the actual name of the child object
        if (capsuleChild != null)
        {
            Renderer capsuleRenderer = capsuleChild.GetComponent<Renderer>();
            if (capsuleRenderer != null)
            {
                capsuleRenderer.enabled = false; // This hides the capsule
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
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
