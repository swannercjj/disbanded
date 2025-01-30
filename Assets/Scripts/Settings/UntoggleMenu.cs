using UnityEngine;
using UnityEngine.UI; // For accessing Button component

public class UntoggleMenu : MonoBehaviour
{
    public OpenSettings settings_script; // Reference to the OpenSettings script
    public Button toggleButton; // Reference to the button that will trigger the toggle

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure the button triggers the ToggleMenu function on click
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSettingsMenu);
        }
    }

    // Method to call ToggleMenu on the OpenSettings script
    void ToggleSettingsMenu()
    {
        // Call ToggleMenu from the OpenSettings script
        if (settings_script != null)
        {
            settings_script.ToggleMenu();
        }
    }
}
