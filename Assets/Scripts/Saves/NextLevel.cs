using UnityEngine;
using UnityEngine.SceneManagement; // Required to manage scenes

public class NextLevel : MonoBehaviour
{
    [SerializeField]
    private string nextLevelName; // Name of the next level to load

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Load the next level
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                SceneManager.LoadScene(nextLevelName);
                Debug.Log("Loading next level: " + nextLevelName);
            }
            else
            {
                Debug.LogError("Next level name is not set.");
            }
        }
    }
}
