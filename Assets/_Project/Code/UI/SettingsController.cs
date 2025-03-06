using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

namespace StarSurgeJourney.UI
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        
        [Header("Graphics")]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        
        [Header("Gameplay")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle invertYToggle;
        [SerializeField] private Toggle showHintsToggle;
        
        [Header("UI")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button defaultsButton;
        
        private Resolution[] resolutions;

        private float originalMasterVolume;
        private float originalMusicVolume;
        private float originalSfxVolume;
        private int originalQualityLevel;
        private bool originalFullscreen;
        private int originalResolutionIndex;
        private float originalSensitivity;
        private bool originalInvertY;
        private bool originalShowHints;
        
        private void Start()
        {
            SetupVolumeSliders();
            SetupQualityDropdown();
            SetupResolutionDropdown();
            SetupFullscreenToggle();
            SetupGameplayControls();
            
            if (applyButton != null)
                applyButton.onClick.AddListener(ApplySettings);
                
            if (cancelButton != null)
                cancelButton.onClick.AddListener(CancelChanges);
                
            if (defaultsButton != null)
                defaultsButton.onClick.AddListener(ResetToDefaults);
            
            SaveOriginalValues();
        }
        
        private void SetupVolumeSliders()
        {
            if (audioMixer == null) return;
            
            audioMixer.GetFloat("MasterVolume", out float masterVolume);
            audioMixer.GetFloat("MusicVolume", out float musicVolume);
            audioMixer.GetFloat("SFXVolume", out float sfxVolume);
            
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = ConvertToLinearValue(masterVolume);
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = ConvertToLinearValue(musicVolume);
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = ConvertToLinearValue(sfxVolume);
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            }
        }
        
        private void SetupQualityDropdown()
        {
            if (qualityDropdown == null) return;
            
            qualityDropdown.ClearOptions();
            
            List<string> qualityOptions = new List<string>(QualitySettings.names);
            qualityDropdown.AddOptions(qualityOptions);
            
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
        }
        
        private void SetupResolutionDropdown()
        {
            if (resolutionDropdown == null) return;
            
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio + "Hz";
                options.Add(option);
                
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                    //resolutions[i].refreshRateRatio == Screen.currentResolution.refreshRateRatio) to do Operator '==' cannot be applied to operands of type 'RefreshRate' and 'RefreshRate'
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        
        private void SetupFullscreenToggle()
        {
            if (fullscreenToggle == null) return;
            
            fullscreenToggle.isOn = Screen.fullScreen;
        }
        
        private void SetupGameplayControls()
        {
            if (sensitivitySlider != null)
            {
                float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
                sensitivitySlider.value = sensitivity;
            }
            
            if (invertYToggle != null)
            {
                bool invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
                invertYToggle.isOn = invertY;
            }
            
            if (showHintsToggle != null)
            {
                bool showHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
                showHintsToggle.isOn = showHints;
            }
        }
        
        
        private void SetMasterVolume(float value)
        {
            if (audioMixer == null) return;
            audioMixer.SetFloat("MasterVolume", ConvertToDBValue(value));
        }
        
        private void SetMusicVolume(float value)
        {
            if (audioMixer == null) return;
            audioMixer.SetFloat("MusicVolume", ConvertToDBValue(value));
        }
        
        private void SetSFXVolume(float value)
        {
            if (audioMixer == null) return;
            audioMixer.SetFloat("SFXVolume", ConvertToDBValue(value));
        }
        
        private float ConvertToDBValue(float linearValue)
        {
            return linearValue > 0.001f ? Mathf.Log10(linearValue) * 20 : -80f;
        }
        
        private float ConvertToLinearValue(float dbValue)
        {
            return Mathf.Pow(10, dbValue / 20);
        }     
        
        private void ApplySettings()
        {
            if (qualityDropdown != null)
                QualitySettings.SetQualityLevel(qualityDropdown.value);
                
            if (resolutionDropdown != null && resolutions != null && resolutions.Length > 0)
            {
                Resolution resolution = resolutions[resolutionDropdown.value];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
            
            if (fullscreenToggle != null)
                Screen.fullScreen = fullscreenToggle.isOn;
            
            if (sensitivitySlider != null)
                PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);
                
            if (invertYToggle != null)
                PlayerPrefs.SetInt("InvertY", invertYToggle.isOn ? 1 : 0);
                
            if (showHintsToggle != null)
                PlayerPrefs.SetInt("ShowHints", showHintsToggle.isOn ? 1 : 0);
            
            PlayerPrefs.Save();
            
            SaveOriginalValues();
            
            ShowConfirmationMessage("Settings Applied");
        }
        
        private void CancelChanges()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = originalMasterVolume;
                SetMasterVolume(originalMasterVolume);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = originalMusicVolume;
                SetMusicVolume(originalMusicVolume);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = originalSfxVolume;
                SetSFXVolume(originalSfxVolume);
            }
            
            if (qualityDropdown != null)
                qualityDropdown.value = originalQualityLevel;
                
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = originalFullscreen;
                
            if (resolutionDropdown != null)
                resolutionDropdown.value = originalResolutionIndex;
                
            if (sensitivitySlider != null)
                sensitivitySlider.value = originalSensitivity;
                
            if (invertYToggle != null)
                invertYToggle.isOn = originalInvertY;
                
            if (showHintsToggle != null)
                showHintsToggle.isOn = originalShowHints;
            
            gameObject.SetActive(false);
        }
        
        private void ResetToDefaults()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = 1.0f;
                SetMasterVolume(1.0f);
            }
            
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = 0.75f;
                SetMusicVolume(0.75f);
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = 0.75f;
                SetSFXVolume(0.75f);
            }
            
            if (qualityDropdown != null)
                qualityDropdown.value = 3;
                
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = true;
                
            if (sensitivitySlider != null)
                sensitivitySlider.value = 1.0f;
                
            if (invertYToggle != null)
                invertYToggle.isOn = false;
                
            if (showHintsToggle != null)
                showHintsToggle.isOn = true;
        }
        
        private void SaveOriginalValues()
        {
            if (audioMixer != null)
            {
                audioMixer.GetFloat("MasterVolume", out float masterVolume);
                audioMixer.GetFloat("MusicVolume", out float musicVolume);
                audioMixer.GetFloat("SFXVolume", out float sfxVolume);
                
                originalMasterVolume = ConvertToLinearValue(masterVolume);
                originalMusicVolume = ConvertToLinearValue(musicVolume);
                originalSfxVolume = ConvertToLinearValue(sfxVolume);
            }
            
            originalQualityLevel = QualitySettings.GetQualityLevel();
            originalFullscreen = Screen.fullScreen;
            originalResolutionIndex = resolutionDropdown != null ? resolutionDropdown.value : 0;
            
            originalSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
            originalInvertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
            originalShowHints = PlayerPrefs.GetInt("ShowHints", 1) == 1;
        }
        
        private void ShowConfirmationMessage(string message)
        {
            Debug.Log(message);
            
            // confirmationPanel.SetActive(true);
            // confirmationText.text = message;
            // Invoke("HideConfirmation", 2f);
        }
        
        // private void HideConfirmation()
        // {
        //     confirmationPanel.SetActive(false);
        // }
    }
}