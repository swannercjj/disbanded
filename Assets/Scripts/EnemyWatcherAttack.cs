using UnityEngine;
using System.Collections;

public class EnemyWatcherAttack : MonoBehaviour
{
    public GameObject cylinderPrefab; // The cylinder prefab to spawn
    public GameObject vfxPrefab; // The VFX prefab to spawn (for barrel)
    public GameObject attackVFXPrefab; // The new VFX prefab to spawn when attack starts
    public Transform barrelTransform; // The barrel's transform to spawn the VFX on
    public float raycastDelay = 1f; // Delay before the raycast is performed
    public float attackInterval = 3f; // Time between attacks
    public float rotationSpeed = 2f; // Base speed at which the ball rotates to track the player
    public LayerMask ignoreLayerMask; // The LayerMask to ignore the ball's children
    public float laserMaxDistance = 50f; // Maximum range of the laser
    public float laserExpansionDuration = 0.5f; // Duration for the laser to expand to full radius
    public float laserInitialRadius = 0.1f; // The initial radius of the laser
    public float laserFullRadius = 1f; // The full radius of the laser when fully expanded

    private Transform playerTransform; // Reference to the player
    private bool isAttacking;
    private GameObject currentLaser; // The currently active laser
    private GameObject attackVFXInstance; // The VFX instance for the attack
    private LineRenderer lineRenderer; // The LineRenderer component
    private Health healthComponent; // Reference to the health component

    void Start()
{
    // Instantiate and disable the laser initially
    currentLaser = Instantiate(cylinderPrefab);
    currentLaser.SetActive(false);

    // Set the shooter reference for the laser
    Laser laserScript = currentLaser.GetComponent<Laser>();
    if (laserScript != null)
    {
        laserScript.shooter = gameObject; // Set the shooter to this enemy
    }

    // Setup LineRenderer
    lineRenderer = GetComponent<LineRenderer>();
    if (lineRenderer == null)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    lineRenderer.startWidth = 0.1f;
    lineRenderer.endWidth = 0.1f;
    lineRenderer.positionCount = 2;
    lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
    lineRenderer.material.color = Color.red;

    // Get the health component
    healthComponent = GetComponent<Health>();

    // Spawn the VFX immediately if it's assigned and there is a barrelTransform
    SpawnVFX();
}


    void Update()
    {
        // Attempt to get the player reference if it's null
        if (playerTransform == null)
        {
            if (PlayerController.player != null)
            {
                playerTransform = PlayerController.player.transform;
            }
            else
            {
                return; // Skip update if player is not yet available
            }
        }

        // Check if health is zero or less; if so, disable the LineRenderer and stop further updates
        if (healthComponent != null && healthComponent.health <= 0)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false; // Disable LineRenderer
                currentLaser.SetActive(false); // Ensure laser is inactive
            }
            return;
        }

        // Determine the current rotation speed (halved if the laser is active)
        float currentRotationSpeed = currentLaser != null && currentLaser.activeSelf
            ? rotationSpeed / 2f
            : rotationSpeed;

        // Smoothly rotate to look at the player using the current rotation speed
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed * Time.deltaTime);

        // Start attack if not already attacking
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }

        // Continuously update the laser position and direction
        if (currentLaser != null && currentLaser.activeSelf)
        {
            UpdateLaser();
        }

        // Update LineRenderer to follow the raycast direction
        UpdateLineRenderer();
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Wait for the raycast delay
        yield return new WaitForSeconds(raycastDelay);

        // Spawn the attack initiation VFX
        SpawnAttackVFX();

        // Ensure the laser is active at the start of the attack
        if (!currentLaser.activeSelf)
        {
            currentLaser.SetActive(true);
        }

        float elapsedTime = 0f;
        float totalDuration = attackInterval; // The total lifespan of the laser

        while (elapsedTime < totalDuration)
        {
            // If health drops to 0 during the attack, stop and disable the laser
            if (healthComponent != null && healthComponent.health <= 0)
            {
                currentLaser.SetActive(false);
                lineRenderer.enabled = false;
                yield break;
            }

            // Calculate the normalized time (0 to 1)
            float normalizedTime = elapsedTime / totalDuration;

            // Determine the scale factor based on the growth/shrink logic
            float scaleFactor;
            if (normalizedTime <= 0.25f) // First quarter: grow to full size
            {
                scaleFactor = Mathf.Lerp(laserInitialRadius, laserFullRadius, normalizedTime / 0.25f);
            }
            else if (0.75 <= normalizedTime && normalizedTime <= 1f) // Remaining time: shrink back to min size
            {
                scaleFactor = Mathf.Lerp(laserFullRadius, laserInitialRadius, (normalizedTime - 0.75f) / 0.25f);
            }
            else
            {
                scaleFactor = laserFullRadius; // Safety fallback
            }

            // Add random variation to the scale factor (between 0 and 0.5)
            float randomVariation = Random.Range(0f, 0.5f);
            scaleFactor += randomVariation;

            // Update the laser's scale
            Vector3 scale = currentLaser.transform.localScale;
            currentLaser.transform.localScale = new Vector3(scaleFactor, scale.y, scaleFactor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Disable the laser after the attack interval
        currentLaser.SetActive(false);

        // Spawn and destroy the VFX after the raycast delay
        StartCoroutine(SpawnAndDestroyVFX());

        // Destroy the attack initiation VFX after the attack
        Destroy(attackVFXInstance);

        isAttacking = false;
    }

    private void SpawnVFX()
    {
        // Ensure that the VFX prefab and barrel transform are assigned
        if (vfxPrefab != null && barrelTransform != null)
        {
            // Instantiate the VFX and make it a child of the barrel
            GameObject vfxInstance = Instantiate(vfxPrefab, barrelTransform.position, barrelTransform.rotation);
            vfxInstance.transform.SetParent(barrelTransform); // Make it a child of the barrel
        }
    }

    private void SpawnAttackVFX()
    {
        // Ensure that the new attack VFX prefab is assigned and create the VFX at the barrel's position
        if (attackVFXPrefab != null && barrelTransform != null)
        {
            // Instantiate the attack VFX and make it a child of the barrel
            attackVFXInstance = Instantiate(attackVFXPrefab, barrelTransform.position, barrelTransform.rotation);
            attackVFXInstance.transform.SetParent(barrelTransform); // Make it a child of the barrel
        }
    }

    private IEnumerator SpawnAndDestroyVFX()
    {
        // Check if the VFX prefab and barrel transform are assigned
        if (vfxPrefab != null && barrelTransform != null)
        {
            // Instantiate the VFX and make it a child of the barrel
            GameObject vfxInstance = Instantiate(vfxPrefab, barrelTransform.position, barrelTransform.rotation);
            vfxInstance.transform.SetParent(barrelTransform); // Make it a child of the barrel

            // Wait for the raycast delay before destroying the VFX
            yield return new WaitForSeconds(raycastDelay);

            // Destroy the VFX after the delay
            Destroy(vfxInstance);
        }
    }

    private void UpdateLaser(Vector3 hitPoint = default)
    {
        if (hitPoint == default)
        {
            hitPoint = transform.position + transform.forward * laserMaxDistance;
        }

        // Set the cylinder's position and scale
        Vector3 start = transform.position;
        Vector3 end = hitPoint;

        currentLaser.transform.position = (start + end) / 2;

        // Align the cylinder to the ray direction
        Vector3 direction = (end - start).normalized;
        currentLaser.transform.rotation = Quaternion.LookRotation(direction);

        // Rotate the cylinder to align its height (y-axis) with the direction
        currentLaser.transform.Rotate(90, 0, 0);

        // Set the cylinder's scale along the ray's length
        float distance = Vector3.Distance(start, end);
        currentLaser.transform.localScale = new Vector3(currentLaser.transform.localScale.x, distance / 2, currentLaser.transform.localScale.z);
    }

    private void UpdateLineRenderer()
    {
        // Perform a raycast in the forward direction
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Set the start point of the LineRenderer
        lineRenderer.SetPosition(0, ray.origin);

        // Check if the ray hits anything
        if (Physics.Raycast(ray, out hit, laserMaxDistance, ~ignoreLayerMask))
        {
            // Set the endpoint of the LineRenderer to the hit point
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // Set the endpoint of the LineRenderer to the maximum distance
            lineRenderer.SetPosition(1, ray.origin + ray.direction * laserMaxDistance);
        }
    }
}
