using UnityEngine;

public class PlayerForceSimulator : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 knockbackVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing from the player!");
        }
    }

    // Method to apply knockback force
    public void ApplyKnockback(Vector3 direction, float strength)
    {
        if (controller != null)
        {
            // Apply a force as a velocity in the opposite direction (knockback)
            knockbackVelocity = direction.normalized * strength;
        }
    }

    void Update()
    {
        // Apply the knockback velocity to the player using the CharacterController
        if (knockbackVelocity.magnitude > 0f)
        {
            // Apply the knockback force
            controller.Move(knockbackVelocity * Time.deltaTime);

            // Gradually reduce the knockback effect (simulating friction)
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, 0.1f);
        }
    }
}
