using UnityEngine;
using System.Collections;

public class VoidAttract : MonoBehaviour
{
    public float pullStrength = 10f;     // Initial strength of the pull toward the center
    public float repelStrength = 20f;   // Strength of the repulsion force
    public float attractDuration = 5f;  // Duration of attraction phase
    public float repelDuration = 3f;    // Duration of repulsion phase

    private bool isRepelling = false;   // Whether the object is repelling instead of attracting

    private void Start()
    {
        // Start the process of switching from attraction to repulsion
        StartCoroutine(SwitchToRepel());
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object has a Rigidbody and doesn't have a PlayerController script
        Rigidbody rb = other.GetComponent<Rigidbody>();
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (rb != null && playerController == null)
        {
            // Apply the appropriate force (attract or repel)
            Vector3 direction = isRepelling 
                ? (rb.position - transform.position).normalized  // Repulsion direction
                : (transform.position - rb.position).normalized; // Attraction direction

            float force = isRepelling ? repelStrength : pullStrength;
            rb.AddForce(direction * force, ForceMode.Force);
        }
    }

    private IEnumerator SwitchToRepel()
    {
        // Wait for the attraction phase to end
        yield return new WaitForSeconds(attractDuration);

        // Switch to repulsion
        isRepelling = true;

        // Wait for the repulsion phase to end
        yield return new WaitForSeconds(repelDuration);

        // Destroy the object after the repulsion phase
        Destroy(gameObject);
    }
}
