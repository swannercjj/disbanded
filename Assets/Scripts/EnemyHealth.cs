using UnityEngine;

public class EnemyHealth : Health
{
    private Rigidbody rb; // Reference to the Rigidbody component

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("Rigidbody component missing on the enemy!");
        }
    }

    public override void TakeDamage(int damage)
    {
        // Reduce health without calling base method to avoid destroying the object
        health -= damage;

        if (health <= 0)
        {
            UnlockRotationEnableGravityAndDisableScripts(); // Custom behavior for enemies
        }
    }

    private void UnlockRotationEnableGravityAndDisableScripts()
    {
        // Unlock rotation on the Rigidbody and enable gravity
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None; // Unlock all rotation constraints
            rb.useGravity = true; // Enable gravity
        }
    }
}
