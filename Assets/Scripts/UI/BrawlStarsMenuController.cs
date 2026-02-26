using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Brawl Stars Menu Controller - comprehensive menu system with authentic Brawl Stars UI/UX
/// including responsive design, animations, and cultural integration for LakbayTala.
/// </summary>
public class BrawlStarsMenuController : MonoBehaviour
{
    [Header("Design System")]
    public BrawlStarsDesignSystem designSystem;
    public BrawlStarsUIConfig uiConfig;
    
    [Header("Main Menu Panels")]
    public Transform mainMenuContainer;
    public Transform mainMenuPanel;
    public Transform playPanel;
    public Transform settingsPanel;
    public Transform shopPanel;
    public Transform profilePanel;
    public Transform leaderboardPanel;
    public Transform achievementsPanel;
    public Transform culturalPanel;
    
    [Header("Navigation Elements")]
    public Transform navigationBar;
    public Transform bottomNavigation;
    public Transform sideNavigation;
    public Button backButton;
    public Button homeButton;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button shopButton;
    public Button profileButton;
    public Button leaderboardButton;
    public Button achievementsButton;
    public Button culturalButton;
    
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image backgroundImage;
    public Image headerImage;
    public Image footerImage;
    
    [Header("Animation Settings")]
    public float panelTransitionDuration = 0.4f;
    public float buttonAnimationDuration = 0.2f;
    public float elementFadeDuration = 0.25f;
    public Ease panelEase = Ease.OutCubic;
    public Ease buttonEase = Ease.OutBack;
    public Ease elementEase = Ease.OutQuad;
    
    [Header("Responsive Design")]
    public bool enableResponsiveDesign = true;
    public float mobileBreakpoint = 768f;
    public float tabletBreakpoint = 1024f;
    public float desktopBreakpoint = 1920f;
    
    [Header("Cultural Integration")]
    public bool enableCulturalElements = true;
    public Image culturalPatternOverlay;
    public TextMeshProUGUI baybayinText;
    public Image[] culturalDecorations;
    
    [Header("Performance Settings")]
    public bool enablePerformanceOptimization = true;
    public int targetFrameRate = 60;
    public bool enableObjectPooling = true;
    public bool enableLazyLoading = true;
    
    // Menu state management
    private MenuState currentMenuState = MenuState.MainMenu;
    private MenuState previousMenuState = MenuState.None;
    private Stack<MenuState> navigationStack = new Stack<MenuState>();
    private Dictionary<MenuState, Transform> panelDictionary;
    private Dictionary<Button, Vector3> originalButtonScales = new Dictionary<Button, Vector3>();
    
    // UI element references for easy access
    private List<Button> allMenuButtons = new List<Button>();
    private List<TextMeshProUGUI> allMenuTexts = new List<TextMeshProUGUI>();
    private List<Image> allMenuImages = new List<Image>();
    private List<Transform> allPanels = new List<Transform>();
    
    // Animation sequences
    private Sequence currentTransitionSequence;
    private Sequence currentButtonSequence;
    private Dictionary<Button, Sequence> buttonAnimationSequences = new Dictionary<Button, Sequence>();
    
    // Responsive state
    private DeviceType currentDeviceType = DeviceType.Desktop;
    private float currentScreenWidth;
    private float currentScreenHeight;
    
    public enum MenuState
    {
        None,
        MainMenu,
        Play,
        Settings,
        Shop,
        Profile,
        Leaderboard,
        Achievements,
        Cultural,
        Loading
    }
    
    public enum DeviceType
    {
        Mobile,
        Tablet,
        Desktop
    }
    
    // Events
    public System.Action<MenuState> OnMenuStateChanged;
    public System.Action<MenuState, MenuState> OnMenuTransition;
    public System.Action<DeviceType> OnDeviceTypeChanged;
    
    void Awake()
    {
        InitializeMenuSystem();
        SetupPanelDictionary();
        CollectUIElements();
        SetupResponsiveDesign();
        SetupPerformanceOptimizations();
    }
    
    void Start()
    {
        ApplyBrawlStarsStyling();
        SetupButtonAnimations();
        SetupCulturalElements();
        TransitionToState(MenuState.MainMenu, false);
    }
    
    void Update()
    {
        if (enableResponsiveDesign)
        {
            CheckScreenSizeChanges();
        }
    }
    
    /// <summary>
    /// Initialize the complete menu system with Brawl Stars styling
    /// </summary>
    private void InitializeMenuSystem()
    {
        if (designSystem == null)
        {
            designSystem = ScriptableObject.CreateInstance<BrawlStarsDesignSystem>();
            Debug.LogWarning("Design system not assigned, creating default instance");
        }
        
        if (uiConfig == null)
        {
            uiConfig = ScriptableObject.CreateInstance<BrawlStarsUIConfig>();
            Debug.LogWarning("UI config not assigned, creating default instance");
        }
        
        // Set target frame rate
        if (enablePerformanceOptimization)
        {
            Application.targetFrameRate = targetFrameRate;
        }
        
        // Initialize navigation stack
        navigationStack.Clear();
        
        Debug.Log("Brawl Stars Menu Controller initialized");
    }
    
    /// <summary>
    /// Setup panel dictionary for easy state management
    /// </summary>
    private void SetupPanelDictionary()
    {
        panelDictionary = new Dictionary<MenuState, Transform>
        {
            { MenuState.MainMenu, mainMenuContainer },
            { MenuState.Play, playPanel },
            { MenuState.Settings, settingsPanel },
            { MenuState.Shop, shopPanel },
            { MenuState.Profile, profilePanel },
            { MenuState.Leaderboard, leaderboardPanel },
            { MenuState.Achievements, achievementsPanel },
            { MenuState.Cultural, culturalPanel }
        };
        
        // Collect all panels for easy management
        allPanels.Clear();
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null)
            {
                allPanels.Add(panel);
            }
        }
    }
    
    /// <summary>
    /// Collect all UI elements for batch processing
    /// </summary>
    private void CollectUIElements()
    {
        // Collect buttons
        allMenuButtons.Clear();
        allMenuButtons.AddRange(GetComponentsInChildren<Button>(true));
        
        // Collect text elements
        allMenuTexts.Clear();
        allMenuTexts.AddRange(GetComponentsInChildren<TextMeshProUGUI>(true));
        
        // Collect images
        allMenuImages.Clear();
        allMenuImages.AddRange(GetComponentsInChildren<Image>(true));
        
        Debug.Log($"Collected {allMenuButtons.Count} buttons, {allMenuTexts.Count} texts, {allMenuImages.Count} images");
    }
    
    /// <summary>
    /// Setup responsive design system
    /// </summary>
    private void SetupResponsiveDesign()
    {
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
        currentDeviceType = GetDeviceType(currentScreenWidth);
        
        Debug.Log($"Initial device type: {currentDeviceType} ({currentScreenWidth}x{currentScreenHeight})");
    }
    
    /// <summary>
    /// Setup performance optimizations
    /// </summary>
    private void SetupPerformanceOptimizations()
    {
        if (enableObjectPooling)
        {
            // Setup object pooling for UI elements
            SetupUIElementPooling();
        }
        
        if (enableLazyLoading)
        {
            // Setup lazy loading for panels
            SetupLazyLoading();
        }
        
        // Optimize canvas settings
        OptimizeCanvasSettings();
    }
    
    /// <summary>
    /// Apply comprehensive Brawl Stars styling to all UI elements
    /// </summary>
    private void ApplyBrawlStarsStyling()
    {
        if (designSystem == null) return;
        
        // Apply background styling
        ApplyBackgroundStyling();
        
        // Apply button styling
        ApplyButtonStyling();
        
        // Apply text styling
        ApplyTextStyling();
        
        // Apply panel styling
        ApplyPanelStyling();
        
        // Apply cultural elements
        if (enableCulturalElements)
        {
            ApplyCulturalStyling();
        }
        
        Debug.Log("Brawl Stars styling applied");
    }
    
    /// <summary>
    /// Apply background styling with Brawl Stars theme
    /// </summary>
    private void ApplyBackgroundStyling()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = designSystem.colors.backgroundDark;
            
            // Gradient (UnityEngine.UI.Gradient not in standard Unity - use color only)
            backgroundImage.color = Color.Lerp(designSystem.colors.backgroundDark, designSystem.colors.backgroundMedium, 0.5f);
        }
    }
    
    /// <summary>
    /// Apply Brawl Stars button styling with proper states
    /// </summary>
    private void ApplyButtonStyling()
    {
        foreach (var button in allMenuButtons)
        {
            if (button == null) continue;
            
            var buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                // Apply button styling based on button type
                if (button == playButton)
                {
                    ApplyPrimaryButtonStyling(button, buttonImage);
                }
                else if (button == settingsButton || button == shopButton)
                {
                    ApplySecondaryButtonStyling(button, buttonImage);
                }
                else
                {
                    ApplyStandardButtonStyling(button, buttonImage);
                }
            }
            
            // Apply text styling for button text
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                ApplyButtonTextStyling(buttonText);
            }
        }
    }
    
    /// <summary>
    /// Apply primary button styling (gold theme)
    /// </summary>
    private void ApplyPrimaryButtonStyling(Button button, Image buttonImage)
    {
        var colors = new ColorBlock
        {
            normalColor = designSystem.colors.goldPrimary,
            highlightedColor = designSystem.colors.goldSecondary,
            pressedColor = designSystem.colors.goldAccent,
            selectedColor = designSystem.colors.goldPrimary,
            disabledColor = Color.Lerp(designSystem.colors.goldPrimary, Color.gray, 0.5f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        
        button.colors = colors;
        
        // Add shadow effect
        var shadow = button.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = button.gameObject.AddComponent<Shadow>();
        }
        shadow.effectColor = new Color(0f, 0f, 0f, 0.3f);
        shadow.effectDistance = new Vector2(2f, -2f);
        
        // Add outline
        var outline = button.GetComponent<Outline>();
        if (outline == null)
        {
            outline = button.gameObject.AddComponent<Outline>();
        }
        outline.effectColor = designSystem.colors.goldAccent;
        outline.effectDistance = new Vector2(1f, -1f);
    }
    
    /// <summary>
    /// Apply secondary button styling (blue theme)
    /// </summary>
    private void ApplySecondaryButtonStyling(Button button, Image buttonImage)
    {
        var colors = new ColorBlock
        {
            normalColor = designSystem.colors.bluePrimary,
            highlightedColor = designSystem.colors.blueSecondary,
            pressedColor = designSystem.colors.blueAccent,
            selectedColor = designSystem.colors.bluePrimary,
            disabledColor = Color.Lerp(designSystem.colors.bluePrimary, Color.gray, 0.5f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        
        button.colors = colors;
        
        // Add subtle shadow
        var shadow = button.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = button.gameObject.AddComponent<Shadow>();
        }
        shadow.effectColor = new Color(0f, 0f, 0f, 0.2f);
        shadow.effectDistance = new Vector2(1f, -1f);
    }
    
    /// <summary>
    /// Apply standard button styling
    /// </summary>
    private void ApplyStandardButtonStyling(Button button, Image buttonImage)
    {
        var colors = new ColorBlock
        {
            normalColor = designSystem.colors.backgroundMedium,
            highlightedColor = designSystem.colors.backgroundLight,
            pressedColor = designSystem.colors.blueAccent,
            selectedColor = designSystem.colors.backgroundMedium,
            disabledColor = designSystem.colors.backgroundDark,
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        
        button.colors = colors;
    }
    
    /// <summary>
    /// Apply button text styling
    /// </summary>
    private void ApplyButtonTextStyling(TextMeshProUGUI buttonText)
    {
        if (buttonText == null) return;
        
        if (designSystem.typography.primaryFontTMP != null) buttonText.font = designSystem.typography.primaryFontTMP;
        buttonText.fontSize = designSystem.GetFontSize("Button");
        buttonText.color = designSystem.colors.textPrimary;
        buttonText.fontWeight = FontWeight.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Apply text effects
        buttonText.enableAutoSizing = false;
        buttonText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        buttonText.overflowMode = TextOverflowModes.Overflow;
        
        // Add shadow effect to text
        buttonText.enableVertexGradient = false;
    }
    
    /// <summary>
    /// Apply text styling across all menu text elements
    /// </summary>
    private void ApplyTextStyling()
    {
        foreach (var text in allMenuTexts)
        {
            if (text == null) continue;
            
            if (designSystem.typography.primaryFontTMP != null) text.font = designSystem.typography.primaryFontTMP;
            text.color = designSystem.colors.textPrimary;
            
            // Apply different styling based on text type
            if (text == titleText)
            {
                text.fontSize = designSystem.GetFontSize("H1");
                text.fontWeight = FontWeight.Black;
                text.color = designSystem.colors.goldPrimary;
            }
            else if (text == subtitleText)
            {
                text.fontSize = designSystem.GetFontSize("H3");
                text.fontWeight = FontWeight.Bold;
                text.color = designSystem.colors.textSecondary;
            }
            else
            {
                text.fontSize = designSystem.GetFontSize("Body");
                text.fontWeight = FontWeight.Regular;
            }
            
            text.enableAutoSizing = true;
            text.fontSizeMin = 12;
            text.fontSizeMax = 72;
        }
    }
    
    /// <summary>
    /// Apply panel styling with Brawl Stars theme
    /// </summary>
    private void ApplyPanelStyling()
    {
        foreach (var panel in allPanels)
        {
            if (panel == null) continue;
            
            var panelImage = panel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = designSystem.colors.backgroundMedium;
                
                // RoundedCorners not in standard Unity.UI - skip
                // Add shadow
                var shadow = panel.GetComponent<Shadow>();
                if (shadow == null)
                {
                    shadow = panel.gameObject.AddComponent<Shadow>();
                }
                shadow.effectColor = new Color(0f, 0f, 0f, 0.3f);
                shadow.effectDistance = designSystem.components.card.shadow.blur * Vector2.one;
            }
        }
    }
    
    /// <summary>
    /// Apply cultural styling with Filipino elements
    /// </summary>
    private void ApplyCulturalStyling()
    {
        if (culturalPatternOverlay != null)
        {
            culturalPatternOverlay.color = designSystem.GetCulturalColor("baybayinGold", true);
            culturalPatternOverlay.SetNativeSize();
        }
        
        if (baybayinText != null)
        {
            if (designSystem.typography.secondaryFontTMP != null) baybayinText.font = designSystem.typography.secondaryFontTMP;
            baybayinText.color = designSystem.GetCulturalColor("baybayinGold", true);
            baybayinText.fontSize = designSystem.GetFontSize("Caption");
        }
        
        // Apply cultural decorations
        foreach (var decoration in culturalDecorations)
        {
            if (decoration != null)
            {
                decoration.color = designSystem.GetCulturalColor("tribalBrown", true);
            }
        }
    }
    
    /// <summary>
    /// Setup button animations with Brawl Stars feel
    /// </summary>
    private void SetupButtonAnimations()
    {
        foreach (var button in allMenuButtons)
        {
            if (button == null) continue;
            
            // Store original scale
            if (!originalButtonScales.ContainsKey(button))
            {
                originalButtonScales[button] = button.transform.localScale;
            }
            
            // Setup button events
            button.onClick.AddListener(() => OnButtonClick(button));
            
            // Setup hover effects
            var hoverHandler = button.gameObject.GetComponent<UIHoverHandler>();
            if (hoverHandler == null)
            {
                hoverHandler = button.gameObject.AddComponent<UIHoverHandler>();
            }
            
            hoverHandler.OnHoverEnter += () => OnButtonHoverEnter(button);
            hoverHandler.OnHoverExit += () => OnButtonHoverExit(button);
        }
    }
    
    /// <summary>
    /// Handle button click with Brawl Stars animation
    /// </summary>
    private void OnButtonClick(Button button)
    {
        // Kill existing animation
        if (buttonAnimationSequences.ContainsKey(button))
        {
            buttonAnimationSequences[button].Kill();
        }
        
        // Create click animation sequence
        var sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(originalButtonScales[button] * 0.95f, buttonAnimationDuration * 0.5f));
        sequence.Append(button.transform.DOScale(originalButtonScales[button] * 1.05f, buttonAnimationDuration * 0.5f));
        sequence.Append(button.transform.DOScale(originalButtonScales[button], buttonAnimationDuration * 0.5f));
        sequence.SetEase(buttonEase);
        
        buttonAnimationSequences[button] = sequence;
    }
    
    /// <summary>
    /// Handle button hover enter with Brawl Stars animation
    /// </summary>
    private void OnButtonHoverEnter(Button button)
    {
        // Kill existing animation
        if (buttonAnimationSequences.ContainsKey(button))
        {
            buttonAnimationSequences[button].Kill();
        }
        
        // Create hover animation
        var sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(originalButtonScales[button] * 1.1f, buttonAnimationDuration));
        sequence.SetEase(buttonEase);
        
        buttonAnimationSequences[button] = sequence;
    }
    
    /// <summary>
    /// Handle button hover exit with Brawl Stars animation
    /// </summary>
    private void OnButtonHoverExit(Button button)
    {
        // Kill existing animation
        if (buttonAnimationSequences.ContainsKey(button))
        {
            buttonAnimationSequences[button].Kill();
        }
        
        // Create exit animation
        var sequence = DOTween.Sequence();
        sequence.Append(button.transform.DOScale(originalButtonScales[button], buttonAnimationDuration));
        sequence.SetEase(buttonEase);
        
        buttonAnimationSequences[button] = sequence;
    }
    
    /// <summary>
    /// Setup cultural elements integration
    /// </summary>
    private void SetupCulturalElements()
    {
        if (!enableCulturalElements) return;
        
        // Add cultural hover effects
        foreach (var button in allMenuButtons)
        {
            var culturalEffect = button.gameObject.GetComponent<CulturalButtonEffect>();
            if (culturalEffect == null)
            {
                culturalEffect = button.gameObject.AddComponent<CulturalButtonEffect>();
            }
            
            culturalEffect.designSystem = designSystem;
            culturalEffect.enableBaybayin = true;
            culturalEffect.enableTribalPatterns = true;
        }
    }
    
    /// <summary>
    /// Transition to a new menu state with Brawl Stars animations
    /// </summary>
    public void TransitionToState(MenuState newState, bool animate = true)
    {
        if (newState == currentMenuState) return;
        
        previousMenuState = currentMenuState;
        currentMenuState = newState;
        
        // Update navigation stack
        if (previousMenuState != MenuState.None)
        {
            navigationStack.Push(previousMenuState);
        }
        
        // Perform transition
        if (animate)
        {
            StartCoroutine(PerformMenuTransition(previousMenuState, newState));
        }
        else
        {
            SetMenuStateDirect(newState);
        }
        
        // Trigger events
        OnMenuStateChanged?.Invoke(newState);
        OnMenuTransition?.Invoke(previousMenuState, newState);
        
        Debug.Log($"Menu transition: {previousMenuState} → {newState}");
    }
    
    /// <summary>
    /// Perform animated menu transition
    /// </summary>
    private IEnumerator PerformMenuTransition(MenuState fromState, MenuState toState)
    {
        // Kill existing transition
        if (currentTransitionSequence != null && currentTransitionSequence.IsActive())
        {
            currentTransitionSequence.Kill();
        }
        
        // Get panels
        Transform fromPanel = GetPanelForState(fromState);
        Transform toPanel = GetPanelForState(toState);
        
        // Exit animation for current panel
        if (fromPanel != null)
        {
            yield return StartCoroutine(AnimatePanelExit(fromPanel));
        }
        
        // Enter animation for new panel
        if (toPanel != null)
        {
            yield return StartCoroutine(AnimatePanelEnter(toPanel));
        }
        
        Debug.Log($"Menu transition completed: {fromState} → {toState}");
    }
    
    /// <summary>
    /// Animate panel exit with Brawl Stars style
    /// </summary>
    private IEnumerator AnimatePanelExit(Transform panel)
    {
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }
        
        // Create exit animation
        var sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(0f, panelTransitionDuration));
        sequence.Join(panel.DOScale(0.9f, panelTransitionDuration));
        sequence.Join(panel.DOLocalMoveY(-50f, panelTransitionDuration));
        sequence.SetEase(panelEase);
        
        currentTransitionSequence = sequence;
        
        yield return sequence.WaitForCompletion();
        
        panel.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Animate panel enter with Brawl Stars style
    /// </summary>
    private IEnumerator AnimatePanelEnter(Transform panel)
    {
        panel.gameObject.SetActive(true);
        
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }
        
        // Setup initial state
        canvasGroup.alpha = 0f;
        panel.localScale = Vector3.one * 0.9f;
        panel.localPosition = new Vector3(panel.localPosition.x, panel.localPosition.y + 50f, panel.localPosition.z);
        
        // Create enter animation
        var sequence = DOTween.Sequence();
        sequence.Join(canvasGroup.DOFade(1f, panelTransitionDuration));
        sequence.Join(panel.DOScale(Vector3.one, panelTransitionDuration));
        sequence.Join(panel.DOLocalMoveY(0f, panelTransitionDuration));
        sequence.SetEase(panelEase);
        
        currentTransitionSequence = sequence;
        
        yield return sequence.WaitForCompletion();
    }
    
    /// <summary>
    /// Set menu state directly without animation
    /// </summary>
    private void SetMenuStateDirect(MenuState state)
    {
        // Hide all panels
        foreach (var panel in allPanels)
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }
        }
        
        // Show target panel
        Transform targetPanel = GetPanelForState(state);
        if (targetPanel != null)
        {
            targetPanel.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Get panel for menu state
    /// </summary>
    private Transform GetPanelForState(MenuState state)
    {
        return panelDictionary.ContainsKey(state) ? panelDictionary[state] : null;
    }
    
    /// <summary>
    /// Navigate back to previous menu state
    /// </summary>
    public void NavigateBack()
    {
        if (navigationStack.Count > 0)
        {
            MenuState previousState = navigationStack.Pop();
            TransitionToState(previousState);
        }
        else
        {
            // Default to main menu
            TransitionToState(MenuState.MainMenu);
        }
    }
    
    /// <summary>
    /// Navigate to home (main menu)
    /// </summary>
    public void NavigateHome()
    {
        navigationStack.Clear();
        TransitionToState(MenuState.MainMenu);
    }
    
    /// <summary>
    /// Get device type based on screen width
    /// </summary>
    private DeviceType GetDeviceType(float screenWidth)
    {
        if (screenWidth <= mobileBreakpoint)
        {
            return DeviceType.Mobile;
        }
        else if (screenWidth <= tabletBreakpoint)
        {
            return DeviceType.Tablet;
        }
        else
        {
            return DeviceType.Desktop;
        }
    }
    
    /// <summary>
    /// Check for screen size changes and update responsive design
    /// </summary>
    private void CheckScreenSizeChanges()
    {
        if (Mathf.Abs(currentScreenWidth - Screen.width) > 1f || Mathf.Abs(currentScreenHeight - Screen.height) > 1f)
        {
            currentScreenWidth = Screen.width;
            currentScreenHeight = Screen.height;
            
            DeviceType newDeviceType = GetDeviceType(currentScreenWidth);
            if (newDeviceType != currentDeviceType)
            {
                currentDeviceType = newDeviceType;
                OnDeviceTypeChanged?.Invoke(currentDeviceType);
                UpdateResponsiveLayout();
                
                Debug.Log($"Device type changed to: {currentDeviceType}");
            }
        }
    }
    
    /// <summary>
    /// Update responsive layout based on device type
    /// </summary>
    private void UpdateResponsiveLayout()
    {
        float responsiveScale = designSystem.GetResponsiveScale();
        
        // Update UI element scales
        foreach (var button in allMenuButtons)
        {
            if (button != null)
            {
                button.transform.localScale = Vector3.one * responsiveScale;
            }
        }
        
        // Update text sizes
        foreach (var text in allMenuTexts)
        {
            if (text != null)
            {
                text.fontSize = Mathf.RoundToInt(text.fontSize * responsiveScale);
            }
        }
        
        // Update panel layouts
        UpdatePanelLayoutsForDeviceType();
    }
    
    /// <summary>
    /// Update panel layouts based on device type
    /// </summary>
    private void UpdatePanelLayoutsForDeviceType()
    {
        switch (currentDeviceType)
        {
            case DeviceType.Mobile:
                ApplyMobileLayout();
                break;
            case DeviceType.Tablet:
                ApplyTabletLayout();
                break;
            case DeviceType.Desktop:
                ApplyDesktopLayout();
                break;
        }
    }
    
    /// <summary>
    /// Apply mobile-specific layout
    /// </summary>
    private void ApplyMobileLayout()
    {
        // Stack navigation vertically
        if (navigationBar != null)
        {
            var layoutGroup = navigationBar.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                Destroy(layoutGroup);
                navigationBar.gameObject.AddComponent<VerticalLayoutGroup>();
            }
        }
        
        // Hide side navigation, show bottom navigation
        if (sideNavigation != null)
        {
            sideNavigation.gameObject.SetActive(false);
        }
        
        if (bottomNavigation != null)
        {
            bottomNavigation.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Apply tablet-specific layout
    /// </summary>
    private void ApplyTabletLayout()
    {
        // Use horizontal navigation with smaller spacing
        if (navigationBar != null)
        {
            var layoutGroup = navigationBar.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.spacing = designSystem.GetSpacing("sm");
            }
        }
        
        // Show both side and bottom navigation with adjustments
        if (sideNavigation != null)
        {
            sideNavigation.gameObject.SetActive(true);
        }
        
        if (bottomNavigation != null)
        {
            bottomNavigation.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Apply desktop-specific layout
    /// </summary>
    private void ApplyDesktopLayout()
    {
        // Use horizontal navigation with standard spacing
        if (navigationBar != null)
        {
            var layoutGroup = navigationBar.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.spacing = designSystem.GetSpacing("md");
            }
        }
        
        // Show side navigation, hide bottom navigation
        if (sideNavigation != null)
        {
            sideNavigation.gameObject.SetActive(true);
        }
        
        if (bottomNavigation != null)
        {
            bottomNavigation.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Setup UI element pooling for performance
    /// </summary>
    private void SetupUIElementPooling()
    {
        // Implementation for UI element object pooling
        Debug.Log("UI element pooling setup");
    }
    
    /// <summary>
    /// Setup lazy loading for panels
    /// </summary>
    private void SetupLazyLoading()
    {
        // Implementation for lazy loading panels
        Debug.Log("Lazy loading setup");
    }
    
    /// <summary>
    /// Optimize canvas settings for performance
    /// </summary>
    private void OptimizeCanvasSettings()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.pixelPerfect = enablePerformanceOptimization;
            canvas.sortingOrder = 0;
        }
        
        var canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = designSystem.responsive.breakpoints[2].minWidth * Vector2.one;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
    
    // Public interface methods
    
    public void ShowMainMenu()
    {
        TransitionToState(MenuState.MainMenu);
    }
    
    public void ShowPlayMenu()
    {
        TransitionToState(MenuState.Play);
    }
    
    public void ShowSettings()
    {
        TransitionToState(MenuState.Settings);
    }
    
    public void ShowShop()
    {
        TransitionToState(MenuState.Shop);
    }
    
    public void ShowProfile()
    {
        TransitionToState(MenuState.Profile);
    }
    
    public void ShowLeaderboard()
    {
        TransitionToState(MenuState.Leaderboard);
    }
    
    public void ShowAchievements()
    {
        TransitionToState(MenuState.Achievements);
    }
    
    public void ShowCultural()
    {
        TransitionToState(MenuState.Cultural);
    }
    
    /// <summary>
    /// Get current menu state
    /// </summary>
    public MenuState GetCurrentMenuState()
    {
        return currentMenuState;
    }
    
    /// <summary>
    /// Get current device type
    /// </summary>
    public DeviceType GetCurrentDeviceType()
    {
        return currentDeviceType;
    }
    
    /// <summary>
    /// Get navigation stack for debugging
    /// </summary>
    public Stack<MenuState> GetNavigationStack()
    {
        return new Stack<MenuState>(navigationStack);
    }
    
    void OnDestroy()
    {
        // Cleanup
        if (currentTransitionSequence != null && currentTransitionSequence.IsActive())
        {
            currentTransitionSequence.Kill();
        }
        
        foreach (var sequence in buttonAnimationSequences.Values)
        {
            if (sequence != null && sequence.IsActive())
            {
                sequence.Kill();
            }
        }
        
        buttonAnimationSequences.Clear();
    }
}

/// <summary>
/// UI Hover Handler - handles hover events for UI elements
/// </summary>
public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public System.Action OnHoverEnter;
    public System.Action OnHoverExit;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }
}

/// <summary>
/// Cultural Button Effect - adds Filipino cultural elements to buttons
/// </summary>
public class CulturalButtonEffect : MonoBehaviour
{
    public BrawlStarsDesignSystem designSystem;
    public bool enableBaybayin = true;
    public bool enableTribalPatterns = true;
    
    private Button targetButton;
    private TextMeshProUGUI baybayinText;
    private Image tribalPattern;
    
    void Awake()
    {
        targetButton = GetComponent<Button>();
        if (targetButton != null)
        {
            SetupCulturalElements();
        }
    }
    
    void SetupCulturalElements()
    {
        if (enableBaybayin)
        {
            CreateBaybayinText();
        }
        
        if (enableTribalPatterns)
        {
            CreateTribalPattern();
        }
    }
    
    void CreateBaybayinText()
    {
        GameObject baybayinGO = new GameObject("BaybayinText");
        baybayinGO.transform.SetParent(targetButton.transform, false);
        
        baybayinText = baybayinGO.AddComponent<TextMeshProUGUI>();
        baybayinText.text = "ᜊᜌ᜔ᜊᜌᜒᜈ᜔"; // "Baybayin" in Baybayin script
        baybayinText.fontSize = 12;
        baybayinText.color = designSystem.GetCulturalColor("baybayinGold", true);
        baybayinText.alignment = TextAlignmentOptions.Center;
        baybayinText.alpha = 0.3f;
        
        RectTransform rectTransform = baybayinGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }
    
    void CreateTribalPattern()
    {
        GameObject patternGO = new GameObject("TribalPattern");
        patternGO.transform.SetParent(targetButton.transform, false);
        
        tribalPattern = patternGO.AddComponent<Image>();
        var tribalColor = designSystem.GetCulturalColor("tribalBrown", true);
        tribalColor.a = 0.1f;
        tribalPattern.color = tribalColor;
        
        RectTransform rectTransform = patternGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }
}