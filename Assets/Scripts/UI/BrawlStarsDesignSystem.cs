using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Brawl Stars Design System - comprehensive design tokens and styling system
/// for implementing authentic Brawl Stars UI/UX across all menu screens.
/// </summary>
[CreateAssetMenu(fileName = "BrawlStarsDesignSystem", menuName = "LakbayTala/Brawl Stars Design System")]
public class BrawlStarsDesignSystem : ScriptableObject
{
    [Header("Brawl Stars Brand Colors")]
    [Tooltip("Official Brawl Stars color palette")]
    public BrawlStarsColors colors = new BrawlStarsColors
    {
        // Primary Brand Colors
        goldPrimary = new Color(1f, 0.8f, 0.2f, 1f),           // #FFCC33 - Main gold
        goldSecondary = new Color(0.95f, 0.7f, 0.1f, 1f),      // #F2B319 - Secondary gold
        goldAccent = new Color(1f, 0.9f, 0.4f, 1f),            // #FFE566 - Light gold accent
        
        // Secondary Brand Colors  
        bluePrimary = new Color(0.2f, 0.4f, 0.8f, 1f),         // #3366CC - Primary blue
        blueSecondary = new Color(0.15f, 0.3f, 0.6f, 1f),       // #264D99 - Dark blue
        blueAccent = new Color(0.4f, 0.6f, 1f, 1f),             // #6699FF - Light blue
        
        // Accent Colors
        purplePrimary = new Color(0.6f, 0.2f, 0.8f, 1f),        // #9933CC - Primary purple
        purpleSecondary = new Color(0.45f, 0.15f, 0.6f, 1f),   // #732699 - Dark purple
        redPrimary = new Color(0.8f, 0.2f, 0.2f, 1f),          // #CC3333 - Primary red
        greenPrimary = new Color(0.2f, 0.8f, 0.2f, 1f),          // #33CC33 - Primary green
        
        // Neutral Colors
        backgroundDark = new Color(0.05f, 0.05f, 0.1f, 0.95f),  // #0D0D19 - Dark background
        backgroundMedium = new Color(0.1f, 0.1f, 0.2f, 0.9f),   // #1A1A33 - Medium background
        backgroundLight = new Color(0.15f, 0.15f, 0.25f, 0.85f), // #262640 - Light background
        
        textPrimary = new Color(0.95f, 0.95f, 0.95f, 1f),      // #F2F2F2 - Primary text
        textSecondary = new Color(0.7f, 0.7f, 0.7f, 1f),        // #B3B3B3 - Secondary text
        textDisabled = new Color(0.4f, 0.4f, 0.4f, 1f),          // #666666 - Disabled text
        
        // Status Colors
        success = new Color(0.2f, 0.8f, 0.2f, 1f),               // #33CC33 - Success
        warning = new Color(1f, 0.6f, 0.2f, 1f),                 // #FF9933 - Warning
        error = new Color(0.8f, 0.2f, 0.2f, 1f),                 // #CC3333 - Error
        info = new Color(0.2f, 0.4f, 0.8f, 1f)                  // #3366CC - Info
    };

    [Header("Typography System")]
    [Tooltip("Complete typography system for Brawl Stars UI")]
    public TypographySystem typography = new TypographySystem
    {
        // Font families
        primaryFont = null, // Will be assigned in Unity
        secondaryFont = null, // Will be assigned in Unity
        monoFont = null, // Will be assigned in Unity
        
        // Font weights
        fontWeights = new FontWeight[]
        {
            new FontWeight { name = "Regular", weight = 400 },
            new FontWeight { name = "Medium", weight = 500 },
            new FontWeight { name = "Bold", weight = 700 },
            new FontWeight { name = "Black", weight = 900 }
        },
        
        // Font sizes (responsive)
        fontSizes = new FontSize[]
        {
            new FontSize { name = "H1", baseSize = 48, minSize = 32, maxSize = 64, lineHeight = 1.2f },
            new FontSize { name = "H2", baseSize = 40, minSize = 28, maxSize = 52, lineHeight = 1.3f },
            new FontSize { name = "H3", baseSize = 32, minSize = 24, maxSize = 40, lineHeight = 1.3f },
            new FontSize { name = "H4", baseSize = 28, minSize = 20, maxSize = 32, lineHeight = 1.4f },
            new FontSize { name = "H5", baseSize = 24, minSize = 18, maxSize = 28, lineHeight = 1.4f },
            new FontSize { name = "H6", baseSize = 20, minSize = 16, maxSize = 24, lineHeight = 1.5f },
            new FontSize { name = "BodyLarge", baseSize = 18, minSize = 14, maxSize = 20, lineHeight = 1.6f },
            new FontSize { name = "Body", baseSize = 16, minSize = 12, maxSize = 18, lineHeight = 1.6f },
            new FontSize { name = "BodySmall", baseSize = 14, minSize = 10, maxSize = 16, lineHeight = 1.6f },
            new FontSize { name = "Caption", baseSize = 12, minSize = 8, maxSize = 14, lineHeight = 1.5f },
            new FontSize { name = "ButtonLarge", baseSize = 18, minSize = 14, maxSize = 20, lineHeight = 1.4f },
            new FontSize { name = "Button", baseSize = 16, minSize = 12, maxSize = 18, lineHeight = 1.4f },
            new FontSize { name = "ButtonSmall", baseSize = 14, minSize = 10, maxSize = 16, lineHeight = 1.4f }
        }
    };

    [Header("Spacing System")]
    [Tooltip("Consistent spacing system based on 8px grid")]
    public SpacingSystem spacing = new SpacingSystem
    {
        baseUnit = 8f,
        scale = new float[] { 0.5f, 1f, 1.5f, 2f, 3f, 4f, 6f, 8f, 12f, 16f, 24f, 32f, 48f, 64f, 96f, 128f, 192f, 256f },
        
        // Named spacing tokens
        tokens = new SpacingToken[]
        {
            new SpacingToken { name = "xs", multiplier = 0.5f },      // 4px
            new SpacingToken { name = "sm", multiplier = 1f },      // 8px
            new SpacingToken { name = "md", multiplier = 2f },      // 16px
            new SpacingToken { name = "lg", multiplier = 3f },      // 24px
            new SpacingToken { name = "xl", multiplier = 4f },      // 32px
            new SpacingToken { name = "2xl", multiplier = 6f },     // 48px
            new SpacingToken { name = "3xl", multiplier = 8f },     // 64px
            new SpacingToken { name = "4xl", multiplier = 12f }     // 96px
        }
    };

    [Header("Component Design Tokens")]
    [Tooltip("Design tokens for UI components")]
    public ComponentTokens components = new ComponentTokens
    {
        // Button tokens
        button = new ButtonTokens
        {
            borderRadius = new float[] { 4f, 8f, 12f, 16f, 9999f }, // none, sm, md, lg, full
            padding = new ComponentSpacing { horizontal = 16f, vertical = 8f },
            minWidth = 120f,
            minHeight = 44f,
            fontWeight = 700,
            textTransform = "uppercase",
            letterSpacing = 0.5f,
            
            // Button variants
            variants = new ButtonVariant[]
            {
                new ButtonVariant { name = "primary", backgroundColor = "goldPrimary", textColor = "backgroundDark" },
                new ButtonVariant { name = "secondary", backgroundColor = "bluePrimary", textColor = "textPrimary" },
                new ButtonVariant { name = "outline", backgroundColor = "transparent", textColor = "goldPrimary", borderColor = "goldPrimary" },
                new ButtonVariant { name = "ghost", backgroundColor = "transparent", textColor = "goldPrimary" }
            }
        },
        
        // Card tokens
        card = new CardTokens
        {
            borderRadius = 16f,
            padding = 24f,
            backgroundColor = "backgroundMedium",
            borderColor = "goldPrimary",
            borderWidth = 2f,
            shadow = new ShadowTokens { blur = 16f, spread = 0f, color = "backgroundDark", opacity = 0.5f }
        },
        
        // Input tokens
        input = new InputTokens
        {
            borderRadius = 8f,
            padding = new ComponentSpacing { horizontal = 16f, vertical = 12f },
            borderWidth = 2f,
            minHeight = 48f,
            fontSize = "Body",
            
            states = new InputState[]
            {
                new InputState { name = "default", borderColor = "backgroundMedium", backgroundColor = "backgroundLight" },
                new InputState { name = "focus", borderColor = "goldPrimary", backgroundColor = "backgroundLight" },
                new InputState { name = "error", borderColor = "error", backgroundColor = "backgroundLight" },
                new InputState { name = "disabled", borderColor = "textDisabled", backgroundColor = "backgroundDark" }
            }
        }
    };

    [Header("Animation System")]
    [Tooltip("Animation timing and easing functions")]
    public AnimationSystem animations = new AnimationSystem
    {
        // Duration tokens
        durations = new AnimationDuration[]
        {
            new AnimationDuration { name = "instant", duration = 0f },
            new AnimationDuration { name = "fast", duration = 0.15f },
            new AnimationDuration { name = "normal", duration = 0.3f },
            new AnimationDuration { name = "slow", duration = 0.5f },
            new AnimationDuration { name = "slower", duration = 0.8f }
        },
        
        // Easing functions
        easings = new AnimationEasing[]
        {
            new AnimationEasing { name = "linear", ease = DG.Tweening.Ease.Linear },
            new AnimationEasing { name = "easeIn", ease = DG.Tweening.Ease.InQuad },
            new AnimationEasing { name = "easeOut", ease = DG.Tweening.Ease.OutQuad },
            new AnimationEasing { name = "easeInOut", ease = DG.Tweening.Ease.InOutQuad },
            new AnimationEasing { name = "easeOutBack", ease = DG.Tweening.Ease.OutBack },
            new AnimationEasing { name = "easeOutBounce", ease = DG.Tweening.Ease.OutBounce },
            new AnimationEasing { name = "easeOutElastic", ease = DG.Tweening.Ease.OutElastic }
        },
        
        // Component-specific animations
        componentAnimations = new ComponentAnimation[]
        {
            new ComponentAnimation 
            { 
                component = "button", 
                states = new AnimationState[] 
                {
                    new AnimationState { name = "hover", duration = "fast", easing = "easeOut", properties = new string[] { "scale", "color" } },
                    new AnimationState { name = "press", duration = "instant", easing = "easeOut", properties = new string[] { "scale", "color" } },
                    new AnimationState { name = "release", duration = "fast", easing = "easeOutBack", properties = new string[] { "scale", "color" } }
                }
            },
            new ComponentAnimation 
            { 
                component = "panel", 
                states = new AnimationState[] 
                {
                    new AnimationState { name = "enter", duration = "normal", easing = "easeOut", properties = new string[] { "opacity", "scale", "position" } },
                    new AnimationState { name = "exit", duration = "normal", easing = "easeIn", properties = new string[] { "opacity", "scale", "position" } }
                }
            }
        }
    };

    [Header("Responsive Design System")]
    [Tooltip("Responsive breakpoints and scaling")]
    public ResponsiveSystem responsive = new ResponsiveSystem
    {
        breakpoints = new Breakpoint[]
        {
            new Breakpoint { name = "mobile", minWidth = 0f, maxWidth = 767f, scale = 0.8f },
            new Breakpoint { name = "tablet", minWidth = 768f, maxWidth = 1023f, scale = 0.9f },
            new Breakpoint { name = "desktop", minWidth = 1024f, maxWidth = 1919f, scale = 1f },
            new Breakpoint { name = "desktopLg", minWidth = 1920f, maxWidth = 9999f, scale = 1.1f }
        },
        
        // Responsive typography scaling
        typographyScaling = new TypographyScaling[]
        {
            new TypographyScaling { breakpoint = "mobile", scale = 0.85f },
            new TypographyScaling { breakpoint = "tablet", scale = 0.95f },
            new TypographyScaling { breakpoint = "desktop", scale = 1f },
            new TypographyScaling { breakpoint = "desktopLg", scale = 1.05f }
        }
    };

    [Header("Cultural Integration")]
    [Tooltip("LakbayTala-specific cultural design elements")]
    public CulturalDesign cultural = new CulturalDesign
    {
        // Filipino color palette
        filipinoColors = new FilipinoColors
        {
            sunYellow = new Color(1f, 0.8f, 0f, 1f),           // Philippine sun yellow
            flagBlue = new Color(0.2f, 0.4f, 0.8f, 1f),        // Philippine flag blue
            flagRed = new Color(0.8f, 0.2f, 0.2f, 1f),         // Philippine flag red
            baybayinGold = new Color(0.9f, 0.7f, 0.3f, 1f),    // Baybayin script gold
            tribalBrown = new Color(0.4f, 0.3f, 0.2f, 1f),     // Tribal patterns brown
            bambooGreen = new Color(0.5f, 0.7f, 0.3f, 1f)      // Bamboo green
        },
        
        // Cultural patterns and textures
        culturalPatterns = new CulturalPattern[]
        {
            new CulturalPattern { name = "BaybayinScript", texture = null, color = "baybayinGold" },
            new CulturalPattern { name = "TribalGeometric", texture = null, color = "tribalBrown" },
            new CulturalPattern { name = "BambooTexture", texture = null, color = "bambooGreen" },
            new CulturalPattern { name = "SunRays", texture = null, color = "sunYellow" }
        }
    };

    // System classes
    [System.Serializable]
    public class BrawlStarsColors
    {
        // Primary brand colors
        public Color goldPrimary;
        public Color goldSecondary;
        public Color goldAccent;
        
        // Secondary brand colors
        public Color bluePrimary;
        public Color blueSecondary;
        public Color blueAccent;
        
        // Accent colors
        public Color purplePrimary;
        public Color purpleSecondary;
        public Color redPrimary;
        public Color greenPrimary;
        
        // Neutral colors
        public Color backgroundDark;
        public Color backgroundMedium;
        public Color backgroundLight;
        public Color textPrimary;
        public Color textSecondary;
        public Color textDisabled;
        
        // Status colors
        public Color success;
        public Color warning;
        public Color error;
        public Color info;
    }

    [System.Serializable]
    public class TypographySystem
    {
        public Font primaryFont;
        public Font secondaryFont;
        public Font monoFont;
        public TMPro.TMP_FontAsset primaryFontTMP;
        public TMPro.TMP_FontAsset secondaryFontTMP;
        public FontWeight[] fontWeights;
        public FontSize[] fontSizes;
    }

    [System.Serializable]
    public class FontWeight
    {
        public string name;
        public int weight;
    }

    [System.Serializable]
    public class FontSize
    {
        public string name;
        public int baseSize;
        public int minSize;
        public int maxSize;
        public float lineHeight;
    }

    [System.Serializable]
    public class SpacingSystem
    {
        public float baseUnit;
        public float[] scale;
        public SpacingToken[] tokens;
    }

    [System.Serializable]
    public class SpacingToken
    {
        public string name;
        public float multiplier;
    }

    [System.Serializable]
    public class ComponentTokens
    {
        public ButtonTokens button;
        public CardTokens card;
        public InputTokens input;
    }

    [System.Serializable]
    public class ButtonTokens
    {
        public float[] borderRadius;
        public ComponentSpacing padding;
        public float minWidth;
        public float minHeight;
        public int fontWeight;
        public string textTransform;
        public float letterSpacing;
        public ButtonVariant[] variants;
    }

    [System.Serializable]
    public class ComponentSpacing
    {
        public float horizontal;
        public float vertical;
    }

    [System.Serializable]
    public class ButtonVariant
    {
        public string name;
        public string backgroundColor;
        public string textColor;
        public string borderColor;
    }

    [System.Serializable]
    public class CardTokens
    {
        public float borderRadius;
        public float padding;
        public string backgroundColor;
        public string borderColor;
        public float borderWidth;
        public ShadowTokens shadow;
    }

    [System.Serializable]
    public class ShadowTokens
    {
        public float blur;
        public float spread;
        public string color;
        public float opacity;
    }

    [System.Serializable]
    public class InputTokens
    {
        public float borderRadius;
        public ComponentSpacing padding;
        public float borderWidth;
        public float minHeight;
        public string fontSize;
        public InputState[] states;
    }

    [System.Serializable]
    public class InputState
    {
        public string name;
        public string borderColor;
        public string backgroundColor;
    }

    [System.Serializable]
    public class AnimationSystem
    {
        public AnimationDuration[] durations;
        public AnimationEasing[] easings;
        public ComponentAnimation[] componentAnimations;
    }

    [System.Serializable]
    public class AnimationDuration
    {
        public string name;
        public float duration;
    }

    [System.Serializable]
    public class AnimationEasing
    {
        public string name;
        public DG.Tweening.Ease ease;
    }

    [System.Serializable]
    public class ComponentAnimation
    {
        public string component;
        public AnimationState[] states;
    }

    [System.Serializable]
    public class AnimationState
    {
        public string name;
        public string duration;
        public string easing;
        public string[] properties;
    }

    [System.Serializable]
    public class ResponsiveSystem
    {
        public Breakpoint[] breakpoints;
        public TypographyScaling[] typographyScaling;
    }

    [System.Serializable]
    public class Breakpoint
    {
        public string name;
        public float minWidth;
        public float maxWidth;
        public float scale;
    }

    [System.Serializable]
    public class TypographyScaling
    {
        public string breakpoint;
        public float scale;
    }

    [System.Serializable]
    public class CulturalDesign
    {
        public FilipinoColors filipinoColors;
        public CulturalPattern[] culturalPatterns;
    }

    [System.Serializable]
    public class FilipinoColors
    {
        public Color sunYellow;
        public Color flagBlue;
        public Color flagRed;
        public Color baybayinGold;
        public Color tribalBrown;
        public Color bambooGreen;
    }

    [System.Serializable]
    public class CulturalPattern
    {
        public string name;
        public Texture2D texture;
        public string color;
    }

    // Helper methods for accessing design tokens
    
    /// <summary>
    /// Get color by name from the design system
    /// </summary>
    public Color GetColor(string colorName)
    {
        var colorType = colors.GetType();
        var field = colorType.GetField(colorName);
        if (field != null)
        {
            return (Color)field.GetValue(colors);
        }
        
        // Check cultural colors
        var culturalType = cultural.filipinoColors.GetType();
        var culturalField = culturalType.GetField(colorName);
        if (culturalField != null)
        {
            return (Color)culturalField.GetValue(cultural.filipinoColors);
        }
        
        return Color.white;
    }
    
    /// <summary>
    /// Get spacing value by token name
    /// </summary>
    public float GetSpacing(string tokenName)
    {
        var token = System.Array.Find(spacing.tokens, t => t.name == tokenName);
        return token != null ? spacing.baseUnit * token.multiplier : spacing.baseUnit;
    }
    
    /// <summary>
    /// Get font size by name with responsive scaling
    /// </summary>
    public int GetFontSize(string sizeName, string breakpoint = "desktop")
    {
        var fontSize = System.Array.Find(typography.fontSizes, s => s.name == sizeName);
        if (fontSize == null) return 16;
        
        var scaling = System.Array.Find(responsive.typographyScaling, s => s.breakpoint == breakpoint);
        float scale = scaling != null ? scaling.scale : 1f;
        
        return Mathf.RoundToInt(fontSize.baseSize * scale);
    }
    
    /// <summary>
    /// Get animation easing by name
    /// </summary>
    public DG.Tweening.Ease GetEasing(string easingName)
    {
        var easing = System.Array.Find(animations.easings, e => e.name == easingName);
        return easing != null ? easing.ease : DG.Tweening.Ease.Linear;
    }
    
    /// <summary>
    /// Get animation duration by name
    /// </summary>
    public float GetDuration(string durationName)
    {
        var duration = System.Array.Find(animations.durations, d => d.name == durationName);
        return duration != null ? duration.duration : 0.3f;
    }
    
    /// <summary>
    /// Apply responsive scaling based on screen size
    /// </summary>
    public float GetResponsiveScale()
    {
        float screenWidth = Screen.width;
        
        foreach (var breakpoint in responsive.breakpoints)
        {
            if (screenWidth >= breakpoint.minWidth && screenWidth <= breakpoint.maxWidth)
            {
                return breakpoint.scale;
            }
        }
        
        return 1f; // Default desktop scale
    }
    
    /// <summary>
    /// Get cultural color with Brawl Stars integration
    /// </summary>
    public Color GetCulturalColor(string culturalColorName, bool integrateWithBrawlStars = true)
    {
        Color culturalColor = GetColor(culturalColorName);
        
        if (integrateWithBrawlStars)
        {
            // Blend cultural colors with Brawl Stars palette for cohesive design
            Color brawlStarsGold = colors.goldPrimary;
            return Color.Lerp(culturalColor, brawlStarsGold, 0.2f);
        }
        
        return culturalColor;
    }
}