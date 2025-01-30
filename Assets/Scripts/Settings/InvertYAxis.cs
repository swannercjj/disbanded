using UnityEngine;
using UnityEngine.UI;

public class InvertYAxis : MonoBehaviour
{
    public Toggle invertToggle; // Assign this in the Inspector

    private void Start()
    {
        // Load the saved inversion setting (default is 1 for normal)
        int isInverted = PlayerPrefs.GetInt("MouseInverted", 1);
        
        // Set the toggle state based on the saved value
        invertToggle.isOn = (isInverted == -1);

        // Add a listener to handle toggle changes
        invertToggle.onValueChanged.AddListener(ToggleInversion);
    }

    private void ToggleInversion(bool isChecked)
    {
        // If checked, set inversion to -1; otherwise, set to 1
        int invertedValue = isChecked ? -1 : 1;

        // Save the new inversion setting
        PlayerPrefs.SetInt("MouseInverted", invertedValue);
        PlayerPrefs.Save();


        if (PlayerController.player){
            PlayerController.player.GetComponent<PlayerController>().is_inverted = invertedValue;
        }
    }
}
