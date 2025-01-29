using UnityEngine;

public class TriggerEnemies : MonoBehaviour
{
    private bool hasTriggered = false; // To ensure the trigger only activates once
    public string name_to_trigger;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger was already activated
        if (hasTriggered) return;

        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Mark as triggered to prevent reactivation
            hasTriggered = true;

            // Find all objects in the scene with the tag "Pillar"
            GameObject[] pillars = GameObject.FindGameObjectsWithTag(name_to_trigger);

            foreach (GameObject pillar in pillars)
            {
                // Check if the pillar has a HiddenPillarScript component
                HiddenPillarScript hiddenPillar = pillar.GetComponent<HiddenPillarScript>();
                if (hiddenPillar != null)
                {
                    // Call the Activate method to trigger the pillars
                    hiddenPillar.Activate();
                }
            }
        }
    }
}
