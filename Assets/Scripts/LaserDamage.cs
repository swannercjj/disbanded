using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject shooter;    // The shooter that created this laser
    public int damageAmount = 1;  // Amount of damage per tick (converted to int)
    public float damageInterval = 0.1f; // Interval between each damage tick (in seconds)

    private float damageTimer; // Timer for the next damage tick

    private void Start()
    {
        // Initialize the damage timer
        damageTimer = damageInterval;
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object is not the shooter
        if (other.gameObject != shooter)
        {
            // If the object has a Health component, apply damage
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                // Apply damage to the object
                ApplyDamage(health);
            }
        }
    }

    private void ApplyDamage(Health health)
    {
        // Apply damage (convert to int)
        health.TakeDamage(damageAmount); // damageAmount is already an int
    }

    private void OnDrawGizmos()
    {
        // For debugging purposes: visualize the trigger zone (showing a cylinder in editor)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Adjust radius to match the laser's size
    }
}
