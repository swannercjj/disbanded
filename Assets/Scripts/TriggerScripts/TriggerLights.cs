using UnityEngine;

public class TriggerLights : MonoBehaviour
{
    public GameObject objectToManage; // Single GameObject to manage its children with DisableDistance
    public TriggerLights previousRoom; // Reference to the previous room's TriggerLights
    public TriggerLights nextRoom;     // Reference to the next room's TriggerLights

    private bool hasEntered = false;   // To track if the trigger was already entered

    private void OnTriggerEnter(Collider other)
    {
        // Check if this is the first time entering the trigger
        if (!hasEntered && other.CompareTag("Player")) // Assuming the player is tagged as "Player"
        {
            hasEntered = true;

            // Turn on lights for this room
            SetLightsActive(objectToManage, true);

            // Turn off lights in the previous room
            if (previousRoom != null)
            {
                previousRoom.SetLightsActive(previousRoom.objectToManage, false);
            }

            // Turn on lights in the next room
            if (nextRoom != null)
            {
                nextRoom.SetLightsActive(nextRoom.objectToManage, true);
            }
        }
    }

    /// <summary>
    /// Activates or deactivates all DisableDistance scripts in the children of a GameObject.
    /// </summary>
    /// <param name="obj">The GameObject whose children to manage.</param>
    /// <param name="isActive">Whether to set them active or inactive.</param>
    public void SetLightsActive(GameObject obj, bool isActive)
    {
        if (obj == null)
        {
            Debug.LogWarning("Object to manage is null!");
            return;
        }

        // Get all children with the DisableDistance component
        DisableDistance[] disableScripts = obj.GetComponentsInChildren<DisableDistance>(true);
        foreach (DisableDistance script in disableScripts)
        {
            if (script != null) // Ensure the script isn't null
            {
                script.SetAwakeness(isActive);
            }
        }
    }
}
