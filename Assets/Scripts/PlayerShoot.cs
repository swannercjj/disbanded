using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform firePoint; // The point from which bullets are fired
    public Canvas canvas; // Reference to the Canvas (which contains the BeatManager)
    public Camera playerCamera; // Reference to the player's camera

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

        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned.");
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
        }
    }

    void Shoot()
    {
        // Ensure the firePoint and bulletPrefab are assigned
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("Bullet prefab or fire point is missing.");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not set.");
            return;
        }

        // Calculate the direction from the firePoint to the mouse position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = (hit.point - firePoint.position).normalized;

            // Rotate the firePoint to face the target direction
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            firePoint.rotation = rotation;

            // Instantiate the bullet at the firePoint's position and orientation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Set the player as the shooter for the bullet
            StraightProjectile bulletScript = bullet.GetComponent<StraightProjectile>();
            if (bulletScript != null)
            {
                bulletScript.shooter = gameObject; // Use gameObject to refer to the player
            }
        }
        else
        {
            Debug.Log("Mouse raycast did not hit any object.");
        }
    }
}
