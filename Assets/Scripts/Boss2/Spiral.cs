using UnityEngine;

public class Spiral : MonoBehaviour
{
    public float initialRadius = 5f; // Starting radius of the spiral
    public float finalRadius = 10f; // Ending radius of the spiral
    public float angularSpeed = 1f; // Speed of rotation in radians per second
    public float ascentSpeed = 1f; // Speed of upward movement
    public float duration = 5f; // Base duration in seconds
    private Rigidbody rb; // Rigidbody component of the GameObject
    private float angle = 0f; // Current angle in radians
    private float elapsedTime = 0f; // Time elapsed since the spiral started
    private Vector3 initialPosition; // Store the initial position of the object

    void Start()
    {
        // Apply random variation of ±1 second to the duration
        duration += Random.Range(-1f, 1f);

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required for the spiral effect.");
        }

        // Store the initial position to calculate relative movement
        initialPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (rb == null || elapsedTime >= duration) return;

        // Update the elapsed time
        elapsedTime += Time.fixedDeltaTime;

        // Calculate the current radius based on elapsed time (interpolating from initial to final radius)
        float currentRadius = Mathf.Lerp(initialRadius, finalRadius, elapsedTime / duration);

        // Calculate the spiral position relative to the initial position
        float x = initialPosition.x + currentRadius * Mathf.Cos(angle); // Start from initial position
        float z = initialPosition.z + currentRadius * Mathf.Sin(angle); // Start from initial position
        float y = initialPosition.y + ascentSpeed * elapsedTime; // Increment y based on time

        // Move relative to initial position (no teleporting)
        Vector3 newPosition = new Vector3(x, y, z);
        rb.MovePosition(newPosition);

        // Update the angle for the spiral effect
        angle += angularSpeed * Time.fixedDeltaTime;
    }
}
