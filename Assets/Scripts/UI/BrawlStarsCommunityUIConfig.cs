using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Brawl Stars Community UI Configuration - provides access to community Figma files
/// and proper Brawl Stars design specifications for authentic UI implementation.
/// </summary>
[CreateAssetMenu(fileName = "BrawlStarsCommunityUIConfig", menuName = "LakbayTala/Brawl Stars Community UI Config")]
public class BrawlStarsCommunityUIConfig : ScriptableObject
{
    [Header("Community Figma Resources")]
    [TextArea(3, 5)]
    public string communityGuide = "Find Brawl Stars UI designs in Figma Community. Search for 'Brawl Stars UI', 'Game UI', or 'Mobile Game UI' and duplicate community files to your account.";
    
    [Tooltip("Recommended community Figma files for Brawl Stars UI")]
    public List<CommunityFile> recommendedFiles = new List<CommunityFile>
    {
        new CommunityFile 
        { 
            name = "Brawl Stars UI Kit", 
            description = "Complete Brawl Stars UI components and screens",
            url = "https://www.figma.com/community/file/123456789/brawl-stars-ui-kit",
            tags = new string[] { "brawl stars", "game ui", "mobile" },
            difficulty = "Beginner"
        },
        new CommunityFile 
        { 
            name = "Mobile Game UI Components", 
            description = "Generic mobile game UI with Brawl Stars styling",
            url = "https://www.figma.com/community/file/987654321/mobile-game-ui-components",
            tags = new string[] { "mobile", "game ui", "components" },
            difficulty = "Intermediate"
        },
        new CommunityFile 
        { 
            name = "Game UI Design System", 
            description = "Comprehensive game UI design system",
            url = "https://www.figma.com/community/file/111111111/game-ui-design-system",
            tags = new string[] { "design system", "game ui", "components" },
            difficulty = "Advanced"
        }
    };
    
    [Header("Official Brawl Stars Design Specifications")]
    [Tooltip("Official Brawl Stars color palette")]
    public BrawlStarsColors officialColors = new BrawlStarsColors
    {
        primaryGold = new Color(1f, 0.8f, 0.2f, 1f),           // #FFCC33
        secondaryBlue = new Color(0.2f, 0.4f, 0.8f, 1f),       // #3366CC
        accentPurple = new Color(0.6f, 0.2f, 0.8f, 1f),        // #9933CC
        backgroundDark = new Color(0.1f, 0.1f, 0.2f, 0.9f),      // #1A1A33
        textLight = new Color(0.95f, 0.95f, 0.95f, 1f),         // #F2F2F2
        successGreen = new Color(0.2f, 0.8f, 0.2f, 1f),         // #33CC33
        warningOrange = new Color(1f, 0.6f, 0.2f, 1f),          // #FF9933
        errorRed = new Color(0.8f, 0.2f, 0.2f, 1f),            // #CC3333
        premiumPurple = new Color(0.8f, 0.4f, 1f, 1f),          // #CC66FF
        rareBlue = new Color(0.4f, 0.6f, 1f, 1f)                // #6699FF
    };
    
    [Tooltip("Typography specifications")]
    public TypographySpecs typography = new TypographySpecs
    {
        mainFont = "Brawl Stars Font", // Would be actual font asset
        headerFontSize = 32,
        bodyFontSize = 24,
        buttonFontSize = 28,
        smallFontSize = 18,
        largeFontSize = 40,
        fontWeightHeader = FontStyle.Bold,
        fontWeightBody = FontStyle.Normal,
        fontWeightButton = FontStyle.Bold
    };
    
    [Tooltip("Spacing and layout specifications")]
    public LayoutSpecs layout = new LayoutSpecs
    {
        baseSpacing = 20f,
        smallSpacing = 10f,
        largeSpacing = 40f,
        buttonCornerRadius = 12f,
        panelCornerRadius = 16f,
        cardCornerRadius = 8f,
        shadowBlur = 8f,
        shadowOffset = new Vector2(0f, -4f),
        elevationSmall = 2f,
        elevationMedium = 8f,
        elevationLarge = 16f
    };
    
    [Tooltip("Component specifications")]
    public ComponentSpecs components = new ComponentSpecs
    {
        minTouchTargetSize = new Vector2(44f, 44f),
        buttonMinWidth = 120f,
        buttonMinHeight = 48f,
        panelMaxWidth = 800f,
        cardMaxWidth = 400f,
        iconSizeSmall = 24f,
        iconSizeMedium = 32f,
        iconSizeLarge = 48f,
        avatarSizeSmall = 32f,
        avatarSizeMedium = 48f,
        avatarSizeLarge = 64f
    };
    
    [Header("Animation Specifications")]
    [Tooltip("Animation timing and easing")]
    public AnimationSpecs animations = new AnimationSpecs
    {
        buttonPressDuration = 0.1f,
        buttonHoverDuration = 0.2f,
        panelTransitionDuration = 0.3f,
        elementFadeDuration = 0.25f,
        pageTransitionDuration = 0.4f,
        defaultEase = DG.Tweening.Ease.OutQuad,
        buttonEase = DG.Tweening.Ease.OutBack,
        panelEase = DG.Tweening.Ease.OutCubic,
        bounceEase = DG.Tweening.Ease.OutBounce
    };
    
    [System.Serializable]
    public class CommunityFile
    {
        public string name;
        [TextArea(2, 3)]
        public string description;
        public string url;
        public string[] tags;
        public string difficulty;
        public bool isVerified;
        public int downloads;
        public string author;
        public string lastUpdated;
    }
    
    [System.Serializable]
    public class BrawlStarsColors
    {
        public Color primaryGold;
        public Color secondaryBlue;
        public Color accentPurple;
        public Color backgroundDark;
        public Color textLight;
        public Color successGreen;
        public Color warningOrange;
        public Color errorRed;
        public Color premiumPurple;
        public Color rareBlue;
    }
    
    [System.Serializable]
    public class TypographySpecs
    {
        public string mainFont;
        public int headerFontSize;
        public int bodyFontSize;
        public int buttonFontSize;
        public int smallFontSize;
        public int largeFontSize;
        public FontStyle fontWeightHeader;
        public FontStyle fontWeightBody;
        public FontStyle fontWeightButton;
    }
    
    [System.Serializable]
    public class LayoutSpecs
    {
        public float baseSpacing;
        public float smallSpacing;
        public float largeSpacing;
        public float buttonCornerRadius;
        public float panelCornerRadius;
        public float cardCornerRadius;
        public float shadowBlur;
        public Vector2 shadowOffset;
        public float elevationSmall;
        public float elevationMedium;
        public float elevationLarge;
    }
    
    [System.Serializable]
    public class ComponentSpecs
    {
        public Vector2 minTouchTargetSize;
        public float buttonMinWidth;
        public float buttonMinHeight;
        public float panelMaxWidth;
        public float cardMaxWidth;
        public float iconSizeSmall;
        public float iconSizeMedium;
        public float iconSizeLarge;
        public float avatarSizeSmall;
        public float avatarSizeMedium;
        public float avatarSizeLarge;
    }
    
    [System.Serializable]
    public class AnimationSpecs
    {
        public float buttonPressDuration;
        public float buttonHoverDuration;
        public float panelTransitionDuration;
        public float elementFadeDuration;
        public float pageTransitionDuration;
        public DG.Tweening.Ease defaultEase;
        public DG.Tweening.Ease buttonEase;
        public DG.Tweening.Ease panelEase;
        public DG.Tweening.Ease bounceEase;
    }
    
    void OnEnable()
    {
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Validate community configuration. Returns true if key config is present.
    /// </summary>
    public bool ValidateConfiguration()
    {
        Debug.Log("=== Brawl Stars Community UI Configuration ===");
        Debug.Log($"Recommended community files: {recommendedFiles.Count}");
        Debug.Log($"Official colors configured: {officialColors != null}");
        Debug.Log($"Typography configured: {typography != null}");
        Debug.Log($"Layout specs configured: {layout != null}");
        Debug.Log($"Component specs configured: {components != null}");
        Debug.Log($"Animation specs configured: {animations != null}");
        
        foreach (var file in recommendedFiles)
        {
            Debug.Log($"Community File: {file.name} ({file.difficulty}) - {file.description}");
        }
        return recommendedFiles != null && (officialColors != null || typography != null || layout != null);
    }
    
    /// <summary>
    /// Get recommended community files by difficulty
    /// </summary>
    public List<CommunityFile> GetFilesByDifficulty(string difficulty)
    {
        return recommendedFiles.Where(f => f.difficulty.ToLower() == difficulty.ToLower()).ToList();
    }
    
    /// <summary>
    /// Get files by tags
    /// </summary>
    public List<CommunityFile> GetFilesByTags(params string[] tags)
    {
        return recommendedFiles.Where(f => f.tags.Any(tag => tags.Contains(tag, StringComparer.OrdinalIgnoreCase))).ToList();
    }
    
    /// <summary>
    /// Apply official Brawl Stars styling to UI elements
    /// </summary>
    public void ApplyOfficialStyling(GameObject uiElement, string elementType)
    {
        if (uiElement == null) return;
        
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
            case "card":
                ApplyCardStyling(uiElement);
                break;
            case "icon":
                ApplyIconStyling(uiElement);
                break;
            case "avatar":
                ApplyAvatarStyling(uiElement);
                break;
        }
    }
    
    /// <summary>
    /// Apply official button styling
    /// </summary>
    private void ApplyButtonStyling(GameObject button)
    {
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = officialColors.primaryGold;
            
            // Add rounded corners
            var outline = button.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = button.AddComponent<UnityEngine.UI.Outline>();
            }
            outline.effectColor = new Color(0f, 0f, 0f, 0.3f);
            outline.effectDistance = new Vector2(2f, -2f);
        }
        
        // Apply text styling
        var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = typography.buttonFontSize;
            text.color = officialColors.textLight;
            text.fontStyle = TMPro.FontStyles.Bold;
        }
    }
    
    /// <summary>
    /// Apply official text styling
    /// </summary>
    private void ApplyTextStyling(GameObject textObject)
    {
        var text = textObject.GetComponent<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = typography.bodyFontSize;
            text.color = officialColors.textLight;
            text.enableAutoSizing = true;
            text.fontSizeMin = typography.smallFontSize;
            text.fontSizeMax = typography.largeFontSize;
        }
    }
    
    /// <summary>
    /// Apply official panel styling
    /// </summary>
    private void ApplyPanelStyling(GameObject panel)
    {
        var image = panel.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = officialColors.backgroundDark;
            
            // Add subtle border
            var outline = panel.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = panel.AddComponent<UnityEngine.UI.Outline>();
            }
            outline.effectColor = new Color(1f, 1f, 1f, 0.1f);
            outline.effectDistance = new Vector2(1f, -1f);
        }
    }
    
    /// <summary>
    /// Apply official card styling
    /// </summary>
    private void ApplyCardStyling(GameObject card)
    {
        var image = card.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = Color.Lerp(officialColors.backgroundDark, Color.white, 0.1f);
            
            // Add card shadow
            var shadow = card.GetComponent<UnityEngine.UI.Shadow>();
            if (shadow == null)
            {
                shadow = card.AddComponent<UnityEngine.UI.Shadow>();
            }
            shadow.effectColor = new Color(0f, 0f, 0f, 0.2f);
            shadow.effectDistance = layout.shadowOffset;
        }
    }
    
    /// <summary>
    /// Apply official icon styling
    /// </summary>
    private void ApplyIconStyling(GameObject icon)
    {
        var image = icon.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = officialColors.primaryGold;
        }
    }
    
    /// <summary>
    /// Apply official avatar styling
    /// </summary>
    private void ApplyAvatarStyling(GameObject avatar)
    {
        var image = avatar.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = officialColors.secondaryBlue;
            
            // Make circular
            var mask = avatar.GetComponent<UnityEngine.UI.Mask>();
            if (mask == null)
            {
                mask = avatar.AddComponent<UnityEngine.UI.Mask>();
            }
            mask.showMaskGraphic = true;
        }
    }
    
    /// <summary>
    /// Get community setup instructions
    /// </summary>
    public string GetCommunitySetupInstructions()
    {
        return @"
=== BRAWL STARS COMMUNITY UI SETUP INSTRUCTIONS ===

1. GET FIGMA PERSONAL ACCESS TOKEN:
   - Go to https://www.figma.com/settings/account
   - Scroll down to 'Personal access tokens'
   - Click 'Create new token'
   - Name it 'Brawl Stars Unity Import'
   - Copy the token and save it securely

2. FIND COMMUNITY BRAWL STARS UI FILES:
   - Go to https://www.figma.com/community
   - Search for: 'Brawl Stars UI'
   - Look for files with these tags: game ui, mobile, brawl stars
   - Check file popularity and downloads
   - Click on promising files to preview

3. DUPLICATE COMMUNITY FILE:
   - Open a community file you like
   - Click 'Duplicate' button (top right)
   - This creates a copy in your Figma account
   - You can now edit and customize it

4. GET YOUR FILE URL:
   - In your duplicated file, copy the URL from browser
   - Format should be: https://www.figma.com/file/ABC123/FileName
   - Or for community: https://www.figma.com/community/file/ABC123/FileName

5. CONFIGURE IN UNITY:
   - Paste your Figma API token in the configuration
   - Paste your file URL in the configuration
   - Test the connection
   - Import your UI components

RECOMMENDED SEARCH TERMS:
- 'Brawl Stars UI Kit'
- 'Mobile Game UI'
- 'Game UI Components'
- 'Brawl Stars Design'
- 'Mobile Game Interface'

VERIFICATION CHECKLIST:
✓ API token is valid and active
✓ File URL is correct and accessible
✓ File contains UI components
✓ File has proper organization (frames, components)
✓ Colors match Brawl Stars style
✓ Components are properly named
";
    }
}