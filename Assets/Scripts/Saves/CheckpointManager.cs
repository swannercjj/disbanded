using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private Vector3 checkpointPosition; // To store player checkpoint position
    public HashSet<int> deadEnemies = new HashSet<int>(); // To track dead enemies
    private string curr_level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scene loads
            curr_level = SceneManager.GetActiveScene().name;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    // Save the checkpoint position and enemy states
    public void SaveCheckpoint(Vector3 position)
    {
        checkpointPosition = position;

        // Save the current state of dead enemies
        foreach (var enemy in FindObjectsByType<EnemyHealth>(0))
        {
            if (enemy.health <= 0 )
            {
                deadEnemies.Add(enemy.GetEnemyID()); // Save the ID of the dead enemy
                Debug.Log("Enemy " + enemy.GetEnemyID() + " is dead, saving state.");
            }
        }
    }
public void LoadCheckpoint()
    {
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;


        if (curr_level != currentSceneName) {
            curr_level = currentSceneName;
            checkpointPosition = Vector3.zero;
            deadEnemies = new HashSet<int>();
        }
        // Reload the current scene and wait for it to be loaded
        SceneManager.LoadScene(currentSceneName);

        // Subscribe to the sceneLoaded event to set the player position after the scene is reloaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    // Load the checkpoint and restore the state of dead enemies
   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the event to avoid multiple calls
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Start the coroutine to wait before setting the player position
        StartCoroutine(WaitForSceneToSettle());
    }

    // Coroutine to wait for a few seconds before performing actions
    private IEnumerator WaitForSceneToSettle()
{
    // Wait for a few seconds (adjust the time as needed)
    yield return new WaitForSeconds(0.2f); // Change 1.0f to whatever delay you need

    // Now the scene is fully loaded and we've waited a little longer
    if (checkpointPosition != Vector3.zero)
        {
            PlayerController.player.transform.position = checkpointPosition;
        }

        // Deactivate dead enemies based on the saved state
        foreach (var enemy in FindObjectsByType<EnemyHealth>(0))
        {
            if (deadEnemies.Contains(enemy.GetEnemyID()))
            {
                enemy.gameObject.SetActive(false); // Deactivate the dead enemy
            }
        }
}   
    // Utility to convert string back to Vector3
    private Vector3 StringToVector3(string str)
    {
        string[] parts = str.Split(',');
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float z = float.Parse(parts[2]);
        return new Vector3(x, y, z);
    }
}
