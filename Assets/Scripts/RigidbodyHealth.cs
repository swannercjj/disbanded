using UnityEngine;

public class RigidbodyHealth : Health
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found! Please attach a Rigidbody to this GameObject.");
        }
    }

    public override void TakeDamage(int damage, bool cause)
    {
        base.TakeDamage(damage, cause); // Call the base class's TakeDamage to reduce health

        if (rb != null)
        {
            // Unlock all rotations and positions
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
