using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;

/// <summary>
/// Brawl Stars UI Controller - manages the complete UI system with Figma integration,
/// animations, and multi-device support.
/// </summary>
public class BrawlStarsUIController : MonoBehaviour
{
    [Header("UI Configuration")]
    public BrawlStarsUIConfig uiConfig;
    public Canvas mainCanvas;
    public CanvasScaler canvasScaler;
    public GraphicRaycaster graphicRaycaster;
    
    [Header("UI Panels")]
    public Transform mainMenuPanel;
    public Transform gameHUDPanel;
    public Transform pausePanel;
    public Transform settingsPanel;
    public Transform shopPanel;
    public Transform brawlerSelectionPanel;
    public Transform battlePanel;
    public Transform resultsPanel;
    
    [Header("Interactive Elements")]
    public List<Button> menuButtons = new List<Button>();
    public List<Slider> uiSliders = new List<Slider>();
    public List<Toggle> uiToggles = new List<Toggle>();
    public List<TMP_Text> uiTexts = new List<TMP_Text>();
    public List<Image> uiImages = new List<Image>();
    
    [Header("Animation Settings")]
    public float panelTransitionDuration = 0.3f;
    public float buttonAnimationDuration = 0.2f;
    public float elementFadeDuration = 0.25f;
    public Ease defaultEase = Ease.OutQuad;
    public Ease buttonEase = Ease.OutBack;
    
    [Header("State Management")]
    private UIState currentState = UIState.MainMenu;
    private UIState previousState = UIState.None;
    private Dictionary<UIState, Transform> panelDictionary;
    private Stack<UIState> navigationStack = new Stack<UIState>();
    
    [Header("Performance Settings")]
    public bool enableSpriteAtlasing = true;
    public int targetFrameRate = 60;
    public bool enablePixelPerfect = false;
    public bool enableBatching = true;
    
    [Header("Touch Settings")]
    public Vector2 minTouchTargetSize = new Vector2(44f, 44f);
    public bool enableTouchPadding = true;
    public float touchPadding = 8f;
    
    // Events
    public System.Action<UIState> OnStateChanged;
    public System.Action<UIState, UIState> OnPanelTransition;
    
    public enum UIState
    {
        None,
        MainMenu,
        GameHUD,
        Pause,
        Settings,
        Shop,
        BrawlerSelection,
        Battle,
        Results,
        Loading
    }
    
    void Awake()
    {
        InitializeUIController();
        SetupPanelDictionary();
        ConfigureCanvas();
        OptimizeForMobile();
    }
    
    void Start()
    {
        SetupInteractiveElements();
        ApplyBrawlStarsStyling();
        InitializeAnimations();
        ShowPanel(UIState.MainMenu, false);
    }
    
    /// <summary>
    /// Initialize the UI controller with proper setup
    /// </summary>
    private void InitializeUIController()
    {
        if (uiConfig == null)
        {
            uiConfig = ScriptableObject.CreateInstance<BrawlStarsUIConfig>();
            Debug.LogWarning("BrawlStarsUIConfig not assigned, using default settings");
        }
        
        if (mainCanvas == null)
            mainCanvas = GetComponent<Canvas>();
        
        if (canvasScaler == null)
            canvasScaler = GetComponent<CanvasScaler>();
        
        if (graphicRaycaster == null)
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        
        // Set target frame rate
        Application.targetFrameRate = targetFrameRate;
        
        Debug.Log("Brawl Stars UI Controller initialized");
    }
    
    /// <summary>
    /// Setup panel dictionary for easy navigation
    /// </summary>
    private void SetupPanelDictionary()
    {
        panelDictionary = new Dictionary<UIState, Transform>
        {
            { UIState.MainMenu, mainMenuPanel },
            { UIState.GameHUD, gameHUDPanel },
            { UIState.Pause, pausePanel },
            { UIState.Settings, settingsPanel },
            { UIState.Shop, shopPanel },
            { UIState.BrawlerSelection, brawlerSelectionPanel },
            { UIState.Battle, battlePanel },
            { UIState.Results, resultsPanel }
        };
        
        // Hide all panels initially
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Configure Canvas for multi-device support
    /// </summary>
    private void ConfigureCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 1;
            mainCanvas.pixelPerfect = enablePixelPerfect;
        }
        
        if (canvasScaler != null && uiConfig != null)
        {
            uiConfig.ConfigureCanvasScaler(canvasScaler);
        }
    }
    
    /// <summary>
    /// Optimize UI for mobile devices
    /// </summary>
    private void OptimizeForMobile()
    {
        // Enable sprite atlasing for better performance
        if (enableSpriteAtlasing)
        {
            // This would be configured in Unity's Sprite Atlas settings
            Debug.Log("Sprite atlasing enabled for UI optimization");
        }
        
        // Configure for mobile rendering
        if (graphicRaycaster != null)
        {
            // Optimize raycasting for mobile
            graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        }
    }
    
    /// <summary>
    /// Setup all interactive UI elements
    /// </summary>
    private void SetupInteractiveElements()
    {
        SetupButtons();
        SetupSliders();
        SetupToggles();
        SetupTexts();
        SetupImages();
    }
    
    /// <summary>
    /// Setup button components with proper styling and interactions
    /// </summary>
    private void SetupButtons()
    {
        foreach (var button in menuButtons)
        {
            if (button != null)
            {
                SetupButtonInteraction(button);
                EnsureTouchTargetSize(button);
            }
        }
    }
    
    /// <summary>
    /// Setup individual button with animations and interactions
    /// </summary>
    private void SetupButtonInteraction(Button button)
    {
        if (button == null) return;
        
        // Store original scale
        Vector3 originalScale = button.transform.localScale;
        
        // Add hover and click animations
        button.onClick.AddListener(() =>
        {
            AnimateButtonClick(button, originalScale);
        });
        
        // Setup hover effects
        var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Add hover enter event
        var hoverEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEnter.callback.AddListener((data) =>
        {
            AnimateButtonHover(button, originalScale, true);
        });
        trigger.triggers.Add(hoverEnter);
        
        // Add hover exit event
        var hoverExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        hoverExit.callback.AddListener((data) =>
        {
            AnimateButtonHover(button, originalScale, false);
        });
        trigger.triggers.Add(hoverExit);
    }
    
    /// <summary>
    /// Ensure minimum touch target size for accessibility
    /// </summary>
    private void EnsureTouchTargetSize(Button button)
    {
        if (!enableTouchPadding) return;
        
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float currentWidth = rectTransform.rect.width;
            float currentHeight = rectTransform.rect.height;
            
            if (currentWidth < minTouchTargetSize.x || currentHeight < minTouchTargetSize.y)
            {
                // Add padding to reach minimum size
                float paddingX = Mathf.Max(0f, (minTouchTargetSize.x - currentWidth) / 2f + touchPadding);
                float paddingY = Mathf.Max(0f, (minTouchTargetSize.y - currentHeight) / 2f + touchPadding);
                
                rectTransform.sizeDelta = new Vector2(
                    currentWidth + paddingX * 2f,
                    currentHeight + paddingY * 2f
                );
            }
        }
    }
    
    /// <summary>
    /// Setup slider components
    /// </summary>
    private void SetupSliders()
    {
        foreach (var slider in uiSliders)
        {
            if (slider != null)
            {
                SetupSliderInteraction(slider);
            }
        }
    }
    
    /// <summary>
    /// Setup individual slider with animations
    /// </summary>
    private void SetupSliderInteraction(Slider slider)
    {
        if (slider == null) return;
        
        // Add value change animation
        slider.onValueChanged.AddListener((value) =>
        {
            AnimateSliderValueChange(slider, value);
        });
    }
    
    /// <summary>
    /// Setup toggle components
    /// </summary>
    private void SetupToggles()
    {
        foreach (var toggle in uiToggles)
        {
            if (toggle != null)
            {
                SetupToggleInteraction(toggle);
            }
        }
    }
    
    /// <summary>
    /// Setup individual toggle with animations
    /// </summary>
    private void SetupToggleInteraction(Toggle toggle)
    {
        if (toggle == null) return;
        
        // Add state change animation
        toggle.onValueChanged.AddListener((isOn) =>
        {
            AnimateToggleStateChange(toggle, isOn);
        });
    }
    
    /// <summary>
    /// Setup text components
    /// </summary>
    private void SetupTexts()
    {
        foreach (var text in uiTexts)
        {
            if (text != null)
            {
                SetupTextStyling(text);
            }
        }
    }
    
    /// <summary>
    /// Setup individual text with proper styling
    /// </summary>
    private void SetupTextStyling(TMP_Text text)
    {
        if (text == null) return;
        
        // Apply Brawl Stars typography
        if (uiConfig != null)
        {
            if (uiConfig.mainFontTMP != null) text.font = uiConfig.mainFontTMP;
            text.fontSize = uiConfig.baseFontSize;
            text.color = uiConfig.textLight;
            text.enableAutoSizing = true;
            text.fontSizeMin = uiConfig.baseFontSize * 0.8f;
            text.fontSizeMax = uiConfig.baseFontSize * 1.2f;
        }
    }
    
    /// <summary>
    /// Setup image components
    /// </summary>
    private void SetupImages()
    {
        foreach (var image in uiImages)
        {
            if (image != null)
            {
                SetupImageStyling(image);
            }
        }
    }
    
    /// <summary>
    /// Setup individual image with proper styling
    /// </summary>
    private void SetupImageStyling(Image image)
    {
        if (image == null) return;
        
        // Apply Brawl Stars color palette
        if (uiConfig != null && image.color == Color.white)
        {
            image.color = uiConfig.primaryGold;
        }
        
        // Enable raycast target for interaction
        image.raycastTarget = true;
    }
    
    /// <summary>
    /// Apply Brawl Stars styling to all UI elements
    /// </summary>
    private void ApplyBrawlStarsStyling()
    {
        if (uiConfig == null) return;
        
        // Apply styling to all buttons
        foreach (var button in menuButtons)
        {
            if (button != null)
            {
                uiConfig.ApplyBrawlStarsStyling(button.gameObject, "Button");
            }
        }
        
        // Apply styling to all panels
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null)
            {
                uiConfig.ApplyBrawlStarsStyling(panel.gameObject, "Panel");
            }
        }
        
        // Apply styling to all images
        foreach (var image in uiImages)
        {
            if (image != null)
            {
                uiConfig.ApplyBrawlStarsStyling(image.gameObject, "Image");
            }
        }
    }
    
    /// <summary>
    /// Initialize animation system
    /// </summary>
    private void InitializeAnimations()
    {
        // Setup DOTween if available
        DOTween.SetTweensCapacity(200, 100);
        DOTween.defaultAutoPlay = AutoPlay.All;
        DOTween.defaultAutoKill = true;
        
        Debug.Log("Animation system initialized");
    }
    
    #region Animation Methods
    
    /// <summary>
    /// Animate button click with scale and color effects
    /// </summary>
    private void AnimateButtonClick(Button button, Vector3 originalScale)
    {
        if (!uiConfig.enableAnimations) return;
        
        // Scale animation
        button.transform.DOScale(originalScale * 0.9f, buttonAnimationDuration * 0.5f)
            .SetEase(buttonEase)
            .OnComplete(() =>
            {
                button.transform.DOScale(originalScale, buttonAnimationDuration * 0.5f)
                    .SetEase(buttonEase);
            });
        
        // Color animation
        var colors = button.colors;
        Color originalColor = colors.normalColor;
        colors.normalColor = Color.Lerp(originalColor, Color.white, 0.3f);
        button.colors = colors;
        
        // Restore color after animation
        DOVirtual.DelayedCall(buttonAnimationDuration, () =>
        {
            colors.normalColor = originalColor;
            button.colors = colors;
        });
    }
    
    /// <summary>
    /// Animate button hover with scale effects
    /// </summary>
    private void AnimateButtonHover(Button button, Vector3 originalScale, bool isHovering)
    {
        if (!uiConfig.enableAnimations) return;
        
        float targetScale = isHovering ? 1.05f : 1f;
        button.transform.DOScale(originalScale * targetScale, buttonAnimationDuration)
            .SetEase(defaultEase);
    }
    
    /// <summary>
    /// Animate slider value change
    /// </summary>
    private void AnimateSliderValueChange(Slider slider, float value)
    {
        if (!uiConfig.enableAnimations) return;
        
        // Add subtle feedback animation
        var fillRect = slider.fillRect;
        if (fillRect != null)
        {
            fillRect.DOScale(1.1f, 0.1f).OnComplete(() =>
            {
                fillRect.DOScale(1f, 0.1f);
            });
        }
    }
    
    /// <summary>
    /// Animate toggle state change
    /// </summary>
    private void AnimateToggleStateChange(Toggle toggle, bool isOn)
    {
        if (!uiConfig.enableAnimations) return;
        
        var toggleTransform = toggle.graphic?.rectTransform;
        if (toggleTransform != null)
        {
            float targetScale = isOn ? 1.1f : 1f;
            toggleTransform.DOScale(targetScale, buttonAnimationDuration)
                .SetEase(defaultEase)
                .OnComplete(() =>
                {
                    toggleTransform.DOScale(1f, buttonAnimationDuration * 0.5f);
                });
        }
    }
    
    #endregion
    
    #region Panel Management
    
    /// <summary>
    /// Show a specific UI panel with transition animation
    /// </summary>
    public void ShowPanel(UIState targetState, bool animate = true)
    {
        if (targetState == currentState) return;
        
        previousState = currentState;
        currentState = targetState;
        
        // Hide current panel
        if (previousState != UIState.None && panelDictionary.ContainsKey(previousState))
        {
            HidePanel(previousState, animate);
        }
        
        // Show target panel
        if (panelDictionary.ContainsKey(targetState))
        {
            var targetPanel = panelDictionary[targetState];
            if (targetPanel != null)
            {
                targetPanel.gameObject.SetActive(true);
                
                if (animate)
                {
                    AnimatePanelTransition(targetPanel, true);
                }
                else
                {
                    // Set to final state without animation
                    var panelRect = targetPanel.GetComponent<RectTransform>();
                    if (panelRect != null)
                    {
                        panelRect.localScale = Vector3.one;
                        panelRect.localPosition = Vector3.zero;
                        var canvasGroup = targetPanel.GetComponent<CanvasGroup>();
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 1f;
                            canvasGroup.interactable = true;
                            canvasGroup.blocksRaycasts = true;
                        }
                    }
                }
            }
        }
        
        // Add to navigation stack
        navigationStack.Push(targetState);
        
        // Trigger events
        OnStateChanged?.Invoke(targetState);
        OnPanelTransition?.Invoke(previousState, targetState);
        
        Debug.Log($"Switched to panel: {targetState}");
    }
    
    /// <summary>
    /// Hide a specific UI panel with transition animation
    /// </summary>
    private void HidePanel(UIState state, bool animate = true)
    {
        if (!panelDictionary.ContainsKey(state)) return;
        
        var panel = panelDictionary[state];
        if (panel == null) return;
        
        if (animate)
        {
            AnimatePanelTransition(panel, false, () =>
            {
                panel.gameObject.SetActive(false);
            });
        }
        else
        {
            panel.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Animate panel transition (fade and scale)
    /// </summary>
    private void AnimatePanelTransition(Transform panel, bool isShowing, System.Action onComplete = null)
    {
        if (!uiConfig.enableAnimations)
        {
            onComplete?.Invoke();
            return;
        }
        
        var panelRect = panel.GetComponent<RectTransform>();
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        }
        
        if (isShowing)
        {
            // Show animation: fade in and scale up
            panelRect.localScale = Vector3.one * 0.9f;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // Scale up
            panelRect.DOScale(Vector3.one, panelTransitionDuration)
                .SetEase(defaultEase);
            
            // Fade in
            canvasGroup.DOFade(1f, panelTransitionDuration)
                .SetEase(defaultEase)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    onComplete?.Invoke();
                });
        }
        else
        {
            // Hide animation: fade out and scale down
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // Scale down
            panelRect.DOScale(Vector3.one * 0.9f, panelTransitionDuration)
                .SetEase(defaultEase);
            
            // Fade out
            canvasGroup.DOFade(0f, panelTransitionDuration)
                .SetEase(defaultEase)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                });
        }
    }
    
    /// <summary>
    /// Navigate back to previous panel
    /// </summary>
    public void GoBack()
    {
        if (navigationStack.Count > 1)
        {
            // Remove current state
            navigationStack.Pop();
            
            // Get previous state
            var previousState = navigationStack.Peek();
            ShowPanel(previousState);
        }
        else
        {
            // Go to main menu if no previous state
            ShowPanel(UIState.MainMenu);
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Show main menu panel
    /// </summary>
    public void ShowMainMenu()
    {
        ShowPanel(UIState.MainMenu);
    }
    
    /// <summary>
    /// Show game HUD panel
    /// </summary>
    public void ShowGameHUD()
    {
        ShowPanel(UIState.GameHUD);
    }
    
    /// <summary>
    /// Show pause panel
    /// </summary>
    public void ShowPausePanel()
    {
        ShowPanel(UIState.Pause);
    }
    
    /// <summary>
    /// Show settings panel
    /// </summary>
    public void ShowSettingsPanel()
    {
        ShowPanel(UIState.Settings);
    }
    
    /// <summary>
    /// Show shop panel
    /// </summary>
    public void ShowShopPanel()
    {
        ShowPanel(UIState.Shop);
    }
    
    /// <summary>
    /// Show brawler selection panel
    /// </summary>
    public void ShowBrawlerSelectionPanel()
    {
        ShowPanel(UIState.BrawlerSelection);
    }
    
    /// <summary>
    /// Show battle panel
    /// </summary>
    public void ShowBattlePanel()
    {
        ShowPanel(UIState.Battle);
    }
    
    /// <summary>
    /// Show results panel
    /// </summary>
    public void ShowResultsPanel()
    {
        ShowPanel(UIState.Results);
    }
    
    /// <summary>
    /// Get current UI state
    /// </summary>
    public UIState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Check if a specific panel is active
    /// </summary>
    public bool IsPanelActive(UIState state)
    {
        return currentState == state;
    }
    
    /// <summary>
    /// Refresh UI styling
    /// </summary>
    public void RefreshUI()
    {
        ApplyBrawlStarsStyling();
        SetupInteractiveElements();
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Validate UI configuration
    /// </summary>
    public bool ValidateUI()
    {
        if (uiConfig == null)
        {
            Debug.LogError("UI Configuration is not assigned");
            return false;
        }
        
        if (mainCanvas == null)
        {
            Debug.LogError("Main Canvas is not assigned");
            return false;
        }
        
        if (canvasScaler == null)
        {
            Debug.LogError("Canvas Scaler is not assigned");
            return false;
        }
        
        return uiConfig.ValidateConfiguration();
    }
    
    /// <summary>
    /// Get performance metrics
    /// </summary>
    public Dictionary<string, float> GetPerformanceMetrics()
    {
        var metrics = new Dictionary<string, float>
        {
            { "Current_FPS", 1f / Time.deltaTime },
            { "Target_FPS", targetFrameRate },
            { "Active_Panels", GetActivePanelCount() },
            { "UI_Elements_Count", GetUIElementsCount() }
        };
        
        return metrics;
    }
    
    /// <summary>
    /// Get number of active panels
    /// </summary>
    private int GetActivePanelCount()
    {
        int activeCount = 0;
        foreach (var panel in panelDictionary.Values)
        {
            if (panel != null && panel.gameObject.activeSelf)
                activeCount++;
        }
        return activeCount;
    }
    
    /// <summary>
    /// Get total UI elements count
    /// </summary>
    private int GetUIElementsCount()
    {
        int count = 0;
        count += menuButtons.Count;
        count += uiSliders.Count;
        count += uiToggles.Count;
        count += uiTexts.Count;
        count += uiImages.Count;
        return count;
    }
    
    void OnDestroy()
    {
        // Cleanup DOTween
        DOTween.KillAll();
        
        // Clear navigation stack
        navigationStack.Clear();
        
        Debug.Log("Brawl Stars UI Controller destroyed");
    }
    
    void OnValidate()
    {
        // Validate settings in editor
        if (targetFrameRate < 30)
            targetFrameRate = 30;
        if (targetFrameRate > 120)
            targetFrameRate = 120;
    }
    #endregion
}