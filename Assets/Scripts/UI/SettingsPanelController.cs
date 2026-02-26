using UnityEngine;
using UnityEngine.UI;
using LakbayTala.Config;

/// <summary>
/// Comprehensive settings panel controller handling game configuration,
/// audio settings, graphics options, and control preferences.
/// </summary>
public class SettingsPanelController : MonoBehaviour
{
    [Header("Settings References")]
    public GameSettings currentSettings;
    
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle audioMuteToggle;
    
    [Header("Graphics Settings")]
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Toggle vsyncToggle;
    
    [Header("Gameplay Settings")]
    public Slider movementSpeedSlider;
    public Slider jumpForceSlider;
    public Toggle autoSaveToggle;
    public Slider encounterRateSlider;
    
    [Header("Control Settings")]
    public Toggle showControlsToggle;
    public Slider controlSensitivitySlider;
    public Toggle vibrationToggle;
    
    [Header("UI References")]
    public Button applyButton;
    public Button resetButton;
    public Button closeButton;
    public Text statusText;
    
    private Resolution[] availableResolutions;
    private bool settingsChanged = false;
    
    void Start()
    {
        InitializeSettings();
        SetupUIListeners();
        RefreshUI();
    }
    
    void OnEnable()
    {
        RefreshUI();
        settingsChanged = false;
        UpdateApplyButtonState();
    }
    
    private void InitializeSettings()
    {
        if (currentSettings == null)
        {
            // Load from MasterGameManager if available
            if (MasterGameManager.Instance != null && MasterGameManager.Instance.Settings != null)
            {
                currentSettings = MasterGameManager.Instance.Settings;
            }
            else
            {
                // Create default settings
                currentSettings = new GameSettings();
            }
        }
        
        // Initialize available resolutions
        InitializeResolutions();
        
        // Load saved settings if available
        LoadSettings();
    }
    
    private void InitializeResolutions()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        var resolutionOptions = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution resolution = availableResolutions[i];
            string option = resolution.width + " x " + resolution.height + " @ " + resolution.refreshRateRatio + "Hz";
            resolutionOptions.Add(option);
            
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    private void SetupUIListeners()
    {
        // Audio listeners
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (audioMuteToggle != null)
            audioMuteToggle.onValueChanged.AddListener(OnAudioMuteChanged);
            
        // Graphics listeners
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            
        // Gameplay listeners
        if (movementSpeedSlider != null)
            movementSpeedSlider.onValueChanged.AddListener(OnMovementSpeedChanged);
        if (jumpForceSlider != null)
            jumpForceSlider.onValueChanged.AddListener(OnJumpForceChanged);
        if (autoSaveToggle != null)
            autoSaveToggle.onValueChanged.AddListener(OnAutoSaveChanged);
        if (encounterRateSlider != null)
            encounterRateSlider.onValueChanged.AddListener(OnEncounterRateChanged);
            
        // Control listeners
        if (showControlsToggle != null)
            showControlsToggle.onValueChanged.AddListener(OnShowControlsChanged);
        if (controlSensitivitySlider != null)
            controlSensitivitySlider.onValueChanged.AddListener(OnControlSensitivityChanged);
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            
        // Button listeners
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplySettings);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetSettings);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseSettings);
    }
    
    public void RefreshUI()
    {
        if (currentSettings == null) return;
        
        // Audio settings
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (audioMuteToggle != null)
            audioMuteToggle.isOn = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            
        // Graphics settings
        if (qualityDropdown != null)
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;
        if (vsyncToggle != null)
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            
        // Gameplay settings
        if (movementSpeedSlider != null)
            movementSpeedSlider.value = currentSettings.movementSpeed;
        if (jumpForceSlider != null)
            jumpForceSlider.value = currentSettings.jumpForce;
        if (autoSaveToggle != null)
            autoSaveToggle.isOn = PlayerPrefs.GetInt("AutoSave", 1) == 1;
        if (encounterRateSlider != null)
            encounterRateSlider.value = currentSettings.encounterRate;
            
        // Control settings
        if (showControlsToggle != null)
            showControlsToggle.isOn = PlayerPrefs.GetInt("ShowControls", 1) == 1;
        if (controlSensitivitySlider != null)
            controlSensitivitySlider.value = PlayerPrefs.GetFloat("ControlSensitivity", 1f);
        if (vibrationToggle != null)
            vibrationToggle.isOn = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
            
        settingsChanged = false;
        UpdateApplyButtonState();
    }
    
    // Audio setting handlers
    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnAudioMuteChanged(bool value)
    {
        PlayerPrefs.SetInt("AudioMuted", value ? 1 : 0);
        AudioListener.pause = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    // Graphics setting handlers
    private void OnQualityChanged(int value)
    {
        QualitySettings.SetQualityLevel(value);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnResolutionChanged(int value)
    {
        Resolution resolution = availableResolutions[value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnVSyncChanged(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    // Gameplay setting handlers
    private void OnMovementSpeedChanged(float value)
    {
        currentSettings.movementSpeed = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnJumpForceChanged(float value)
    {
        currentSettings.jumpForce = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnAutoSaveChanged(bool value)
    {
        PlayerPrefs.SetInt("AutoSave", value ? 1 : 0);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnEncounterRateChanged(float value)
    {
        currentSettings.encounterRate = value;
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    // Control setting handlers
    private void OnShowControlsChanged(bool value)
    {
        PlayerPrefs.SetInt("ShowControls", value ? 1 : 0);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnControlSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("ControlSensitivity", value);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    private void OnVibrationChanged(bool value)
    {
        PlayerPrefs.SetInt("VibrationEnabled", value ? 1 : 0);
        settingsChanged = true;
        UpdateApplyButtonState();
    }
    
    // Button handlers
    private void OnApplySettings()
    {
        ApplySettings();
        SaveSettings();
        UpdateStatusText("Settings Applied!");
        settingsChanged = false;
        UpdateApplyButtonState();
    }
    
    private void OnResetSettings()
    {
        ResetToDefaults();
        RefreshUI();
        UpdateStatusText("Settings Reset to Defaults");
    }
    
    private void OnCloseSettings()
    {
        if (settingsChanged)
        {
            // Optionally show confirmation dialog
            ApplySettings();
        }
        
        // Return to main menu
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }
    
    private void ApplySettings()
    {
        // Apply audio settings
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.pause = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
        
        // Apply graphics settings
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        
        // Apply gameplay settings to current session
        if (MasterGameManager.Instance != null && MasterGameManager.Instance.Settings != null)
        {
            MasterGameManager.Instance.Settings.movementSpeed = currentSettings.movementSpeed;
            MasterGameManager.Instance.Settings.jumpForce = currentSettings.jumpForce;
            MasterGameManager.Instance.Settings.encounterRate = currentSettings.encounterRate;
        }
        
        // Apply control settings
        // These are typically used by input controllers
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.Save();
        
        // Save current settings to JSON if path is available
        if (MasterGameManager.Instance != null)
        {
            // Settings are automatically saved through the GameManager
        }
    }
    
    public void LoadSettings()
    {
        // Settings are loaded through PlayerPrefs in RefreshUI()
    }
    
    public void ResetToDefaults()
    {
        // Reset to default GameSettings
        currentSettings = new GameSettings();
        
        // Reset PlayerPrefs
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
        PlayerPrefs.DeleteKey("AudioMuted");
        PlayerPrefs.DeleteKey("AutoSave");
        PlayerPrefs.DeleteKey("ShowControls");
        PlayerPrefs.DeleteKey("ControlSensitivity");
        PlayerPrefs.DeleteKey("VibrationEnabled");
        
        // Reset graphics settings
        QualitySettings.SetQualityLevel(2); // Medium quality
        Screen.fullScreen = true;
        QualitySettings.vSyncCount = 1;
        
        settingsChanged = true;
    }
    
    private void UpdateApplyButtonState()
    {
        if (applyButton != null)
            applyButton.interactable = settingsChanged;
    }
    
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            CancelInvoke(nameof(ClearStatusText));
            Invoke(nameof(ClearStatusText), 2f);
        }
    }
    
    private void ClearStatusText()
    {
        if (statusText != null)
            statusText.text = "";
    }
}