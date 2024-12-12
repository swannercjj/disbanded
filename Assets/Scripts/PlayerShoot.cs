using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float shootRange = 100f; // Maximum distance for shooting
    public float knockbackForce = 5f; // Knockback force applied to hit objects
    public int damage = 10; // Damage dealt to the object
    public Camera playerCamera; // The camera used for shooting (assign in the inspector)

    public GameObject impactEffectPrefab; // Assign your Visual Effect prefab in the Inspector
    public Canvas canvas; // Reference to the Canvas (which contains the BeatManager)

    private BeatManager beatManager; // Reference to the BeatManager

    void Start()
    {
        if (canvas != null)
        {
            // Get the BeatManager component from the Canvas
            beatManager = canvas.GetComponentInChildren<BeatManager>();
            if (beatManager == null)
            {
                Debug.LogError("No BeatManager found in the Canvas.");
            }
        }
        else
        {
            Debug.LogError("Canvas reference is missing.");
        }
    }

    void Update()
    {
        // Check if the player presses the fire button (left mouse button)
        if (Input.GetMouseButtonDown(0)) // Left-click for shooting
        {
            // Check if there is a beat in the middle before shooting
            GameObject beatInMiddle = beatManager.GetBeatInMiddle();

            if (beatInMiddle != null) // If there's a beat in the middle, shoot
            {
                Shoot();
            }
            else
            {
                Debug.Log("No beat in the middle. Cannot shoot.");
            }
        }
    }

    void Shoot()
    {
        // Create a ray from the camera's position in the direction the camera is facing
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an object within the specified range
        if (Physics.Raycast(ray, out hit, shootRange))
        {
            // Check if the object hit has a "Health" script
            Health healthScript = hit.collider.GetComponent<Health>();
            if (healthScript != null)
            {
                // Deal damage to the object
                healthScript.TakeDamage(damage);

                // Apply knockback to the object's Rigidbody, if it has one
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 knockbackDirection = (hit.collider.transform.position - transform.position).normalized;
                    rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                }
            }

            // Trigger the impact effect
            TriggerImpactEffect(hit.point, hit.normal);
        }
    }

    void TriggerImpactEffect(Vector3 position, Vector3 normal)
    {
        // Instantiate the impact effect prefab at the hit position
        if (impactEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(impactEffectPrefab, position, Quaternion.LookRotation(normal));

            Destroy(impactEffect, 2f); // Automatically destroy the effect after 2 seconds
        }
    }
}
