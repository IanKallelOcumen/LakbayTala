using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using LakbayTala.Leaderboard;

/// <summary>
/// Enhanced leaderboard entry UI component with visual elements, animations, and cultural integration.
/// Supports rank changes, user interactions, and accessibility features.
/// </summary>
public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("Core UI Elements")]
    public Text rankText;
    public Text rankChangeText;
    public Image rankBadgeImage;
    public Text usernameText;
    public Text displayNameText;
    public Text scoreText;
    public Text culturalLevelText;
    public Text countryText;
    public Text lastActiveText;
    public Image avatarImage;
    public Image backgroundImage;
    public Button entryButton;
    
    [Header("Cultural Elements")]
    public Text creatureNameText;
    public Text baybayinNameText;
    public Text creatureDescriptionText;
    public Image creatureIconImage;
    public Image creatureBadgeImage;
    public GameObject culturalElementsContainer;
    
    [Header("Visual Indicators")]
    public GameObject onlineIndicator;
    public GameObject currentUserIndicator;
    public Image rankChangeArrow;
    public Sprite rankUpSprite;
    public Sprite rankDownSprite;
    public Sprite rankSameSprite;
    public Color rankUpColor = Color.green;
    public Color rankDownColor = Color.red;
    public Color rankSameColor = Color.gray;
    
    [Header("Animation Settings")]
    public float fadeInDuration = 0.3f;
    public float slideInDuration = 0.4f;
    public float rankChangeDuration = 0.6f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool enableHoverEffects = true;
    public float hoverScale = 1.05f;
    public float hoverDuration = 0.2f;
    
    [Header("Accessibility")]
    public bool enableAccessibilityFeatures = true;
    public bool enableBaybayinScript = true;
    public float highContrastMultiplier = 1.5f;
    public float largeTextScale = 1.2f;
    public bool enableScreenReaderSupport = true;
    
    [Header("Performance")]
    public bool useObjectPooling = true;
    public bool lazyLoadImages = true;
    public bool enableCaching = true;
    
    // State tracking
    private LeaderboardEntry currentEntry;
    private Vector3 originalScale;
    private Color originalColor;
    private CanvasGroup canvasGroup;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;
    
    // Animation coroutines
    private Coroutine fadeInCoroutine;
    private Coroutine slideInCoroutine;
    private Coroutine rankChangeCoroutine;
    private Coroutine hoverCoroutine;
    
    // Accessibility (internal for tests)
    private string screenReaderText = "";
    internal bool isHighContrast = false;
    internal bool isLargeText = false;
    
    void Awake()
    {
        InitializeComponents();
        CacheOriginalValues();
        SetupEventListeners();
    }
    
    void OnDestroy()
    {
        StopAllCoroutines();
    }
    
    /// <summary>
    /// Initialize UI components and references.
    /// </summary>
    private void InitializeComponents()
    {
        // Get required components
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
        
        // Setup button if not present
        if (entryButton == null)
        {
            entryButton = GetComponent<Button>();
            if (entryButton == null)
            {
                entryButton = gameObject.AddComponent<Button>();
            }
        }
        
        // Setup raycast target for accessibility
        var graphic = GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
    }
    
    /// <summary>
    /// Cache original values for animations and resets.
    /// </summary>
    private void CacheOriginalValues()
    {
        originalScale = transform.localScale;
        
        if (backgroundImage != null)
        {
            originalColor = backgroundImage.color;
        }
    }
    
    /// <summary>
    /// Setup event listeners for user interactions.
    /// </summary>
    private void SetupEventListeners()
    {
        if (entryButton != null)
        {
            entryButton.onClick.AddListener(OnEntryClicked);
        }
        
        // Setup hover events if enabled
        if (enableHoverEffects)
        {
            SetupHoverEvents();
        }
        
        // Setup accessibility events
        if (enableAccessibilityFeatures)
        {
            SetupAccessibilityEvents();
        }
    }
    
    /// <summary>
    /// Setup hover event listeners.
    /// </summary>
    private void SetupHoverEvents()
    {
        var trigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Add hover enter event
        var hoverEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEnter.callback.AddListener((data) => { OnHoverEnter(); });
        trigger.triggers.Add(hoverEnter);
        
        // Add hover exit event
        var hoverExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        hoverExit.callback.AddListener((data) => { OnHoverExit(); });
        trigger.triggers.Add(hoverExit);
    }
    
    /// <summary>
    /// Setup accessibility event listeners.
    /// </summary>
    private void SetupAccessibilityEvents()
    {
        // Add keyboard navigation support
        if (entryButton != null)
        {
            entryButton.navigation = new Navigation
            {
                mode = Navigation.Mode.Automatic
            };
        }
    }
    
    /// <summary>
    /// Setup entry from simple local leaderboard data (LeaderboardPanelController).
    /// </summary>
    public void SetupEntry(LocalLeaderboardEntry entry, int rank, Color rankColor)
    {
        if (rankText != null) { rankText.text = $"#{rank}"; rankText.color = rankColor; }
        if (usernameText != null) usernameText.text = entry.playerName;
        if (scoreText != null) scoreText.text = entry.score.ToString();
    }
    
    /// <summary>
    /// Setup enhanced leaderboard entry with cultural theming and all visual elements.
    /// </summary>
    public void SetupEnhancedEntry(LeaderboardEntry entry, int rank, string creatureName, Color creatureColor, string creatureDescription, string baybayinName)
    {
        currentEntry = entry;
        userId = entry.user.userId;
        
        // Setup core UI elements
        SetupCoreUI(entry, rank, creatureColor);
        
        // Setup cultural elements
        SetupCulturalUI(creatureName, creatureColor, creatureDescription, baybayinName);
        
        // Setup visual indicators
        SetupVisualIndicators(entry);
        
        // Setup accessibility
        SetupAccessibility(entry);
        
        // Generate screen reader text
        GenerateScreenReaderText(entry, rank, creatureName);
        
        // Start entry animation
        StartEntryAnimation();
    }
    
    /// <summary>
    /// Setup core UI elements.
    /// </summary>
    private void SetupCoreUI(LeaderboardEntry entry, int rank, Color creatureColor)
    {
        // Rank display
        if (rankText != null)
        {
            rankText.text = $"#{rank}";
            rankText.color = creatureColor;
            
            if (isLargeText)
            {
                rankText.fontSize = Mathf.RoundToInt(rankText.fontSize * largeTextScale);
            }
        }
        
        // Username and display name
        if (usernameText != null)
        {
            usernameText.text = entry.user.username;
            usernameText.color = isHighContrast ? Color.white : Color.black;
            
            if (isLargeText)
            {
                usernameText.fontSize = Mathf.RoundToInt(usernameText.fontSize * largeTextScale);
            }
        }
        
        if (displayNameText != null)
        {
            displayNameText.text = entry.user.displayName;
            displayNameText.color = isHighContrast ? Color.white : Color.gray;
            
            if (isLargeText)
            {
                displayNameText.fontSize = Mathf.RoundToInt(displayNameText.fontSize * largeTextScale);
            }
        }
        
        // Score display
        if (scoreText != null)
        {
            scoreText.text = entry.score.ToString("N0");
            scoreText.color = creatureColor;
            
            if (isLargeText)
            {
                scoreText.fontSize = Mathf.RoundToInt(scoreText.fontSize * largeTextScale);
            }
        }
        
        // Cultural level
        if (culturalLevelText != null)
        {
            culturalLevelText.text = entry.user.culturalLevel;
            culturalLevelText.color = isHighContrast ? Color.white : Color.gray;
            
            if (isLargeText)
            {
                culturalLevelText.fontSize = Mathf.RoundToInt(culturalLevelText.fontSize * largeTextScale);
            }
        }
        
        // Country
        if (countryText != null)
        {
            countryText.text = entry.user.country;
            countryText.color = isHighContrast ? Color.white : Color.gray;
            
            if (isLargeText)
            {
                countryText.fontSize = Mathf.RoundToInt(countryText.fontSize * largeTextScale);
            }
        }
        
        // Last active
        if (lastActiveText != null)
        {
            lastActiveText.text = GetTimeAgo(entry.user.lastActive);
            lastActiveText.color = isHighContrast ? Color.white : Color.gray;
            
            if (isLargeText)
            {
                lastActiveText.fontSize = Mathf.RoundToInt(lastActiveText.fontSize * largeTextScale);
            }
        }
        
        // Background
        if (backgroundImage != null)
        {
            Color bgColor = creatureColor;
            bgColor.a = 0.2f; // Semi-transparent
            backgroundImage.color = bgColor;
            
            if (isHighContrast)
            {
                backgroundImage.color = Color.Lerp(bgColor, Color.white, 0.5f);
            }
        }
        
        // Avatar (lazy loading)
        if (avatarImage != null && lazyLoadImages)
        {
            StartCoroutine(LoadAvatarImage(entry.user.avatarUrl));
        }
    }
    
    /// <summary>
    /// Setup cultural UI elements with Filipino mythology integration.
    /// </summary>
    private void SetupCulturalUI(string creatureName, Color creatureColor, string creatureDescription, string baybayinName)
    {
        if (creatureNameText != null)
        {
            creatureNameText.text = creatureName;
            creatureNameText.color = creatureColor;
            
            if (isLargeText)
            {
                creatureNameText.fontSize = Mathf.RoundToInt(creatureNameText.fontSize * largeTextScale);
            }
        }
        
        if (baybayinNameText != null && enableBaybayinScript)
        {
            baybayinNameText.text = baybayinName;
            baybayinNameText.color = creatureColor;
            
            if (isLargeText)
            {
                baybayinNameText.fontSize = Mathf.RoundToInt(baybayinNameText.fontSize * largeTextScale);
            }
        }
        
        if (creatureDescriptionText != null)
        {
            creatureDescriptionText.text = creatureDescription;
            creatureDescriptionText.color = isHighContrast ? Color.white : Color.gray;
            
            if (isLargeText)
            {
                creatureDescriptionText.fontSize = Mathf.RoundToInt(creatureDescriptionText.fontSize * largeTextScale);
            }
        }
        
        if (creatureIconImage != null)
        {
            creatureIconImage.color = creatureColor;
        }
        
        if (creatureBadgeImage != null)
        {
            creatureBadgeImage.color = creatureColor;
        }
        
        if (culturalElementsContainer != null)
        {
            culturalElementsContainer.SetActive(true);
        }
    }
    
    /// <summary>
    /// Setup visual indicators for user status and rank changes.
    /// </summary>
    private void SetupVisualIndicators(LeaderboardEntry entry)
    {
        // Online indicator
        if (onlineIndicator != null)
        {
            onlineIndicator.SetActive(entry.user.isOnline);
        }
        
        // Current user indicator
        if (currentUserIndicator != null)
        {
            currentUserIndicator.SetActive(entry.isCurrentUser);
        }
        
        // Rank change indicator
        SetupRankChangeIndicator(entry);
    }
    
    /// <summary>
    /// Setup rank change indicator with arrows and colors.
    /// </summary>
    private void SetupRankChangeIndicator(LeaderboardEntry entry)
    {
        if (rankChangeText != null)
        {
            int rankChange = entry.user.GetRankChange();
            
            if (rankChange > 0)
            {
                rankChangeText.text = $"+{rankChange}";
                rankChangeText.color = rankUpColor;
                
                if (rankChangeArrow != null && rankUpSprite != null)
                {
                    rankChangeArrow.sprite = rankUpSprite;
                    rankChangeArrow.color = rankUpColor;
                }
            }
            else if (rankChange < 0)
            {
                rankChangeText.text = rankChange.ToString();
                rankChangeText.color = rankDownColor;
                
                if (rankChangeArrow != null && rankDownSprite != null)
                {
                    rankChangeArrow.sprite = rankDownSprite;
                    rankChangeArrow.color = rankDownColor;
                }
            }
            else
            {
                rankChangeText.text = "-";
                rankChangeText.color = rankSameColor;
                
                if (rankChangeArrow != null && rankSameSprite != null)
                {
                    rankChangeArrow.sprite = rankSameSprite;
                    rankChangeArrow.color = rankSameColor;
                }
            }
        }
    }
    
    /// <summary>
    /// Setup accessibility features.
    /// </summary>
    private void SetupAccessibility(LeaderboardEntry entry)
    {
        // Apply high contrast if enabled
        if (isHighContrast)
        {
            ApplyHighContrast();
        }
        
        // Apply large text if enabled
        if (isLargeText)
        {
            ApplyLargeText();
        }
        
        // Setup screen reader support
        if (enableScreenReaderSupport)
        {
            SetupScreenReaderSupport(entry);
        }
    }
    
    /// <summary>
    /// Apply high contrast styling.
    /// </summary>
    private void ApplyHighContrast()
    {
        var texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            if (text.color.grayscale > 0.5f)
            {
                text.color = Color.white;
            }
            else
            {
                text.color = Color.black;
            }
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, Color.white, 0.5f);
        }
    }
    
    /// <summary>
    /// Apply large text scaling.
    /// </summary>
    private void ApplyLargeText()
    {
        var texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            text.fontSize = Mathf.RoundToInt(text.fontSize * largeTextScale);
        }
    }
    
    /// <summary>
    /// Setup screen reader support.
    /// </summary>
    private void SetupScreenReaderSupport(LeaderboardEntry entry)
    {
        // This would integrate with platform-specific screen reader APIs
        // For now, we'll set up the text content
        if (entryButton != null)
        {
            // Set accessibility label
            entryButton.gameObject.name = $"Leaderboard Entry: {entry.user.username}, Rank {entry.rank}, Score {entry.score}";
        }
    }
    
    /// <summary>
    /// Generate screen reader text.
    /// </summary>
    private void GenerateScreenReaderText(LeaderboardEntry entry, int rank, string creatureName)
    {
        screenReaderText = $"Rank {rank}, Player {entry.user.username}, Score {entry.score:N0}, " +
                          $"Cultural Level {entry.user.culturalLevel}, Country {entry.user.country}, " +
                          $"Last Active {GetTimeAgo(entry.user.lastActive)}, " +
                          $"Mythological Creature: {creatureName}";
        
        if (entry.user.HasRankImproved())
        {
            screenReaderText += $", Rank improved by {entry.user.GetRankChange()} positions";
        }
        else if (entry.user.GetRankChange() < 0)
        {
            screenReaderText += $", Rank decreased by {Mathf.Abs(entry.user.GetRankChange())} positions";
        }
        else
        {
            screenReaderText += ", Rank unchanged";
        }
        
        if (entry.user.isOnline)
        {
            screenReaderText += ", Currently online";
        }
        
        if (entry.isCurrentUser)
        {
            screenReaderText += ", This is you";
        }
    }
    
    /// <summary>
    /// Start entry animation.
    /// </summary>
    private void StartEntryAnimation()
    {
        if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
        if (slideInCoroutine != null) StopCoroutine(slideInCoroutine);
        
        fadeInCoroutine = StartCoroutine(FadeInAnimation());
        slideInCoroutine = StartCoroutine(SlideInAnimation());
    }
    
    /// <summary>
    /// Fade in animation.
    /// </summary>
    private IEnumerator FadeInAnimation()
    {
        if (canvasGroup == null) yield break;
        
        canvasGroup.alpha = 0f;
        
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            canvasGroup.alpha = animationCurve.Evaluate(t);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// Slide in animation.
    /// </summary>
    private IEnumerator SlideInAnimation()
    {
        if (rectTransform == null) yield break;
        
        Vector3 startPos = rectTransform.anchoredPosition;
        startPos.x += 100f; // Start from right
        rectTransform.anchoredPosition = startPos;
        
        float elapsed = 0f;
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideInDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            Vector3 currentPos = rectTransform.anchoredPosition;
            currentPos.x = Mathf.Lerp(startPos.x, 0f, curveValue);
            rectTransform.anchoredPosition = currentPos;
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = Vector3.zero;
    }
    
    /// <summary>
    /// Animate rank change with visual effects.
    /// </summary>
    public void AnimateRankChange(int rankChange)
    {
        if (rankChangeCoroutine != null) StopCoroutine(rankChangeCoroutine);
        rankChangeCoroutine = StartCoroutine(RankChangeAnimation(rankChange));
    }
    
    /// <summary>
    /// Rank change animation.
    /// </summary>
    private IEnumerator RankChangeAnimation(int rankChange)
    {
        // Bounce animation for rank change
        Vector3 originalPos = rectTransform.anchoredPosition;
        float bounceHeight = 30f * Mathf.Sign(rankChange);
        
        float elapsed = 0f;
        while (elapsed < rankChangeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rankChangeDuration;
            float bounceValue = Mathf.Sin(t * Mathf.PI * 2) * bounceHeight * (1f - t);
            
            rectTransform.anchoredPosition = originalPos + Vector3.up * bounceValue;
            yield return null;
        }
        
        rectTransform.anchoredPosition = originalPos;
    }
    
    /// <summary>
    /// Handle hover enter.
    /// </summary>
    private void OnHoverEnter()
    {
        if (isAnimating || !enableHoverEffects) return;
        
        isHovered = true;
        
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(HoverAnimation(true));
    }
    
    /// <summary>
    /// Handle hover exit.
    /// </summary>
    private void OnHoverExit()
    {
        if (!enableHoverEffects) return;
        
        isHovered = false;
        
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(HoverAnimation(false));
    }
    
    /// <summary>
    /// Hover animation.
    /// </summary>
    private IEnumerator HoverAnimation(bool isHovering)
    {
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;
        float elapsed = 0f;
        
        while (elapsed < hoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hoverDuration;
            
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    /// <summary>
    /// Handle entry click.
    /// </summary>
    private void OnEntryClicked()
    {
        // Trigger click animation
        StartCoroutine(ClickAnimation());
        
        // Notify parent of click
        var parentController = transform.GetComponentInParent<LakbayTalaLeaderboardUIController>();
        if (parentController != null)
        {
            // This would trigger user profile display
            Debug.Log($"Clicked on user: {currentEntry.user.username}");
        }
    }
    
    /// <summary>
    /// Click animation.
    /// </summary>
    private IEnumerator ClickAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 clickScale = originalScale * 0.95f;
        
        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, clickScale, elapsed / 0.1f);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(clickScale, originalScale, elapsed / 0.1f);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    /// <summary>
    /// Load avatar image with lazy loading.
    /// </summary>
    private IEnumerator LoadAvatarImage(string avatarUrl)
    {
        if (string.IsNullOrEmpty(avatarUrl) || avatarImage == null) yield break;
        
        // Simulate image loading (would be replaced with actual image loading)
        yield return new WaitForSeconds(0.1f);
        
        // Set placeholder color based on user ID hash
        Color placeholderColor = GenerateColorFromUserId(userId);
        avatarImage.color = placeholderColor;
    }
    
    /// <summary>
    /// Generate color from user ID for placeholder avatars.
    /// </summary>
    private Color GenerateColorFromUserId(string userId)
    {
        int hash = userId.GetHashCode();
        float r = (hash & 0xFF) / 255f;
        float g = ((hash >> 8) & 0xFF) / 255f;
        float b = ((hash >> 16) & 0xFF) / 255f;
        return new Color(r, g, b, 0.8f);
    }
    
    /// <summary>
    /// Get time ago string for last active time.
    /// </summary>
    private string GetTimeAgo(DateTime lastActive)
    {
        TimeSpan timeAgo = DateTime.Now - lastActive;
        
        if (timeAgo.TotalDays >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalDays)}d ago";
        else if (timeAgo.TotalHours >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalHours)}h ago";
        else if (timeAgo.TotalMinutes >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalMinutes)}m ago";
        else
            return "Just now";
    }
    
    /// <summary>
    /// Update accessibility features based on settings.
    /// </summary>
    public void UpdateAccessibility(bool highContrast, bool largeText)
    {
        isHighContrast = highContrast;
        isLargeText = largeText;
        
        if (highContrast)
        {
            ApplyHighContrast();
        }
        else
        {
            // Reset to original colors
            if (backgroundImage != null)
            {
                backgroundImage.color = originalColor;
            }
        }
        
        if (largeText)
        {
            ApplyLargeText();
        }
    }
    
    /// <summary>
    /// Get current user ID.
    /// </summary>
    public string GetUserId()
    {
        return userId;
    }
    
    /// <summary>
    /// Get current entry data.
    /// </summary>
    public LeaderboardEntry GetEntry()
    {
        return currentEntry;
    }
    
    // Public properties for external access
    public string userId { get; private set; }
    public bool isAnimating { get; private set; }
    public bool isHovered { get; private set; }
}