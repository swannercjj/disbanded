using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damageAmount = 100; // The amount of damage dealt when touched

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a Health component
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            // Deal damage to the target
            targetHealth.TakeDamage(damageAmount, false);
        }
    }
}
