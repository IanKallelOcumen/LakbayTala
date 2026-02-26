using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using LakbayTala.Leaderboard;

/// <summary>
/// User profile modal controller for displaying detailed user information,
/// score history, achievements, and cultural progress in LakbayTala leaderboard system.
/// </summary>
public class UserProfileModalController : MonoBehaviour
{
    [Header("Modal UI")]
    public GameObject modalContainer;
    public Button closeButton;
    public Button minimizeButton;
    public Button shareButton;
    public Button reportButton;
    public Button addFriendButton;
    
    [Header("User Information")]
    public Image avatarImage;
    public Text usernameText;
    public Text displayNameText;
    public Text bioText;
    public Text culturalLevelText;
    public Text countryText;
    public Text joinedDateText;
    public Text lastActiveText;
    public GameObject onlineIndicator;
    public GameObject currentUserIndicator;
    
    [Header("Statistics")]
    public Text totalScoreText;
    public Text gamesPlayedText;
    public Text totalPlayTimeText;
    public Text averageScoreText;
    public Text highestScoreText;
    public Text winRateText;
    public Text rankText;
    public Text rankChangeText;
    
    [Header("Cultural Information")]
    public Text favoriteCreatureText;
    public Text culturalKnowledgeText;
    public Text artifactsCollectedText;
    public Text storiesDiscoveredText;
    public Text baybayinProgressText;
    public Transform culturalBadgesContainer;
    public GameObject culturalBadgePrefab;
    
    [Header("Score History")]
    public Transform scoreHistoryContainer;
    public GameObject scoreHistoryItemPrefab;
    public ScrollRect scoreHistoryScrollRect;
    public Button loadMoreScoresButton;
    public Text scoreHistoryEmptyText;
    
    [Header("Achievements")]
    public Transform achievementsContainer;
    public GameObject achievementItemPrefab;
    public ScrollRect achievementsScrollRect;
    public Text achievementsEmptyText;
    public Button showAllAchievementsButton;
    
    [Header("Recent Activity")]
    public Transform recentActivityContainer;
    public GameObject activityItemPrefab;
    public ScrollRect activityScrollRect;
    public Text activityEmptyText;
    
    [Header("Social Features")]
    public Text friendsCountText;
    public Text followersCountText;
    public Text followingCountText;
    public Button viewFriendsButton;
    public Button viewFollowersButton;
    public Button viewFollowingButton;
    public GameObject friendStatusIndicator;
    public Button sendMessageButton;
    
    [Header("Settings")]
    public bool enableAnimations = true;
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool enableLazyLoading = true;
    public int itemsPerLoad = 10;
    public bool enableCaching = true;
    
    [Header("Cultural Theming")]
    public LakbayTalaUITheme uiTheme;
    public bool enableCulturalTooltips = true;
    public bool enableBaybayinScript = true;
    public bool enableTraditionalColors = true;
    
    [Header("Error Handling")]
    public GameObject errorPanel;
    public Text errorText;
    public Button retryButton;
    public GameObject loadingPanel;
    public Text loadingText;
    
    // State management
    private LeaderboardUser currentUser;
#pragma warning disable CS0414
    private bool isLoading = false;
    private bool isInitialized = false;
    private int currentScoreHistoryPage = 0;
    private int currentAchievementsPage = 0;
    private int currentActivityPage = 0;
#pragma warning restore CS0414
    private bool isOwnProfile = false;
    
    // Data caches
    private List<ScoreHistory> scoreHistoryCache = new List<ScoreHistory>();
    private List<string> achievementsCache = new List<string>();
    private List<string> activityCache = new List<string>();
    private Dictionary<string, GameObject> cachedObjects = new Dictionary<string, GameObject>();
    
    // Animation coroutines
    private Coroutine modalAnimationCoroutine;
    private Coroutine contentLoadCoroutine;
    private Coroutine avatarLoadCoroutine;
    
    // Services
    private LeaderboardService leaderboardService;
    private LakbayTalaLeaderboardPanel culturalLeaderboard;
    
    // Event handlers
    public event Action<LeaderboardUser> OnProfileLoaded;
    public event Action<string> OnProfileError;
    public event Action OnProfileClosed;
    
    void Start()
    {
        InitializeController();
        SetupEventListeners();
        ApplyCulturalTheme();
    }
    
    void OnEnable()
    {
        if (isInitialized)
        {
            RefreshProfile();
        }
    }
    
    /// <summary>
    /// Initialize the profile modal controller with services and configuration.
    /// </summary>
    private void InitializeController()
    {
        // Get service references
        leaderboardService = LeaderboardService.Instance;
        culturalLeaderboard = LakbayTalaLeaderboardPanel.Instance;
        
        if (uiTheme == null)
            uiTheme = LakbayTalaUITheme.Instance;
        
        // Initialize UI state
        SetupInitialUIState();
        
        // Setup object pooling
        SetupObjectPooling();
        
        // Setup lazy loading
        SetupLazyLoading();
        
        isInitialized = true;
        Log("User Profile Modal Controller initialized");
    }
    
    /// <summary>
    /// Show user profile with the specified user ID.
    /// </summary>
    public void ShowProfile(LeaderboardUser user)
    {
        if (user == null)
        {
            LogError("Cannot show profile: user is null");
            return;
        }
        
        currentUser = user;
        isOwnProfile = user.userId == leaderboardService.GetCurrentUserId();
        
        // Reset pagination
        currentScoreHistoryPage = 0;
        currentAchievementsPage = 0;
        currentActivityPage = 0;
        
        // Clear caches
        scoreHistoryCache.Clear();
        achievementsCache.Clear();
        activityCache.Clear();
        
        // Show modal with animation
        StartCoroutine(ShowModalWithAnimation());
        
        // Load profile data
        StartCoroutine(LoadProfileData());
    }
    
    /// <summary>
    /// Show modal with entrance animation.
    /// </summary>
    private IEnumerator ShowModalWithAnimation()
    {
        if (modalContainer == null) yield break;
        
        modalContainer.SetActive(true);
        
        if (!enableAnimations) yield break;
        
        var canvasGroup = modalContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = modalContainer.AddComponent<CanvasGroup>();
        }
        
        var rectTransform = modalContainer.GetComponent<RectTransform>();
        
        // Start with invisible and scaled down
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            canvasGroup.alpha = curveValue;
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
    }
    
    /// <summary>
    /// Load comprehensive profile data.
    /// </summary>
    private IEnumerator LoadProfileData()
    {
        if (currentUser == null) yield break;
        
        isLoading = true;
        ShowLoadingState();
        
        // Load all profile sections (yield outside try to avoid CS1626)
        yield return StartCoroutine(LoadUserInformation());
        yield return StartCoroutine(LoadUserStatistics());
        yield return StartCoroutine(LoadCulturalInformation());
        yield return StartCoroutine(LoadScoreHistory());
        yield return StartCoroutine(LoadAchievements());
        yield return StartCoroutine(LoadRecentActivity());
        yield return StartCoroutine(LoadSocialInformation());
        
        try
        {
            OnProfileLoaded?.Invoke(currentUser);
            ShowContentState();
        }
        catch (Exception ex)
        {
            LogError($"Error loading profile data: {ex.Message}");
            ShowErrorState(ex.Message);
            OnProfileError?.Invoke(ex.Message);
        }
        finally
        {
            isLoading = false;
        }
    }
    
    /// <summary>
    /// Load user basic information.
    /// </summary>
    private IEnumerator LoadUserInformation()
    {
        // Update UI with user information
        if (usernameText != null)
            usernameText.text = currentUser.username;
        
        if (displayNameText != null)
            displayNameText.text = currentUser.displayName;
        
        if (bioText != null && !string.IsNullOrEmpty(currentUser.profile.bio))
            bioText.text = currentUser.profile.bio;
        
        if (culturalLevelText != null)
            culturalLevelText.text = currentUser.culturalLevel;
        
        if (countryText != null)
            countryText.text = currentUser.country;
        
        if (joinedDateText != null)
            joinedDateText.text = currentUser.profile.joinedDate.ToString("MMM dd, yyyy");
        
        if (lastActiveText != null)
            lastActiveText.text = GetTimeAgo(currentUser.lastActive);
        
        if (onlineIndicator != null)
            onlineIndicator.SetActive(currentUser.isOnline);
        
        if (currentUserIndicator != null)
            currentUserIndicator.SetActive(isOwnProfile);
        
        // Load avatar
        if (avatarImage != null && !string.IsNullOrEmpty(currentUser.avatarUrl))
        {
            yield return StartCoroutine(LoadAvatarImage(currentUser.avatarUrl));
        }
        
        // Apply cultural theming
        ApplyUserCulturalTheme();
        
        yield return null;
    }
    
    /// <summary>
    /// Load user statistics.
    /// </summary>
    private IEnumerator LoadUserStatistics()
    {
        // Calculate statistics
        int totalScore = currentUser.totalScore;
        int gamesPlayed = currentUser.profile.gamesPlayed;
        int totalPlayTime = currentUser.profile.totalPlayTime;
        float averageScore = gamesPlayed > 0 ? (float)totalScore / gamesPlayed : 0f;
        float winRate = gamesPlayed > 0 ? (float)currentUser.profile.gamesPlayed * 0.6f / gamesPlayed : 0f; // Simulated win rate
        
        // Update UI
        if (totalScoreText != null)
            totalScoreText.text = totalScore.ToString("N0");
        
        if (gamesPlayedText != null)
            gamesPlayedText.text = gamesPlayed.ToString();
        
        if (totalPlayTimeText != null)
            totalPlayTimeText.text = FormatPlayTime(totalPlayTime);
        
        if (averageScoreText != null)
            averageScoreText.text = averageScore.ToString("N0");
        
        if (highestScoreText != null)
            highestScoreText.text = (totalScore * 1.2f).ToString("N0"); // Simulated highest score
        
        if (winRateText != null)
            winRateText.text = $"{winRate:P1}";
        
        if (rankText != null)
            rankText.text = $"#{currentUser.currentRank}";
        
        if (rankChangeText != null)
        {
            int rankChange = currentUser.GetRankChange();
            rankChangeText.text = currentUser.GetRankChangeText();
            rankChangeText.color = rankChange > 0 ? Color.green : (rankChange < 0 ? Color.red : Color.gray);
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Load cultural information.
    /// </summary>
    private IEnumerator LoadCulturalInformation()
    {
        // Update cultural information
        if (favoriteCreatureText != null && !string.IsNullOrEmpty(currentUser.profile.favoriteCreature))
            favoriteCreatureText.text = currentUser.profile.favoriteCreature;
        
        if (culturalKnowledgeText != null)
        {
            int totalKnowledge = currentUser.profile.culturalKnowledge.Values.Sum();
            culturalKnowledgeText.text = $"{totalKnowledge} points";
        }
        
        if (artifactsCollectedText != null)
        {
            // Simulated artifact count
            artifactsCollectedText.text = UnityEngine.Random.Range(10, 50).ToString();
        }
        
        if (storiesDiscoveredText != null)
        {
            // Simulated stories count
            storiesDiscoveredText.text = UnityEngine.Random.Range(5, 25).ToString();
        }
        
        if (baybayinProgressText != null)
        {
            // Simulated Baybayin progress
            int baybayinProgress = UnityEngine.Random.Range(20, 80);
            baybayinProgressText.text = $"{baybayinProgress}%";
        }
        
        // Load cultural badges
        yield return StartCoroutine(LoadCulturalBadges());
        
        yield return null;
    }
    
    /// <summary>
    /// Load cultural badges.
    /// </summary>
    private IEnumerator LoadCulturalBadges()
    {
        if (culturalBadgesContainer == null || culturalBadgePrefab == null) yield break;
        
        // Clear existing badges
        foreach (Transform child in culturalBadgesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create sample cultural badges
        string[] badgeNames = { "Baybayin Scholar", "Mythology Master", "Cultural Explorer", "Story Keeper", "Artifact Collector" };
        string[] badgeDescriptions = { "Learned 10 Baybayin characters", "Mastered all mythological creatures", "Explored 5 cultural sites", "Discovered 10 stories", "Collected 20 artifacts" };
        
        for (int i = 0; i < badgeNames.Length; i++)
        {
            var badgeObject = Instantiate(culturalBadgePrefab, culturalBadgesContainer);
            
            var badgeNameText = badgeObject.GetComponentInChildren<Text>();
            if (badgeNameText != null)
            {
                badgeNameText.text = badgeNames[i];
            }
            
            // Apply cultural theming
            ApplyBadgeCulturalTheme(badgeObject, i);
            
            yield return new WaitForSeconds(0.1f); // Stagger creation for animation
        }
    }
    
    /// <summary>
    /// Load score history.
    /// </summary>
    private IEnumerator LoadScoreHistory()
    {
        if (scoreHistoryContainer == null) yield break;
        
        // Clear existing history
        foreach (Transform child in scoreHistoryContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Load sample score history
        if (currentUser.scoreHistory != null && currentUser.scoreHistory.Count > 0)
        {
            scoreHistoryEmptyText?.gameObject.SetActive(false);
            
            for (int i = 0; i < Mathf.Min(currentUser.scoreHistory.Count, 10); i++)
            {
                var scoreEntry = currentUser.scoreHistory[i];
                yield return StartCoroutine(CreateScoreHistoryItem(scoreEntry));
            }
            
            if (loadMoreScoresButton != null)
            {
                loadMoreScoresButton.gameObject.SetActive(currentUser.scoreHistory.Count > 10);
            }
        }
        else
        {
            if (scoreHistoryEmptyText != null)
            {
                scoreHistoryEmptyText.gameObject.SetActive(true);
                scoreHistoryEmptyText.text = "No score history available";
            }
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Create score history item.
    /// </summary>
    private IEnumerator CreateScoreHistoryItem(ScoreHistory scoreEntry)
    {
        if (scoreHistoryItemPrefab == null) yield break;
        
        var itemObject = Instantiate(scoreHistoryItemPrefab, scoreHistoryContainer);
        
        var scoreText = itemObject.transform.Find("ScoreText")?.GetComponent<Text>();
        if (scoreText != null)
        {
            scoreText.text = scoreEntry.score.ToString("N0");
        }
        
        var dateText = itemObject.transform.Find("DateText")?.GetComponent<Text>();
        if (dateText != null)
        {
            dateText.text = scoreEntry.timestamp.ToString("MMM dd, yyyy");
        }
        
        var gameModeText = itemObject.transform.Find("GameModeText")?.GetComponent<Text>();
        if (gameModeText != null)
        {
            gameModeText.text = scoreEntry.gameMode ?? "Standard";
        }
        
        // Apply cultural theming
        ApplyScoreHistoryCulturalTheme(itemObject, scoreEntry);
        
        yield return null;
    }
    
    /// <summary>
    /// Load achievements.
    /// </summary>
    private IEnumerator LoadAchievements()
    {
        if (achievementsContainer == null) yield break;
        
        // Clear existing achievements
        foreach (Transform child in achievementsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Load sample achievements
        string[] achievementNames = { "First Steps in Laguna", "Mount Makiling Explorer", "Lake Mohikap Visitor", "Sampaloc Lake Master", "Botocan Falls Adventurer" };
        
        if (achievementsEmptyText != null)
        {
            achievementsEmptyText.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < achievementNames.Length; i++)
        {
            var achievementObject = Instantiate(achievementItemPrefab, achievementsContainer);
            
            var achievementNameText = achievementObject.transform.Find("NameText")?.GetComponent<Text>();
            if (achievementNameText != null)
            {
                achievementNameText.text = achievementNames[i];
            }
            
            // Apply cultural theming
            ApplyAchievementCulturalTheme(achievementObject, i);
            
            yield return new WaitForSeconds(0.1f); // Stagger creation for animation
        }
        
        if (showAllAchievementsButton != null)
        {
            showAllAchievementsButton.gameObject.SetActive(true);
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Load recent activity.
    /// </summary>
    private IEnumerator LoadRecentActivity()
    {
        if (recentActivityContainer == null) yield break;
        
        // Clear existing activity
        foreach (Transform child in recentActivityContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Load sample recent activity
        string[] activities = { "Completed Mount Makiling level", "Discovered new Baybayin character", "Collected rare artifact", "Unlocked new creature", "Completed cultural quiz" };
        
        if (activityEmptyText != null)
        {
            activityEmptyText.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < activities.Length; i++)
        {
            var activityObject = Instantiate(activityItemPrefab, recentActivityContainer);
            
            var activityText = activityObject.GetComponentInChildren<Text>();
            if (activityText != null)
            {
                activityText.text = activities[i];
            }
            
            // Apply cultural theming
            ApplyActivityCulturalTheme(activityObject, i);
            
            yield return new WaitForSeconds(0.1f); // Stagger creation for animation
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Load social information.
    /// </summary>
    private IEnumerator LoadSocialInformation()
    {
        // Update social counts (simulated)
        if (friendsCountText != null)
        {
            friendsCountText.text = UnityEngine.Random.Range(10, 100).ToString();
        }
        
        if (followersCountText != null)
        {
            followersCountText.text = UnityEngine.Random.Range(20, 200).ToString();
        }
        
        if (followingCountText != null)
        {
            followingCountText.text = UnityEngine.Random.Range(15, 150).ToString();
        }
        
        // Update friend status
        if (friendStatusIndicator != null)
        {
            friendStatusIndicator.SetActive(!isOwnProfile && UnityEngine.Random.Range(0, 2) == 1);
        }
        
        // Update button states
        if (addFriendButton != null)
        {
            addFriendButton.gameObject.SetActive(!isOwnProfile);
        }
        
        if (sendMessageButton != null)
        {
            sendMessageButton.gameObject.SetActive(!isOwnProfile);
        }
        
        yield return null;
    }
    
    /// <summary>
    /// Apply cultural theming to the profile modal.
    /// </summary>
    private void ApplyCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply background colors
        var backgroundImages = GetComponentsInChildren<Image>();
        foreach (var image in backgroundImages)
        {
            if (image.gameObject.name.Contains("Background"))
            {
                image.color = uiTheme.backgroundColor;
            }
        }
        
        // Apply text colors
        var texts = GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            text.color = uiTheme.textColor;
        }
        
        // Apply button colors
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            var colors = button.colors;
            colors.normalColor = uiTheme.primaryColor;
            colors.highlightedColor = uiTheme.secondaryColor;
            colors.pressedColor = uiTheme.accentColor;
            button.colors = colors;
        }
    }
    
    /// <summary>
    /// Apply user-specific cultural theming.
    /// </summary>
    private void ApplyUserCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply user-specific colors based on cultural level
        Color userColor = GetUserColorByCulturalLevel(currentUser.culturalLevel);
        
        if (avatarImage != null)
        {
            avatarImage.color = userColor;
        }
        
        if (culturalLevelText != null)
        {
            culturalLevelText.color = userColor;
        }
        
        if (rankText != null)
        {
            rankText.color = userColor;
        }
    }
    
    /// <summary>
    /// Get user color based on cultural level.
    /// </summary>
    private Color GetUserColorByCulturalLevel(string culturalLevel)
    {
        if (culturalLeaderboard != null && culturalLeaderboard.rankColors != null)
        {
            // Map cultural level to rank colors
            string[] levels = { "Novice", "Apprentice", "Explorer", "Scholar", "Master", "Legend" };
            int levelIndex = Array.IndexOf(levels, culturalLevel);
            if (levelIndex >= 0 && levelIndex < culturalLeaderboard.rankColors.Length)
            {
                return culturalLeaderboard.rankColors[levelIndex];
            }
        }
        
        return uiTheme?.primaryColor ?? Color.white;
    }
    
    /// <summary>
    /// Apply cultural theming to badge.
    /// </summary>
    private void ApplyBadgeCulturalTheme(GameObject badgeObject, int index)
    {
        if (uiTheme == null) return;
        
        var badgeImage = badgeObject.GetComponent<Image>();
        if (badgeImage != null)
        {
            Color badgeColor = Color.Lerp(uiTheme.primaryColor, uiTheme.secondaryColor, (float)index / 5f);
            badgeImage.color = badgeColor;
        }
    }
    
    /// <summary>
    /// Apply cultural theming to score history item.
    /// </summary>
    private void ApplyScoreHistoryCulturalTheme(GameObject itemObject, ScoreHistory scoreEntry)
    {
        if (uiTheme == null) return;
        
        var itemImage = itemObject.GetComponent<Image>();
        if (itemImage != null)
        {
            Color itemColor = Color.Lerp(uiTheme.backgroundColor, uiTheme.primaryColor, 0.3f);
            itemImage.color = itemColor;
        }
    }
    
    /// <summary>
    /// Apply cultural theming to achievement.
    /// </summary>
    private void ApplyAchievementCulturalTheme(GameObject achievementObject, int index)
    {
        if (uiTheme == null) return;
        
        var achievementImage = achievementObject.GetComponent<Image>();
        if (achievementImage != null)
        {
            Color achievementColor = Color.Lerp(uiTheme.accentColor, uiTheme.primaryColor, (float)index / 5f);
            achievementImage.color = achievementColor;
        }
    }
    
    /// <summary>
    /// Apply cultural theming to activity.
    /// </summary>
    private void ApplyActivityCulturalTheme(GameObject activityObject, int index)
    {
        if (uiTheme == null) return;
        
        var activityImage = activityObject.GetComponent<Image>();
        if (activityImage != null)
        {
            Color activityColor = Color.Lerp(uiTheme.secondaryColor, uiTheme.accentColor, (float)index / 5f);
            activityImage.color = activityColor;
        }
    }
    
    /// <summary>
    /// Load avatar image.
    /// </summary>
    private IEnumerator LoadAvatarImage(string avatarUrl)
    {
        if (string.IsNullOrEmpty(avatarUrl) || avatarImage == null) yield break;
        
        // Simulate avatar loading
        yield return new WaitForSeconds(0.2f);
        
        // Set placeholder color based on user ID
        Color avatarColor = GenerateAvatarColor(currentUser.userId);
        avatarImage.color = avatarColor;
    }
    
    /// <summary>
    /// Generate avatar color from user ID.
    /// </summary>
    private Color GenerateAvatarColor(string userId)
    {
        int hash = userId.GetHashCode();
        float r = (hash & 0xFF) / 255f;
        float g = ((hash >> 8) & 0xFF) / 255f;
        float b = ((hash >> 16) & 0xFF) / 255f;
        return new Color(r, g, b, 0.8f);
    }
    
    /// <summary>
    /// Get time ago string.
    /// </summary>
    private string GetTimeAgo(DateTime dateTime)
    {
        TimeSpan timeAgo = DateTime.Now - dateTime;
        
        if (timeAgo.TotalDays >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalDays)} days ago";
        else if (timeAgo.TotalHours >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalHours)} hours ago";
        else if (timeAgo.TotalMinutes >= 1)
            return $"{Mathf.FloorToInt((float)timeAgo.TotalMinutes)} minutes ago";
        else
            return "Just now";
    }
    
    /// <summary>
    /// Format play time.
    /// </summary>
    private string FormatPlayTime(int minutes)
    {
        if (minutes < 60)
            return $"{minutes} minutes";
        else if (minutes < 1440)
            return $"{Mathf.FloorToInt(minutes / 60f)} hours";
        else
            return $"{Mathf.FloorToInt(minutes / 1440f)} days";
    }
    
    /// <summary>
    /// Setup event listeners.
    /// </summary>
    private void SetupEventListeners()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseProfile);
        
        if (minimizeButton != null)
            minimizeButton.onClick.AddListener(MinimizeProfile);
        
        if (shareButton != null)
            shareButton.onClick.AddListener(ShareProfile);
        
        if (reportButton != null)
            reportButton.onClick.AddListener(ReportUser);
        
        if (addFriendButton != null)
            addFriendButton.onClick.AddListener(AddFriend);
        
        if (sendMessageButton != null)
            sendMessageButton.onClick.AddListener(SendMessage);
        
        if (loadMoreScoresButton != null)
            loadMoreScoresButton.onClick.AddListener(LoadMoreScores);
        
        if (showAllAchievementsButton != null)
            showAllAchievementsButton.onClick.AddListener(ShowAllAchievements);
        
        if (viewFriendsButton != null)
            viewFriendsButton.onClick.AddListener(ViewFriends);
        
        if (viewFollowersButton != null)
            viewFollowersButton.onClick.AddListener(ViewFollowers);
        
        if (viewFollowingButton != null)
            viewFollowingButton.onClick.AddListener(ViewFollowing);
        
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLoadProfile);
    }
    
    /// <summary>
    /// Setup object pooling.
    /// </summary>
    private void SetupObjectPooling()
    {
        // Initialize object pools for performance
        // Implementation would depend on specific requirements
    }
    
    /// <summary>
    /// Setup lazy loading.
    /// </summary>
    private void SetupLazyLoading()
    {
        if (scoreHistoryScrollRect != null)
        {
            scoreHistoryScrollRect.onValueChanged.AddListener(OnScoreHistoryScrollChanged);
        }
        
        if (achievementsScrollRect != null)
        {
            achievementsScrollRect.onValueChanged.AddListener(OnAchievementsScrollChanged);
        }
        
        if (activityScrollRect != null)
        {
            activityScrollRect.onValueChanged.AddListener(OnActivityScrollChanged);
        }
    }
    
    /// <summary>
    /// Setup initial UI state.
    /// </summary>
    private void SetupInitialUIState()
    {
        if (modalContainer != null)
            modalContainer.SetActive(false);
        
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
        
        if (errorPanel != null)
            errorPanel.SetActive(false);
        
        if (loadingText != null)
            loadingText.text = "Loading profile...";
    }
    
    /// <summary>
    /// Show loading state.
    /// </summary>
    private void ShowLoadingState()
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (errorPanel != null) errorPanel.SetActive(false);
    }
    
    /// <summary>
    /// Show content state.
    /// </summary>
    private void ShowContentState()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
    }
    
    /// <summary>
    /// Show error state.
    /// </summary>
    private void ShowErrorState(string errorMessage)
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(true);
        
        if (errorText != null)
        {
            errorText.text = errorMessage;
        }
    }
    
    /// <summary>
    /// Refresh profile data.
    /// </summary>
    public void RefreshProfile()
    {
        if (currentUser != null)
        {
            StartCoroutine(LoadProfileData());
        }
    }
    
    /// <summary>
    /// Close profile modal.
    /// </summary>
    public void CloseProfile()
    {
        StartCoroutine(CloseModalWithAnimation());
    }
    
    /// <summary>
    /// Close modal with exit animation.
    /// </summary>
    private IEnumerator CloseModalWithAnimation()
    {
        if (!enableAnimations || modalContainer == null)
        {
            modalContainer?.SetActive(false);
            OnProfileClosed?.Invoke();
            yield break;
        }
        
        var canvasGroup = modalContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            modalContainer.SetActive(false);
            OnProfileClosed?.Invoke();
            yield break;
        }
        
        var rectTransform = modalContainer.GetComponent<RectTransform>();
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = 1f - animationCurve.Evaluate(t);
            
            canvasGroup.alpha = 1f - curveValue;
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, curveValue);
            
            yield return null;
        }
        
        modalContainer.SetActive(false);
        OnProfileClosed?.Invoke();
    }
    
    // Event handlers
    private void MinimizeProfile() { /* Implementation */ }
    private void ShareProfile() { /* Implementation */ }
    private void ReportUser() { /* Implementation */ }
    private void AddFriend() { /* Implementation */ }
    private void SendMessage() { /* Implementation */ }
    private void LoadMoreScores() { /* Implementation */ }
    private void ShowAllAchievements() { /* Implementation */ }
    private void ViewFriends() { /* Implementation */ }
    private void ViewFollowers() { /* Implementation */ }
    private void ViewFollowing() { /* Implementation */ }
    private void RetryLoadProfile() { StartCoroutine(LoadProfileData()); }
    
    // Scroll event handlers
    private void OnScoreHistoryScrollChanged(Vector2 scrollPosition) { /* Lazy loading implementation */ }
    private void OnAchievementsScrollChanged(Vector2 scrollPosition) { /* Lazy loading implementation */ }
    private void OnActivityScrollChanged(Vector2 scrollPosition) { /* Lazy loading implementation */ }
    
    // Utility methods
    private void Log(string message) => Debug.Log($"[UserProfileModal] {message}");
    private void LogWarning(string message) => Debug.LogWarning($"[UserProfileModal] {message}");
    private void LogError(string message) => Debug.LogError($"[UserProfileModal] {message}");
}