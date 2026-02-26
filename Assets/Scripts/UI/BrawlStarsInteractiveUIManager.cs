using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using TMPro;

/// <summary>
/// Brawl Stars Interactive UI Manager - handles interactive UI elements with proper state management,
/// animations, and touch target optimization for mobile devices.
/// </summary>
public class BrawlStarsInteractiveUIManager : MonoBehaviour
{
    [Header("Button Settings")]
    [Tooltip("Enable button state animations")]
    public bool enableButtonAnimations = true;
    
    [Tooltip("Button press animation duration")]
    public float buttonPressDuration = 0.1f;
    
    [Tooltip("Button hover animation duration")]
    public float buttonHoverDuration = 0.2f;
    
    [Tooltip("Button scale multiplier on press")]
    public float buttonPressScale = 0.95f;
    
    [Tooltip("Button scale multiplier on hover")]
    public float buttonHoverScale = 1.05f;
    
    [Tooltip("Enable button sound effects")]
    public bool enableButtonSounds = true;
    
    [Header("Slider Settings")]
    [Tooltip("Enable slider value change animations")]
    public bool enableSliderAnimations = true;
    
    [Tooltip("Slider animation duration")]
    public float sliderAnimationDuration = 0.3f;
    
    [Tooltip("Slider fill animation ease")]
    public Ease sliderEase = Ease.OutQuad;
    
    [Header("Toggle Settings")]
    [Tooltip("Enable toggle state change animations")]
    public bool enableToggleAnimations = true;
    
    [Tooltip("Toggle animation duration")]
    public float toggleAnimationDuration = 0.2f;
    
    [Tooltip("Toggle scale animation on state change")]
    public float toggleScaleMultiplier = 1.1f;
    
    [Header("Input Field Settings")]
    [Tooltip("Enable input field focus animations")]
    public bool enableInputFieldAnimations = true;
    
    [Tooltip("Input field focus animation duration")]
    public float inputFieldFocusDuration = 0.2f;
    
    [Tooltip("Input field scale on focus")]
    public float inputFieldFocusScale = 1.02f;
    
    [Header("Touch Target Settings")]
    [Tooltip("Minimum touch target size (44x44 pixels recommended)")]
    public Vector2 minTouchTargetSize = new Vector2(44f, 44f);
    
    [Tooltip("Enable automatic touch target padding")]
    public bool enableAutoTouchPadding = true;
    
    [Tooltip("Touch target padding amount")]
    public float touchPadding = 8f;
    
    [Header("State Management")]
    [Tooltip("Enable state persistence across scenes")]
    public bool enableStatePersistence = true;
    
    [Tooltip("State save key prefix")]
    public string stateSaveKey = "BrawlStarsUIState_";
    
    [Header("Performance Settings")]
    [Tooltip("Enable pooling for better performance")]
    public bool enablePooling = true;
    
    [Tooltip("Maximum pooled objects")]
    public int maxPooledObjects = 50;
    
    [Tooltip("Enable batching for similar elements")]
    public bool enableBatching = true;
    
    // State tracking
    private Dictionary<GameObject, UIElementState> elementStates = new Dictionary<GameObject, UIElementState>();
    private Dictionary<string, object> persistentStates = new Dictionary<string, object>();
    private Dictionary<GameObject, Coroutine> activeAnimations = new Dictionary<GameObject, Coroutine>();
    
    // Pooling
    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, int> poolUsage = new Dictionary<string, int>();
    
    // Events
    public System.Action<GameObject, ButtonState> OnButtonStateChanged;
    public System.Action<GameObject, float> OnSliderValueChanged;
    public System.Action<GameObject, bool> OnToggleStateChanged;
    public System.Action<GameObject, bool> OnInputFieldFocusChanged;
    
    public enum ButtonState
    {
        Normal,
        Highlighted,
        Pressed,
        Disabled
    }
    
    [System.Serializable]
    public class UIElementState
    {
        public GameObject gameObject;
        public string elementType;
        public bool isInteractable;
        public Vector3 originalScale;
        public Color originalColor;
        public Vector2 originalSize;
        public bool hasTouchPadding;
        public Vector2 touchTargetSize;
        
        public UIElementState(GameObject obj)
        {
            gameObject = obj;
            elementType = obj.GetComponent<Button>() != null ? "Button" :
                         obj.GetComponent<Slider>() != null ? "Slider" :
                         obj.GetComponent<Toggle>() != null ? "Toggle" :
                         obj.GetComponent<TMP_InputField>() != null ? "InputField" : "Unknown";
            
            var rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                originalScale = rectTransform.localScale;
                originalSize = rectTransform.rect.size;
            }
            
            var graphic = obj.GetComponent<Graphic>();
            if (graphic != null)
            {
                originalColor = graphic.color;
            }
            
            isInteractable = true;
        }
    }
    
    void Awake()
    {
        InitializeManager();
        LoadPersistentStates();
    }
    
    void Start()
    {
        SetupAllInteractiveElements();
        OptimizeForPerformance();
    }
    
    /// <summary>
    /// Initialize the interactive UI manager
    /// </summary>
    private void InitializeManager()
    {
        // Initialize pooling system
        if (enablePooling)
        {
            InitializePooling();
        }
        
        // Load persistent states
        if (enableStatePersistence)
        {
            LoadPersistentStates();
        }
        
        Debug.Log("Brawl Stars Interactive UI Manager initialized");
    }
    
    /// <summary>
    /// Initialize object pooling system
    /// </summary>
    private void InitializePooling()
    {
        objectPools.Clear();
        poolUsage.Clear();
        
        // Create pools for different UI element types
        string[] poolTypes = { "Button", "Slider", "Toggle", "InputField", "Text", "Image" };
        
        foreach (string type in poolTypes)
        {
            objectPools[type] = new Queue<GameObject>();
            poolUsage[type] = 0;
        }
        
        Debug.Log("Object pooling system initialized");
    }
    
    /// <summary>
    /// Load persistent UI states
    /// </summary>
    private void LoadPersistentStates()
    {
        // Load saved states from PlayerPrefs or other storage
        // This is a simplified implementation
        persistentStates.Clear();
        
        // Load button states
        for (int i = 0; i < 100; i++) // Support up to 100 buttons
        {
            string key = stateSaveKey + "Button_" + i;
            if (PlayerPrefs.HasKey(key))
            {
                bool state = PlayerPrefs.GetInt(key) == 1;
                persistentStates[key] = state;
            }
        }
        
        Debug.Log("Persistent UI states loaded");
    }
    
    /// <summary>
    /// Save persistent UI states
    /// </summary>
    private void SavePersistentStates()
    {
        // Save current states to persistent storage
        foreach (var kvp in persistentStates)
        {
            if (kvp.Value is bool boolValue)
            {
                PlayerPrefs.SetInt(kvp.Key, boolValue ? 1 : 0);
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log("Persistent UI states saved");
    }
    
    /// <summary>
    /// Setup all interactive UI elements in the scene
    /// </summary>
    private void SetupAllInteractiveElements()
    {
        // Find all interactive UI elements
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Toggle[] toggles = FindObjectsByType<Toggle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        TMP_InputField[] inputFields = FindObjectsByType<TMP_InputField>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        // Setup buttons
        foreach (Button button in buttons)
        {
            SetupButton(button);
        }
        
        // Setup sliders
        foreach (Slider slider in sliders)
        {
            SetupSlider(slider);
        }
        
        // Setup toggles
        foreach (Toggle toggle in toggles)
        {
            SetupToggle(toggle);
        }
        
        // Setup input fields
        foreach (TMP_InputField inputField in inputFields)
        {
            SetupInputField(inputField);
        }
        
        Debug.Log($"Setup {buttons.Length} buttons, {sliders.Length} sliders, {toggles.Length} toggles, {inputFields.Length} input fields");
    }
    
    /// <summary>
    /// Setup individual button with animations and state management
    /// </summary>
    private void SetupButton(Button button)
    {
        if (button == null) return;
        
        // Create state tracking
        UIElementState state = new UIElementState(button.gameObject);
        elementStates[button.gameObject] = state;
        
        // Ensure minimum touch target size
        if (enableAutoTouchPadding)
        {
            EnsureTouchTargetSize(button);
        }
        
        // Setup event listeners
        SetupButtonEventListeners(button);
        
        // Apply initial styling
        ApplyButtonStyling(button, ButtonState.Normal);
        
        // Load persistent state
        LoadElementPersistentState(button.gameObject);
    }
    
    /// <summary>
    /// Setup button event listeners
    /// </summary>
    private void SetupButtonEventListeners(Button button)
    {
        // Remove existing listeners to avoid duplicates
        button.onClick.RemoveAllListeners();
        
        // Add click animation
        button.onClick.AddListener(() =>
        {
            AnimateButtonClick(button);
            PlayButtonSound();
        });
        
        // Setup hover effects using EventTrigger
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Hover enter
        var hoverEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEnter.callback.AddListener((data) =>
        {
            AnimateButtonHover(button, true);
        });
        eventTrigger.triggers.Add(hoverEnter);
        
        // Hover exit
        var hoverExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        hoverExit.callback.AddListener((data) =>
        {
            AnimateButtonHover(button, false);
        });
        eventTrigger.triggers.Add(hoverExit);
        
        // Pointer down
        var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            AnimateButtonPress(button, true);
        });
        eventTrigger.triggers.Add(pointerDown);
        
        // Pointer up
        var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) =>
        {
            AnimateButtonPress(button, false);
        });
        eventTrigger.triggers.Add(pointerUp);
    }
    
    /// <summary>
    /// Ensure minimum touch target size for accessibility
    /// </summary>
    private void EnsureTouchTargetSize(Button button)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector2 currentSize = rectTransform.rect.size;
        
        if (currentSize.x < minTouchTargetSize.x || currentSize.y < minTouchTargetSize.y)
        {
            // Calculate required padding
            float paddingX = Mathf.Max(0f, (minTouchTargetSize.x - currentSize.x) / 2f + touchPadding);
            float paddingY = Mathf.Max(0f, (minTouchTargetSize.y - currentSize.y) / 2f + touchPadding);
            
            // Apply padding
            rectTransform.sizeDelta = new Vector2(
                currentSize.x + paddingX * 2f,
                currentSize.y + paddingY * 2f
            );
            
            // Store state
            if (elementStates.ContainsKey(button.gameObject))
            {
                elementStates[button.gameObject].hasTouchPadding = true;
                elementStates[button.gameObject].touchTargetSize = new Vector2(
                    currentSize.x + paddingX * 2f,
                    currentSize.y + paddingY * 2f
                );
            }
        }
    }
    
    /// <summary>
    /// Setup individual slider with animations
    /// </summary>
    private void SetupSlider(Slider slider)
    {
        if (slider == null) return;
        
        // Create state tracking
        UIElementState state = new UIElementState(slider.gameObject);
        elementStates[slider.gameObject] = state;
        
        // Setup event listeners
        slider.onValueChanged.AddListener((value) =>
        {
            AnimateSliderValueChange(slider, value);
        });
    }
    
    /// <summary>
    /// Setup individual toggle with animations
    /// </summary>
    private void SetupToggle(Toggle toggle)
    {
        if (toggle == null) return;
        
        // Create state tracking
        UIElementState state = new UIElementState(toggle.gameObject);
        elementStates[toggle.gameObject] = state;
        
        // Setup event listeners
        toggle.onValueChanged.AddListener((isOn) =>
        {
            AnimateToggleStateChange(toggle, isOn);
        });
        
        // Load persistent state
        LoadElementPersistentState(toggle.gameObject);
    }
    
    /// <summary>
    /// Setup individual input field with animations
    /// </summary>
    private void SetupInputField(TMP_InputField inputField)
    {
        if (inputField == null) return;
        
        // Create state tracking
        UIElementState state = new UIElementState(inputField.gameObject);
        elementStates[inputField.gameObject] = state;
        
        // Setup event listeners
        inputField.onSelect.AddListener((text) =>
        {
            AnimateInputFieldFocus(inputField, true);
        });
        
        inputField.onDeselect.AddListener((text) =>
        {
            AnimateInputFieldFocus(inputField, false);
        });
    }
    
    #region Animation Methods
    
    /// <summary>
    /// Animate button click
    /// </summary>
    private void AnimateButtonClick(Button button)
    {
        if (!enableButtonAnimations) return;
        
        // Stop any existing animations
        if (activeAnimations.ContainsKey(button.gameObject))
        {
            StopCoroutine(activeAnimations[button.gameObject]);
        }
        
        // Start click animation
        activeAnimations[button.gameObject] = StartCoroutine(ButtonClickAnimation(button));
    }
    
    /// <summary>
    /// Button click animation coroutine
    /// </summary>
    private IEnumerator ButtonClickAnimation(Button button)
    {
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null) yield break;
        
        Vector3 originalScale = elementStates.ContainsKey(button.gameObject) ? 
            elementStates[button.gameObject].originalScale : rectTransform.localScale;
        
        // Scale down
        float elapsed = 0f;
        while (elapsed < buttonPressDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (buttonPressDuration * 0.5f);
            float scale = Mathf.Lerp(1f, buttonPressScale, t);
            rectTransform.localScale = originalScale * scale;
            yield return null;
        }
        
        // Scale back up
        elapsed = 0f;
        while (elapsed < buttonPressDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (buttonPressDuration * 0.5f);
            float scale = Mathf.Lerp(buttonPressScale, 1f, t);
            rectTransform.localScale = originalScale * scale;
            yield return null;
        }
        
        rectTransform.localScale = originalScale;
        
        // Remove from active animations
        if (activeAnimations.ContainsKey(button.gameObject))
        {
            activeAnimations.Remove(button.gameObject);
        }
    }
    
    /// <summary>
    /// Animate button hover
    /// </summary>
    private void AnimateButtonHover(Button button, bool isHovering)
    {
        if (!enableButtonAnimations) return;
        
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector3 originalScale = elementStates.ContainsKey(button.gameObject) ? 
            elementStates[button.gameObject].originalScale : rectTransform.localScale;
        
        float targetScale = isHovering ? buttonHoverScale : 1f;
        
        // Use DOTween for smooth hover animation
        rectTransform.DOScale(originalScale * targetScale, buttonHoverDuration)
            .SetEase(Ease.OutQuad);
    }
    
    /// <summary>
    /// Animate button press
    /// </summary>
    private void AnimateButtonPress(Button button, bool isPressed)
    {
        if (!enableButtonAnimations) return;
        
        var rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector3 originalScale = elementStates.ContainsKey(button.gameObject) ? 
            elementStates[button.gameObject].originalScale : rectTransform.localScale;
        
        float targetScale = isPressed ? buttonPressScale : 1f;
        
        rectTransform.DOScale(originalScale * targetScale, buttonPressDuration)
            .SetEase(Ease.OutQuad);
    }
    
    /// <summary>
    /// Animate slider value change
    /// </summary>
    private void AnimateSliderValueChange(Slider slider, float value)
    {
        if (!enableSliderAnimations) return;
        
        var fillRect = slider.fillRect;
        if (fillRect != null)
        {
            // Add subtle scale animation to fill area
            fillRect.DOScale(1.1f, 0.1f).OnComplete(() =>
            {
                fillRect.DOScale(1f, 0.1f);
            });
        }
        
        OnSliderValueChanged?.Invoke(slider.gameObject, value);
    }
    
    /// <summary>
    /// Animate toggle state change
    /// </summary>
    private void AnimateToggleStateChange(Toggle toggle, bool isOn)
    {
        if (!enableToggleAnimations) return;
        
        var toggleGraphic = toggle.graphic?.rectTransform;
        if (toggleGraphic != null)
        {
            // Scale animation
            float targetScale = isOn ? toggleScaleMultiplier : 1f;
            toggleGraphic.DOScale(targetScale, toggleAnimationDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    toggleGraphic.DOScale(1f, toggleAnimationDuration * 0.5f);
                });
        }
        
        OnToggleStateChanged?.Invoke(toggle.gameObject, isOn);
    }
    
    /// <summary>
    /// Animate input field focus
    /// </summary>
    private void AnimateInputFieldFocus(TMP_InputField inputField, bool isFocused)
    {
        if (!enableInputFieldAnimations) return;
        
        var rectTransform = inputField.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector3 originalScale = elementStates.ContainsKey(inputField.gameObject) ? 
            elementStates[inputField.gameObject].originalScale : rectTransform.localScale;
        
        float targetScale = isFocused ? inputFieldFocusScale : 1f;
        
        rectTransform.DOScale(originalScale * targetScale, inputFieldFocusDuration)
            .SetEase(Ease.OutQuad);
        
        OnInputFieldFocusChanged?.Invoke(inputField.gameObject, isFocused);
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Apply button styling based on state
    /// </summary>
    private void ApplyButtonStyling(Button button, ButtonState state)
    {
        var colors = button.colors;
        
        switch (state)
        {
            case ButtonState.Normal:
                // Use default colors
                break;
            case ButtonState.Highlighted:
                colors.normalColor = Color.Lerp(colors.normalColor, Color.white, 0.2f);
                break;
            case ButtonState.Pressed:
                colors.normalColor = Color.Lerp(colors.normalColor, Color.black, 0.2f);
                break;
            case ButtonState.Disabled:
                colors.normalColor = Color.Lerp(colors.normalColor, Color.gray, 0.5f);
                break;
        }
        
        button.colors = colors;
    }
    
    /// <summary>
    /// Play button sound effect
    /// </summary>
    private void PlayButtonSound()
    {
        if (!enableButtonSounds) return;
        
        // This would integrate with your audio system
        // For now, just log the action
        Debug.Log("Button sound played");
    }
    
    /// <summary>
    /// Load persistent state for UI element
    /// </summary>
    private void LoadElementPersistentState(GameObject element)
    {
        if (!enableStatePersistence) return;
        
        string key = stateSaveKey + element.GetInstanceID();
        if (persistentStates.ContainsKey(key))
        {
            if (persistentStates[key] is bool boolValue)
            {
                var toggle = element.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = boolValue;
                }
            }
        }
    }
    
    /// <summary>
    /// Save persistent state for UI element
    /// </summary>
    private void SaveElementPersistentState(GameObject element, object state)
    {
        if (!enableStatePersistence) return;
        
        string key = stateSaveKey + element.GetInstanceID();
        persistentStates[key] = state;
        
        // Save immediately for important states
        if (state is bool)
        {
            SavePersistentStates();
        }
    }
    
    /// <summary>
    /// Get UI element from pool
    /// </summary>
    public GameObject GetElementFromPool(string elementType)
    {
        if (!enablePooling || !objectPools.ContainsKey(elementType))
            return null;
        
        if (objectPools[elementType].Count > 0)
        {
            GameObject element = objectPools[elementType].Dequeue();
            element.SetActive(true);
            poolUsage[elementType]++;
            return element;
        }
        
        return null;
    }
    
    /// <summary>
    /// Return UI element to pool
    /// </summary>
    public void ReturnElementToPool(GameObject element, string elementType)
    {
        if (!enablePooling || !objectPools.ContainsKey(elementType))
            return;
        
        if (objectPools[elementType].Count < maxPooledObjects)
        {
            element.SetActive(false);
            objectPools[elementType].Enqueue(element);
            poolUsage[elementType]--;
        }
        else
        {
            // Destroy if pool is full
            Destroy(element);
        }
    }
    
    /// <summary>
    /// Optimize UI for performance
    /// </summary>
    private void OptimizeForPerformance()
    {
        if (!enableBatching) return;
        
        // Batch similar UI elements
        BatchSimilarElements();
        
        // Optimize raycast targets
        OptimizeRaycastTargets();
        
        Debug.Log("UI performance optimization completed");
    }
    
    /// <summary>
    /// Batch similar UI elements
    /// </summary>
    private void BatchSimilarElements()
    {
        // Group similar elements for better rendering performance
        // This is a simplified implementation
        Debug.Log("Batching similar UI elements for performance");
    }
    
    /// <summary>
    /// Optimize raycast targets
    /// </summary>
    private void OptimizeRaycastTargets()
    {
        // Disable raycast on non-interactive elements
        foreach (var kvp in elementStates)
        {
            GameObject element = kvp.Key;
            string elementType = kvp.Value.elementType;
            
            if (elementType != "Button" && elementType != "Toggle" && elementType != "Slider")
            {
                var graphic = element.GetComponent<Graphic>();
                if (graphic != null)
                {
                    graphic.raycastTarget = false;
                }
            }
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Set button interactable state
    /// </summary>
    public void SetButtonInteractable(Button button, bool interactable)
    {
        if (button == null) return;
        
        button.interactable = interactable;
        
        if (elementStates.ContainsKey(button.gameObject))
        {
            elementStates[button.gameObject].isInteractable = interactable;
        }
        
        ApplyButtonStyling(button, interactable ? ButtonState.Normal : ButtonState.Disabled);
    }
    
    /// <summary>
    /// Set slider value with animation
    /// </summary>
    public void SetSliderValue(Slider slider, float value, bool animate = true)
    {
        if (slider == null) return;
        
        if (animate && enableSliderAnimations)
        {
            DOTween.To(() => slider.value, x => slider.value = x, value, sliderAnimationDuration)
                .SetEase(sliderEase);
        }
        else
        {
            slider.value = value;
        }
    }
    
    /// <summary>
    /// Set toggle state with animation
    /// </summary>
    public void SetToggleState(Toggle toggle, bool isOn, bool animate = true)
    {
        if (toggle == null) return;
        
        if (animate && enableToggleAnimations)
        {
            // Animate state change
            AnimateToggleStateChange(toggle, isOn);
        }
        else
        {
            toggle.isOn = isOn;
        }
    }
    
    /// <summary>
    /// Get current state of UI element
    /// </summary>
    public UIElementState GetElementState(GameObject element)
    {
        return elementStates.ContainsKey(element) ? elementStates[element] : null;
    }
    
    /// <summary>
    /// Get all UI element states
    /// </summary>
    public Dictionary<GameObject, UIElementState> GetAllElementStates()
    {
        return new Dictionary<GameObject, UIElementState>(elementStates);
    }
    
    /// <summary>
    /// Refresh all UI elements
    /// </summary>
    public void RefreshAllElements()
    {
        foreach (var kvp in elementStates)
        {
            GameObject element = kvp.Key;
            UIElementState state = kvp.Value;
            
            switch (state.elementType)
            {
                case "Button":
                    var button = element.GetComponent<Button>();
                    if (button != null)
                        ApplyButtonStyling(button, button.interactable ? ButtonState.Normal : ButtonState.Disabled);
                    break;
                // Add other element types as needed
            }
        }
    }
    
    /// <summary>
    /// Save all persistent states
    /// </summary>
    public void SaveAllStates()
    {
        SavePersistentStates();
    }
    
    /// <summary>
    /// Get performance metrics
    /// </summary>
    public Dictionary<string, int> GetPerformanceMetrics()
    {
        var metrics = new Dictionary<string, int>
        {
            { "Total_Elements", elementStates.Count },
            { "Active_Animations", activeAnimations.Count },
            { "Pooled_Objects", GetTotalPooledObjects() },
            { "Persistent_States", persistentStates.Count }
        };
        
        return metrics;
    }
    
    /// <summary>
    /// Get total pooled objects
    /// </summary>
    private int GetTotalPooledObjects()
    {
        int total = 0;
        foreach (var pool in objectPools.Values)
        {
            total += pool.Count;
        }
        return total;
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Stop all animations
        foreach (var coroutine in activeAnimations.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        
        // Save persistent states
        if (enableStatePersistence)
        {
            SavePersistentStates();
        }
        
        // Clear dictionaries
        elementStates.Clear();
        persistentStates.Clear();
        activeAnimations.Clear();
        objectPools.Clear();
        poolUsage.Clear();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Save states when app is paused
            SavePersistentStates();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Save states when app loses focus
            SavePersistentStates();
        }
    }
}