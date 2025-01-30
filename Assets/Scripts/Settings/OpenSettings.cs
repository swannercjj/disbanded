using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject settingsMenu; // Assign the settings menu GameObject in the Inspector
    private bool isMenuOpen = false; // Track the state of the settings menu

    void Start()
    {
        // Initially hide the settings menu and lock the cursor at the start
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false); // Ensure it starts off as hidden
        }

        // Hide the cursor at the start (game should control cursor visibility)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Check if the Escape key is pressed to toggle the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu(); // Call the ToggleMenu function when Escape is pressed
        }
    }

    // Function to toggle the settings menu and cursor visibility/lock state
    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen; // Toggle the menu state

        if (settingsMenu != null)
        {
            // Toggle the settings menu visibility based on isMenuOpen
            settingsMenu.SetActive(isMenuOpen);
        }

        // Toggle mouse cursor visibility and lock state based on isMenuOpen
        if (isMenuOpen)
        {
            // When menu is open, show the cursor and unlock it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlayerController.player.GetComponent<PlayerController>().super_froze = true;
            PlayerController.player.GetComponent<PlayerShoot>().frozen = true;
            PlayerController.player.GetComponent<PlayerDash>().frozen = true;
        }
        else
        {
            // When menu is closed, lock the cursor and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            PlayerController.player.GetComponent<PlayerController>().super_froze = false;
            PlayerController.player.GetComponent<PlayerShoot>().frozen = false;
            PlayerController.player.GetComponent<PlayerDash>().frozen = false;
        }
    }
}
