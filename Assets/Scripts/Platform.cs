using UnityEngine;

public class Platform : MonoBehaviour
{
    private Vector3 platformVelocity;

private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {

        // Track platform velocity
        Rigidbody platformRb = GetComponent<Rigidbody>();
        if (platformRb != null)
        {
            platformVelocity = platformRb.linearVelocity;
        }
    }
}

private void OnTriggerStay(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Rigidbody playerRb = other.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            // Apply platform's velocity to the player
            playerRb.linearVelocity += platformVelocity * 0.1f;
        }
    }
}

private void OnTriggerExit(Collider other)
{
    
}

}
