using UnityEngine;
using UnityEngine.UI;

public class SensitivitySetter : MonoBehaviour
{
    public Slider sensitivitySlider; // Assign in Inspector

    private const string SensitivityKey = "MouseSensitivity";

    void Start()
    {
        // Load the saved sensitivity or use a default value (e.g., 2.0f)
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 2.0f);

        // Set the slider value
        sensitivitySlider.value = savedSensitivity;
        if (PlayerController.player){
            PlayerController.player.GetComponent<PlayerController>().mouseSensitivity = savedSensitivity;
        }

        // Add listener for when the slider value changes
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void SetSensitivity(float value)
    {

        // Save the new sensitivity setting
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        if (PlayerController.player){
            PlayerController.player.GetComponent<PlayerController>().mouseSensitivity = value;
        }
    }
}
