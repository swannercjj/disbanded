using UnityEngine;

public class EnemyMovingLaser : MonoBehaviour
{
    public GameObject cylinder; // Assign your laser cylinder here
    public Transform emittingPoint; // The point where the laser originates from
    public Transform targetPoint; // The point the object should move toward
    public float laserExpansionDuration = 0.5f; // Duration for the laser to expand to full size
    public float laserInitialRadius = 0.1f; // The initial radius of the laser
    public float laserFullRadius = 1f; // The full radius of the laser when fully expanded
    public float fluctuationRange = 0.2f; // Range for random fluctuations during expansion
    public float laserLength = 50f; // Fixed length of the laser (Y-axis scaling for the cylinder)

    public float oscillationSpeed = 1f; // Speed of the oscillation along the path
    public float oscillationAmplitude = 1f; // Amplitude of the oscillation (how far left/right it moves)
    public float movementSpeedFactor = 1f; // Speed factor to control how fast the object moves forward

    private Transform cylinderTransform; // Transform of the cylinder
    private float currentRadius; // Current radius of the laser
    private float movementStartTime; // Time when movement started
    private Vector3 startPosition; // Starting position of the object
    private Vector3 directionToTarget; // Direction vector towards the target
    private Vector3 perpendicularDirection; // Perpendicular direction for oscillation

    void Start()
    {
        if (cylinder == null || emittingPoint == null || targetPoint == null)
        {
            Debug.LogError("Cylinder, Emitting Point, or Target Point is not assigned. Please assign them in the Inspector.");
            return;
        }

        cylinderTransform = cylinder.transform;

        // Set the initial scale and position of the cylinder
        // Position the cylinder at the emitting point and ensure it grows downward
        cylinderTransform.position = emittingPoint.position + new Vector3(0f, -laserLength / 2, 0f);

        // Set the initial scale, with the Y scale reflecting the length
        cylinderTransform.localScale = new Vector3(laserInitialRadius, laserLength / 2, laserInitialRadius);

        // Store the start position and initialize the movement
        startPosition = transform.position;
        movementStartTime = Time.time;

        // Calculate the direction vector towards the target
        directionToTarget = (targetPoint.position - startPosition).normalized;

        // Calculate the perpendicular direction for oscillation (cross product with Vector3.up)
        perpendicularDirection = Vector3.Cross(directionToTarget, Vector3.up).normalized;

        // Start the expansion process
        StartCoroutine(ExpandLaser());
    }

    private System.Collections.IEnumerator ExpandLaser()
    {
        float elapsedTime = 0f;

        while (elapsedTime < laserExpansionDuration)
        {
            // Calculate the normalized time (0 to 1)
            float normalizedTime = elapsedTime / laserExpansionDuration;

            // Interpolate the radius and add random fluctuations
            float targetRadius = Mathf.Lerp(laserInitialRadius, laserFullRadius, normalizedTime);
            float fluctuation = Random.Range(-fluctuationRange, fluctuationRange);
            currentRadius = Mathf.Clamp(targetRadius + fluctuation, laserInitialRadius, laserFullRadius);

            // Update the cylinder's scale (only X and Z, Y remains fixed for length)
            cylinderTransform.localScale = new Vector3(currentRadius, laserLength / 2, currentRadius);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After reaching full expansion, keep fluctuating the radius continuously
        while (true)
        {
            // Apply continuous fluctuations even after reaching the maximum radius
            float fluctuation = Random.Range(-fluctuationRange, fluctuationRange);
            currentRadius = Mathf.Clamp(laserFullRadius + fluctuation, laserInitialRadius, laserFullRadius);

            // Update the cylinder's scale (only X and Z, Y remains fixed for length)
            cylinderTransform.localScale = new Vector3(currentRadius, laserLength / 2, currentRadius);

            yield return null;
        }
    }

    // Update method for handling the movement and oscillation
    void Update()
    {
        // Calculate the distance to the target
        float totalDistance = Vector3.Distance(startPosition, targetPoint.position);

        // Adjust the normalized time for the movement based on movementSpeedFactor
        float elapsedTime = (Time.time - movementStartTime) * movementSpeedFactor;
        float normalizedTime = Mathf.Clamp01(elapsedTime / totalDistance);

        // Calculate the current position of the object along the straight line
        Vector3 targetPosition = Vector3.Lerp(startPosition, targetPoint.position, normalizedTime);

        // Calculate the oscillation offset using a sine wave
        float oscillation = Mathf.Sin(elapsedTime * oscillationSpeed) * oscillationAmplitude;

        // Apply the oscillation along the perpendicular direction
        targetPosition += perpendicularDirection * oscillation;

        // Move the object to the calculated target position with oscillation
        transform.position = targetPosition;

        // Stop the movement when the object reaches the target (or close to it)
        if (normalizedTime >= 1f)
        {
            enabled = false; // Disable the script once it reaches the target
        }
    }

    // Public function to update the target point
    public void SetTargetPoint(Transform newTargetPoint)
    {
        if (newTargetPoint == null)
        {
            Debug.LogError("New target point is not assigned.");
            return;
        }

        // Update the target point and re-calculate direction and perpendicular direction
        targetPoint = newTargetPoint;
        startPosition = transform.position;  // Reset the start position to the current position
        movementStartTime = Time.time; // Reset movement start time

        // Recalculate the direction vector towards the new target
        directionToTarget = (targetPoint.position - startPosition).normalized;

        // Recalculate the perpendicular direction for oscillation (cross product with Vector3.up)
        perpendicularDirection = Vector3.Cross(directionToTarget, Vector3.up).normalized;
    }
}
