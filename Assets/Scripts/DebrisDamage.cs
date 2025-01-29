using UnityEngine;

public class DebrisDamage : MonoBehaviour
{
    public int damage = 10; // Amount of damage to deal to the player
    private bool hasDamagedPlayer = false; // Flag to prevent multiple damage

    // This is triggered when the debris collides with something
    void Start() {
        Destroy(gameObject, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object collided with is the player (assuming PlayerController has a tag "Player")
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the debris has already damaged the player
            if (!hasDamagedPlayer)
            {
                // Get the Health component of the player
                Health playerHealth = collision.gameObject.GetComponent<Health>();

                // If the player has a Health component, deal damage
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage, false);
                }

                // Set the flag to prevent further damage from the same debris object
                hasDamagedPlayer = true;
                
                // Destroy the debris after 3 seconds
            }
        }
    }
}
