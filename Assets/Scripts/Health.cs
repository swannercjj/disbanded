using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100; // Starting health

    public virtual void TakeDamage(int damage, bool cause)
    {
        health -= damage;
        // if (health <= 0)
        // {
        //     Destroy(gameObject); // Destroy the object if health drops to zero or below
        // }
    }
}
