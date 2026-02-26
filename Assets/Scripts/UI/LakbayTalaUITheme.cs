using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Comprehensive Filipino cultural UI theme system for LakbayTala.
/// Manages traditional colors, patterns, fonts, and cultural elements including Baybayin script.
/// </summary>
public class LakbayTalaUITheme : MonoBehaviour
{
    [Header("Filipino Cultural Colors")]
    [Tooltip("Primary color inspired by Philippine flag and golden sunsets")]
    public Color primaryColor = new Color(0.9f, 0.7f, 0.3f);      // Golden yellow
    [Tooltip("Secondary color representing Philippine seas and skies")]
    public Color secondaryColor = new Color(0.2f, 0.4f, 0.6f);   // Deep blue
    [Tooltip("Accent color from traditional Filipino textiles")]
    public Color accentColor = new Color(0.8f, 0.3f, 0.2f);     // Warm red
    [Tooltip("Background color inspired by native paper and bamboo")]
    public Color backgroundColor = new Color(0.95f, 0.92f, 0.88f); // Cream
    [Tooltip("Text color for readability on light backgrounds")]
    public Color textColor = new Color(0.2f, 0.15f, 0.1f);     // Dark brown
    
    [Header("Traditional Filipino Patterns")]
    [Tooltip("Enable traditional weaving patterns in UI elements")]
    public bool enableTraditionalPatterns = true;
    [Tooltip("Pattern intensity (0-1) for subtle cultural elements")]
    public float patternIntensity = 0.3f;
    [Tooltip("Weaving pattern texture for backgrounds")]
    public Texture2D weavingPattern;
    [Tooltip("Bamboo texture for natural elements")]
    public Texture2D bambooTexture;
    
    [Header("Baybayin Script Integration")]
    [Tooltip("Enable ancient Filipino Baybayin script in UI")]
    public bool enableBaybayin = true;
    [Tooltip("Baybayin font for traditional text")]
    public Font baybayinFont;
    [Tooltip("Modern Filipino font for readability")]
    public Font modernFilipinoFont;
    [Tooltip("Show Baybayin alongside modern text")]
    public bool showBaybayinWithModern = true;
    
    [Header("Mythological Elements")]
    [Tooltip("Color palette for Diwata (forest spirits)")]
    public Color diwataColor = new Color(0.4f, 0.7f, 0.4f);      // Forest green
    [Tooltip("Color palette for Water Spirits")]
    public Color waterSpiritColor = new Color(0.3f, 0.5f, 0.8f);  // Water blue
    [Tooltip("Color palette for Mountain Guardians")]
    public Color mountainGuardianColor = new Color(0.6f, 0.4f, 0.3f); // Earth brown
    [Tooltip("Color palette for Lake Spirits")]
    public Color lakeSpiritColor = new Color(0.2f, 0.6f, 0.7f);    // Lake teal
    
    [Header("Laguna-Specific Elements")]
    [Tooltip("Mount Makiling theme colors")]
    public Color mountMakilingColor = new Color(0.3f, 0.5f, 0.2f);   // Forest green
    [Tooltip("Lake Mohikap theme colors")]
    public Color lakeMohikapColor = new Color(0.2f, 0.4f, 0.6f);     // Deep lake blue
    [Tooltip("Sampaloc Lake theme colors")]
    public Color sampalocLakeColor = new Color(0.4f, 0.6f, 0.7f);   // Crater lake teal
    [Tooltip("Botocan Falls theme colors")]
    public Color botocanFallsColor = new Color(0.5f, 0.7f, 0.9f);    // Misty waterfall
    
    [Header("Cultural Sound Design")]
    [Tooltip("Traditional Filipino instruments sound effects")]
    public AudioClip[] kulintangSounds;
    [Tooltip("Bamboo instrument sounds")]
    public AudioClip[] bambooSounds;
    [Tooltip("Nature sounds from Philippine forests")]
    public AudioClip[] forestSounds;
    [Tooltip("Water sounds from Philippine lakes and falls")]
    public AudioClip[] waterSounds;
    
    [Header("Educational Integration")]
    [Tooltip("Enable cultural learning tooltips")]
    public bool enableCulturalTooltips = true;
    [Tooltip("Show cultural context for UI elements")]
    public bool showCulturalContext = true;
    [Tooltip("Language options: Filipino, Tagalog, English, Cebuano")]
    public string currentLanguage = "Filipino";
    [Tooltip("Enable traditional story introductions")]
    public bool enableStoryIntroductions = true;
    
    [Header("Accessibility Features")]
    [Tooltip("High contrast mode for better visibility")]
    public bool highContrastMode = false;
    [Tooltip("Large text mode for readability")]
    public bool largeTextMode = false;
    [Tooltip("Color blind friendly palette")]
    public bool colorBlindFriendly = false;
    
    // Baybayin character mappings
    private Dictionary<string, string> baybayinMap = new Dictionary<string, string>
    {
        {"A", "ᜀ"}, {"BA", "ᜊ"}, {"KA", "ᜃ"}, {"DA", "ᜇ"}, {"GA", "ᜄ"},
        {"HA", "ᜑ"}, {"LA", "ᜎ"}, {"MA", "ᜋ"}, {"NA", "ᜈ"}, {"NGA", "ᜅ"},
        {"PA", "ᜉ"}, {"SA", "ᜐ"}, {"TA", "ᜆ"}, {"WA", "ᜏ"}, {"YA", "ᜌ"},
        {"I", "ᜁ"}, {"U", "ᜂ"}
    };
    
    // Cultural context database
    private Dictionary<string, string> culturalContext = new Dictionary<string, string>
    {
        {"primaryColor", "Inspired by the golden sunsets over Philippine seas and the warmth of Filipino hospitality"},
        {"secondaryColor", "Represents the deep blue waters surrounding the Philippine archipelago"},
        {"accentColor", "From the vibrant reds found in traditional Filipino textiles and celebrations"},
        {"diwataColor", "The gentle green of forest spirits who protect Mount Makiling's flora and fauna"},
        {"waterSpiritColor", "The mystical blue of lake and river spirits in Filipino mythology"},
        {"mountainGuardianColor", "The earthy brown of mountain spirits who watch over sacred peaks"},
        {"lakeSpiritColor", "The serene teal of crater lakes like Sampaloc Lake in Laguna"}
    };
    
    private static LakbayTalaUITheme instance;
    public static LakbayTalaUITheme Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<LakbayTalaUITheme>();
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTheme();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initialize the Filipino cultural theme system.
    /// </summary>
    private void InitializeTheme()
    {
        ApplyThemeColors();
        SetupTraditionalPatterns();
        ConfigureBaybayinSystem();
        LoadCulturalSounds();
        SetupEducationalFeatures();
        ApplyAccessibilitySettings();
    }
    
    /// <summary>
    /// Apply theme colors to UI elements throughout the game.
    /// </summary>
    private void ApplyThemeColors()
    {
        // Apply colors to UI components
        var images = FindObjectsByType<Image>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var texts = FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (var image in images)
        {
            if (image.gameObject.name.Contains("Background"))
            {
                image.color = backgroundColor;
            }
            else if (image.gameObject.name.Contains("Primary"))
            {
                image.color = primaryColor;
            }
            else if (image.gameObject.name.Contains("Secondary"))
            {
                image.color = secondaryColor;
            }
            else if (image.gameObject.name.Contains("Accent"))
            {
                image.color = accentColor;
            }
        }
        
        foreach (var text in texts)
        {
            text.color = textColor;
            if (enableBaybayin && baybayinFont != null)
            {
                text.font = modernFilipinoFont;
            }
        }
    }
    
    /// <summary>
    /// Setup traditional Filipino weaving patterns for UI backgrounds.
    /// </summary>
    private void SetupTraditionalPatterns()
    {
        if (!enableTraditionalPatterns) return;
        
        // Create pattern materials
        if (weavingPattern != null)
        {
            Material patternMaterial = new Material(Shader.Find("UI/Default"));
            patternMaterial.mainTexture = weavingPattern;
            patternMaterial.color = Color.Lerp(backgroundColor, primaryColor, patternIntensity);
        }
        
        if (bambooTexture != null)
        {
            Material bambooMaterial = new Material(Shader.Find("UI/Default"));
            bambooMaterial.mainTexture = bambooTexture;
            bambooMaterial.color = Color.Lerp(backgroundColor, secondaryColor, patternIntensity);
        }
    }
    
    /// <summary>
    /// Configure the Baybayin script system for traditional Filipino text.
    /// </summary>
    private void ConfigureBaybayinSystem()
    {
        if (!enableBaybayin) return;
        
        // Setup Baybayin font if available
        if (baybayinFont != null)
        {
            // Configure text components to support Baybayin
            var texts = FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var text in texts)
            {
                if (text.gameObject.name.Contains("Baybayin"))
                {
                    text.font = baybayinFont;
                    if (showBaybayinWithModern && modernFilipinoFont != null)
                    {
                        text.text = GetBaybayinWithModern(text.text);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Convert modern Filipino text to include Baybayin script.
    /// </summary>
    /// <param name="modernText">Modern Filipino text</param>
    /// <returns>Text with Baybayin characters</returns>
    private string GetBaybayinWithModern(string modernText)
    {
        string result = modernText;
        foreach (var mapping in baybayinMap)
        {
            result = result.Replace(mapping.Key, mapping.Value);
        }
        return result;
    }
    
    /// <summary>
    /// Load cultural sound effects for authentic Filipino audio experience.
    /// </summary>
    private void LoadCulturalSounds()
    {
        // Setup audio sources for cultural sounds
        var audioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var audioSource in audioSources)
        {
            if (audioSource.gameObject.name.Contains("Cultural"))
            {
                SetupCulturalAudioSource(audioSource);
            }
        }
    }
    
    /// <summary>
    /// Setup individual audio source with cultural sound effects.
    /// </summary>
    /// <param name="audioSource">Audio source to configure</param>
    private void SetupCulturalAudioSource(AudioSource audioSource)
    {
        // Assign appropriate cultural sounds based on context
        if (audioSource.gameObject.name.Contains("Kulintang"))
        {
            if (kulintangSounds != null && kulintangSounds.Length > 0)
            {
                audioSource.clip = kulintangSounds[Random.Range(0, kulintangSounds.Length)];
            }
        }
        else if (audioSource.gameObject.name.Contains("Bamboo"))
        {
            if (bambooSounds != null && bambooSounds.Length > 0)
            {
                audioSource.clip = bambooSounds[Random.Range(0, bambooSounds.Length)];
            }
        }
        else if (audioSource.gameObject.name.Contains("Forest"))
        {
            if (forestSounds != null && forestSounds.Length > 0)
            {
                audioSource.clip = forestSounds[Random.Range(0, forestSounds.Length)];
            }
        }
        else if (audioSource.gameObject.name.Contains("Water"))
        {
            if (waterSounds != null && waterSounds.Length > 0)
            {
                audioSource.clip = waterSounds[Random.Range(0, waterSounds.Length)];
            }
        }
    }
    
    /// <summary>
    /// Setup educational features for cultural learning.
    /// </summary>
    private void SetupEducationalFeatures()
    {
        if (!enableCulturalTooltips) return;
        
        // Add cultural tooltips to UI elements
        var uiElements = FindObjectsByType<Graphic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var element in uiElements)
        {
            if (element.gameObject.name.Contains("Cultural"))
            {
                AddCulturalTooltip(element);
            }
        }
    }
    
    /// <summary>
    /// Add cultural tooltip to UI element.
    /// </summary>
    /// <param name="element">UI element to add tooltip to</param>
    private void AddCulturalTooltip(Graphic element)
    {
        // Implementation for cultural tooltips
        // This would integrate with a tooltip system
        var tooltip = element.gameObject.AddComponent<CulturalTooltip>();
        tooltip.culturalContext = GetCulturalContext(element.gameObject.name);
    }
    
    /// <summary>
    /// Get cultural context for UI element.
    /// </summary>
    /// <param name="elementName">Name of the UI element</param>
    /// <returns>Cultural context description</returns>
    private string GetCulturalContext(string elementName)
    {
        foreach (var context in culturalContext)
        {
            if (elementName.Contains(context.Key))
            {
                return context.Value;
            }
        }
        return "A traditional Filipino cultural element";
    }
    
    /// <summary>
    /// Apply accessibility settings for inclusive design.
    /// </summary>
    private void ApplyAccessibilitySettings()
    {
        if (highContrastMode)
        {
            // Increase contrast between colors
            primaryColor = Color.Lerp(primaryColor, Color.white, 0.3f);
            textColor = Color.Lerp(textColor, Color.black, 0.5f);
        }
        
        if (largeTextMode)
        {
            // Increase font sizes
            var texts = FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var text in texts)
            {
                text.fontSize = Mathf.RoundToInt(text.fontSize * 1.5f);
            }
        }
        
        if (colorBlindFriendly)
        {
            // Adjust colors for color blind users
            primaryColor = new Color(0.8f, 0.6f, 0.2f);    // More yellow
            secondaryColor = new Color(0.2f, 0.5f, 0.8f);   // More blue
            accentColor = new Color(0.8f, 0.4f, 0.2f);     // More orange
        }
    }
    
    /// <summary>
    /// Get location-specific theme colors for Laguna sites.
    /// </summary>
    /// <param name="location">Location name (Mount Makiling, Lake Mohikap, Sampaloc Lake, Botocan Falls)</param>
    /// <returns>Theme color for the location</returns>
    public Color GetLocationThemeColor(string location)
    {
        switch (location.ToLower())
        {
            case "mount makiling":
                return mountMakilingColor;
            case "lake mohikap":
                return lakeMohikapColor;
            case "sampaloc lake":
                return sampalocLakeColor;
            case "botocan falls":
                return botocanFallsColor;
            default:
                return primaryColor;
        }
    }
    
    /// <summary>
    /// Get mythological creature theme colors.
    /// </summary>
    /// <param name="creatureType">Type of creature (Diwata, WaterSpirit, MountainGuardian, LakeSpirit)</param>
    /// <returns>Theme color for the creature</returns>
    public Color GetMythologicalCreatureColor(string creatureType)
    {
        switch (creatureType.ToLower())
        {
            case "diwata":
                return diwataColor;
            case "waterspirit":
                return waterSpiritColor;
            case "mountainguardian":
                return mountainGuardianColor;
            case "lakespirit":
                return lakeSpiritColor;
            default:
                return secondaryColor;
        }
    }
    
    /// <summary>
    /// Apply cultural theme to a specific UI element.
    /// </summary>
    /// <param name="element">UI element to theme</param>
    /// <param name="themeType">Type of theme to apply</param>
    public void ApplyThemeToElement(Graphic element, string themeType)
    {
        if (element == null) return;
        
        Color themeColor = GetThemeColorByType(themeType);
        element.color = themeColor;
        
        // Add cultural context if available
        if (enableCulturalTooltips)
        {
            AddCulturalTooltip(element);
        }
    }
    
    /// <summary>
    /// Get theme color by type.
    /// </summary>
    /// <param name="themeType">Type of theme</param>
    /// <returns>Appropriate color</returns>
    private Color GetThemeColorByType(string themeType)
    {
        switch (themeType.ToLower())
        {
            case "primary":
                return primaryColor;
            case "secondary":
                return secondaryColor;
            case "accent":
                return accentColor;
            case "background":
                return backgroundColor;
            case "text":
                return textColor;
            case "diwata":
                return diwataColor;
            case "water":
                return waterSpiritColor;
            case "mountain":
                return mountainGuardianColor;
            case "lake":
                return lakeSpiritColor;
            default:
                return primaryColor;
        }
    }
}

/// <summary>
/// Cultural tooltip component for UI elements.
/// </summary>
public class CulturalTooltip : MonoBehaviour
{
    public string culturalContext = "";
    public bool showOnHover = true;
    public float tooltipDelay = 1.0f;
    
    // Implementation for tooltip display would go here
    // This is a placeholder for the tooltip system
}