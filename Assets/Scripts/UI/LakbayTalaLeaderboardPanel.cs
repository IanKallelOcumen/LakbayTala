using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using LakbayTala.Leaderboard;

/// <summary>
/// Enhanced leaderboard panel with Filipino cultural elements and mythological creature rankings.
/// Integrates with LakbayTala's educational platform featuring Laguna mythology.
/// </summary>
public class LakbayTalaLeaderboardPanel : MonoBehaviour
{
    [Header("Cultural Ranking System")]
    [Tooltip("Filipino mythological creature ranks from highest to lowest")]
    public string[] rankTitles = { "Diwata", "Tikbalang", "Kapre", "Tiyanak", "Aswang", "Nuno", "Duwende", "Manananggal" };
    [Tooltip("Traditional Filipino colors for each rank")]
    public Color[] rankColors = new Color[8];
    [Tooltip("Baybayin script names for each rank")]
    public string[] rankBaybayinNames = { "ᜇᜒᜏᜆ", "ᜆᜒᜃ᜔ᜊᜎᜅ᜔", "ᜃᜉ᜔ᜇᜒ", "ᜆᜒᜌᜈᜃ᜔", "ᜀᜐᜏᜅ᜔", "ᜈᜓᜈᜓ", "ᜇᜓᜏᜒᜈ᜔ᜇᜒ", "ᜋᜈᜈᜅ᜔ᜄᜎ᜔" };
    [Tooltip("Cultural descriptions for each mythological creature")]
    public string[] rankDescriptions = {
        "Guardian spirits of nature and forests",
        "Mischievous horse-headed creatures of the mountains",
        "Gentle giants who protect sacred trees",
        "Mischievous child-like spirits",
        "Shape-shifting creatures of folklore",
        "Ancient dwarves who guard the land",
        "Playful little people of the household",
        "Flying creatures of the night"
    };
    
    [Header("Laguna-Specific Elements")]
    [Tooltip("Mount Makiling rankings with Diwata Maria Makiling")]
    public bool enableMountMakilingTheme = true;
    [Tooltip("Lake Mohikap rankings with water spirits")]
    public bool enableLakeMohikapTheme = true;
    [Tooltip("Sampaloc Lake rankings with crater lake mythology")]
    public bool enableSampalocLakeTheme = true;
    [Tooltip("Botocan Falls rankings with water nymph themes")]
    public bool enableBotocanFallsTheme = true;
    
    [Header("Educational Integration")]
    [Tooltip("Show cultural learning tips with rankings")]
    public bool enableCulturalLearning = true;
    [Tooltip("Display Filipino language alongside English")]
    public bool enableBilingualDisplay = true;
    [Tooltip("Show mythological creature information")]
    public bool enableCreatureInfo = true;
    [Tooltip("Track cultural knowledge progress")]
    public bool trackCulturalProgress = true;
    
    [Header("UI References")]
    public Transform leaderboardContainer;
    [Tooltip("Optional: sortable columns (score, time, level, etc.).")]
    public LeaderboardSortController sortController;
    public GameObject culturalEntryPrefab;
    public Text titleText;
    public Text subtitleText;
    public Text culturalTipText;
    public Button closeButton;
    public Button refreshButton;
    public Button culturalInfoButton;
    public Image bannerImage;
    
    [Header("Cultural UI Elements")]
    public Image backgroundPattern;
    public Image sideDecorations;
    public Text baybayinTitleText;
    public GameObject culturalAnimationContainer;
    
    public static LakbayTalaLeaderboardPanel Instance { get; private set; }
    
    private List<GameObject> entryObjects = new List<GameObject>();
    private LakbayTalaUITheme uiTheme;
    private string currentLocationTheme = "General";
    private Dictionary<string, int> culturalKnowledgeScores = new Dictionary<string, int>();
    private bool isLoading = false;
    
    // Filipino translations
    private Dictionary<string, string> filipinoTranslations = new Dictionary<string, string>
    {
        {"Leaderboard", "Talaan ng mga Pinakamahusay"},
        {"Rank", "Ranggo"},
        {"Player", "Manlalaro"},
        {"Score", "Iskor"},
        {"Time", "Oras"},
        {"Location", "Lokasyon"},
        {"Cultural Knowledge", "Kaalamang Kultural"},
        {"Tala Collected", "Tala na Nakalap"},
        {"Mythology Master", "Panday sa Mitolohiya"}
    };
    
    // Cultural learning database
    private Dictionary<string, string> culturalLearningTips = new Dictionary<string, string>
    {
        {"Diwata", "Ang mga Diwata ay mga espiritu ng kalikasan na nagbabantay sa mga kagubatan tulad ng Bundok Makiling."},
        {"Tikbalang", "Ang Tikbalang ay may ulo ng kabayo at katawan ng tao, naninirahan sa mga bundok at mahilig magpahamak."},
        {"Kapre", "Ang Kapre ay malaking nilalang na naninirahan sa mga puno, kadalasang mabait pero mahilig manuot ng tabako."},
        {"Tiyanak", "Ang Tiyanak ay espiritu ng batang namatay na hindi nabinyagan, lumalabas sa gabi."},
        {"Aswang", "Ang Aswang ay halimaw na nagkakatawang-tao, may kakayahang magpalit ng anyo."},
        {"Nuno", "Ang Nuno ay matandang duwende na nakatira sa mga puno o ilalim ng lupa, dapat pagpasensiyahan."},
        {"Duwende", "Ang Duwende ay maliliit na nilalang na maaaring maging mabait o masungit depende sa pakikitungo."},
        {"Manananggal", "Ang Manananggal ay halimaw na nagkakahiwalay ang katawan at lumilipad sa gabi."}
    };

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        InitializeCulturalLeaderboard();
        SetupUIElements();
        RefreshCulturalLeaderboard();
    }
    
    void OnEnable()
    {
        RefreshCulturalLeaderboard();
        UpdateCulturalTheme();
    }
    
    private void InitializeCulturalLeaderboard()
    {
        // Get UI theme reference
        uiTheme = LakbayTalaUITheme.Instance;
        
        // Setup default colors if not assigned
        if (rankColors == null || rankColors.Length == 0)
        {
            rankColors = new Color[]
            {
                new Color(1f, 0.84f, 0f),      // Gold for Diwata
                new Color(0.75f, 0.75f, 0.75f), // Silver for Tikbalang
                new Color(0.8f, 0.5f, 0.2f),   // Bronze for Kapre
                new Color(0.6f, 0.3f, 0.8f),   // Purple for Tiyanak
                new Color(0.8f, 0.2f, 0.2f),   // Red for Aswang
                new Color(0.4f, 0.6f, 0.4f),   // Green for Nuno
                new Color(0.5f, 0.3f, 0.2f),   // Brown for Duwende
                new Color(0.3f, 0.3f, 0.3f)    // Gray for Manananggal
            };
        }
        
        // Load cultural knowledge scores
        LoadCulturalKnowledge();
        
        // Setup UI theme
        ApplyCulturalTheme();
    }
    
    private void SetupUIElements()
    {
        // Setup title with cultural elements
        if (titleText != null)
        {
            titleText.text = enableBilingualDisplay ? 
                "Cultural Leaderboard / Talaan ng mga Pinakamahusay" : 
                "Cultural Leaderboard";
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = "Discover the mythological creatures of Laguna";
        }
        
        if (baybayinTitleText != null && enableBilingualDisplay)
        {
            baybayinTitleText.text = "ᜆᜎᜀᜈ᜔ ᜈᜅ᜔ ᜋᜅ᜔ ᜉᜒᜈᜃᜋᜑᜓᜐᜌ᜔";
            baybayinTitleText.gameObject.SetActive(true);
        }
        else if (baybayinTitleText != null)
        {
            baybayinTitleText.gameObject.SetActive(false);
        }
        
        // Setup cultural banner
        if (bannerImage != null)
        {
            SetupCulturalBanner();
        }
        
        // Setup button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseCulturalLeaderboard);
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshCulturalLeaderboard);
        if (culturalInfoButton != null)
            culturalInfoButton.onClick.AddListener(OnShowCulturalInfo);
    }
    
    private void SetupCulturalBanner()
    {
        if (bannerImage == null) return;
        
        // Create banner with traditional Filipino patterns
        bannerImage.color = uiTheme != null ? uiTheme.primaryColor : rankColors[0];
        
        // Add cultural decorations
        if (sideDecorations != null)
        {
            sideDecorations.color = uiTheme != null ? uiTheme.accentColor : rankColors[2];
        }
    }
    
    private void ApplyCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply theme to background
        if (backgroundPattern != null)
        {
            backgroundPattern.color = uiTheme.backgroundColor;
        }
        
        // Apply location-specific themes
        UpdateCulturalTheme();
    }
    
    private void UpdateCulturalTheme()
    {
        if (uiTheme == null) return;
        
        // Apply location-specific colors
        Color locationColor = GetCurrentLocationThemeColor();
        
        if (bannerImage != null)
        {
            bannerImage.color = locationColor;
        }
        
        if (culturalTipText != null)
        {
            culturalTipText.color = uiTheme.textColor;
        }
    }
    
    private Color GetCurrentLocationThemeColor()
    {
        if (uiTheme == null) return rankColors[0];
        
        switch (currentLocationTheme.ToLower())
        {
            case "mount makiling":
                return uiTheme.GetLocationThemeColor("Mount Makiling");
            case "lake mohikap":
                return uiTheme.GetLocationThemeColor("Lake Mohikap");
            case "sampaloc lake":
                return uiTheme.GetLocationThemeColor("Sampaloc Lake");
            case "botocan falls":
                return uiTheme.GetLocationThemeColor("Botocan Falls");
            default:
                return uiTheme.primaryColor;
        }
    }
    
    public void RefreshCulturalLeaderboard()
    {
        if (isLoading) return;
        
        StartCoroutine(RefreshCulturalLeaderboardCoroutine());
    }
    
    private System.Collections.IEnumerator RefreshCulturalLeaderboardCoroutine()
    {
        isLoading = true;
        
        ClearEntryObjects();
        
        List<LeaderboardEntry> entries = GetCulturalEntries();
        if (sortController != null)
            sortController.SortEntries(entries);

        DisplayCulturalEntries(entries);
        
        // Update cultural tip
        UpdateCulturalTip();
        
        isLoading = false;
        yield return null;
    }
    
    private List<LeaderboardEntry> GetCulturalEntries()
    {
        // Get entries from base leaderboard
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        
        // Add cultural scoring
        foreach (var entry in entries)
        {
            AddCulturalScore(entry);
        }
        
        // Sort by cultural knowledge score + regular score
        entries.Sort((a, b) => 
        {
            int scoreA = GetCulturalScore(a) + a.score;
            int scoreB = GetCulturalScore(b) + b.score;
            return scoreB.CompareTo(scoreA);
        });
        
        return entries;
    }
    
    public static string GetEntryPlayerName(LeaderboardEntry e) => e?.user?.displayName ?? e?.user?.username ?? "";
    public static float GetEntryCompletionTime(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("completionTime", out var t) ? System.Convert.ToSingle(t) : 0f;
    public static string GetEntryLevelName(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("levelName", out var l) ? l?.ToString() ?? "" : e?.user?.culturalLevel ?? "";

    private void AddCulturalScore(LeaderboardEntry entry)
    {
        string playerKey = GetEntryPlayerName(entry) + "_cultural";
        if (culturalKnowledgeScores.ContainsKey(playerKey))
        {
            entry.score += culturalKnowledgeScores[playerKey];
        }
    }
    
    private int GetCulturalScore(LeaderboardEntry entry)
    {
        string playerKey = GetEntryPlayerName(entry) + "_cultural";
        return culturalKnowledgeScores.ContainsKey(playerKey) ? culturalKnowledgeScores[playerKey] : 0;
    }
    
    private void DisplayCulturalEntries(List<LeaderboardEntry> entries)
    {
        int rank = 1;
        foreach (var entry in entries.Take(8)) // Show top 8 (mythological creatures)
        {
            GameObject entryObj = CreateCulturalEntry(entry, rank);
            if (entryObj != null)
            {
                entryObjects.Add(entryObj);
                
                // Animate entry
                CulturalEntryUI entryUI = entryObj.GetComponent<CulturalEntryUI>();
                if (entryUI != null)
                {
                    entryUI.AnimateEntry(rank * 0.1f);
                }
            }
            rank++;
        }
    }
    
    private GameObject CreateCulturalEntry(LeaderboardEntry entry, int rank)
    {
        if (culturalEntryPrefab == null || leaderboardContainer == null) return null;
        
        GameObject entryObj = Instantiate(culturalEntryPrefab, leaderboardContainer);
        
        CulturalEntryUI entryUI = entryObj.GetComponent<CulturalEntryUI>();
        if (entryUI == null)
        {
            entryUI = entryObj.AddComponent<CulturalEntryUI>();
        }
        
        // Setup cultural entry
        string creatureName = rankTitles[Mathf.Clamp(rank - 1, 0, rankTitles.Length - 1)];
        Color creatureColor = rankColors[Mathf.Clamp(rank - 1, 0, rankColors.Length - 1)];
        string creatureDescription = rankDescriptions[Mathf.Clamp(rank - 1, 0, rankDescriptions.Length - 1)];
        string baybayinName = rankBaybayinNames[Mathf.Clamp(rank - 1, 0, rankBaybayinNames.Length - 1)];
        
        entryUI.SetupCulturalEntry(entry, rank, creatureName, creatureColor, creatureDescription, baybayinName);
        
        return entryObj;
    }
    
    private void UpdateCulturalTip()
    {
        if (culturalTipText == null) return;
        
        if (enableCulturalLearning && culturalLearningTips.Count > 0)
        {
            string randomCreature = rankTitles[Random.Range(0, rankTitles.Length)];
            if (culturalLearningTips.ContainsKey(randomCreature))
            {
                culturalTipText.text = "💡 " + culturalLearningTips[randomCreature];
            }
        }
    }
    
    private void LoadCulturalKnowledge()
    {
        // Load cultural knowledge scores from PlayerPrefs
        culturalKnowledgeScores.Clear();
        
        int count = PlayerPrefs.GetInt("CulturalKnowledgeCount", 0);
        for (int i = 0; i < count; i++)
        {
            string player = PlayerPrefs.GetString($"CulturalPlayer_{i}", "");
            int score = PlayerPrefs.GetInt($"CulturalScore_{i}", 0);
            if (!string.IsNullOrEmpty(player))
            {
                culturalKnowledgeScores[player] = score;
            }
        }
    }
    
    private void SaveCulturalKnowledge()
    {
        PlayerPrefs.SetInt("CulturalKnowledgeCount", culturalKnowledgeScores.Count);
        
        int i = 0;
        foreach (var kvp in culturalKnowledgeScores)
        {
            PlayerPrefs.SetString($"CulturalPlayer_{i}", kvp.Key);
            PlayerPrefs.SetInt($"CulturalScore_{i}", kvp.Value);
            i++;
        }
        
        PlayerPrefs.Save();
    }
    
    public void AddCulturalKnowledgeScore(string playerName, int score)
    {
        string playerKey = playerName + "_cultural";
        if (culturalKnowledgeScores.ContainsKey(playerKey))
        {
            culturalKnowledgeScores[playerKey] += score;
        }
        else
        {
            culturalKnowledgeScores[playerKey] = score;
        }
        
        SaveCulturalKnowledge();
        RefreshCulturalLeaderboard();
    }
    
    public void SetCurrentLocation(string location)
    {
        currentLocationTheme = location;
        UpdateCulturalTheme();
    }
    
    // Event handlers
    private void OnCloseCulturalLeaderboard()
    {
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }
    
    private void OnRefreshCulturalLeaderboard()
    {
        RefreshCulturalLeaderboard();
    }
    
    private void OnShowCulturalInfo()
    {
        // Show cultural information panel
        ShowCulturalInformation();
    }
    
    private void ShowCulturalInformation()
    {
        // Implementation for showing detailed cultural information
        Debug.Log("Showing cultural information about Filipino mythological creatures");
    }
    
    private void ClearEntryObjects()
    {
        foreach (GameObject obj in entryObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        entryObjects.Clear();
    }
    
    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SaveCulturalKnowledge();
    }
}

/// <summary>
/// Cultural leaderboard entry UI component with mythological creature theming.
/// </summary>
public class CulturalEntryUI : MonoBehaviour
{
    static string EntryPlayerName(LakbayTala.Leaderboard.LeaderboardEntry e) => e?.user?.displayName ?? e?.user?.username ?? "";
    static float EntryCompletionTime(LakbayTala.Leaderboard.LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("completionTime", out var t) ? System.Convert.ToSingle(t) : 0f;
    static string EntryLevelName(LakbayTala.Leaderboard.LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("levelName", out var l) ? l?.ToString() ?? "" : e?.user?.culturalLevel ?? "";

    [Header("Cultural UI References")]
    public Text rankText;
    public Text creatureNameText;
    public Text baybayinNameText;
    public Text playerNameText;
    public Text scoreText;
    public Text timeText;
    public Text locationText;
    public Text creatureDescriptionText;
    public Image creatureIcon;
    public Image backgroundImage;
    public Image creatureBadgeImage;
    public GameObject culturalKnowledgeIndicator;
    public Text culturalKnowledgeText;
    
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private bool isAnimating = false;
    
    public void SetupCulturalEntry(LeaderboardEntry entry, int rank, string creatureName, Color creatureColor, string creatureDescription, string baybayinName)
    {
        // Setup rank display
        if (rankText != null)
        {
            rankText.text = $"#{rank}";
            rankText.color = creatureColor;
        }
        
        // Setup creature information
        if (creatureNameText != null)
        {
            creatureNameText.text = creatureName;
            creatureNameText.color = creatureColor;
        }
        
        if (baybayinNameText != null)
        {
            baybayinNameText.text = baybayinName;
            baybayinNameText.color = creatureColor;
        }
        
        if (creatureDescriptionText != null)
        {
            creatureDescriptionText.text = creatureDescription;
        }
        
        // Setup player information
        if (playerNameText != null)
        {
            playerNameText.text = EntryPlayerName(entry);
        }
        
        if (scoreText != null)
        {
            scoreText.text = entry.score.ToString("N0");
        }
        
        if (timeText != null)
        {
            timeText.text = FormatTime(EntryCompletionTime(entry));
        }
        
        if (locationText != null)
        {
            locationText.text = EntryLevelName(entry);
        }
        
        // Setup visual elements
        if (backgroundImage != null)
        {
            Color bgColor = creatureColor;
            bgColor.a = 0.3f; // Semi-transparent
            backgroundImage.color = bgColor;
        }
        
        if (creatureBadgeImage != null)
        {
            creatureBadgeImage.color = creatureColor;
        }
        
        // Setup cultural knowledge indicator
        if (culturalKnowledgeIndicator != null && culturalKnowledgeText != null)
        {
            SetupCulturalKnowledge(entry);
        }
        
        // Apply theme
        ApplyCulturalTheme(creatureColor);
    }
    
    private void SetupCulturalKnowledge(LeaderboardEntry entry)
    {
        // Calculate cultural knowledge score
        int culturalScore = 0; // This would come from the entry data
        
        if (culturalScore > 0)
        {
            culturalKnowledgeIndicator.SetActive(true);
            culturalKnowledgeText.text = $"+{culturalScore} Cultural Knowledge";
            culturalKnowledgeText.color = Color.yellow;
        }
        else
        {
            culturalKnowledgeIndicator.SetActive(false);
        }
    }
    
    private void ApplyCulturalTheme(Color creatureColor)
    {
        // Apply creature-specific theming
        if (creatureIcon != null)
        {
            creatureIcon.color = creatureColor;
        }
        
        // Add creature-specific decorations
        // This could include particle effects, animations, etc.
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:D2}:{secs:D2}";
    }
    
    public void AnimateEntry(float delay = 0f)
    {
        if (isAnimating) return;
        StartCoroutine(EntryAnimationCoroutine(delay));
    }
    
    private System.Collections.IEnumerator EntryAnimationCoroutine(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        isAnimating = true;
        
        // Get starting values
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        Vector3 originalScale = transform.localScale;
        Vector3 startScale = Vector3.zero;
        
        float elapsed = 0f;
        
        // Setup for animation
        canvasGroup.alpha = 0f;
        transform.localScale = startScale;
        
        // Animate entry
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            float curveValue = animationCurve.Evaluate(t);
            
            canvasGroup.alpha = curveValue;
            transform.localScale = Vector3.Lerp(startScale, originalScale, curveValue);
            
            yield return null;
        }
        
        // Final values
        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;
        
        isAnimating = false;
    }
}