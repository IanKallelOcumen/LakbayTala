using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Comprehensive integration script that ties together all the menu system components,
/// cloud background system, and checkpoint functionality.
/// This script should be attached to a GameManager object in the scene.
/// </summary>
public class LakbayTalaIntegration : MonoBehaviour
{
    [Header("Core Systems")]
    public MasterGameManager masterGameManager;
    public BackgroundCloudController cloudController;
    
    [Header("UI References")]
    public CanvasGroup lobbyScreen;
    public CanvasGroup mapScreen;
    public CanvasGroup bestiaryScreen;
    public CanvasGroup infoScreen;
    public CanvasGroup leaderboardScreen;
    public CanvasGroup settingsScreen;
    public CanvasGroup achievementsScreen;
    
    [Header("Cloud Assets")]
    public GameObject[] cloudPrefabs;
    
    [Header("Menu Buttons")]
    public Button playButton;
    public Button mapButton;
    public Button bestiaryButton;
    public Button leaderboardButton;
    public Button settingsButton;
    public Button achievementsButton;
    public Button quitButton;
    
    [Header("Settings Panel")]
    public SettingsPanelController settingsController;
    
    [Header("Leaderboard Panel")]
    public LeaderboardPanelController leaderboardController;
    
    [Header("Achievements Panel")]
    public AchievementsPanelController achievementsController;
    
    [Header("Integration Settings")]
    public bool autoInitialize = true;
    public bool enableClouds = true;
    public bool enableMenuSystem = true;
    
    private bool isInitialized = false;

    void Start()
    {
        if (autoInitialize)
        {
            InitializeSystem();
        }
    }

    /// <summary>
    /// Initialize the complete Lakbay Tala system integration.
    /// </summary>
    public void InitializeSystem()
    {
        if (isInitialized) return;
        
        Debug.Log("Initializing Lakbay Tala Integration...");
        
        SetupMasterGameManager();
        SetupCloudBackground();
        SetupMenuSystem();
        SetupPanelControllers();
        SetupButtonListeners();
        
        isInitialized = true;
        Debug.Log("Lakbay Tala Integration initialized successfully!");
    }

    private void SetupMasterGameManager()
    {
        if (masterGameManager == null)
        {
            masterGameManager = Object.FindFirstObjectByType<MasterGameManager>();
            if (masterGameManager == null)
            {
                Debug.LogWarning("MasterGameManager not found. Creating temporary instance.");
                GameObject managerObj = new GameObject("MasterGameManager");
                masterGameManager = managerObj.AddComponent<MasterGameManager>();
            }
        }
        
        // Assign UI panels to MasterGameManager
        if (masterGameManager != null)
        {
            masterGameManager.lobbyScreen = lobbyScreen;
            masterGameManager.mapScreen = mapScreen;
            masterGameManager.bestiaryScreen = bestiaryScreen;
            masterGameManager.infoScreen = infoScreen;
            masterGameManager.leaderboardScreen = leaderboardScreen;
            masterGameManager.settingsScreen = settingsScreen;
            masterGameManager.achievementsScreen = achievementsScreen;
        }
    }

    private void SetupCloudBackground()
    {
        if (!enableClouds) return;
        
        if (cloudController == null)
        {
            cloudController = Object.FindFirstObjectByType<BackgroundCloudController>();
            if (cloudController == null)
            {
                GameObject cloudObj = new GameObject("BackgroundCloudController");
                cloudController = cloudObj.AddComponent<BackgroundCloudController>();
            }
        }
        
        // Assign cloud prefabs
        if (cloudController != null && cloudPrefabs != null && cloudPrefabs.Length > 0)
        {
            cloudController.cloudPrefabs = cloudPrefabs;
        }
    }

    private void SetupMenuSystem()
    {
        if (!enableMenuSystem) return;
        
        // Ensure all panels start hidden except lobby
        HideAllPanels();
        if (lobbyScreen != null)
        {
            lobbyScreen.alpha = 1f;
            lobbyScreen.interactable = true;
            lobbyScreen.blocksRaycasts = true;
        }
    }

    private void SetupPanelControllers()
    {
        // Setup Settings Panel
        if (settingsController != null && settingsScreen != null)
        {
            settingsController.gameObject.SetActive(true);
        }
        
        // Setup Leaderboard Panel
        if (leaderboardController != null && leaderboardScreen != null)
        {
            leaderboardController.gameObject.SetActive(true);
        }
        
        // Setup Achievements Panel
        if (achievementsController != null && achievementsScreen != null)
        {
            achievementsController.gameObject.SetActive(true);
        }
    }

    private void SetupButtonListeners()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (mapButton != null)
            mapButton.onClick.AddListener(OnMapClicked);
        if (bestiaryButton != null)
            bestiaryButton.onClick.AddListener(OnBestiaryClicked);
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (achievementsButton != null)
            achievementsButton.onClick.AddListener(OnAchievementsClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void HideAllPanels()
    {
        CanvasGroup[] allPanels = { lobbyScreen, mapScreen, bestiaryScreen, infoScreen, leaderboardScreen, settingsScreen, achievementsScreen };
        
        foreach (CanvasGroup panel in allPanels)
        {
            if (panel != null)
            {
                panel.alpha = 0f;
                panel.interactable = false;
                panel.blocksRaycasts = false;
            }
        }
    }

    // Button click handlers
    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked - loading game scene...");
        if (masterGameManager != null)
        {
            masterGameManager.LoadLevel("GameScene"); // Replace with your game scene name
        }
    }

    private void OnMapClicked()
    {
        Debug.Log("Map button clicked");
        if (masterGameManager != null)
        {
            masterGameManager.OnMap();
        }
    }

    private void OnBestiaryClicked()
    {
        Debug.Log("Bestiary button clicked");
        if (masterGameManager != null)
        {
            masterGameManager.OnBestiary();
        }
    }

    private void OnLeaderboardClicked()
    {
        Debug.Log("Leaderboard button clicked");
        if (masterGameManager != null)
        {
            masterGameManager.OnLeaderboard();
        }
        
        // Refresh leaderboard data
        if (leaderboardController != null)
        {
            leaderboardController.RefreshLeaderboard();
        }
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings button clicked");
        if (masterGameManager != null)
        {
            masterGameManager.OnSettings();
        }
        
        // Refresh settings UI
        if (settingsController != null)
        {
            settingsController.RefreshUI();
        }
    }

    private void OnAchievementsClicked()
    {
        Debug.Log("Achievements button clicked");
        if (masterGameManager != null)
        {
            masterGameManager.OnAchievements();
        }
        
        // Refresh achievements data
        if (achievementsController != null)
        {
            achievementsController.RefreshAchievements();
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");
        
        // Save any unsaved data before quitting
        SaveAllData();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Save all game data including settings, progress, and achievements.
    /// </summary>
    public void SaveAllData()
    {
        Debug.Log("Saving all game data...");
        
        // Save settings
        if (settingsController != null)
        {
            settingsController.SaveSettings();
        }
        
        // Save leaderboard
        if (leaderboardController != null)
        {
            // Leaderboard data is saved automatically when entries are added
        }
        
        // Save achievements
        if (achievementsController != null)
        {
            achievementsController.SaveAchievementData();
        }
        
        // Force PlayerPrefs save
        PlayerPrefs.Save();
        
        Debug.Log("All game data saved successfully!");
    }

    /// <summary>
    /// Load all game data including settings, progress, and achievements.
    /// </summary>
    public void LoadAllData()
    {
        Debug.Log("Loading all game data...");
        
        // Load settings
        if (settingsController != null)
        {
            settingsController.LoadSettings();
        }
        
        // Load leaderboard
        if (leaderboardController != null)
        {
            leaderboardController.LoadLocalLeaderboard();
        }
        
        // Load achievements
        if (achievementsController != null)
        {
            achievementsController.LoadAchievementData();
            achievementsController.LoadPlayerProgress();
        }
        
        Debug.Log("All game data loaded successfully!");
    }

    /// <summary>
    /// Reset all game data to default values.
    /// </summary>
    public void ResetAllData()
    {
        Debug.LogWarning("Resetting all game data...");
        
        // Reset settings
        if (settingsController != null)
        {
            settingsController.ResetToDefaults();
        }
        
        // Reset leaderboard
        if (leaderboardController != null)
        {
            leaderboardController.leaderboardEntries.Clear();
            leaderboardController.SaveLocalLeaderboard();
        }
        
        // Reset achievements
        if (achievementsController != null)
        {
            // Reset all achievement progress
            foreach (var achievement in achievementsController.allAchievements.Values)
            {
                achievement.isUnlocked = false;
                achievement.unlockDate = System.DateTime.MinValue;
            }
            achievementsController.SaveAchievementData();
        }
        
        // Clear PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("All game data reset successfully!");
    }

    /// <summary>
    /// Test the cloud system (disabled — no test-only flows; use production systems only).
    /// </summary>
    public void TestCloudSystem()
    {
        Debug.LogWarning("TestCloudSystem is disabled. Use production systems only; Firebase will be used for leaderboard when implemented.");
    }

    /// <summary>
    /// Clear all clouds from the scene.
    /// </summary>
    public void ClearClouds()
    {
        if (cloudController != null)
        {
            cloudController.ClearAllClouds();
            Debug.Log("All clouds cleared");
        }
    }

    /// <summary>
    /// Add a test entry to the leaderboard (disabled — leaderboard is Firebase-only; implement Firebase later).
    /// </summary>
    public void AddTestLeaderboardEntry()
    {
        Debug.LogWarning("AddTestLeaderboardEntry is disabled. Leaderboard uses Firebase only; implement Firebase SDK to add entries.");
    }

    /// <summary>
    /// Unlock a test achievement (disabled — no test-only flows; use production systems only).
    /// </summary>
    public void UnlockTestAchievement()
    {
        Debug.LogWarning("UnlockTestAchievement is disabled. Use production systems only; Firebase will be used for leaderboard when implemented.");
    }

    void OnDestroy()
    {
        // Clean up when the integration object is destroyed
        if (isInitialized)
        {
            SaveAllData();
        }
    }
}