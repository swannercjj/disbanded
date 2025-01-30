using UnityEngine;
using UnityEngine.UI;

public class HideAndShow : MonoBehaviour
{
    public GameObject hide; // Assign in Inspector (object to hide)
    public GameObject show; // Assign in Inspector (object to show)
    public Button toggleButton; // Assign in Inspector (button to trigger the action)

    void Start()
    {
        // Add listener to the button
        toggleButton.onClick.AddListener(ToggleVisibility);
    }

    void ToggleVisibility()
    {
        // Hide the specified object
        if (hide != null)
            hide.SetActive(false);

        // Show the specified object
        if (show != null)
            show.SetActive(true);
    }
}
