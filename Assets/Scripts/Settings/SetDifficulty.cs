using UnityEngine;
using UnityEngine.UI; // Required for Button component

public class SetDifficulty : MonoBehaviour
{
    public int difficultyMaxHealth = 100; // Set this value for each difficulty
    public Button difficultyButton; // Assign in Inspector

    void Start()
    {
        // Ensure the button is linked and add listener to trigger the health change
        if (difficultyButton != null)
        {
            difficultyButton.onClick.AddListener(SetPlayerHealth);
        }
    }

    void SetPlayerHealth()
    {

        // Optionally, save the health value
        PlayerPrefs.SetInt("PlayerMaxHealth", difficultyMaxHealth);
        PlayerPrefs.Save();

    }
}
