using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Brawl Stars Responsive Layout System - handles responsive design for all screen sizes
/// ensuring consistent Brawl Stars UI/UX across mobile, tablet, and desktop devices.
/// </summary>
public class BrawlStarsResponsiveLayout : MonoBehaviour
{
    [Header("Responsive Configuration")]
    public BrawlStarsDesignSystem designSystem;
    public bool enableResponsiveLayout = true;
    public bool enableDynamicScaling = true;
    public bool enableOrientationHandling = true;
    
    [Header("Breakpoint Settings")]
    public float mobileMaxWidth = 767f;
    public float tabletMaxWidth = 1023f;
    public float desktopMinWidth = 1024f;
    public float desktopLargeMinWidth = 1920f;
    
    [Header("Scaling Factors")]
    public float mobileScale = 0.8f;
    public float tabletScale = 0.9f;
    public float desktopScale = 1.0f;
    public float desktopLargeScale = 1.1f;
    
    [Header("Orientation Settings")]
    public float portraitScale = 1.0f;
    public float landscapeScale = 1.2f;
    public bool autoRotateUI = true;
    
    [Header("Safe Area Handling")]
    public bool enableSafeAreaHandling = true;
    public RectTransform safeAreaContainer;
    public Vector2 safeAreaPadding = new Vector2(16f, 16f);
    
    [Header("Component References")]
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public RectTransform mainContainer;
    public RectTransform headerContainer;
    public RectTransform contentContainer;
    public RectTransform footerContainer;
    
    // Current state
    private DeviceType currentDeviceType = DeviceType.Desktop;
    private ScreenOrientation currentOrientation = ScreenOrientation.Portrait;
    private float currentScreenWidth;
    private float currentScreenHeight;
    private float currentScale = 1.0f;
    private Vector2 currentSafeArea = Vector2.zero;
    
    // Component tracking
    private List<ResponsiveComponent> responsiveComponents = new List<ResponsiveComponent>();
    private List<ResponsiveText> responsiveTexts = new List<ResponsiveText>();
    private List<ResponsiveImage> responsiveImages = new List<ResponsiveImage>();
    private List<ResponsiveLayoutGroup> responsiveLayoutGroups = new List<ResponsiveLayoutGroup>();
    
    // Layout groups
    private Dictionary<DeviceType, LayoutConfiguration> layoutConfigurations;
    
    public enum DeviceType
    {
        Mobile,
        Tablet,
        Desktop,
        DesktopLarge
    }
    
    [System.Serializable]
    public class ResponsiveComponent
    {
        public RectTransform rectTransform;
        public Vector2 originalSize;
        public Vector2 originalPosition;
        public Vector2 originalAnchorMin;
        public Vector2 originalAnchorMax;
        public Vector2 originalPivot;
        public DeviceType[] hideOnDevices;
        public bool preserveAspectRatio = true;
        public bool enableDynamicScaling = true;
        public float customScaleMultiplier = 1.0f;
    }
    
    [System.Serializable]
    public class ResponsiveText
    {
        public TextMeshProUGUI textComponent;
        public int baseFontSize;
        public bool enableAutoScaling = true;
        public int minFontSize = 8;
        public int maxFontSize = 72;
        public bool wrapText = true;
        public float lineHeightMultiplier = 1.0f;
        public DeviceType[] hideOnDevices;
    }
    
    [System.Serializable]
    public class ResponsiveImage
    {
        public Image imageComponent;
        public bool preserveAspectRatio = true;
        public bool enableDynamicScaling = true;
        public float customScaleMultiplier = 1.0f;
        public DeviceType[] hideOnDevices;
        public Sprite mobileSprite;
        public Sprite tabletSprite;
        public Sprite desktopSprite;
    }
    
    [System.Serializable]
    public class ResponsiveLayoutGroup
    {
        public LayoutGroup layoutGroup;
        public float mobileSpacing = 8f;
        public float tabletSpacing = 12f;
        public float desktopSpacing = 16f;
        public TextAnchor mobileChildAlignment = TextAnchor.MiddleCenter;
        public TextAnchor tabletChildAlignment = TextAnchor.MiddleCenter;
        public TextAnchor desktopChildAlignment = TextAnchor.MiddleCenter;
        public bool enableDynamicPadding = true;
        public Vector2 mobilePadding = new Vector2(8f, 8f);
        public Vector2 tabletPadding = new Vector2(12f, 12f);
        public Vector2 desktopPadding = new Vector2(16f, 16f);
    }
    
    [System.Serializable]
    public class LayoutConfiguration
    {
        public DeviceType deviceType;
        public Vector2 containerSize;
        public Vector2 containerPosition;
        public Vector2 containerAnchor;
        public Vector2 containerPivot;
        public float scaleMultiplier;
        public bool showHeader;
        public bool showFooter;
        public bool showSideNavigation;
        public bool showBottomNavigation;
        public TextAnchor contentAlignment;
    }
    
    void Awake()
    {
        InitializeResponsiveSystem();
        SetupLayoutConfigurations();
        CollectResponsiveComponents();
    }
    
    void Start()
    {
        UpdateResponsiveLayout();
        ApplyInitialLayout();
    }
    
    void Update()
    {
        if (enableResponsiveLayout)
        {
            CheckScreenChanges();
        }
        
        if (enableOrientationHandling)
        {
            CheckOrientationChanges();
        }
    }
    
    /// <summary>
    /// Initialize the responsive layout system
    /// </summary>
    private void InitializeResponsiveSystem()
    {
        if (designSystem == null)
        {
            designSystem = ScriptableObject.CreateInstance<BrawlStarsDesignSystem>();
            Debug.LogWarning("Design system not assigned, creating default instance");
        }
        
        // Setup canvas and canvas scaler
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
        }
        
        if (canvasScaler == null)
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (canvasScaler == null)
            {
                canvasScaler = gameObject.AddComponent<CanvasScaler>();
            }
        }
        
        // Configure canvas scaler for responsive design
        ConfigureCanvasScaler();
        
        // Initialize current screen dimensions
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
        currentOrientation = Screen.orientation;
        
        Debug.Log($"Responsive layout system initialized: {currentScreenWidth}x{currentScreenHeight}");
    }
    
    /// <summary>
    /// Configure canvas scaler for optimal responsive design
    /// </summary>
    private void ConfigureCanvasScaler()
    {
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f); // Base resolution
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f; // Balance between width and height
        
        // Enable pixel perfect for mobile devices
        canvas.pixelPerfect = (currentDeviceType == DeviceType.Mobile);
    }
    
    /// <summary>
    /// Setup layout configurations for different device types
    /// </summary>
    private void SetupLayoutConfigurations()
    {
        layoutConfigurations = new Dictionary<DeviceType, LayoutConfiguration>
        {
            {
                DeviceType.Mobile,
                new LayoutConfiguration
                {
                    deviceType = DeviceType.Mobile,
                    containerSize = new Vector2(360f, 640f),
                    containerPosition = Vector2.zero,
                    containerAnchor = new Vector2(0.5f, 0.5f),
                    containerPivot = new Vector2(0.5f, 0.5f),
                    scaleMultiplier = mobileScale,
                    showHeader = true,
                    showFooter = true,
                    showSideNavigation = false,
                    showBottomNavigation = true,
                    contentAlignment = TextAnchor.MiddleCenter
                }
            },
            {
                DeviceType.Tablet,
                new LayoutConfiguration
                {
                    deviceType = DeviceType.Tablet,
                    containerSize = new Vector2(768f, 1024f),
                    containerPosition = Vector2.zero,
                    containerAnchor = new Vector2(0.5f, 0.5f),
                    containerPivot = new Vector2(0.5f, 0.5f),
                    scaleMultiplier = tabletScale,
                    showHeader = true,
                    showFooter = true,
                    showSideNavigation = true,
                    showBottomNavigation = false,
                    contentAlignment = TextAnchor.MiddleCenter
                }
            },
            {
                DeviceType.Desktop,
                new LayoutConfiguration
                {
                    deviceType = DeviceType.Desktop,
                    containerSize = new Vector2(1920f, 1080f),
                    containerPosition = Vector2.zero,
                    containerAnchor = new Vector2(0.5f, 0.5f),
                    containerPivot = new Vector2(0.5f, 0.5f),
                    scaleMultiplier = desktopScale,
                    showHeader = true,
                    showFooter = true,
                    showSideNavigation = true,
                    showBottomNavigation = false,
                    contentAlignment = TextAnchor.MiddleCenter
                }
            },
            {
                DeviceType.DesktopLarge,
                new LayoutConfiguration
                {
                    deviceType = DeviceType.DesktopLarge,
                    containerSize = new Vector2(2560f, 1440f),
                    containerPosition = Vector2.zero,
                    containerAnchor = new Vector2(0.5f, 0.5f),
                    containerPivot = new Vector2(0.5f, 0.5f),
                    scaleMultiplier = desktopLargeScale,
                    showHeader = true,
                    showFooter = true,
                    showSideNavigation = true,
                    showBottomNavigation = false,
                    contentAlignment = TextAnchor.MiddleCenter
                }
            }
        };
    }
    
    /// <summary>
    /// Collect all responsive components in the hierarchy
    /// </summary>
    private void CollectResponsiveComponents()
    {
        responsiveComponents.Clear();
        responsiveTexts.Clear();
        responsiveImages.Clear();
        responsiveLayoutGroups.Clear();
        
        // Collect responsive components
        var allRectTransforms = GetComponentsInChildren<RectTransform>(true);
        foreach (var rectTransform in allRectTransforms)
        {
            if (rectTransform != mainContainer && rectTransform != transform)
            {
                var responsiveComponent = CreateResponsiveComponent(rectTransform);
                if (responsiveComponent != null)
                {
                    responsiveComponents.Add(responsiveComponent);
                }
            }
        }
        
        // Collect responsive texts
        var allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in allTexts)
        {
            var responsiveText = CreateResponsiveText(text);
            if (responsiveText != null)
            {
                responsiveTexts.Add(responsiveText);
            }
        }
        
        // Collect responsive images
        var allImages = GetComponentsInChildren<Image>(true);
        foreach (var image in allImages)
        {
            var responsiveImage = CreateResponsiveImage(image);
            if (responsiveImage != null)
            {
                responsiveImages.Add(responsiveImage);
            }
        }
        
        // Collect responsive layout groups
        var allLayoutGroups = GetComponentsInChildren<LayoutGroup>(true);
        foreach (var layoutGroup in allLayoutGroups)
        {
            var responsiveLayoutGroup = CreateResponsiveLayoutGroup(layoutGroup);
            if (responsiveLayoutGroup != null)
            {
                responsiveLayoutGroups.Add(responsiveLayoutGroup);
            }
        }
        
        Debug.Log($"Collected {responsiveComponents.Count} components, {responsiveTexts.Count} texts, {responsiveImages.Count} images, {responsiveLayoutGroups.Count} layout groups");
    }
    
    /// <summary>
    /// Create responsive component data
    /// </summary>
    private ResponsiveComponent CreateResponsiveComponent(RectTransform rectTransform)
    {
        return new ResponsiveComponent
        {
            rectTransform = rectTransform,
            originalSize = rectTransform.sizeDelta,
            originalPosition = rectTransform.anchoredPosition,
            originalAnchorMin = rectTransform.anchorMin,
            originalAnchorMax = rectTransform.anchorMax,
            originalPivot = rectTransform.pivot,
            hideOnDevices = new DeviceType[0],
            preserveAspectRatio = true,
            enableDynamicScaling = true,
            customScaleMultiplier = 1.0f
        };
    }
    
    /// <summary>
    /// Create responsive text data
    /// </summary>
    private ResponsiveText CreateResponsiveText(TextMeshProUGUI textComponent)
    {
        return new ResponsiveText
        {
            textComponent = textComponent,
            baseFontSize = Mathf.RoundToInt(textComponent.fontSize),
            enableAutoScaling = true,
            minFontSize = 8,
            maxFontSize = 72,
            wrapText = true,
            lineHeightMultiplier = 1.0f,
            hideOnDevices = new DeviceType[0]
        };
    }
    
    /// <summary>
    /// Create responsive image data
    /// </summary>
    private ResponsiveImage CreateResponsiveImage(Image imageComponent)
    {
        return new ResponsiveImage
        {
            imageComponent = imageComponent,
            preserveAspectRatio = true,
            enableDynamicScaling = true,
            customScaleMultiplier = 1.0f,
            hideOnDevices = new DeviceType[0],
            mobileSprite = null,
            tabletSprite = null,
            desktopSprite = null
        };
    }
    
    /// <summary>
    /// Create responsive layout group data
    /// </summary>
    private ResponsiveLayoutGroup CreateResponsiveLayoutGroup(LayoutGroup layoutGroup)
    {
        float baseSpacing = 16f;
        if (layoutGroup is HorizontalLayoutGroup horizontalGroup)
        {
            baseSpacing = horizontalGroup.spacing;
        }
        else if (layoutGroup is VerticalLayoutGroup verticalGroup)
        {
            baseSpacing = verticalGroup.spacing;
        }
        else if (layoutGroup is GridLayoutGroup gridGroup)
        {
            baseSpacing = gridGroup.spacing.x; // Use X spacing as base
        }
        
        return new ResponsiveLayoutGroup
        {
            layoutGroup = layoutGroup,
            mobileSpacing = baseSpacing * 0.75f,
            tabletSpacing = baseSpacing * 0.875f,
            desktopSpacing = baseSpacing,
            mobileChildAlignment = TextAnchor.MiddleCenter,
            tabletChildAlignment = TextAnchor.MiddleCenter,
            desktopChildAlignment = TextAnchor.MiddleCenter,
            enableDynamicPadding = true,
            mobilePadding = new Vector2(8f, 8f),
            tabletPadding = new Vector2(12f, 12f),
            desktopPadding = new Vector2(16f, 16f)
        };
    }
    
    /// <summary>
    /// Check for screen size changes
    /// </summary>
    private void CheckScreenChanges()
    {
        if (Mathf.Abs(currentScreenWidth - Screen.width) > 1f || Mathf.Abs(currentScreenHeight - Screen.height) > 1f)
        {
            currentScreenWidth = Screen.width;
            currentScreenHeight = Screen.height;
            
            DeviceType newDeviceType = GetDeviceType(currentScreenWidth);
            if (newDeviceType != currentDeviceType)
            {
                currentDeviceType = newDeviceType;
                UpdateResponsiveLayout();
                
                Debug.Log($"Device type changed to: {currentDeviceType} ({currentScreenWidth}x{currentScreenHeight})");
            }
        }
    }
    
    /// <summary>
    /// Check for orientation changes
    /// </summary>
    private void CheckOrientationChanges()
    {
        ScreenOrientation newOrientation = Screen.orientation;
        if (newOrientation != currentOrientation)
        {
            currentOrientation = newOrientation;
            UpdateOrientationLayout();
            
            Debug.Log($"Orientation changed to: {currentOrientation}");
        }
    }
    
    /// <summary>
    /// Get device type based on screen width
    /// </summary>
    private DeviceType GetDeviceType(float screenWidth)
    {
        if (screenWidth <= mobileMaxWidth)
        {
            return DeviceType.Mobile;
        }
        else if (screenWidth <= tabletMaxWidth)
        {
            return DeviceType.Tablet;
        }
        else if (screenWidth >= desktopLargeMinWidth)
        {
            return DeviceType.DesktopLarge;
        }
        else
        {
            return DeviceType.Desktop;
        }
    }
    
    /// <summary>
    /// Update responsive layout based on current device type
    /// </summary>
    private void UpdateResponsiveLayout()
    {
        if (!enableResponsiveLayout) return;
        
        currentScale = GetScaleForDevice(currentDeviceType);
        
        // Update main container
        UpdateMainContainer();
        
        // Update responsive components
        UpdateResponsiveComponents();
        
        // Update responsive texts
        UpdateResponsiveTexts();
        
        // Update responsive images
        UpdateResponsiveImages();
        
        // Update responsive layout groups
        UpdateResponsiveLayoutGroups();
        
        // Update safe area
        UpdateSafeArea();
        
        // Update canvas scaler
        UpdateCanvasScaler();
        
        Debug.Log($"Responsive layout updated for {currentDeviceType} with scale {currentScale}");
    }
    
    /// <summary>
    /// Get scale multiplier for device type
    /// </summary>
    private float GetScaleForDevice(DeviceType deviceType)
    {
        switch (deviceType)
        {
            case DeviceType.Mobile:
                return mobileScale;
            case DeviceType.Tablet:
                return tabletScale;
            case DeviceType.Desktop:
                return desktopScale;
            case DeviceType.DesktopLarge:
                return desktopLargeScale;
            default:
                return 1.0f;
        }
    }
    
    /// <summary>
    /// Update main container based on device type
    /// </summary>
    private void UpdateMainContainer()
    {
        if (mainContainer == null) return;
        
        var config = layoutConfigurations[currentDeviceType];
        
        mainContainer.anchorMin = config.containerAnchor;
        mainContainer.anchorMax = config.containerAnchor;
        mainContainer.pivot = config.containerPivot;
        mainContainer.anchoredPosition = config.containerPosition;
        mainContainer.sizeDelta = config.containerSize * currentScale;
    }
    
    /// <summary>
    /// Update responsive components
    /// </summary>
    private void UpdateResponsiveComponents()
    {
        foreach (var component in responsiveComponents)
        {
            if (component.rectTransform == null) continue;
            
            // Check if component should be hidden on current device
            if (ShouldHideOnDevice(component.hideOnDevices, currentDeviceType))
            {
                component.rectTransform.gameObject.SetActive(false);
                continue;
            }
            else
            {
                component.rectTransform.gameObject.SetActive(true);
            }
            
            if (component.enableDynamicScaling)
            {
                float scale = currentScale * component.customScaleMultiplier;
                
                // Update size
                component.rectTransform.sizeDelta = component.originalSize * scale;
                
                // Update position (maintain relative positioning)
                component.rectTransform.anchoredPosition = component.originalPosition * scale;
                
                // Update anchors if needed
                if (currentDeviceType == DeviceType.Mobile)
                {
                    // Adjust anchors for mobile layout
                    component.rectTransform.anchorMin = new Vector2(0.1f, component.originalAnchorMin.y);
                    component.rectTransform.anchorMax = new Vector2(0.9f, component.originalAnchorMax.y);
                }
                else
                {
                    // Restore original anchors for larger screens
                    component.rectTransform.anchorMin = component.originalAnchorMin;
                    component.rectTransform.anchorMax = component.originalAnchorMax;
                }
            }
        }
    }
    
    /// <summary>
    /// Update responsive texts
    /// </summary>
    private void UpdateResponsiveTexts()
    {
        foreach (var responsiveText in responsiveTexts)
        {
            if (responsiveText.textComponent == null) continue;
            
            // Check if text should be hidden on current device
            if (ShouldHideOnDevice(responsiveText.hideOnDevices, currentDeviceType))
            {
                responsiveText.textComponent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                responsiveText.textComponent.gameObject.SetActive(true);
            }
            
            if (responsiveText.enableAutoScaling)
            {
                float scale = currentScale;
                int newFontSize = Mathf.RoundToInt(responsiveText.baseFontSize * scale);
                
                // Clamp font size within min/max bounds
                newFontSize = Mathf.Clamp(newFontSize, responsiveText.minFontSize, responsiveText.maxFontSize);
                
                responsiveText.textComponent.fontSize = newFontSize;
                responsiveText.textComponent.lineSpacing = responsiveText.lineHeightMultiplier;
                
                // Enable text wrapping for mobile devices
                if (currentDeviceType == DeviceType.Mobile)
                {
                    responsiveText.textComponent.textWrappingMode = responsiveText.wrapText ? TMPro.TextWrappingModes.Normal : TMPro.TextWrappingModes.NoWrap;
                    responsiveText.textComponent.overflowMode = TextOverflowModes.Truncate;
                }
                else
                {
                    responsiveText.textComponent.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
                    responsiveText.textComponent.overflowMode = TextOverflowModes.Overflow;
                }
            }
        }
    }
    
    /// <summary>
    /// Update responsive images
    /// </summary>
    private void UpdateResponsiveImages()
    {
        foreach (var responsiveImage in responsiveImages)
        {
            if (responsiveImage.imageComponent == null) continue;
            
            // Check if image should be hidden on current device
            if (ShouldHideOnDevice(responsiveImage.hideOnDevices, currentDeviceType))
            {
                responsiveImage.imageComponent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                responsiveImage.imageComponent.gameObject.SetActive(true);
            }
            
            if (responsiveImage.enableDynamicScaling)
            {
                float scale = currentScale * responsiveImage.customScaleMultiplier;
                
                // Update image scale
                responsiveImage.imageComponent.transform.localScale = Vector3.one * scale;
                
                // Update sprite based on device type
                UpdateImageSprite(responsiveImage);
                
                // Preserve aspect ratio
                if (responsiveImage.preserveAspectRatio)
                {
                    responsiveImage.imageComponent.preserveAspect = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Update image sprite based on device type
    /// </summary>
    private void UpdateImageSprite(ResponsiveImage responsiveImage)
    {
        Sprite targetSprite = null;
        
        switch (currentDeviceType)
        {
            case DeviceType.Mobile:
                targetSprite = responsiveImage.mobileSprite;
                break;
            case DeviceType.Tablet:
                targetSprite = responsiveImage.tabletSprite;
                break;
            case DeviceType.Desktop:
            case DeviceType.DesktopLarge:
                targetSprite = responsiveImage.desktopSprite;
                break;
        }
        
        // Use target sprite if available, otherwise keep current sprite
        if (targetSprite != null)
        {
            responsiveImage.imageComponent.sprite = targetSprite;
        }
    }
    
    /// <summary>
    /// Update responsive layout groups
    /// </summary>
    private void UpdateResponsiveLayoutGroups()
    {
        foreach (var responsiveLayoutGroup in responsiveLayoutGroups)
        {
            if (responsiveLayoutGroup.layoutGroup == null) continue;
            
            float spacing = 0f;
            TextAnchor childAlignment = TextAnchor.MiddleCenter;
            Vector2 padding = Vector2.zero;
            
            // Get device-specific values
            switch (currentDeviceType)
            {
                case DeviceType.Mobile:
                    spacing = responsiveLayoutGroup.mobileSpacing;
                    childAlignment = responsiveLayoutGroup.mobileChildAlignment;
                    padding = responsiveLayoutGroup.mobilePadding;
                    break;
                case DeviceType.Tablet:
                    spacing = responsiveLayoutGroup.tabletSpacing;
                    childAlignment = responsiveLayoutGroup.tabletChildAlignment;
                    padding = responsiveLayoutGroup.tabletPadding;
                    break;
                case DeviceType.Desktop:
                case DeviceType.DesktopLarge:
                    spacing = responsiveLayoutGroup.desktopSpacing;
                    childAlignment = responsiveLayoutGroup.desktopChildAlignment;
                    padding = responsiveLayoutGroup.desktopPadding;
                    break;
            }
            
            // Apply spacing
            if (responsiveLayoutGroup.layoutGroup is HorizontalLayoutGroup horizontalGroup)
            {
                horizontalGroup.spacing = spacing;
                horizontalGroup.childAlignment = childAlignment;
                
                if (responsiveLayoutGroup.enableDynamicPadding)
                {
                    horizontalGroup.padding = new RectOffset(
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.y),
                        Mathf.RoundToInt(padding.y)
                    );
                }
            }
            else if (responsiveLayoutGroup.layoutGroup is VerticalLayoutGroup verticalGroup)
            {
                verticalGroup.spacing = spacing;
                verticalGroup.childAlignment = childAlignment;
                
                if (responsiveLayoutGroup.enableDynamicPadding)
                {
                    verticalGroup.padding = new RectOffset(
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.y),
                        Mathf.RoundToInt(padding.y)
                    );
                }
            }
            else if (responsiveLayoutGroup.layoutGroup is GridLayoutGroup gridGroup)
            {
                gridGroup.spacing = new Vector2(spacing, spacing);
                
                if (responsiveLayoutGroup.enableDynamicPadding)
                {
                    gridGroup.padding = new RectOffset(
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.x),
                        Mathf.RoundToInt(padding.y),
                        Mathf.RoundToInt(padding.y)
                    );
                }
            }
        }
    }
    
    /// <summary>
    /// Update safe area for notched devices
    /// </summary>
    private void UpdateSafeArea()
    {
        if (!enableSafeAreaHandling || safeAreaContainer == null) return;
        
        Rect safeArea = Screen.safeArea;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        
        // Convert safe area to normalized coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= screenSize.x;
        anchorMin.y /= screenSize.y;
        anchorMax.x /= screenSize.x;
        anchorMax.y /= screenSize.y;
        
        // Apply padding
        float paddingX = safeAreaPadding.x / screenSize.x;
        float paddingY = safeAreaPadding.y / screenSize.y;
        
        anchorMin.x += paddingX;
        anchorMin.y += paddingY;
        anchorMax.x -= paddingX;
        anchorMax.y -= paddingY;
        
        // Apply to safe area container
        safeAreaContainer.anchorMin = anchorMin;
        safeAreaContainer.anchorMax = anchorMax;
        safeAreaContainer.anchoredPosition = Vector2.zero;
        safeAreaContainer.sizeDelta = Vector2.zero;
    }
    
    /// <summary>
    /// Update canvas scaler based on device type
    /// </summary>
    private void UpdateCanvasScaler()
    {
        if (canvasScaler == null) return;
        
        // Adjust canvas scaler based on device type
        switch (currentDeviceType)
        {
            case DeviceType.Mobile:
                canvasScaler.referenceResolution = new Vector2(750f, 1334f); // iPhone resolution
                canvasScaler.matchWidthOrHeight = 0.5f;
                break;
            case DeviceType.Tablet:
                canvasScaler.referenceResolution = new Vector2(1024f, 768f); // iPad resolution
                canvasScaler.matchWidthOrHeight = 0.5f;
                break;
            case DeviceType.Desktop:
                canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
                canvasScaler.matchWidthOrHeight = 0.5f;
                break;
            case DeviceType.DesktopLarge:
                canvasScaler.referenceResolution = new Vector2(2560f, 1440f);
                canvasScaler.matchWidthOrHeight = 0.5f;
                break;
        }
    }
    
    /// <summary>
    /// Update layout for orientation changes
    /// </summary>
    private void UpdateOrientationLayout()
    {
        float orientationScale = (currentOrientation == ScreenOrientation.LandscapeLeft || currentOrientation == ScreenOrientation.LandscapeLeft || currentOrientation == ScreenOrientation.LandscapeRight) ? landscapeScale : portraitScale;
        
        // Apply orientation-specific adjustments
        foreach (var component in responsiveComponents)
        {
            if (component.rectTransform == null) continue;
            
            // Adjust anchors and positioning for landscape vs portrait
            if (currentOrientation == ScreenOrientation.LandscapeLeft || currentOrientation == ScreenOrientation.LandscapeLeft || currentOrientation == ScreenOrientation.LandscapeRight)
            {
                // Landscape layout adjustments
                if (currentDeviceType == DeviceType.Mobile)
                {
                    // Use side-by-side layout for landscape mobile
                    component.rectTransform.anchorMin = new Vector2(0.05f, component.originalAnchorMin.y);
                    component.rectTransform.anchorMax = new Vector2(0.45f, component.originalAnchorMax.y);
                }
            }
            else
            {
                // Portrait layout adjustments
                component.rectTransform.anchorMin = component.originalAnchorMin;
                component.rectTransform.anchorMax = component.originalAnchorMax;
            }
        }
        
        Debug.Log($"Orientation layout updated for {currentOrientation}");
    }
    
    /// <summary>
    /// Check if component should be hidden on specific device
    /// </summary>
    private bool ShouldHideOnDevice(DeviceType[] hideOnDevices, DeviceType currentDevice)
    {
        foreach (var deviceType in hideOnDevices)
        {
            if (deviceType == currentDevice)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Apply initial layout based on current device
    /// </summary>
    private void ApplyInitialLayout()
    {
        UpdateResponsiveLayout();
        UpdateOrientationLayout();
        
        Debug.Log($"Initial layout applied for {currentDeviceType} device");
    }
    
    /// <summary>
    /// Add responsive component manually
    /// </summary>
    public void AddResponsiveComponent(RectTransform rectTransform)
    {
        var responsiveComponent = CreateResponsiveComponent(rectTransform);
        responsiveComponents.Add(responsiveComponent);
        
        // Update layout immediately
        UpdateResponsiveLayout();
    }
    
    /// <summary>
    /// Remove responsive component
    /// </summary>
    public void RemoveResponsiveComponent(RectTransform rectTransform)
    {
        responsiveComponents.RemoveAll(c => c.rectTransform == rectTransform);
        UpdateResponsiveLayout();
    }
    
    /// <summary>
    /// Force update responsive layout
    /// </summary>
    public void ForceUpdateLayout()
    {
        UpdateResponsiveLayout();
    }
    
    /// <summary>
    /// Get current device type
    /// </summary>
    public DeviceType GetCurrentDeviceType()
    {
        return currentDeviceType;
    }
    
    /// <summary>
    /// Get current scale
    /// </summary>
    public float GetCurrentScale()
    {
        return currentScale;
    }
    
    /// <summary>
    /// Get current orientation
    /// </summary>
    public ScreenOrientation GetCurrentOrientation()
    {
        return currentOrientation;
    }
    
    /// <summary>
    /// Get responsive component data
    /// </summary>
    public ResponsiveComponent GetResponsiveComponent(RectTransform rectTransform)
    {
        return responsiveComponents.Find(c => c.rectTransform == rectTransform);
    }
    
    /// <summary>
    /// Get all responsive components
    /// </summary>
    public List<ResponsiveComponent> GetAllResponsiveComponents()
    {
        return new List<ResponsiveComponent>(responsiveComponents);
    }
}