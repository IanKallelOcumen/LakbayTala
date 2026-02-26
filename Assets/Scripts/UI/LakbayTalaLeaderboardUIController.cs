using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using LakbayTala.Leaderboard;
using LakbayTala.UI.Core;

/// <summary>
/// Enhanced leaderboard UI controller with comprehensive features including real-time updates,
/// advanced filtering/sorting, search functionality, and responsive design.
/// </summary>
public class LakbayTalaLeaderboardUIController : UIPanel
{
    [Header("UI References")]
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;
    public GameObject userProfileModal;
    public GameObject loadingPanel;
    public GameObject errorPanel;
    public GameObject emptyStatePanel;
    
    [Header("Header Controls")]
    public Text titleText;
    public Text subtitleText;
    public Button refreshButton;
    public Button closeButton;
    public Button searchButton;
    public Button filterButton;
    
    [Header("Search and Filter")]
    public InputField searchInput;
    public Dropdown filterDropdown;
    public Dropdown timeFrameDropdown;
    public Dropdown leaderboardTypeDropdown;
    public GameObject searchPanel;
    public GameObject filterPanel;
    
    [Header("Sorting Controls")]
    public Button sortByScoreButton;
    public Button sortByRankButton;
    public Button sortByNameButton;
    public Button sortByCountryButton;
    public Button sortByRecentActivityButton;
    public Color activeSortColor = Color.yellow;
    public Color inactiveSortColor = Color.white;
    
    [Header("Pagination Controls")]
    public Button previousPageButton;
    public Button nextPageButton;
    public Text pageInfoText;
    public Button[] pageButtons;
    public GameObject paginationPanel;
    
    [Header("Visual Elements")]
    public Image backgroundImage;
    public Image bannerImage;
    public GameObject skeletonLoader;
    public ScrollRect scrollRect;
    public RectTransform contentRectTransform;
    
    [Header("Performance Settings")]
    public bool enableVirtualScrolling = true;
    public int itemsPerPage = 25;
    public float scrollThreshold = 0.8f;
    public bool enableLazyLoading = true;
    public int preloadCount = 10;
    
    [Header("Animation Settings")]
    public float entryAnimationDuration = 0.3f;
    public AnimationCurve entryAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool enableAnimations = true;
    public bool enableRankChangeAnimations = true;
    public float rankChangeAnimationDuration = 0.5f;
    
    [Header("Cultural Integration")]
    public LakbayTalaUITheme uiTheme;
    public bool enableCulturalTooltips = true;
    public bool enableBaybayinScript = true;
    public bool enableTraditionalColors = true;
    
    // State management
    private LeaderboardData currentLeaderboard;
    private SearchQuery currentQuery;
    private List<GameObject> activeEntryObjects = new List<GameObject>();
    private Dictionary<string, GameObject> entryObjectPool = new Dictionary<string, GameObject>();
    private bool isLoading = false;
    private bool isInitialized = false;
    private int currentPage = 1;
    private int totalPages = 1;
    internal SortCriteria currentSort = SortCriteria.Score;
    private bool sortDescending = true;
    private string currentSearchTerm = "";
    private FilterCriteria currentFilter = FilterCriteria.All;
    private LeaderboardType currentType = LeaderboardType.Global;
    private LeaderboardTimeFrame currentTimeFrame = LeaderboardTimeFrame.AllTime;
    
    // Services
    private LeaderboardService leaderboardService;
    private LakbayTalaLeaderboardPanel culturalLeaderboard;
    
    // Performance tracking
    private float lastScrollPosition = 0f;
#pragma warning disable CS0414
    private bool isScrolling = false;
#pragma warning restore CS0414
    private Coroutine scrollCoroutine;
    private Dictionary<string, Coroutine> activeAnimations = new Dictionary<string, Coroutine>();
    
    // Debouncing
    private float searchDebounceTime = 0.3f;
    private Coroutine searchDebounceCoroutine;
    
    void Start()
    {
        InitializeController();
        SetupEventListeners();
        ApplyCulturalTheme();
        LoadInitialData();
    }
    
    void OnEnable()
    {
        if (isInitialized)
        {
            RefreshLeaderboard();
        }
    }
    
    /// <summary>
    /// Initialize the leaderboard UI controller with services and configuration.
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
        
        // Setup pagination
        SetupPagination();
        
        // Setup search debouncing
        SetupSearchDebounce();
        
        isInitialized = true;
        Log("Leaderboard UI Controller initialized");
    }
    
    /// <summary>
    /// Setup event listeners for UI interactions.
    /// </summary>
    private void SetupEventListeners()
    {
        // Header controls
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshClicked);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
        
        if (searchButton != null)
            searchButton.onClick.AddListener(ToggleSearchPanel);
        
        if (filterButton != null)
            filterButton.onClick.AddListener(ToggleFilterPanel);
        
        // Search and filter inputs
        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchInputChanged);
        
        if (filterDropdown != null)
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        
        if (timeFrameDropdown != null)
            timeFrameDropdown.onValueChanged.AddListener(OnTimeFrameChanged);
        
        if (leaderboardTypeDropdown != null)
            leaderboardTypeDropdown.onValueChanged.AddListener(OnLeaderboardTypeChanged);
        
        // Sorting buttons
        if (sortByScoreButton != null)
            sortByScoreButton.onClick.AddListener(() => SetSortCriteria(SortCriteria.Score));
        
        if (sortByRankButton != null)
            sortByRankButton.onClick.AddListener(() => SetSortCriteria(SortCriteria.Rank));
        
        if (sortByNameButton != null)
            sortByNameButton.onClick.AddListener(() => SetSortCriteria(SortCriteria.Name));
        
        if (sortByCountryButton != null)
            sortByCountryButton.onClick.AddListener(() => SetSortCriteria(SortCriteria.Country));
        
        if (sortByRecentActivityButton != null)
            sortByRecentActivityButton.onClick.AddListener(() => SetSortCriteria(SortCriteria.RecentActivity));
        
        // Pagination
        if (previousPageButton != null)
            previousPageButton.onClick.AddListener(() => ChangePage(currentPage - 1));
        
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(() => ChangePage(currentPage + 1));
        
        // Scroll events
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
        
        // Service events
        if (leaderboardService != null)
        {
            leaderboardService.OnLeaderboardUpdate += OnLeaderboardUpdateReceived;
            leaderboardService.OnConnectionStatusChanged += OnConnectionStatusChanged;
        }
    }
    
    /// <summary>
    /// Apply cultural theming to UI elements.
    /// </summary>
    private void ApplyCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply colors to background and banner
        if (backgroundImage != null)
        {
            backgroundImage.color = uiTheme.backgroundColor;
        }
        
        if (bannerImage != null)
        {
            bannerImage.color = uiTheme.primaryColor;
        }
        
        // Apply cultural elements to text
        if (titleText != null)
        {
            titleText.text = "Cultural Leaderboard";
            titleText.color = uiTheme.textColor;
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = "Discover the mythological creatures of Laguna";
            subtitleText.color = uiTheme.textColor;
        }
        
        // Apply traditional colors to buttons
        ApplyButtonColors();
    }
    
    /// <summary>
    /// Apply traditional Filipino colors to UI buttons.
    /// </summary>
    private void ApplyButtonColors()
    {
        if (!enableTraditionalColors) return;
        
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
    /// Load initial leaderboard data.
    /// </summary>
    private void LoadInitialData()
    {
        ShowLoadingState();
        
        // Create initial search query
        currentQuery = new SearchQuery
        {
            page = currentPage,
            pageSize = itemsPerPage,
            sortBy = currentSort,
            sortDescending = sortDescending,
            searchTerm = currentSearchTerm,
            additionalFilters = new Dictionary<string, object>()
        };
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Load leaderboard data from service.
    /// </summary>
    private IEnumerator LoadLeaderboardData(SearchQuery query)
    {
        isLoading = true;
        ShowLoadingState();
        
        yield return StartCoroutine(leaderboardService.LoadLeaderboard(query, (response) =>
        {
            isLoading = false;
            
            if (response.success)
            {
                currentLeaderboard = response.data;
                totalPages = Mathf.CeilToInt((float)response.totalCount / query.pageSize);
                
                RenderLeaderboardEntries(response.data.entries);
                UpdatePagination();
                
                if (response.data.entries.Count == 0)
                {
                    ShowEmptyState();
                }
                else
                {
                    ShowContentState();
                }
            }
            else
            {
                ShowErrorState(response.error);
            }
        }));
    }
    
    /// <summary>
    /// Render leaderboard entries with animations and visual elements.
    /// </summary>
    private void RenderLeaderboardEntries(List<LeaderboardEntry> entries)
    {
        // Clear existing entries
        ClearLeaderboardEntries();
        
        // Use object pooling for performance
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var entryObject = GetOrCreateEntryObject(entry.user.userId);
            
            if (entryObject != null)
            {
                SetupEntryUI(entryObject, entry, i);
                AnimateEntry(entryObject, i);
                
                activeEntryObjects.Add(entryObject);
            }
        }
        
        Log($"Rendered {entries.Count} leaderboard entries");
    }
    
    /// <summary>
    /// Setup UI for individual leaderboard entry.
    /// </summary>
    private void SetupEntryUI(GameObject entryObject, LeaderboardEntry entry, int index)
    {
        var entryUI = entryObject.GetComponent<LeaderboardEntryUI>();
        if (entryUI == null)
        {
            entryUI = entryObject.AddComponent<LeaderboardEntryUI>();
        }
        
        // Setup entry with cultural theming
        string creatureName = GetCreatureNameForRank(entry.rank);
        Color creatureColor = GetCreatureColorForRank(entry.rank);
        string creatureDescription = GetCreatureDescriptionForRank(entry.rank);
        string baybayinName = GetBaybayinNameForRank(entry.rank);
        
        entryUI.SetupEnhancedEntry(entry, entry.rank, creatureName, creatureColor, creatureDescription, baybayinName);
        
        // Add click handler for user profile
        var button = entryObject.GetComponent<Button>();
        if (button == null)
        {
            button = entryObject.AddComponent<Button>();
        }
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ShowUserProfile(entry.user));
        
        // Apply rank change animation if applicable
        if (enableRankChangeAnimations && entry.user.HasRankImproved())
        {
            AnimateRankChange(entryObject, entry.user.GetRankChange());
        }
    }
    
    /// <summary>
    /// Get mythological creature name for rank position.
    /// </summary>
    private string GetCreatureNameForRank(int rank)
    {
        if (culturalLeaderboard != null && culturalLeaderboard.rankTitles != null && rank <= culturalLeaderboard.rankTitles.Length)
        {
            return culturalLeaderboard.rankTitles[rank - 1];
        }
        return $"Rank {rank}";
    }
    
    /// <summary>
    /// Get creature color for rank position.
    /// </summary>
    private Color GetCreatureColorForRank(int rank)
    {
        if (culturalLeaderboard != null && culturalLeaderboard.rankColors != null && rank <= culturalLeaderboard.rankColors.Length)
        {
            return culturalLeaderboard.rankColors[rank - 1];
        }
        return Color.white;
    }
    
    /// <summary>
    /// Get creature description for rank position.
    /// </summary>
    private string GetCreatureDescriptionForRank(int rank)
    {
        if (culturalLeaderboard != null && culturalLeaderboard.rankDescriptions != null && rank <= culturalLeaderboard.rankDescriptions.Length)
        {
            return culturalLeaderboard.rankDescriptions[rank - 1];
        }
        return "";
    }
    
    /// <summary>
    /// Get Baybayin name for rank position.
    /// </summary>
    private string GetBaybayinNameForRank(int rank)
    {
        if (culturalLeaderboard != null && culturalLeaderboard.rankBaybayinNames != null && rank <= culturalLeaderboard.rankBaybayinNames.Length)
        {
            return culturalLeaderboard.rankBaybayinNames[rank - 1];
        }
        return "";
    }
    
    /// <summary>
    /// Animate entry appearance.
    /// </summary>
    private void AnimateEntry(GameObject entryObject, int index)
    {
        if (!enableAnimations) return;
        
        var canvasGroup = entryObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = entryObject.AddComponent<CanvasGroup>();
        }
        
        var rectTransform = entryObject.GetComponent<RectTransform>();
        
        // Start with invisible and scaled down
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.zero;
        
        // Animate in
        var animationKey = $"entry_{entryObject.GetInstanceID()}";
        if (activeAnimations.ContainsKey(animationKey))
        {
            StopCoroutine(activeAnimations[animationKey]);
        }
        
        activeAnimations[animationKey] = StartCoroutine(EntryAnimationCoroutine(canvasGroup, rectTransform, index * 0.1f));
    }
    
    /// <summary>
    /// Entry animation coroutine.
    /// </summary>
    private IEnumerator EntryAnimationCoroutine(CanvasGroup canvasGroup, RectTransform rectTransform, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one;
        
        while (elapsed < entryAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / entryAnimationDuration;
            float curveValue = entryAnimationCurve.Evaluate(t);
            
            canvasGroup.alpha = curveValue;
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        rectTransform.localScale = targetScale;
    }
    
    /// <summary>
    /// Animate rank change.
    /// </summary>
    private void AnimateRankChange(GameObject entryObject, int rankChange)
    {
        var animationKey = $"rank_{entryObject.GetInstanceID()}";
        if (activeAnimations.ContainsKey(animationKey))
        {
            StopCoroutine(activeAnimations[animationKey]);
        }
        
        activeAnimations[animationKey] = StartCoroutine(RankChangeAnimationCoroutine(entryObject, rankChange));
    }
    
    /// <summary>
    /// Rank change animation coroutine.
    /// </summary>
    private IEnumerator RankChangeAnimationCoroutine(GameObject entryObject, int rankChange)
    {
        var rectTransform = entryObject.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.anchoredPosition;
        
        float elapsed = 0f;
        float bounceHeight = 20f * Mathf.Sign(rankChange);
        
        while (elapsed < rankChangeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rankChangeAnimationDuration;
            float bounceValue = Mathf.Sin(t * Mathf.PI) * bounceHeight;
            
            rectTransform.anchoredPosition = originalPosition + Vector3.up * bounceValue;
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = originalPosition;
    }
    
    /// <summary>
    /// Handle search input changes with debouncing.
    /// </summary>
    internal void OnSearchInputChanged(string searchTerm)
    {
        currentSearchTerm = searchTerm;
        
        if (searchDebounceCoroutine != null)
        {
            StopCoroutine(searchDebounceCoroutine);
        }
        
        searchDebounceCoroutine = StartCoroutine(SearchDebounceCoroutine());
    }
    
    /// <summary>
    /// Search debouncing coroutine.
    /// </summary>
    private IEnumerator SearchDebounceCoroutine()
    {
        yield return new WaitForSeconds(searchDebounceTime);
        
        currentQuery.searchTerm = currentSearchTerm;
        currentQuery.page = 1; // Reset to first page
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Handle filter changes.
    /// </summary>
    private void OnFilterChanged(int filterIndex)
    {
        currentFilter = (FilterCriteria)filterIndex;
        currentQuery.additionalFilters["filter"] = currentFilter;
        currentQuery.page = 1; // Reset to first page
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Handle time frame changes.
    /// </summary>
    private void OnTimeFrameChanged(int timeFrameIndex)
    {
        currentTimeFrame = (LeaderboardTimeFrame)timeFrameIndex;
        currentQuery.additionalFilters["timeFrame"] = currentTimeFrame;
        currentQuery.page = 1; // Reset to first page
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Handle leaderboard type changes.
    /// </summary>
    private void OnLeaderboardTypeChanged(int typeIndex)
    {
        currentType = (LeaderboardType)typeIndex;
        currentQuery.additionalFilters["type"] = currentType;
        currentQuery.page = 1; // Reset to first page
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Handle scroll changes for virtual scrolling.
    /// </summary>
    private void OnScrollChanged(Vector2 scrollPosition)
    {
        if (!enableVirtualScrolling) return;
        
        float currentScroll = scrollPosition.y;
        
        // Check if user has scrolled near the bottom
        if (currentScroll > scrollThreshold && currentScroll > lastScrollPosition)
        {
            if (!isLoading && currentPage < totalPages)
            {
                LoadMoreData();
            }
        }
        
        lastScrollPosition = currentScroll;
    }
    
    /// <summary>
    /// Load more data for virtual scrolling.
    /// </summary>
    private void LoadMoreData()
    {
        if (isLoading) return;
        
        currentPage++;
        currentQuery.page = currentPage;
        
        StartCoroutine(LoadMoreLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Load additional leaderboard data for pagination.
    /// </summary>
    private IEnumerator LoadMoreLeaderboardData(SearchQuery query)
    {
        isLoading = true;
        
        yield return StartCoroutine(leaderboardService.LoadLeaderboard(query, (response) =>
        {
            isLoading = false;
            
            if (response.success && response.data.entries.Count > 0)
            {
                // Append new entries to existing ones
                var existingEntries = currentLeaderboard.entries;
                existingEntries.AddRange(response.data.entries);
                currentLeaderboard.entries = existingEntries;
                
                // Render only the new entries
                int startIndex = existingEntries.Count - response.data.entries.Count;
                for (int i = 0; i < response.data.entries.Count; i++)
                {
                    var entry = response.data.entries[i];
                    var entryObject = GetOrCreateEntryObject(entry.user.userId);
                    
                    if (entryObject != null)
                    {
                        SetupEntryUI(entryObject, entry, startIndex + i);
                        AnimateEntry(entryObject, startIndex + i);
                        
                        activeEntryObjects.Add(entryObject);
                    }
                }
            }
        }));
    }
    
    /// <summary>
    /// Set sorting criteria and update UI.
    /// </summary>
    internal void SetSortCriteria(SortCriteria criteria)
    {
        currentSort = criteria;
        
        // Toggle sort direction if same criteria
        if (currentQuery.sortBy == criteria)
        {
            sortDescending = !sortDescending;
        }
        else
        {
            sortDescending = true; // Default to descending for new criteria
        }
        
        currentQuery.sortBy = criteria;
        currentQuery.sortDescending = sortDescending;
        currentQuery.page = 1; // Reset to first page
        
        UpdateSortButtonColors();
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Update sort button colors to indicate active sort.
    /// </summary>
    private void UpdateSortButtonColors()
    {
        var sortButtons = new[] { sortByScoreButton, sortByRankButton, sortByNameButton, sortByCountryButton, sortByRecentActivityButton };
        var sortCriteria = new[] { SortCriteria.Score, SortCriteria.Rank, SortCriteria.Name, SortCriteria.Country, SortCriteria.RecentActivity };
        
        for (int i = 0; i < sortButtons.Length; i++)
        {
            if (sortButtons[i] != null)
            {
                var colors = sortButtons[i].colors;
                colors.normalColor = (currentSort == sortCriteria[i]) ? activeSortColor : inactiveSortColor;
                sortButtons[i].colors = colors;
            }
        }
    }
    
    /// <summary>
    /// Show user profile modal.
    /// </summary>
    private void ShowUserProfile(LeaderboardUser user)
    {
        if (userProfileModal != null)
        {
            var profileController = userProfileModal.GetComponent<UserProfileModalController>();
            if (profileController != null)
            {
                profileController.ShowProfile(user);
                userProfileModal.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// Toggle search panel visibility.
    /// </summary>
    private void ToggleSearchPanel()
    {
        if (searchPanel != null)
        {
            searchPanel.SetActive(!searchPanel.activeSelf);
        }
    }
    
    /// <summary>
    /// Toggle filter panel visibility.
    /// </summary>
    private void ToggleFilterPanel()
    {
        if (filterPanel != null)
        {
            filterPanel.SetActive(!filterPanel.activeSelf);
        }
    }
    
    /// <summary>
    /// Handle leaderboard update from service.
    /// </summary>
    private void OnLeaderboardUpdateReceived(LeaderboardUpdate update)
    {
        // Find the updated entry and refresh it
        var existingEntry = currentLeaderboard.entries.Find(e => e.user.userId == update.userId);
        if (existingEntry != null)
        {
            existingEntry.score = update.newScore;
            existingEntry.lastUpdated = update.timestamp;
            
            // Re-render the updated entry
            var entryObject = activeEntryObjects.Find(obj => obj.GetComponent<LeaderboardEntryUI>()?.userId == update.userId);
            if (entryObject != null)
            {
                SetupEntryUI(entryObject, existingEntry, currentLeaderboard.entries.IndexOf(existingEntry));
                AnimateRankChange(entryObject, update.newRank - update.oldRank);
            }
        }
    }
    
    /// <summary>
    /// Handle connection status changes.
    /// </summary>
    private void OnConnectionStatusChanged(bool isOnline)
    {
        // Update UI to reflect connection status
        if (isOnline)
        {
            Log("Connection restored - syncing data");
            leaderboardService.ForceSync();
        }
        else
        {
            LogWarning("Connection lost - switching to offline mode");
        }
    }
    
    /// <summary>
    /// Handle refresh button click.
    /// </summary>
    private void OnRefreshClicked()
    {
        RefreshLeaderboard();
    }
    
    /// <summary>
    /// Handle close button click.
    /// </summary>
    private void OnCloseClicked()
    {
        Hide(); // Use UIPanel's Hide method
        
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }
    
    /// <summary>
    /// Refresh leaderboard data.
    /// </summary>
    public void RefreshLeaderboard()
    {
        currentQuery.page = 1;
        StartCoroutine(LoadLeaderboardData(currentQuery));
    }
    
    /// <summary>
    /// Change current page.
    /// </summary>
    private void ChangePage(int newPage)
    {
        if (newPage < 1 || newPage > totalPages) return;
        
        currentPage = newPage;
        currentQuery.page = currentPage;
        
        StartCoroutine(LoadLeaderboardData(currentQuery));
        
        // Scroll to top
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
    
    /// <summary>
    /// Setup pagination controls.
    /// </summary>
    private void SetupPagination()
    {
        if (pageButtons != null)
        {
            for (int i = 0; i < pageButtons.Length; i++)
            {
                int pageNum = i + 1;
                pageButtons[i].onClick.AddListener(() => ChangePage(pageNum));
            }
        }
    }
    
    /// <summary>
    /// Update pagination display.
    /// </summary>
    private void UpdatePagination()
    {
        if (pageInfoText != null)
        {
            pageInfoText.text = $"Page {currentPage} of {totalPages}";
        }
        
        if (previousPageButton != null)
        {
            previousPageButton.interactable = currentPage > 1;
        }
        
        if (nextPageButton != null)
        {
            nextPageButton.interactable = currentPage < totalPages;
        }
        
        // Update page buttons
        if (pageButtons != null)
        {
            for (int i = 0; i < pageButtons.Length; i++)
            {
                if (pageButtons[i] != null)
                {
                    pageButtons[i].interactable = (i + 1) != currentPage;
                }
            }
        }
    }
    
    /// <summary>
    /// Setup initial UI state.
    /// </summary>
    private void SetupInitialUIState()
    {
        // Hide panels initially
        if (searchPanel != null) searchPanel.SetActive(false);
        if (filterPanel != null) filterPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(false);
        if (userProfileModal != null) userProfileModal.SetActive(false);
        
        // Setup dropdowns
        SetupDropdowns();
        
        // Update sort button colors
        UpdateSortButtonColors();
    }
    
    /// <summary>
    /// Setup dropdown options.
    /// </summary>
    private void SetupDropdowns()
    {
        if (filterDropdown != null)
        {
            filterDropdown.ClearOptions();
            var filterOptions = Enum.GetNames(typeof(FilterCriteria)).ToList();
            filterDropdown.AddOptions(filterOptions);
        }
        
        if (timeFrameDropdown != null)
        {
            timeFrameDropdown.ClearOptions();
            var timeFrameOptions = Enum.GetNames(typeof(LeaderboardTimeFrame)).ToList();
            timeFrameDropdown.AddOptions(timeFrameOptions);
        }
        
        if (leaderboardTypeDropdown != null)
        {
            leaderboardTypeDropdown.ClearOptions();
            var typeOptions = Enum.GetNames(typeof(LeaderboardType)).ToList();
            leaderboardTypeDropdown.AddOptions(typeOptions);
        }
    }
    
    /// <summary>
    /// Setup search debouncing.
    /// </summary>
    private void SetupSearchDebounce()
    {
        // Search debouncing is handled in OnSearchInputChanged
    }
    
    /// <summary>
    /// Clear all leaderboard entries.
    /// </summary>
    private void ClearLeaderboardEntries()
    {
        foreach (var entryObject in activeEntryObjects)
        {
            if (entryObject != null)
            {
                entryObject.SetActive(false);
                ReturnEntryToPool(entryObject);
            }
        }
        
        activeEntryObjects.Clear();
    }
    
    /// <summary>
    /// Get or create entry object from pool.
    /// </summary>
    private GameObject GetOrCreateEntryObject(string userId)
    {
        GameObject entryObject = null;
        
        // Try to get from pool
        if (entryObjectPool.ContainsKey(userId))
        {
            entryObject = entryObjectPool[userId];
        }
        else if (leaderboardEntryPrefab != null)
        {
            entryObject = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entryObjectPool[userId] = entryObject;
        }
        
        if (entryObject != null)
        {
            entryObject.SetActive(true);
        }
        
        return entryObject;
    }
    
    /// <summary>
    /// Return entry object to pool.
    /// </summary>
    private void ReturnEntryToPool(GameObject entryObject)
    {
        // Keep object in pool but inactive
        entryObject.SetActive(false);
    }
    
    /// <summary>
    /// Show loading state.
    /// </summary>
    private void ShowLoadingState()
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(false);
        if (leaderboardContainer != null) leaderboardContainer.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Show content state.
    /// </summary>
    private void ShowContentState()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(false);
        if (leaderboardContainer != null) leaderboardContainer.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Show error state.
    /// </summary>
    private void ShowErrorState(string errorMessage)
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(true);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(false);
        if (leaderboardContainer != null) leaderboardContainer.gameObject.SetActive(false);
        
        // Update error message
        var errorText = errorPanel?.GetComponentInChildren<Text>();
        if (errorText != null)
        {
            errorText.text = $"Error: {errorMessage}";
        }
    }
    
    /// <summary>
    /// Show empty state.
    /// </summary>
    private void ShowEmptyState()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (emptyStatePanel != null) emptyStatePanel.SetActive(true);
        if (leaderboardContainer != null) leaderboardContainer.gameObject.SetActive(false);
    }
    
    // Utility methods
    
    private void Log(string message)
    {
        Debug.Log($"[LeaderboardUI] {message}");
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[LeaderboardUI] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[LeaderboardUI] {message}");
    }
    
    void OnDestroy()
    {
        // Clean up event listeners
        if (leaderboardService != null)
        {
            leaderboardService.OnLeaderboardUpdate -= OnLeaderboardUpdateReceived;
            leaderboardService.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        }
        
        // Stop all active coroutines
        foreach (var animation in activeAnimations.Values)
        {
            if (animation != null)
                StopCoroutine(animation);
        }
        
        if (searchDebounceCoroutine != null)
        {
            StopCoroutine(searchDebounceCoroutine);
        }
        
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
        }
    }
}