using UnityEngine;

public class EnemyHealth : Health
{
    public float recoveryDuration; // Time before returning upright
    private Rigidbody rb; // Reference to the Rigidbody
    private Quaternion initialRotation; // Initial rotation when created
    private bool isRecovering = false; // Tracks if rotation recovery is in progress

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Lock rotation constraints to keep the enemy upright by default
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Store the initial upright rotation
        initialRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Call the base health logic

        if (!isRecovering)
        {
            StartCoroutine(RecoverRotation());
        }
    }

    private System.Collections.IEnumerator RecoverRotation()
    {
        isRecovering = true;

        // Temporarily unlock all rotation constraints to allow free rotation
        rb.constraints = RigidbodyConstraints.None;

        // Wait for the recovery duration
        yield return new WaitForSeconds(recoveryDuration);

        // Forcefully rotate to the initial upright position
        float elapsedTime = 0f;
        while (elapsedTime < recoveryDuration)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, (elapsedTime / recoveryDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the final upright position to avoid any inaccuracy
        transform.rotation = initialRotation;

        // Re-lock rotation constraints to keep the enemy upright
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        isRecovering = false;
    }
}
