using UnityEngine;
using UnityEngine.UI; // For accessing the Button component
using UnityEngine.SceneManagement; // For loading scenes by name

public class LevelSelect : MonoBehaviour
{
    public Button levelButton; // Assign in the Inspector
    public string levelName;

    void Start()
    {
        if (levelButton != null)
        {
            // Add listener to the button's onClick event
            levelButton.onClick.AddListener(LoadLevel);
        }
    }

    void LoadLevel()
    {

        // Load the scene with the same name as the button
        SceneManager.LoadScene(levelName);
    }
}
