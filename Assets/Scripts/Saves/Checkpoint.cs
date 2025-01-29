using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string checkpointName; // Unique identifier for each checkpoint
    private bool used = false; // Flag to ensure the checkpoint is only used once

    private void OnTriggerEnter(Collider other)
    {
        // Ensure the checkpoint only triggers once and only for the player
        if (used) 
        {
            return; // If used already, exit early
        }

        // Check if the object colliding with the checkpoint is the player
        if (other.CompareTag("Player"))
        {
            used = true; // Mark checkpoint as used

            // Notify the GameManager to save the player's position
            Vector3 playerPosition = other.transform.position;
            GameManager.Instance.SaveCheckpoint(playerPosition);
        }
    }
}
