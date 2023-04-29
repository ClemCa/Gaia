using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour
{
	public Slider volumeSlider;
	public TMP_Dropdown qualityDropdown;
	public Toggle fullscreenToggle;

	private void Start()
	{
		// Load the current settings
		LoadSettings();
	}

	public void SaveSettings()
	{
		// Save the settings to PlayerPrefs
		PlayerPrefs.SetFloat("volume", volumeSlider.value);
		PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
		PlayerPrefs.SetInt("quality", qualityDropdown.value);

		// Apply the settings
		ApplySettings();
	}

	public void LoadSettings()
	{
		// Load the settings from PlayerPrefs
		float volume = PlayerPrefs.GetFloat("volume", 1f);
		bool fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
		int quality = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());

		// Update the UI elements
		volumeSlider.value = volume;
		fullscreenToggle.isOn = fullscreen;
		qualityDropdown.value = quality;

		// Apply the settings
		ApplySettings();
	}

	public void ApplySettings()
	{
		// Apply the settings
		AudioListener.volume = volumeSlider.value;
		Screen.fullScreen = fullscreenToggle.isOn;
		QualitySettings.SetQualityLevel(qualityDropdown.value, true);
	}
}
