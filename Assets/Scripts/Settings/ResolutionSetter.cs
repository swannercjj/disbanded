using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionSetter : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Assign in Inspector
    public TMP_Dropdown screenModeDropdown; // Assign in Inspector for selecting screen mode
    private List<Resolution> resolutions;

    void Start()
    {
        // Get available screen resolutions
        resolutions = new List<Resolution>(Screen.resolutions);

        // Clear TMP Dropdown options
        resolutionDropdown.ClearOptions();

        // Create a list of resolution strings (e.g., "1920x1080")
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            resolutionOptions.Add(option);

            // Check if this resolution matches the current screen resolution
            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add options to TMP Dropdown
        resolutionDropdown.AddOptions(resolutionOptions);

        // Load last saved resolution or default to current
        int savedIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        // Add listener for when dropdown value changes
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Set the screen mode dropdown options
        List<string> screenModeOptions = new List<string> { "Fullscreen", "Windowed", "Fullscreen Windowed" };
        screenModeDropdown.AddOptions(screenModeOptions);

        // Load last saved screen mode or default to current
        int savedScreenMode = PlayerPrefs.GetInt("ScreenMode", (int)Screen.fullScreenMode);
        screenModeDropdown.value = savedScreenMode;
        screenModeDropdown.RefreshShownValue();

        // Add listener for when screen mode dropdown value changes
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
    }

    void SetResolution(int index)
    {
        // Get selected resolution
        Resolution selectedResolution = resolutions[index];

        // Apply new resolution (fullscreen enabled)
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);

        // Save the selected resolution index
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    void SetScreenMode(int index)
    {
        // Set the screen mode based on the selected value
        FullScreenMode mode = FullScreenMode.Windowed;

        switch (index)
        {
            case 0: // Fullscreen
                mode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // Windowed
                mode = FullScreenMode.Windowed;
                break;
            case 2: // Fullscreen Windowed
                mode = FullScreenMode.FullScreenWindow;
                break;
        }

        // Apply the selected screen mode
        Screen.fullScreenMode = mode;

        // Save the selected screen mode
        PlayerPrefs.SetInt("ScreenMode", index);
        PlayerPrefs.Save();
    }
}
