using UnityEngine;

public class EnemyLaserLerp : MonoBehaviour
{
    public GameObject cylinder; // Assign your laser cylinder here
    public Transform emittingPoint; // The point where the laser originates from
    public Transform targetPoint; // The point where the laser should move toward
    public float laserExpansionDuration = 0.5f; // Duration for the laser to expand to full size
    public float laserInitialRadius = 0.1f; // The initial radius of the laser
    public float laserFullRadius = 1f; // The full radius of the laser when fully expanded
    public float fluctuationRange = 0.2f; // Range for random fluctuations during expansion
    public float laserLength = 50f; // Fixed length of the laser (Y-axis scaling for the cylinder)
    public float movementDuration = 5f; // Time to reach the target point

    private Transform cylinderTransform; // Transform of the cylinder
    private float currentRadius; // Current radius of the laser
    private Vector3 startPosition; // Starting position of the object
    private Vector3 targetPositionCopy; // Fixed position copy of the target point
    private float movementStartTime; // Time when movement started
    private EnemyHealth health;

    void Start()
    {
        if (cylinder == null || emittingPoint == null)
        {
            Debug.LogError("Cylinder, Emitting Point, or Target Point is not assigned. Please assign them in the Inspector.");
            return;
        }

        cylinderTransform = cylinder.transform;

        // Set the initial scale and position of the cylinder
        cylinderTransform.position = emittingPoint.position + new Vector3(0f, -laserLength / 2, 0f);
        cylinderTransform.localScale = new Vector3(laserInitialRadius, laserLength / 2, laserInitialRadius);

        // Store the starting position and the target position copy
        startPosition = transform.position;
        if (targetPoint != null) {
            targetPositionCopy = targetPoint.position; // Create a copy of the target point's position
        }
        
        movementStartTime = Time.time;

        health = gameObject.GetComponent<EnemyHealth>();

        // Start the expansion process
        StartCoroutine(ExpandLaser());
    }

    public void Initial(Vector3 position) {
        targetPositionCopy = position;
    }

    private System.Collections.IEnumerator ExpandLaser()
    {
        float elapsedTime = 0f;

        while (elapsedTime < laserExpansionDuration)
        {
            float normalizedTime = elapsedTime / laserExpansionDuration;
            float targetRadius = Mathf.Lerp(laserInitialRadius, laserFullRadius, normalizedTime);
            float fluctuation = Random.Range(-fluctuationRange, fluctuationRange);
            currentRadius = Mathf.Clamp(targetRadius + fluctuation, laserInitialRadius, laserFullRadius);

            cylinderTransform.localScale = new Vector3(currentRadius, laserLength / 2, currentRadius);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            if (health.health <= 0)
            {
                if (cylinder != null)
                {
                    Destroy(cylinder);
                }
                yield break;
            }

            float fluctuation = Random.Range(-fluctuationRange, fluctuationRange);
            currentRadius = Mathf.Clamp(laserFullRadius + fluctuation, laserInitialRadius, laserFullRadius);
            cylinderTransform.localScale = new Vector3(currentRadius, laserLength / 2, currentRadius);

            yield return null;
        }
    }

    void Update()
    {

        if (health.health <= 0 || targetPositionCopy == Vector3.zero)
        {
            return; // Stop movement when the enemy is dead
        }

        float elapsedTime = Time.time - movementStartTime;
        float normalizedTime = Mathf.Clamp01(elapsedTime / movementDuration);

        // Move towards the fixed copy of the target position
        transform.position = Vector3.Lerp(startPosition, targetPositionCopy, normalizedTime);

        if (normalizedTime >= 1f)
        {
            enabled = false; // Disable the script once it reaches the target
        }
    }
}
