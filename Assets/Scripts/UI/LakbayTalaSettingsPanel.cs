using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Enhanced settings panel with Filipino cultural integration and educational features.
/// Provides comprehensive settings for cultural learning, language preferences, and educational modes.
/// </summary>
public class LakbayTalaSettingsPanel : MonoBehaviour
{
    [Header("Cultural Settings")]
    [Tooltip("Enable Baybayin script display in UI")]
    public Toggle enableBaybayinToggle;
    [Tooltip("Enable cultural learning tips and tooltips")]
    public Toggle enableCulturalTipsToggle;
    [Tooltip("Enable traditional Filipino music and sounds")]
    public Toggle enableTraditionalAudioToggle;
    [Tooltip("Enable location-specific cultural themes")]
    public Toggle enableLocationThemesToggle;
    
    [Header("Language Settings")]
    [Tooltip("Language selection dropdown")]
    public Dropdown languageDropdown; // English, Filipino, Tagalog, Cebuano
    [Tooltip("Enable bilingual display (English + Filipino)")]
    public Toggle enableBilingualToggle;
    [Tooltip("Enable cultural context translations")]
    public Toggle enableCulturalTranslationsToggle;
    
    [Header("Educational Settings")]
    [Tooltip("Enable educational mode with learning objectives")]
    public Toggle enableEducationalModeToggle;
    [Tooltip("Enable teacher tools and classroom features")]
    public Toggle enableTeacherModeToggle;
    [Tooltip("Enable cultural quiz and assessment features")]
    public Toggle enableCulturalQuizToggle;
    [Tooltip("Enable progress tracking for cultural learning")]
    public Toggle enableProgressTrackingToggle;
    [Tooltip("Enable accessibility features for inclusive design")]
    public Toggle enableAccessibilityToggle;
    
    [Header("Audio Settings")]
    [Tooltip("Master volume slider")]
    public Slider masterVolumeSlider;
    [Tooltip("Music volume slider")]
    public Slider musicVolumeSlider;
    [Tooltip("Sound effects volume slider")]
    public Slider sfxVolumeSlider;
    [Tooltip("Cultural audio volume slider")]
    public Slider culturalAudioSlider;
    [Tooltip("Voice over volume slider")]
    public Slider voiceOverSlider;
    
    [Header("Visual Settings")]
    [Tooltip("Graphics quality dropdown")]
    public Dropdown graphicsQualityDropdown;
    [Tooltip("Enable traditional Filipino color palette")]
    public Toggle enableTraditionalColorsToggle;
    [Tooltip("Enable cultural pattern overlays")]
    public Toggle enablePatternOverlaysToggle;
    [Tooltip("Enable animation effects")]
    public Toggle enableAnimationEffectsToggle;
    [Tooltip("Enable particle effects")]
    public Toggle enableParticleEffectsToggle;
    
    [Header("Gameplay Settings")]
    [Tooltip("Enable checkpoint system")]
    public Toggle enableCheckpointToggle;
    [Tooltip("Enable save/load functionality")]
    public Toggle enableSaveLoadToggle;
    [Tooltip("Enable cultural artifact collection")]
    public Toggle enableArtifactCollectionToggle;
    [Tooltip("Enable mythological creature encounters")]
    public Toggle enableCreatureEncountersToggle;
    [Tooltip("Enable location discovery system")]
    public Toggle enableLocationDiscoveryToggle;
    
    [Header("UI References")]
    public Text titleText;
    public Text subtitleText;
    public Text baybayinTitleText;
    public Button saveButton;
    public Button resetButton;
    public Button closeButton;
    public Button applyButton;
    public Text saveStatusText;
    public GameObject settingsContainer;
    public GameObject culturalSettingsPanel;
    public GameObject educationalSettingsPanel;
    public GameObject audioSettingsPanel;
    
    public static LakbayTalaSettingsPanel Instance { get; private set; }
    public GameObject visualSettingsPanel;
    public GameObject gameplaySettingsPanel;
    
    [Header("Cultural UI Elements")]
    public Image backgroundPattern;
    public Image sideDecorations;
    public GameObject culturalAnimationContainer;
    public AudioSource culturalAudioSource;
    
    private LakbayTalaUITheme uiTheme;
    private bool hasUnsavedChanges = false;
    private Dictionary<string, object> originalSettings = new Dictionary<string, object>();
    
    // Language translations
    private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>()
    {
        {
            "English", new Dictionary<string, string>
            {
                {"Settings", "Settings"},
                {"Cultural Settings", "Cultural Settings"},
                {"Language Settings", "Language Settings"},
                {"Educational Settings", "Educational Settings"},
                {"Audio Settings", "Audio Settings"},
                {"Visual Settings", "Visual Settings"},
                {"Gameplay Settings", "Gameplay Settings"},
                {"Enable Baybayin", "Enable Baybayin Script"},
                {"Cultural Tips", "Show Cultural Tips"},
                {"Traditional Audio", "Traditional Filipino Audio"},
                {"Location Themes", "Location-Specific Themes"},
                {"Educational Mode", "Educational Learning Mode"},
                {"Teacher Mode", "Teacher Tools Mode"},
                {"Cultural Quiz", "Cultural Quiz System"},
                {"Progress Tracking", "Track Learning Progress"},
                {"Accessibility", "Accessibility Features"},
                {"Save Settings", "Save Settings"},
                {"Reset Settings", "Reset to Defaults"},
                {"Close", "Close"},
                {"Apply", "Apply Changes"},
                {"Settings Saved", "Settings saved successfully!"},
                {"Settings Reset", "Settings reset to defaults"}
            }
        },
        {
            "Filipino", new Dictionary<string, string>
            {
                {"Settings", "Mga Setting"},
                {"Cultural Settings", "Mga Setting ng Kultura"},
                {"Language Settings", "Mga Setting ng Wika"},
                {"Educational Settings", "Mga Setting ng Edukasyon"},
                {"Audio Settings", "Mga Setting ng Tunog"},
                {"Visual Settings", "Mga Setting ng Tanaw"},
                {"Gameplay Settings", "Mga Setting ng Laro"},
                {"Enable Baybayin", "Paganahin ang Baybayin"},
                {"Cultural Tips", "Ipakita ang mga Tip sa Kultura"},
                {"Traditional Audio", "Tradisyonal na Tunog na Filipino"},
                {"Location Themes", "Tema ayon sa Lokasyon"},
                {"Educational Mode", "Mode ng Edukasyonal na Pag-aaral"},
                {"Teacher Mode", "Mode ng mga Kasangkapan ng Guro"},
                {"Cultural Quiz", "Sistema ng Kultura na Pagsusulit"},
                {"Progress Tracking", "Subaybayan ang Progreso sa Pag-aaral"},
                {"Accessibility", "Mga Tampok na Pang-aksesibilidad"},
                {"Save Settings", "I-save ang mga Setting"},
                {"Reset Settings", "I-reset sa Default"},
                {"Close", "Isara"},
                {"Apply", "Ilapat ang mga Pagbabago"},
                {"Settings Saved", "Matagumpay na na-save ang mga setting!"},
                {"Settings Reset", "Na-reset ang mga setting sa default"}
            }
        }
    };

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        InitializeSettingsPanel();
        LoadCurrentSettings();
        SetupUIElements();
        ApplyCulturalTheme();
        SetupEventListeners();
    }
    
    void OnEnable()
    {
        RefreshUI();
        hasUnsavedChanges = false;
    }
    
    private void InitializeSettingsPanel()
    {
        // Get UI theme reference
        uiTheme = LakbayTalaUITheme.Instance;
        
        // Setup language dropdown
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new List<string>(translations.Keys));
            
            // Set current language
            string currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "English");
            int languageIndex = translations.Keys.ToList().IndexOf(currentLanguage);
            if (languageIndex >= 0)
            {
                languageDropdown.value = languageIndex;
            }
        }
        
        // Setup graphics quality dropdown
        if (graphicsQualityDropdown != null)
        {
            graphicsQualityDropdown.ClearOptions();
            graphicsQualityDropdown.AddOptions(new List<string> { "Low", "Medium", "High", "Ultra" });
            graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        }
        
        // Load original settings for comparison
        SaveOriginalSettings();
    }
    
    private void LoadCurrentSettings()
    {
        // Load cultural settings
        if (enableBaybayinToggle != null)
            enableBaybayinToggle.isOn = PlayerPrefs.GetInt("EnableBaybayin", 1) == 1;
        
        if (enableCulturalTipsToggle != null)
            enableCulturalTipsToggle.isOn = PlayerPrefs.GetInt("EnableCulturalTips", 1) == 1;
        
        if (enableTraditionalAudioToggle != null)
            enableTraditionalAudioToggle.isOn = PlayerPrefs.GetInt("EnableTraditionalAudio", 1) == 1;
        
        if (enableLocationThemesToggle != null)
            enableLocationThemesToggle.isOn = PlayerPrefs.GetInt("EnableLocationThemes", 1) == 1;
        
        // Load language settings
        if (enableBilingualToggle != null)
            enableBilingualToggle.isOn = PlayerPrefs.GetInt("EnableBilingual", 1) == 1;
        
        if (enableCulturalTranslationsToggle != null)
            enableCulturalTranslationsToggle.isOn = PlayerPrefs.GetInt("EnableCulturalTranslations", 1) == 1;
        
        // Load educational settings
        if (enableEducationalModeToggle != null)
            enableEducationalModeToggle.isOn = PlayerPrefs.GetInt("EnableEducationalMode", 1) == 1;
        
        if (enableTeacherModeToggle != null)
            enableTeacherModeToggle.isOn = PlayerPrefs.GetInt("EnableTeacherMode", 0) == 1;
        
        if (enableCulturalQuizToggle != null)
            enableCulturalQuizToggle.isOn = PlayerPrefs.GetInt("EnableCulturalQuiz", 1) == 1;
        
        if (enableProgressTrackingToggle != null)
            enableProgressTrackingToggle.isOn = PlayerPrefs.GetInt("EnableProgressTracking", 1) == 1;
        
        if (enableAccessibilityToggle != null)
            enableAccessibilityToggle.isOn = PlayerPrefs.GetInt("EnableAccessibility", 1) == 1;
        
        // Load audio settings
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        if (culturalAudioSlider != null)
            culturalAudioSlider.value = PlayerPrefs.GetFloat("CulturalAudioVolume", 0.6f);
        
        if (voiceOverSlider != null)
            voiceOverSlider.value = PlayerPrefs.GetFloat("VoiceOverVolume", 0.7f);
        
        // Load visual settings
        if (graphicsQualityDropdown != null)
            graphicsQualityDropdown.value = PlayerPrefs.GetInt("GraphicsQuality", 2);
        
        if (enableTraditionalColorsToggle != null)
            enableTraditionalColorsToggle.isOn = PlayerPrefs.GetInt("EnableTraditionalColors", 1) == 1;
        
        if (enablePatternOverlaysToggle != null)
            enablePatternOverlaysToggle.isOn = PlayerPrefs.GetInt("EnablePatternOverlays", 1) == 1;
        
        if (enableAnimationEffectsToggle != null)
            enableAnimationEffectsToggle.isOn = PlayerPrefs.GetInt("EnableAnimationEffects", 1) == 1;
        
        if (enableParticleEffectsToggle != null)
            enableParticleEffectsToggle.isOn = PlayerPrefs.GetInt("EnableParticleEffects", 1) == 1;
        
        // Load gameplay settings
        if (enableCheckpointToggle != null)
            enableCheckpointToggle.isOn = PlayerPrefs.GetInt("EnableCheckpoint", 1) == 1;
        
        if (enableSaveLoadToggle != null)
            enableSaveLoadToggle.isOn = PlayerPrefs.GetInt("EnableSaveLoad", 1) == 1;
        
        if (enableArtifactCollectionToggle != null)
            enableArtifactCollectionToggle.isOn = PlayerPrefs.GetInt("EnableArtifactCollection", 1) == 1;
        
        if (enableCreatureEncountersToggle != null)
            enableCreatureEncountersToggle.isOn = PlayerPrefs.GetInt("EnableCreatureEncounters", 1) == 1;
        
        if (enableLocationDiscoveryToggle != null)
            enableLocationDiscoveryToggle.isOn = PlayerPrefs.GetInt("EnableLocationDiscovery", 1) == 1;
    }
    
    private void SetupUIElements()
    {
        // Setup title with cultural elements
        if (titleText != null)
        {
            titleText.text = GetLocalizedText("Settings");
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = "Customize your LakbayTala experience";
        }
        
        if (baybayinTitleText != null)
        {
            baybayinTitleText.text = "ᜋᜅ ᜐᜒᜆᜒᜅ᜔";
            baybayinTitleText.gameObject.SetActive(PlayerPrefs.GetInt("EnableBaybayin", 1) == 1);
        }
        
        // Setup cultural banner
        if (backgroundPattern != null && uiTheme != null)
        {
            backgroundPattern.color = uiTheme.backgroundColor;
        }
        
        if (sideDecorations != null && uiTheme != null)
        {
            sideDecorations.color = uiTheme.accentColor;
        }
        
        // Setup button listeners
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveSettings);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetSettings);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseSettings);
        
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplySettings);
        
        // Setup toggle listeners for change detection
        SetupToggleListeners();
        
        // Setup slider listeners
        SetupSliderListeners();
        
        // Setup dropdown listeners
        SetupDropdownListeners();
    }
    
    private void ApplyCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply theme to UI elements
        var images = GetComponentsInChildren<Image>();
        var texts = GetComponentsInChildren<Text>();
        
        foreach (var image in images)
        {
            if (image.gameObject.name.Contains("Background"))
            {
                image.color = uiTheme.backgroundColor;
            }
            else if (image.gameObject.name.Contains("Primary"))
            {
                image.color = uiTheme.primaryColor;
            }
            else if (image.gameObject.name.Contains("Accent"))
            {
                image.color = uiTheme.accentColor;
            }
        }
        
        foreach (var text in texts)
        {
            text.color = uiTheme.textColor;
            if (PlayerPrefs.GetInt("EnableBaybayin", 1) == 1 && text.gameObject.name.Contains("Baybayin"))
            {
                text.font = uiTheme.baybayinFont;
            }
        }
    }
    
    private void SetupEventListeners()
    {
        // Add listeners for all UI elements to detect changes
        // This would be implemented based on specific UI framework
    }
    
    private void SetupToggleListeners()
    {
        Toggle[] allToggles = GetComponentsInChildren<Toggle>();
        foreach (Toggle toggle in allToggles)
        {
            toggle.onValueChanged.AddListener((value) => OnSettingsChanged());
        }
    }
    
    private void SetupSliderListeners()
    {
        Slider[] allSliders = GetComponentsInChildren<Slider>();
        foreach (Slider slider in allSliders)
        {
            slider.onValueChanged.AddListener((value) => OnSettingsChanged());
        }
    }
    
    private void SetupDropdownListeners()
    {
        Dropdown[] allDropdowns = GetComponentsInChildren<Dropdown>();
        foreach (Dropdown dropdown in allDropdowns)
        {
            dropdown.onValueChanged.AddListener((value) => OnSettingsChanged());
        }
    }
    
    private void OnSettingsChanged()
    {
        hasUnsavedChanges = true;
        if (saveStatusText != null)
        {
            saveStatusText.text = "* Unsaved changes";
            saveStatusText.color = Color.yellow;
        }
    }
    
    private void OnSaveSettings()
    {
        SaveAllSettings();
        ApplySettings();
        
        if (saveStatusText != null)
        {
            saveStatusText.text = GetLocalizedText("Settings Saved");
            saveStatusText.color = Color.green;
        }
        
        hasUnsavedChanges = false;
        
        // Play cultural sound
        PlayCulturalSound("save");
    }
    
    private void OnApplySettings()
    {
        SaveAllSettings();
        ApplySettings();
        
        if (saveStatusText != null)
        {
            saveStatusText.text = GetLocalizedText("Settings Saved");
            saveStatusText.color = Color.green;
        }
        
        hasUnsavedChanges = false;
        
        // Play cultural sound
        PlayCulturalSound("apply");
    }
    
    private void OnResetSettings()
    {
        ResetToDefaults();
        
        if (saveStatusText != null)
        {
            saveStatusText.text = GetLocalizedText("Settings Reset");
            saveStatusText.color = Color.white;
        }
        
        hasUnsavedChanges = false;
        
        // Play cultural sound
        PlayCulturalSound("reset");
    }
    
    private void OnCloseSettings()
    {
        if (hasUnsavedChanges)
        {
            // Show confirmation dialog
            ShowUnsavedChangesDialog();
        }
        else
        {
            CloseSettingsPanel();
        }
    }
    
    private void SaveAllSettings()
    {
        // Save cultural settings
        if (enableBaybayinToggle != null)
            PlayerPrefs.SetInt("EnableBaybayin", enableBaybayinToggle.isOn ? 1 : 0);
        
        if (enableCulturalTipsToggle != null)
            PlayerPrefs.SetInt("EnableCulturalTips", enableCulturalTipsToggle.isOn ? 1 : 0);
        
        if (enableTraditionalAudioToggle != null)
            PlayerPrefs.SetInt("EnableTraditionalAudio", enableTraditionalAudioToggle.isOn ? 1 : 0);
        
        if (enableLocationThemesToggle != null)
            PlayerPrefs.SetInt("EnableLocationThemes", enableLocationThemesToggle.isOn ? 1 : 0);
        
        // Save language settings
        if (languageDropdown != null)
        {
            string selectedLanguage = languageDropdown.options[languageDropdown.value].text;
            PlayerPrefs.SetString("CurrentLanguage", selectedLanguage);
        }
        
        if (enableBilingualToggle != null)
            PlayerPrefs.SetInt("EnableBilingual", enableBilingualToggle.isOn ? 1 : 0);
        
        if (enableCulturalTranslationsToggle != null)
            PlayerPrefs.SetInt("EnableCulturalTranslations", enableCulturalTranslationsToggle.isOn ? 1 : 0);
        
        // Save educational settings
        if (enableEducationalModeToggle != null)
            PlayerPrefs.SetInt("EnableEducationalMode", enableEducationalModeToggle.isOn ? 1 : 0);
        
        if (enableTeacherModeToggle != null)
            PlayerPrefs.SetInt("EnableTeacherMode", enableTeacherModeToggle.isOn ? 1 : 0);
        
        if (enableCulturalQuizToggle != null)
            PlayerPrefs.SetInt("EnableCulturalQuiz", enableCulturalQuizToggle.isOn ? 1 : 0);
        
        if (enableProgressTrackingToggle != null)
            PlayerPrefs.SetInt("EnableProgressTracking", enableProgressTrackingToggle.isOn ? 1 : 0);
        
        if (enableAccessibilityToggle != null)
            PlayerPrefs.SetInt("EnableAccessibility", enableAccessibilityToggle.isOn ? 1 : 0);
        
        // Save audio settings
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        
        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        
        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        
        if (culturalAudioSlider != null)
            PlayerPrefs.SetFloat("CulturalAudioVolume", culturalAudioSlider.value);
        
        if (voiceOverSlider != null)
            PlayerPrefs.SetFloat("VoiceOverVolume", voiceOverSlider.value);
        
        // Save visual settings
        if (graphicsQualityDropdown != null)
            PlayerPrefs.SetInt("GraphicsQuality", graphicsQualityDropdown.value);
        
        if (enableTraditionalColorsToggle != null)
            PlayerPrefs.SetInt("EnableTraditionalColors", enableTraditionalColorsToggle.isOn ? 1 : 0);
        
        if (enablePatternOverlaysToggle != null)
            PlayerPrefs.SetInt("EnablePatternOverlays", enablePatternOverlaysToggle.isOn ? 1 : 0);
        
        if (enableAnimationEffectsToggle != null)
            PlayerPrefs.SetInt("EnableAnimationEffects", enableAnimationEffectsToggle.isOn ? 1 : 0);
        
        if (enableParticleEffectsToggle != null)
            PlayerPrefs.SetInt("EnableParticleEffects", enableParticleEffectsToggle.isOn ? 1 : 0);
        
        // Save gameplay settings
        if (enableCheckpointToggle != null)
            PlayerPrefs.SetInt("EnableCheckpoint", enableCheckpointToggle.isOn ? 1 : 0);
        
        if (enableSaveLoadToggle != null)
            PlayerPrefs.SetInt("EnableSaveLoad", enableSaveLoadToggle.isOn ? 1 : 0);
        
        if (enableArtifactCollectionToggle != null)
            PlayerPrefs.SetInt("EnableArtifactCollection", enableArtifactCollectionToggle.isOn ? 1 : 0);
        
        if (enableCreatureEncountersToggle != null)
            PlayerPrefs.SetInt("EnableCreatureEncounters", enableCreatureEncountersToggle.isOn ? 1 : 0);
        
        if (enableLocationDiscoveryToggle != null)
            PlayerPrefs.SetInt("EnableLocationDiscovery", enableLocationDiscoveryToggle.isOn ? 1 : 0);
        
        // Force save
        PlayerPrefs.Save();
    }
    
    private void ApplySettings()
    {
        // Apply audio settings (AudioManager optional — implement when available)
        // if (AudioManager.Instance != null) { ... }
        
        // Apply graphics quality
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsQuality", 2));
        
        // Apply cultural theme
        if (uiTheme != null)
        {
            uiTheme.enableBaybayin = PlayerPrefs.GetInt("EnableBaybayin", 1) == 1;
            uiTheme.enableCulturalTooltips = PlayerPrefs.GetInt("EnableCulturalTips", 1) == 1;
            uiTheme.currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "English");
            uiTheme.highContrastMode = PlayerPrefs.GetInt("EnableAccessibility", 1) == 1;
            
            // uiTheme.InitializeTheme(); // internal — use public theme API if needed
        }
        
        // Apply educational settings
        if (LakbayTalaEnhancedIntegration.Instance != null)
        {
            LakbayTalaEnhancedIntegration.Instance.enableEducationalMode = PlayerPrefs.GetInt("EnableEducationalMode", 1) == 1;
            LakbayTalaEnhancedIntegration.Instance.enableTeacherMode = PlayerPrefs.GetInt("EnableTeacherMode", 0) == 1;
            LakbayTalaEnhancedIntegration.Instance.currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "English");
        }
        
        // Refresh UI
        RefreshUI();
    }
    
    private void ResetToDefaults()
    {
        // Reset to default values
        if (enableBaybayinToggle != null) enableBaybayinToggle.isOn = true;
        if (enableCulturalTipsToggle != null) enableCulturalTipsToggle.isOn = true;
        if (enableTraditionalAudioToggle != null) enableTraditionalAudioToggle.isOn = true;
        if (enableLocationThemesToggle != null) enableLocationThemesToggle.isOn = true;
        
        if (enableBilingualToggle != null) enableBilingualToggle.isOn = true;
        if (enableCulturalTranslationsToggle != null) enableCulturalTranslationsToggle.isOn = true;
        
        if (enableEducationalModeToggle != null) enableEducationalModeToggle.isOn = true;
        if (enableTeacherModeToggle != null) enableTeacherModeToggle.isOn = false;
        if (enableCulturalQuizToggle != null) enableCulturalQuizToggle.isOn = true;
        if (enableProgressTrackingToggle != null) enableProgressTrackingToggle.isOn = true;
        if (enableAccessibilityToggle != null) enableAccessibilityToggle.isOn = true;
        
        if (masterVolumeSlider != null) masterVolumeSlider.value = 1.0f;
        if (musicVolumeSlider != null) musicVolumeSlider.value = 0.8f;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = 0.8f;
        if (culturalAudioSlider != null) culturalAudioSlider.value = 0.6f;
        if (voiceOverSlider != null) voiceOverSlider.value = 0.7f;
        
        if (graphicsQualityDropdown != null) graphicsQualityDropdown.value = 2;
        if (enableTraditionalColorsToggle != null) enableTraditionalColorsToggle.isOn = true;
        if (enablePatternOverlaysToggle != null) enablePatternOverlaysToggle.isOn = true;
        if (enableAnimationEffectsToggle != null) enableAnimationEffectsToggle.isOn = true;
        if (enableParticleEffectsToggle != null) enableParticleEffectsToggle.isOn = true;
        
        if (enableCheckpointToggle != null) enableCheckpointToggle.isOn = true;
        if (enableSaveLoadToggle != null) enableSaveLoadToggle.isOn = true;
        if (enableArtifactCollectionToggle != null) enableArtifactCollectionToggle.isOn = true;
        if (enableCreatureEncountersToggle != null) enableCreatureEncountersToggle.isOn = true;
        if (enableLocationDiscoveryToggle != null) enableLocationDiscoveryToggle.isOn = true;
        
        if (languageDropdown != null)
        {
            int englishIndex = languageDropdown.options.FindIndex(option => option.text == "English");
            if (englishIndex >= 0)
                languageDropdown.value = englishIndex;
        }
    }
    
    private void RefreshUI()
    {
        // Refresh all UI elements based on current settings
        string currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "English");
        
        // Update text elements
        if (titleText != null)
            titleText.text = GetLocalizedText("Settings");
        
        // Update Baybayin text visibility
        if (baybayinTitleText != null)
        {
            baybayinTitleText.gameObject.SetActive(PlayerPrefs.GetInt("EnableBaybayin", 1) == 1);
        }
        
        // Apply cultural theme
        ApplyCulturalTheme();
    }
    
    private string GetLocalizedText(string key)
    {
        string currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "English");
        
        if (translations.ContainsKey(currentLanguage) && translations[currentLanguage].ContainsKey(key))
        {
            return translations[currentLanguage][key];
        }
        
        return key; // Return key if translation not found
    }
    
    private void PlayCulturalSound(string soundType)
    {
        if (culturalAudioSource != null && uiTheme != null && uiTheme.kulintangSounds != null && uiTheme.kulintangSounds.Length > 0)
        {
            AudioClip clip = uiTheme.kulintangSounds[Random.Range(0, uiTheme.kulintangSounds.Length)];
            culturalAudioSource.clip = clip;
            culturalAudioSource.volume = PlayerPrefs.GetFloat("CulturalAudioVolume", 0.6f);
            culturalAudioSource.Play();
        }
    }
    
    private void ShowUnsavedChangesDialog()
    {
        // Implementation for showing unsaved changes dialog
        // This would show a confirmation dialog asking if user wants to save changes
        Debug.Log("Showing unsaved changes dialog");
    }
    
    private void CloseSettingsPanel()
    {
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }
    
    private void SaveOriginalSettings()
    {
        // Save original settings for comparison
        originalSettings.Clear();
        
        // This would save all current settings to compare against changes
        // Implementation depends on specific requirements
    }
    
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (hasUnsavedChanges)
        {
            // Optionally save settings on destroy
            // SaveAllSettings();
        }
    }
}