using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Brawl Stars Canvas Manager - handles multi-device Canvas configuration and scaling
/// for optimal UI rendering across different screen sizes and aspect ratios.
/// </summary>
public class BrawlStarsCanvasManager : MonoBehaviour
{
    [Header("Canvas References")]
    public Canvas mainCanvas;
    public CanvasScaler canvasScaler;
    public GraphicRaycaster graphicRaycaster;
    
    [Header("Multi-Device Configuration")]
    [Tooltip("Reference resolution for Canvas Scaler")]
    public Vector2 referenceResolution = new Vector2(1920f, 1080f);
    
    [Tooltip("Screen match mode for responsive design")]
    public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    
    [Tooltip("Match width/height ratio (0 = width, 1 = height)")]
    [Range(0f, 1f)]
    public float matchWidthOrHeight = 0.5f;
    
    [Tooltip("Enable pixel perfect rendering")]
    public bool pixelPerfect = false;
    
    [Tooltip("Reference pixels per unit")]
    public float referencePixelsPerUnit = 100f;
    
    [Header("Device-Specific Settings")]
    [Tooltip("Phone configuration")]
    public DeviceConfig phoneConfig = new DeviceConfig
    {
        minScreenSize = 4.7f,
        maxScreenSize = 6.7f,
        referenceResolution = new Vector2(1334f, 750f),
        matchWidthOrHeight = 0.6f,
        scaleFactor = 1.0f
    };
    
    [Tooltip("Tablet configuration")]
    public DeviceConfig tabletConfig = new DeviceConfig
    {
        minScreenSize = 7.0f,
        maxScreenSize = 13.0f,
        referenceResolution = new Vector2(2048f, 1536f),
        matchWidthOrHeight = 0.5f,
        scaleFactor = 1.2f
    };
    
    [Tooltip("Large tablet configuration")]
    public DeviceConfig largeTabletConfig = new DeviceConfig
    {
        minScreenSize = 13.0f,
        maxScreenSize = 20.0f,
        referenceResolution = new Vector2(2732f, 2048f),
        matchWidthOrHeight = 0.4f,
        scaleFactor = 1.5f
    };
    
    [Header("Aspect Ratio Settings")]
    [Tooltip("Standard 16:9 aspect ratio")]
    public AspectRatioConfig standardAspect = new AspectRatioConfig
    {
        aspectRatio = 16f / 9f,
        tolerance = 0.1f,
        matchWidthOrHeight = 0.5f
    };
    
    [Tooltip("Tall 19.5:9 aspect ratio (modern phones)")]
    public AspectRatioConfig tallAspect = new AspectRatioConfig
    {
        aspectRatio = 19.5f / 9f,
        tolerance = 0.1f,
        matchWidthOrHeight = 0.7f
    };
    
    [Tooltip("Tablet 4:3 aspect ratio")]
    public AspectRatioConfig tabletAspect = new AspectRatioConfig
    {
        aspectRatio = 4f / 3f,
        tolerance = 0.1f,
        matchWidthOrHeight = 0.3f
    };
    
    [Header("Performance Settings")]
    [Tooltip("Enable dynamic resolution scaling")]
    public bool enableDynamicResolution = true;
    
    [Tooltip("Minimum scale factor")]
    public float minScaleFactor = 0.5f;
    
    [Tooltip("Maximum scale factor")]
    public float maxScaleFactor = 2.0f;
    
    [Tooltip("Enable batching for better performance")]
    public bool enableBatching = true;
    
    [Tooltip("Target frame rate for UI")]
    public int targetFrameRate = 60;
    
    [System.Serializable]
    public class DeviceConfig
    {
        public float minScreenSize;
        public float maxScreenSize;
        public Vector2 referenceResolution;
        public float matchWidthOrHeight;
        public float scaleFactor;
    }
    
    [System.Serializable]
    public class AspectRatioConfig
    {
        public float aspectRatio;
        public float tolerance;
        public float matchWidthOrHeight;
    }
    
    private Vector2 currentScreenSize;
    private float currentAspectRatio;
    private DeviceType currentDeviceType;
#pragma warning disable CS0414
    private bool isInitialized = false;
#pragma warning restore CS0414
    
    public enum DeviceType
    {
        Phone,
        Tablet,
        LargeTablet,
        Unknown
    }
    
    void Awake()
    {
        InitializeCanvas();
        DetectDeviceType();
        ApplyDeviceConfiguration();
        isInitialized = true;
    }
    
    void Start()
    {
        OptimizeForCurrentDevice();
        SetTargetFrameRate();
    }
    
    void Update()
    {
        // Monitor for screen size changes (orientation changes)
        if (Screen.width != currentScreenSize.x || Screen.height != currentScreenSize.y)
        {
            HandleScreenSizeChange();
        }
    }
    
    /// <summary>
    /// Initialize Canvas and related components
    /// </summary>
    private void InitializeCanvas()
    {
        if (mainCanvas == null)
            mainCanvas = GetComponent<Canvas>();
        
        if (canvasScaler == null)
            canvasScaler = GetComponent<CanvasScaler>();
        
        if (graphicRaycaster == null)
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        
        // Configure canvas properties
        if (mainCanvas != null)
        {
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 1;
            mainCanvas.pixelPerfect = pixelPerfect;
        }
        
        Debug.Log("Canvas initialized successfully");
    }
    
    /// <summary>
    /// Detect current device type based on screen size
    /// </summary>
    private void DetectDeviceType()
    {
        currentScreenSize = new Vector2(Screen.width, Screen.height);
        currentAspectRatio = currentScreenSize.x / currentScreenSize.y;
        
        // Calculate diagonal screen size in inches (assuming standard DPI)
        float diagonalInches = CalculateDiagonalInches();
        
        if (diagonalInches >= phoneConfig.minScreenSize && diagonalInches <= phoneConfig.maxScreenSize)
        {
            currentDeviceType = DeviceType.Phone;
        }
        else if (diagonalInches >= tabletConfig.minScreenSize && diagonalInches <= tabletConfig.maxScreenSize)
        {
            currentDeviceType = DeviceType.Tablet;
        }
        else if (diagonalInches >= largeTabletConfig.minScreenSize && diagonalInches <= largeTabletConfig.maxScreenSize)
        {
            currentDeviceType = DeviceType.LargeTablet;
        }
        else
        {
            currentDeviceType = DeviceType.Unknown;
        }
        
        Debug.Log($"Detected device type: {currentDeviceType} (Diagonal: {diagonalInches:F1} inches, Aspect: {currentAspectRatio:F2})");
    }
    
    /// <summary>
    /// Calculate diagonal screen size in inches
    /// </summary>
    private float CalculateDiagonalInches()
    {
        // Get DPI (dots per inch)
        float dpi = Screen.dpi;
        
        if (dpi <= 0)
        {
            // Fallback DPI values for common devices
            dpi = GetFallbackDPI();
        }
        
        // Calculate diagonal in pixels
        float diagonalPixels = Mathf.Sqrt(currentScreenSize.x * currentScreenSize.x + currentScreenSize.y * currentScreenSize.y);
        
        // Convert to inches
        float diagonalInches = diagonalPixels / dpi;
        
        return diagonalInches;
    }
    
    /// <summary>
    /// Get fallback DPI for common devices
    /// </summary>
    private float GetFallbackDPI()
    {
        // Common DPI values
        if (currentScreenSize.x <= 800) return 160f;  // LDPI
        if (currentScreenSize.x <= 1280) return 240f; // MDPI
        if (currentScreenSize.x <= 1920) return 320f; // HDPI
        if (currentScreenSize.x <= 2560) return 480f; // XHDPI
        return 640f; // XXHDPI
    }
    
    /// <summary>
    /// Apply device-specific configuration
    /// </summary>
    private void ApplyDeviceConfiguration()
    {
        if (canvasScaler == null) return;
        
        DeviceConfig config = GetCurrentDeviceConfig();
        
        // Apply configuration based on device type
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = config.referenceResolution;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = config.matchWidthOrHeight;
        canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        
        // Apply aspect ratio specific adjustments
        ApplyAspectRatioConfiguration();
        
        Debug.Log($"Applied {currentDeviceType} configuration: {config.referenceResolution} @ {config.matchWidthOrHeight:F2}");
    }
    
    /// <summary>
    /// Get current device configuration
    /// </summary>
    private DeviceConfig GetCurrentDeviceConfig()
    {
        switch (currentDeviceType)
        {
            case DeviceType.Phone:
                return phoneConfig;
            case DeviceType.Tablet:
                return tabletConfig;
            case DeviceType.LargeTablet:
                return largeTabletConfig;
            default:
                return phoneConfig; // Default to phone config
        }
    }
    
    /// <summary>
    /// Apply aspect ratio specific configuration
    /// </summary>
    private void ApplyAspectRatioConfiguration()
    {
        AspectRatioConfig aspectConfig = GetAspectRatioConfig();
        
        if (canvasScaler != null)
        {
            canvasScaler.matchWidthOrHeight = aspectConfig.matchWidthOrHeight;
        }
        
        Debug.Log($"Applied aspect ratio configuration: {currentAspectRatio:F2} @ {aspectConfig.matchWidthOrHeight:F2}");
    }
    
    /// <summary>
    /// Get aspect ratio configuration
    /// </summary>
    private AspectRatioConfig GetAspectRatioConfig()
    {
        // Check for standard 16:9
        if (Mathf.Abs(currentAspectRatio - standardAspect.aspectRatio) <= standardAspect.tolerance)
        {
            return standardAspect;
        }
        
        // Check for tall 19.5:9
        if (Mathf.Abs(currentAspectRatio - tallAspect.aspectRatio) <= tallAspect.tolerance)
        {
            return tallAspect;
        }
        
        // Check for tablet 4:3
        if (Mathf.Abs(currentAspectRatio - tabletAspect.aspectRatio) <= tabletAspect.tolerance)
        {
            return tabletAspect;
        }
        
        // Default to standard aspect ratio
        return standardAspect;
    }
    
    /// <summary>
    /// Optimize UI for current device
    /// </summary>
    private void OptimizeForCurrentDevice()
    {
        // Configure graphic raycaster for current device
        if (graphicRaycaster != null)
        {
            ConfigureGraphicRaycaster();
        }
        
        // Apply dynamic resolution scaling if enabled
        if (enableDynamicResolution)
        {
            ApplyDynamicResolution();
        }
        
        Debug.Log($"Optimized UI for {currentDeviceType} device");
    }
    
    /// <summary>
    /// Configure graphic raycaster for current device
    /// </summary>
    private void ConfigureGraphicRaycaster()
    {
        // Optimize raycasting based on device capabilities
        switch (currentDeviceType)
        {
            case DeviceType.Phone:
                graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
                break;
            case DeviceType.Tablet:
            case DeviceType.LargeTablet:
                graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;
                break;
            default:
                graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
                break;
        }
    }
    
    /// <summary>
    /// Apply dynamic resolution scaling
    /// </summary>
    private void ApplyDynamicResolution()
    {
        DeviceConfig config = GetCurrentDeviceConfig();
        
        // Calculate scale factor based on screen size and device type
        float scaleFactor = config.scaleFactor;
        
        // Clamp scale factor to valid range
        scaleFactor = Mathf.Clamp(scaleFactor, minScaleFactor, maxScaleFactor);
        
        // Apply scale factor (this would affect UI element scaling)
        // Note: This is a simplified implementation
        Debug.Log($"Applied dynamic resolution scaling: {scaleFactor:F2}x");
    }
    
    /// <summary>
    /// Set target frame rate for UI
    /// </summary>
    private void SetTargetFrameRate()
    {
        Application.targetFrameRate = targetFrameRate;
        
        // VSync settings based on device type
        switch (currentDeviceType)
        {
            case DeviceType.Phone:
                QualitySettings.vSyncCount = 1; // 60 FPS on 60Hz displays
                break;
            case DeviceType.Tablet:
            case DeviceType.LargeTablet:
                QualitySettings.vSyncCount = 1; // 60 FPS on most tablets
                break;
            default:
                QualitySettings.vSyncCount = 1;
                break;
        }
        
        Debug.Log($"Set target frame rate: {targetFrameRate} FPS");
    }
    
    /// <summary>
    /// Handle screen size changes (orientation changes)
    /// </summary>
    private void HandleScreenSizeChange()
    {
        Debug.Log($"Screen size changed: {Screen.width}x{Screen.height}");
        
        // Re-detect device type
        DetectDeviceType();
        
        // Re-apply configuration
        ApplyDeviceConfiguration();
        
        // Re-optimize for current device
        OptimizeForCurrentDevice();
        
        // Notify listeners
        OnScreenConfigurationChanged?.Invoke(currentDeviceType, currentScreenSize, currentAspectRatio);
    }
    
    #region Public Methods
    
    /// <summary>
    /// Get current device type
    /// </summary>
    public DeviceType GetCurrentDeviceType()
    {
        return currentDeviceType;
    }
    
    /// <summary>
    /// Get current screen size
    /// </summary>
    public Vector2 GetCurrentScreenSize()
    {
        return currentScreenSize;
    }
    
    /// <summary>
    /// Get current aspect ratio
    /// </summary>
    public float GetCurrentAspectRatio()
    {
        return currentAspectRatio;
    }
    
    /// <summary>
    /// Get current device configuration
    /// </summary>
    public DeviceConfig GetCurrentConfiguration()
    {
        return GetCurrentDeviceConfig();
    }
    
    /// <summary>
    /// Force reconfiguration for current device
    /// </summary>
    public void ForceReconfiguration()
    {
        DetectDeviceType();
        ApplyDeviceConfiguration();
        OptimizeForCurrentDevice();
    }
    
    /// <summary>
    /// Apply custom device configuration
    /// </summary>
    public void ApplyCustomConfiguration(DeviceConfig config)
    {
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = config.referenceResolution;
            canvasScaler.matchWidthOrHeight = config.matchWidthOrHeight;
        }
        
        Debug.Log($"Applied custom configuration: {config.referenceResolution}");
    }
    
    /// <summary>
    /// Get device performance metrics
    /// </summary>
    public Dictionary<string, float> GetPerformanceMetrics()
    {
        var metrics = new Dictionary<string, float>
        {
            { "Screen_Width", currentScreenSize.x },
            { "Screen_Height", currentScreenSize.y },
            { "Aspect_Ratio", currentAspectRatio },
            { "Diagonal_Inches", CalculateDiagonalInches() },
            { "DPI", Screen.dpi > 0 ? Screen.dpi : GetFallbackDPI() },
            { "Scale_Factor", GetCurrentDeviceConfig().scaleFactor },
            { "Match_Ratio", GetCurrentDeviceConfig().matchWidthOrHeight }
        };
        
        return metrics;
    }
    
    /// <summary>
    /// Validate canvas configuration
    /// </summary>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        if (mainCanvas == null)
        {
            Debug.LogError("Main Canvas is not assigned");
            isValid = false;
        }
        
        if (canvasScaler == null)
        {
            Debug.LogError("Canvas Scaler is not assigned");
            isValid = false;
        }
        
        if (referenceResolution.x <= 0 || referenceResolution.y <= 0)
        {
            Debug.LogError("Reference resolution must be positive values");
            isValid = false;
        }
        
        if (matchWidthOrHeight < 0f || matchWidthOrHeight > 1f)
        {
            Debug.LogError("Match width or height must be between 0 and 1");
            isValid = false;
        }
        
        return isValid;
    }
    
    #endregion
    
    #region Events
    
    public delegate void ScreenConfigurationChanged(DeviceType deviceType, Vector2 screenSize, float aspectRatio);
    public event ScreenConfigurationChanged OnScreenConfigurationChanged;
    
    #endregion
    
    #region Editor Methods
    
    /// <summary>
    /// Preview configuration in editor
    /// </summary>
    public void PreviewConfiguration()
    {
        #if UNITY_EDITOR
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = referenceResolution;
            canvasScaler.screenMatchMode = screenMatchMode;
            canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }
        #endif
    }
    
    #endregion
    
    void OnValidate()
    {
        // Validate settings in editor
        if (matchWidthOrHeight < 0f) matchWidthOrHeight = 0f;
        if (matchWidthOrHeight > 1f) matchWidthOrHeight = 1f;
        if (minScaleFactor < 0.1f) minScaleFactor = 0.1f;
        if (maxScaleFactor > 5.0f) maxScaleFactor = 5.0f;
        if (targetFrameRate < 30) targetFrameRate = 30;
        if (targetFrameRate > 120) targetFrameRate = 120;
    }
    
    void OnDestroy()
    {
        // Cleanup
        OnScreenConfigurationChanged = null;
    }
}