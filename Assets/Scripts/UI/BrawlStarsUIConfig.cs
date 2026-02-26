using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DA_Assets.FCU;

/// <summary>
/// Brawl Stars UI configuration for Figma import pipeline.
/// Defines specific settings for importing Brawl Stars UI components with proper styling and animations.
/// </summary>
[CreateAssetMenu(fileName = "BrawlStarsUIConfig", menuName = "LakbayTala/Brawl Stars UI Config")]
public class BrawlStarsUIConfig : ScriptableObject
{
    [Header("Figma Project Settings")]
    [Tooltip("Figma file URL containing Brawl Stars UI designs - Get from Figma Community or create your own")]
    public string figmaFileUrl = "https://www.figma.com/design/BOMLvyJ570CbsohHwPtEGB/Brawl-Stars-Game-UI-Kit--Community-?node-id=1-892&t=q4MCZssNkwucf3LB-1";
    
    [Tooltip("Figma API token for authentication - Get from figma.com/settings/account")]
    public string figmaApiToken = "";
    
    [Header("UI Component Settings")]
    [Tooltip("Enable Brawl Stars specific UI styling")]
    public bool enableBrawlStarsStyling = true;
    
    [Tooltip("Import button components with proper states")]
    public bool importButtons = true;
    
    [Tooltip("Import panel and container components")]
    public bool importPanels = true;
    
    [Tooltip("Import text and label components")]
    public bool importTextComponents = true;
    
    [Tooltip("Import icon and image components")]
    public bool importIcons = true;
    
    [Header("Animation Settings")]
    [Tooltip("Enable smooth animations for UI transitions")]
    public bool enableAnimations = true;
    
    [Tooltip("Animation duration for UI transitions (seconds)")]
    public float animationDuration = 0.3f;
    
    [Tooltip("Enable button press animations")]
    public bool enableButtonAnimations = true;
    
    [Tooltip("Enable panel transition animations")]
    public bool enablePanelAnimations = true;
    
    [Header("Brawl Stars Color Palette")]
    [Tooltip("Primary gold color from Brawl Stars")]
    public Color primaryGold = new Color(1f, 0.8f, 0.2f, 1f);
    
    [Tooltip("Secondary blue color from Brawl Stars")]
    public Color secondaryBlue = new Color(0.2f, 0.4f, 0.8f, 1f);
    
    [Tooltip("Accent purple color from Brawl Stars")]
    public Color accentPurple = new Color(0.6f, 0.2f, 0.8f, 1f);
    
    [Tooltip("Background dark color")]
    public Color backgroundDark = new Color(0.1f, 0.1f, 0.2f, 0.9f);
    
    [Tooltip("Text light color")]
    public Color textLight = new Color(0.95f, 0.95f, 0.95f, 1f);
    
    [Header("Typography Settings")]
    [Tooltip("Main font for Brawl Stars UI (legacy Font)")]
    public Font mainFont;
    [Tooltip("TMP font for TextMeshPro components")]
    public TMPro.TMP_FontAsset mainFontTMP;
    
    [Tooltip("Secondary font for special text")]
    public Font secondaryFont;
    
    [Tooltip("Base font size for UI elements")]
    public int baseFontSize = 24;
    
    [Tooltip("Header font size")]
    public int headerFontSize = 32;
    
    [Tooltip("Button font size")]
    public int buttonFontSize = 28;
    
    [Header("Layout Settings")]
    [Tooltip("Base spacing between UI elements")]
    public float baseSpacing = 20f;
    
    [Tooltip("Button corner radius")]
    public float buttonCornerRadius = 12f;
    
    [Tooltip("Panel corner radius")]
    public float panelCornerRadius = 16f;
    
    [Tooltip("Shadow blur radius")]
    public float shadowBlur = 8f;
    
    [Tooltip("Shadow offset")]
    public Vector2 shadowOffset = new Vector2(0f, -4f);
    
    [Header("Multi-Device Settings")]
    [Tooltip("Reference resolution for Canvas Scaler")]
    public Vector2 referenceResolution = new Vector2(1920f, 1080f);
    
    [Tooltip("Screen match mode for responsive design")]
    public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    
    [Tooltip("Match width/height ratio (0 = width, 1 = height)")]
    public float matchWidthOrHeight = 0.5f;
    
    [Tooltip("Enable pixel perfect rendering")]
    public bool pixelPerfect = false;
    
    [Header("Touch Target Settings")]
    [Tooltip("Minimum touch target size (44x44 pixels recommended)")]
    public Vector2 minTouchTargetSize = new Vector2(44f, 44f);
    
    [Tooltip("Enable touch target padding for better accessibility")]
    public bool enableTouchPadding = true;
    
    [Tooltip("Touch target padding amount")]
    public float touchPadding = 8f;
    
    [Header("Performance Settings")]
    [Tooltip("Enable sprite atlasing for better performance")]
    public bool enableSpriteAtlasing = true;
    
    [Tooltip("Maximum texture size for UI sprites")]
    public int maxTextureSize = 2048;
    
    [Tooltip("Enable texture compression")]
    public bool enableTextureCompression = true;
    
    [Tooltip("Target frame rate for UI animations")]
    public int targetFrameRate = 60;
    
    [Header("Component Mapping")]
    [Tooltip("Map Figma components to Unity UI components")]
    public List<ComponentMapping> componentMappings = new List<ComponentMapping>
    {
        new ComponentMapping { figmaComponent = "Button", unityComponent = "UnityEngine.UI.Button" },
        new ComponentMapping { figmaComponent = "Text", unityComponent = "UnityEngine.UI.Text" },
        new ComponentMapping { figmaComponent = "Image", unityComponent = "UnityEngine.UI.Image" },
        new ComponentMapping { figmaComponent = "Panel", unityComponent = "UnityEngine.UI.Image" },
        new ComponentMapping { figmaComponent = "Slider", unityComponent = "UnityEngine.UI.Slider" },
        new ComponentMapping { figmaComponent = "Toggle", unityComponent = "UnityEngine.UI.Toggle" },
        new ComponentMapping { figmaComponent = "InputField", unityComponent = "UnityEngine.UI.InputField" }
    };
    
    [System.Serializable]
    public class ComponentMapping
    {
        public string figmaComponent;
        public string unityComponent;
    }
    
    /// <summary>
    /// Apply Brawl Stars styling to a UI element
    /// </summary>
    public void ApplyBrawlStarsStyling(GameObject uiElement, string elementType)
    {
        if (!enableBrawlStarsStyling) return;
        
        switch (elementType.ToLower())
        {
            case "button":
                ApplyButtonStyling(uiElement);
                break;
            case "text":
                ApplyTextStyling(uiElement);
                break;
            case "panel":
                ApplyPanelStyling(uiElement);
                break;
            case "image":
                ApplyImageStyling(uiElement);
                break;
        }
    }
    
    private void ApplyButtonStyling(GameObject button)
    {
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            // Apply gradient background
            var colors = new ColorBlock
            {
                normalColor = primaryGold,
                highlightedColor = Color.Lerp(primaryGold, Color.white, 0.2f),
                pressedColor = Color.Lerp(primaryGold, Color.black, 0.2f),
                selectedColor = primaryGold,
                disabledColor = Color.Lerp(primaryGold, Color.gray, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
            
            var buttonComponent = button.GetComponent<UnityEngine.UI.Button>();
            if (buttonComponent != null)
            {
                buttonComponent.colors = colors;
            }
        }
        
        // Apply text styling
        var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = buttonFontSize;
            text.color = textLight;
            if (mainFontTMP != null) text.font = mainFontTMP;
        }
    }
    
    private void ApplyTextStyling(GameObject textObject)
    {
        var text = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            if (mainFontTMP != null) text.font = mainFontTMP;
            text.fontSize = baseFontSize;
            text.color = textLight;
            text.enableAutoSizing = true;
            text.fontSizeMin = baseFontSize * 0.8f;
            text.fontSizeMax = baseFontSize * 1.2f;
        }
    }
    
    private void ApplyPanelStyling(GameObject panel)
    {
        var image = panel.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = backgroundDark;
            
            // Add rounded corners
            var outline = panel.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = panel.AddComponent<UnityEngine.UI.Outline>();
            }
            outline.effectColor = new Color(0f, 0f, 0f, 0.5f);
            outline.effectDistance = shadowOffset;
        }
    }
    
    private void ApplyImageStyling(GameObject imageObject)
    {
        var image = imageObject.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            // Apply Brawl Stars color palette
            if (image.color == Color.white) // Only change if default
            {
                image.color = primaryGold;
            }
        }
    }
    
    /// <summary>
    /// Configure Canvas Scaler for multi-device support
    /// </summary>
    public void ConfigureCanvasScaler(CanvasScaler canvasScaler)
    {
        if (canvasScaler == null) return;
        
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = referenceResolution;
        canvasScaler.screenMatchMode = screenMatchMode;
        canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        canvasScaler.referencePixelsPerUnit = 100f;
    }
    
    /// <summary>
    /// Validate configuration settings
    /// </summary>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        if (string.IsNullOrEmpty(figmaFileUrl))
        {
            Debug.LogError("Figma file URL is required");
            isValid = false;
        }
        
        if (string.IsNullOrEmpty(figmaApiToken))
        {
            Debug.LogWarning("Figma API token is recommended for authentication");
        }
        
        if (referenceResolution.x <= 0 || referenceResolution.y <= 0)
        {
            Debug.LogError("Reference resolution must be positive values");
            isValid = false;
        }
        
        if (minTouchTargetSize.x < 44f || minTouchTargetSize.y < 44f)
        {
            Debug.LogWarning("Touch target size is below recommended 44x44 pixels");
        }
        
        return isValid;
    }
}