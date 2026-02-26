using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Leaderboard panel controller managing player rankings, scores, and leaderboard data.
/// Handles both local and online leaderboard functionality with sorting and filtering.
/// </summary>
public class LeaderboardPanelController : MonoBehaviour
{
    [Header("Leaderboard Data")]
    public List<LocalLeaderboardEntry> leaderboardEntries = new List<LocalLeaderboardEntry>();
    public int maxEntries = 100;
    
    [Header("UI References")]
    public Transform leaderboardContainer;
    public GameObject leaderboardEntryPrefab;
    public Text titleText;
    public Text statusText;
    public Button refreshButton;
    public Button closeButton;
    public Button clearButton;
    
    [Header("Filter Controls")]
    public Dropdown filterDropdown;
    public InputField searchInput;
    public Toggle showLocalToggle;
    public Toggle showGlobalToggle;
    
    [Header("Sorting Options")]
    public Button sortByScoreButton;
    public Button sortByTimeButton;
    public Button sortByNameButton;
    public Button sortByDateButton;
    
    [Header("Visual Settings")]
    public Color goldColor = Color.yellow;
    public Color silverColor = Color.gray;
    public Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);
    public Color defaultColor = Color.white;
    
    private List<GameObject> entryObjects = new List<GameObject>();
    private bool isLoading = false;
    private string currentFilter = "All";
    private string currentSort = "Score";
    private bool showLocal = true;

    void Start()
    {
        SetupUIListeners();
        LoadLeaderboardData();
        RefreshLeaderboard();
    }

    void OnEnable()
    {
        RefreshLeaderboard();
    }

    private void SetupUIListeners()
    {
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshLeaderboard);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseLeaderboard);
        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearLeaderboard);
            
        if (filterDropdown != null)
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        if (showLocalToggle != null)
            showLocalToggle.onValueChanged.AddListener(OnShowLocalChanged);
        if (showGlobalToggle != null)
            showGlobalToggle.onValueChanged.AddListener(OnShowGlobalChanged);
            
        if (sortByScoreButton != null)
            sortByScoreButton.onClick.AddListener(() => SortLeaderboard("Score"));
        if (sortByTimeButton != null)
            sortByTimeButton.onClick.AddListener(() => SortLeaderboard("Time"));
        if (sortByNameButton != null)
            sortByNameButton.onClick.AddListener(() => SortLeaderboard("Name"));
        if (sortByDateButton != null)
            sortByDateButton.onClick.AddListener(() => SortLeaderboard("Date"));
    }

    private void LoadLeaderboardData()
    {
        // Load local leaderboard data from PlayerPrefs
        LoadLocalLeaderboard();
        
        // Generate some sample data if no data exists
        if (leaderboardEntries.Count == 0)
        {
            GenerateSampleData();
        }
    }

    public void LoadLocalLeaderboard()
    {
        leaderboardEntries.Clear();
        
        int entryCount = PlayerPrefs.GetInt("LeaderboardEntryCount", 0);
        for (int i = 0; i < entryCount; i++)
        {
            LocalLeaderboardEntry entry = new LocalLeaderboardEntry
            {
                playerName = PlayerPrefs.GetString($"LeaderboardName_{i}", "Unknown"),
                score = PlayerPrefs.GetInt($"LeaderboardScore_{i}", 0),
                completionTime = PlayerPrefs.GetFloat($"LeaderboardTime_{i}", 0f),
                levelName = PlayerPrefs.GetString($"LeaderboardLevel_{i}", "Unknown"),
                date = System.DateTime.Parse(PlayerPrefs.GetString($"LeaderboardDate_{i}", System.DateTime.Now.ToString())),
                isLocal = true
            };
            leaderboardEntries.Add(entry);
        }
    }

    public void SaveLocalLeaderboard()
    {
        PlayerPrefs.SetInt("LeaderboardEntryCount", leaderboardEntries.Count);
        
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            LocalLeaderboardEntry entry = leaderboardEntries[i];
            PlayerPrefs.SetString($"LeaderboardName_{i}", entry.playerName);
            PlayerPrefs.SetInt($"LeaderboardScore_{i}", entry.score);
            PlayerPrefs.SetFloat($"LeaderboardTime_{i}", entry.completionTime);
            PlayerPrefs.SetString($"LeaderboardLevel_{i}", entry.levelName);
            PlayerPrefs.SetString($"LeaderboardDate_{i}", entry.date.ToString());
        }
        
        PlayerPrefs.Save();
    }

    private void GenerateSampleData()
    {
        string[] sampleNames = { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7", "Player8", "Player9", "Player10" };
        
        for (int i = 0; i < 10; i++)
        {
            LocalLeaderboardEntry entry = new LocalLeaderboardEntry
            {
                playerName = sampleNames[i],
                score = Random.Range(1000, 10000),
                completionTime = Random.Range(60f, 300f),
                levelName = "Level " + Random.Range(1, 5),
                date = System.DateTime.Now.AddDays(-Random.Range(0, 30)),
                isLocal = true
            };
            leaderboardEntries.Add(entry);
        }
        
        SaveLocalLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        ClearEntryObjects();
        
        List<LocalLeaderboardEntry> filteredEntries = FilterEntries();
        SortLeaderboardEntries(filteredEntries);
        
        DisplayEntries(filteredEntries);
        UpdateStatusText($"Showing {filteredEntries.Count} entries");
    }

    private List<LocalLeaderboardEntry> FilterEntries()
    {
        List<LocalLeaderboardEntry> filtered = new List<LocalLeaderboardEntry>();
        
        // Filter by local/global
        if (showLocal)
        {
            filtered = leaderboardEntries.Where(e => e.isLocal).ToList();
        }
        else
        {
            // For now, show all entries. In a real implementation, this would filter online entries
            filtered = new List<LocalLeaderboardEntry>(leaderboardEntries);
        }
        
        // Filter by search term
        if (!string.IsNullOrEmpty(searchInput.text))
        {
            string searchTerm = searchInput.text.ToLower();
            filtered = filtered.Where(e => 
                e.playerName.ToLower().Contains(searchTerm) || 
                e.levelName.ToLower().Contains(searchTerm)).ToList();
        }
        
        // Filter by category
        if (currentFilter != "All")
        {
            filtered = filtered.Where(e => e.levelName.Contains(currentFilter)).ToList();
        }
        
        return filtered;
    }

    private void SortLeaderboardEntries(List<LocalLeaderboardEntry> entries)
    {
        switch (currentSort)
        {
            case "Score":
                entries.Sort((a, b) => b.score.CompareTo(a.score));
                break;
            case "Time":
                entries.Sort((a, b) => a.completionTime.CompareTo(b.completionTime));
                break;
            case "Name":
                entries.Sort((a, b) => string.Compare(a.playerName, b.playerName));
                break;
            case "Date":
                entries.Sort((a, b) => b.date.CompareTo(a.date));
                break;
        }
    }

    public void SortLeaderboard(string sortType)
    {
        currentSort = sortType;
        RefreshLeaderboard();
    }

    private void DisplayEntries(List<LocalLeaderboardEntry> entries)
    {
        int rank = 1;
        foreach (LocalLeaderboardEntry entry in entries.Take(maxEntries))
        {
            GameObject entryObj = CreateEntryObject(entry, rank);
            if (entryObj != null)
            {
                entryObjects.Add(entryObj);
            }
            rank++;
        }
    }

    private GameObject CreateEntryObject(LocalLeaderboardEntry entry, int rank)
    {
        if (leaderboardEntryPrefab == null || leaderboardContainer == null)
            return null;
            
        GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
        
        // Set up entry display
        LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
        if (entryUI == null)
        {
            // If no dedicated UI component, try to find basic components
            Text[] texts = entryObj.GetComponentsInChildren<Text>();
            Image[] images = entryObj.GetComponentsInChildren<Image>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = $"#{rank}";
                texts[1].text = entry.playerName;
                texts[2].text = entry.score.ToString();
                
                // Set rank color
                if (texts.Length > 0)
                {
                    texts[0].color = GetRankColor(rank);
                }
            }
        }
        else
        {
            entryUI.SetupEntry(entry, rank, GetRankColor(rank));
        }
        
        return entryObj;
    }

    private Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1: return goldColor;
            case 2: return silverColor;
            case 3: return bronzeColor;
            default: return defaultColor;
        }
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

    // Event handlers
    private void OnRefreshLeaderboard()
    {
        if (isLoading) return;
        
        StartCoroutine(RefreshLeaderboardCoroutine());
    }

    private System.Collections.IEnumerator RefreshLeaderboardCoroutine()
    {
        isLoading = true;
        UpdateStatusText("Refreshing leaderboard...");
        
        // Local panel: loads from PlayerPrefs. Online leaderboard will use Firebase when implemented.
        LoadLocalLeaderboard();
        RefreshLeaderboard();
        
        isLoading = false;
        UpdateStatusText("Leaderboard refreshed!");
        yield return null;
    }

    private void OnCloseLeaderboard()
    {
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }

    private void OnClearLeaderboard()
    {
        leaderboardEntries.Clear();
        PlayerPrefs.DeleteKey("LeaderboardEntryCount");
        
        // Clear all leaderboard keys
        for (int i = 0; i < maxEntries; i++)
        {
            PlayerPrefs.DeleteKey($"LeaderboardName_{i}");
            PlayerPrefs.DeleteKey($"LeaderboardScore_{i}");
            PlayerPrefs.DeleteKey($"LeaderboardTime_{i}");
            PlayerPrefs.DeleteKey($"LeaderboardLevel_{i}");
            PlayerPrefs.DeleteKey($"LeaderboardDate_{i}");
        }
        
        PlayerPrefs.Save();
        RefreshLeaderboard();
        UpdateStatusText("Leaderboard cleared!");
    }

    private void OnFilterChanged(int value)
    {
        currentFilter = filterDropdown.options[value].text;
        RefreshLeaderboard();
    }

    private void OnSearchChanged(string searchTerm)
    {
        RefreshLeaderboard();
    }

    private void OnShowLocalChanged(bool value)
    {
        showLocal = value;
        if (value && showGlobalToggle != null)
            showGlobalToggle.isOn = false;
        RefreshLeaderboard();
    }

    private void OnShowGlobalChanged(bool value)
    {
        showLocal = !value;
        if (value && showLocalToggle != null)
            showLocalToggle.isOn = false;
        RefreshLeaderboard();
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // Public methods for adding entries
    public void AddLocalLeaderboardEntry(string playerName, int score, float completionTime, string levelName)
    {
        LocalLeaderboardEntry newEntry = new LocalLeaderboardEntry
        {
            playerName = playerName,
            score = score,
            completionTime = completionTime,
            levelName = levelName,
            date = System.DateTime.Now,
            isLocal = true
        };
        
        leaderboardEntries.Add(newEntry);
        
        // Keep only top entries
        if (leaderboardEntries.Count > maxEntries)
        {
            leaderboardEntries.Sort((a, b) => b.score.CompareTo(a.score));
            leaderboardEntries.RemoveAt(leaderboardEntries.Count - 1);
        }
        
        SaveLocalLeaderboard();
        RefreshLeaderboard();
    }
}

[System.Serializable]
/// <summary>Simple local leaderboard entry (avoids conflict with LakbayTala.Leaderboard.LeaderboardEntry).</summary>
public class LocalLeaderboardEntry
{
    public string playerName;
    public int score;
    public float completionTime;
    public string levelName;
    public System.DateTime date;
    public bool isLocal;
}