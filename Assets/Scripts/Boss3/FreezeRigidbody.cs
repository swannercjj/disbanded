using UnityEngine;
using System.Collections;
public class FreezeRigidbody : MonoBehaviour
{
    public float freezeDelay = 2f; // Time in seconds before freezing the Rigidbody
    private Rigidbody rb; // Reference to the Rigidbody

    void Start()
    {
        // Get the Rigidbody attached to the parent GameObject
        rb = GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            // Start the coroutine to freeze the Rigidbody
            StartCoroutine(FreezeAfterDelay());
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on the parent!");
        }
    }

    private IEnumerator FreezeAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(freezeDelay);

        // Stop all movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Freeze the Rigidbody by setting it to kinematic
        rb.isKinematic = true;
    }
}
