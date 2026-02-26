using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Achievements panel controller managing achievement tracking, unlocking, and display.
/// Handles achievement categories, progress tracking, and reward systems.
/// </summary>
public class AchievementsPanelController : MonoBehaviour
{
    [Header("Achievement Categories")]
    public AchievementCategory[] categories;
    public string defaultCategory = "General";
    
    [Header("UI References")]
    public Transform achievementsContainer;
    public GameObject achievementEntryPrefab;
    public Text categoryTitleText;
    public Text progressText;
    public Slider overallProgressSlider;
    public Button closeButton;
    public Button refreshButton;
    public Text statusText;
    
    [Header("Category Navigation")]
    public Transform categoryButtonsContainer;
    public GameObject categoryButtonPrefab;
    public Color selectedCategoryColor = Color.white;
    public Color unselectedCategoryColor = Color.gray;
    
    [Header("Achievement Display")]
    public Sprite lockedAchievementIcon;
    public Sprite unlockedAchievementIcon;
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    
    [Header("Reward Settings")]
    public bool enableRewards = true;
    public string rewardCurrency = "Tala";
    
    private List<GameObject> achievementObjects = new List<GameObject>();
    private List<Button> categoryButtons = new List<Button>();
    private string currentCategory;
    private bool isRefreshing = false;
    
    // Achievement data
    public Dictionary<string, UIAchievementData> allAchievements = new Dictionary<string, UIAchievementData>();
    private Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
    private Dictionary<string, int> achievementProgress = new Dictionary<string, int>();

    void Start()
    {
        InitializeAchievements();
        SetupCategoryButtons();
        SetupUIListeners();
        RefreshAchievements();
    }

    void OnEnable()
    {
        RefreshAchievements();
    }

    private void InitializeAchievements()
    {
        LoadAchievementData();
        LoadPlayerProgress();
        
        // If no achievements exist, create default ones
        if (allAchievements.Count == 0)
        {
            CreateDefaultAchievements();
        }
    }

    public void LoadAchievementData()
    {
        // Load achievement definitions from PlayerPrefs or JSON
        int achievementCount = PlayerPrefs.GetInt("AchievementCount", 0);
        
        for (int i = 0; i < achievementCount; i++)
        {
            string id = PlayerPrefs.GetString($"AchievementID_{i}", "");
            if (!string.IsNullOrEmpty(id))
            {
                UIAchievementData achievement = new UIAchievementData
                {
                    id = id,
                    title = PlayerPrefs.GetString($"AchievementTitle_{i}", "Unknown Achievement"),
                    description = PlayerPrefs.GetString($"AchievementDesc_{i}", ""),
                    category = PlayerPrefs.GetString($"AchievementCategory_{i}", "General"),
                    targetProgress = PlayerPrefs.GetInt($"AchievementTarget_{i}", 1),
                    rewardAmount = PlayerPrefs.GetInt($"AchievementReward_{i}", 0),
                    isUnlocked = PlayerPrefs.GetInt($"AchievementUnlocked_{i}", 0) == 1,
                    unlockDate = System.DateTime.Parse(PlayerPrefs.GetString($"AchievementUnlockDate_{i}", System.DateTime.MinValue.ToString()))
                };
                
                allAchievements[id] = achievement;
            }
        }
    }

    public void LoadPlayerProgress()
    {
        // Load player progress for each achievement
        foreach (var achievement in allAchievements.Values)
        {
            achievementProgress[achievement.id] = PlayerPrefs.GetInt($"AchievementProgress_{achievement.id}", 0);
            unlockedAchievements[achievement.id] = PlayerPrefs.GetInt($"AchievementUnlocked_{achievement.id}", 0) == 1;
        }
    }

    public void SaveAchievementData()
    {
        PlayerPrefs.SetInt("AchievementCount", allAchievements.Count);
        
        int i = 0;
        foreach (var achievement in allAchievements.Values)
        {
            PlayerPrefs.SetString($"AchievementID_{i}", achievement.id);
            PlayerPrefs.SetString($"AchievementTitle_{i}", achievement.title);
            PlayerPrefs.SetString($"AchievementDesc_{i}", achievement.description);
            PlayerPrefs.SetString($"AchievementCategory_{i}", achievement.category);
            PlayerPrefs.SetInt($"AchievementTarget_{i}", achievement.targetProgress);
            PlayerPrefs.SetInt($"AchievementReward_{i}", achievement.rewardAmount);
            PlayerPrefs.SetInt($"AchievementUnlocked_{i}", achievement.isUnlocked ? 1 : 0);
            PlayerPrefs.SetString($"AchievementUnlockDate_{i}", achievement.unlockDate.ToString());
            i++;
        }
        
        // Save progress
        foreach (var progress in achievementProgress)
        {
            PlayerPrefs.SetInt($"AchievementProgress_{progress.Key}", progress.Value);
        }
        
        PlayerPrefs.Save();
    }

    private void CreateDefaultAchievements()
    {
        // Create some default achievements
        CreateAchievement("first_death", "First Steps", "Die for the first time", "General", 1, 10);
        CreateAchievement("checkpoint_master", "Checkpoint Master", "Reach 10 checkpoints", "Progress", 10, 50);
        CreateAchievement("speed_runner", "Speed Runner", "Complete a level in under 60 seconds", "Challenge", 1, 100);
        CreateAchievement("collector", "Tala Collector", "Collect 100 Tala fragments", "Collection", 100, 75);
        CreateAchievement("explorer", "Explorer", "Discover 5 hidden areas", "Exploration", 5, 150);
        CreateAchievement("perfectionist", "Perfectionist", "Complete a level without taking damage", "Challenge", 1, 200);
        CreateAchievement("persistent", "Persistent", "Die 50 times", "General", 50, 25);
        CreateAchievement("jumper", "Jumper", "Jump 500 times", "Movement", 500, 30);
        
        SaveAchievementData();
    }

    private void CreateAchievement(string id, string title, string description, string category, int target, int reward)
    {
        UIAchievementData achievement = new UIAchievementData
        {
            id = id,
            title = title,
            description = description,
            category = category,
            targetProgress = target,
            rewardAmount = reward,
            isUnlocked = false,
            unlockDate = System.DateTime.MinValue
        };
        
        allAchievements[id] = achievement;
        achievementProgress[id] = 0;
        unlockedAchievements[id] = false;
    }

    private void SetupCategoryButtons()
    {
        if (categoryButtonsContainer == null || categoryButtonPrefab == null) return;
        
        // Clear existing buttons
        foreach (Button btn in categoryButtons)
        {
            if (btn != null)
                Destroy(btn.gameObject);
        }
        categoryButtons.Clear();
        
        // Create category buttons
        foreach (AchievementCategory category in categories)
        {
            GameObject btnObj = Instantiate(categoryButtonPrefab, categoryButtonsContainer);
            Button btn = btnObj.GetComponent<Button>();
            
            if (btn != null)
            {
                Text btnText = btn.GetComponentInChildren<Text>();
                if (btnText != null)
                    btnText.text = category.name;
                
                btn.onClick.AddListener(() => SelectCategory(category.name));
                categoryButtons.Add(btn);
            }
        }
        
        // Select default category
        SelectCategory(defaultCategory);
    }

    private void SetupUIListeners()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseAchievements);
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshAchievements);
    }

    public void SelectCategory(string categoryName)
    {
        currentCategory = categoryName;
        
        // Update button colors
        for (int i = 0; i < categoryButtons.Count && i < categories.Length; i++)
        {
            ColorBlock colors = categoryButtons[i].colors;
            colors.normalColor = (categories[i].name == categoryName) ? selectedCategoryColor : unselectedCategoryColor;
            categoryButtons[i].colors = colors;
        }
        
        RefreshAchievements();
    }

    public void RefreshAchievements()
    {
        ClearAchievementObjects();
        
        List<UIAchievementData> categoryAchievements = GetAchievementsByCategory(currentCategory);
        DisplayAchievements(categoryAchievements);
        UpdateOverallProgress();
    }

    private List<UIAchievementData> GetAchievementsByCategory(string category)
    {
        return allAchievements.Values.Where(a => a.category == category).ToList();
    }

    private void DisplayAchievements(List<UIAchievementData> achievements)
    {
        if (categoryTitleText != null)
            categoryTitleText.text = currentCategory + " Achievements";
        
        int unlockedCount = 0;
        int totalCount = achievements.Count;
        
        for (int i = 0; i < achievements.Count; i++)
        {
            UIAchievementData achievement = achievements[i];
            GameObject achievementObj = CreateAchievementObject(achievement);
            
            if (achievementObj != null)
            {
                achievementObjects.Add(achievementObj);
                
                if (achievement.isUnlocked)
                    unlockedCount++;
                
                // Alternate background colors
                AchievementEntryUI entryUI = achievementObj.GetComponent<AchievementEntryUI>();
                if (entryUI != null)
                {
                    entryUI.SetAlternateBackground(i % 2 == 0);
                }
            }
        }
        
        if (progressText != null)
            progressText.text = $"{unlockedCount}/{totalCount} Unlocked";
    }

    private GameObject CreateAchievementObject(UIAchievementData achievement)
    {
        if (achievementEntryPrefab == null || achievementsContainer == null)
            return null;
            
        GameObject achievementObj = Instantiate(achievementEntryPrefab, achievementsContainer);
        
        AchievementEntryUI entryUI = achievementObj.GetComponent<AchievementEntryUI>();
        if (entryUI == null)
        {
            // Fallback to basic setup if no dedicated UI component
            Text[] texts = achievementObj.GetComponentsInChildren<Text>();
            Image[] images = achievementObj.GetComponentsInChildren<Image>();
            
            if (texts.Length >= 2)
            {
                texts[0].text = achievement.title;
                texts[1].text = achievement.description;
                
                if (achievement.isUnlocked)
                {
                    texts[0].color = unlockedColor;
                    texts[1].color = unlockedColor;
                }
                else
                {
                    texts[0].color = lockedColor;
                    texts[1].color = lockedColor;
                }
            }
        }
        else
        {
            entryUI.SetupAchievement(achievement, GetAchievementProgress(achievement.id));
        }
        
        return achievementObj;
    }

    private void UpdateOverallProgress()
    {
        if (overallProgressSlider == null) return;
        
        int totalAchievements = allAchievements.Count;
        int unlockedCount = this.unlockedAchievements.Values.Count(v => v);
        
        float progress = (float)unlockedCount / totalAchievements;
        overallProgressSlider.value = progress;
    }

    private void ClearAchievementObjects()
    {
        foreach (GameObject obj in achievementObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        achievementObjects.Clear();
    }

    // Achievement tracking methods
    public void UpdateAchievementProgress(string achievementId, int progress)
    {
        if (!allAchievements.ContainsKey(achievementId))
            return;
            
        if (unlockedAchievements[achievementId])
            return; // Already unlocked
            
        achievementProgress[achievementId] = progress;
        
        UIAchievementData achievement = allAchievements[achievementId];
        if (progress >= achievement.targetProgress)
        {
            UnlockAchievement(achievementId);
        }
        
        PlayerPrefs.SetInt($"AchievementProgress_{achievementId}", progress);
        PlayerPrefs.Save();
        
        RefreshAchievements();
    }

    public void IncrementAchievement(string achievementId, int increment = 1)
    {
        if (!allAchievements.ContainsKey(achievementId))
            return;
            
        int currentProgress = GetAchievementProgress(achievementId);
        UpdateAchievementProgress(achievementId, currentProgress + increment);
    }

    public void UnlockAchievement(string achievementId)
    {
        if (!allAchievements.ContainsKey(achievementId))
            return;
            
        if (unlockedAchievements[achievementId])
            return; // Already unlocked
            
        UIAchievementData achievement = allAchievements[achievementId];
        achievement.isUnlocked = true;
        achievement.unlockDate = System.DateTime.Now;
        unlockedAchievements[achievementId] = true;
        
        // Award rewards if enabled
        if (enableRewards && achievement.rewardAmount > 0)
        {
            AwardReward(achievement.rewardAmount);
        }
        
        SaveAchievementData();
        RefreshAchievements();
        
        // Show unlock notification (would be handled by a notification system)
        ShowUnlockNotification(achievement);
    }

    private void AwardReward(int amount)
    {
        // Award currency or other rewards
        int currentCurrency = PlayerPrefs.GetInt(rewardCurrency, 0);
        PlayerPrefs.SetInt(rewardCurrency, currentCurrency + amount);
        PlayerPrefs.Save();
    }

    private void ShowUnlockNotification(UIAchievementData achievement)
    {
        // This would integrate with a notification system
        if (statusText != null)
        {
            statusText.text = $"Achievement Unlocked: {achievement.title}!";
            Invoke(nameof(ClearStatusText), 3f);
        }
    }

    private void ClearStatusText()
    {
        if (statusText != null)
            statusText.text = "";
    }

    // Utility methods
    public int GetAchievementProgress(string achievementId)
    {
        return achievementProgress.ContainsKey(achievementId) ? achievementProgress[achievementId] : 0;
    }

    public bool IsAchievementUnlocked(string achievementId)
    {
        return unlockedAchievements.ContainsKey(achievementId) && unlockedAchievements[achievementId];
    }

    public int GetUnlockedCount()
    {
        return unlockedAchievements.Values.Count(v => v);
    }

    public int GetTotalCount()
    {
        return allAchievements.Count;
    }

    // Event handlers
    private void OnRefreshAchievements()
    {
        if (isRefreshing) return;
        StartCoroutine(RefreshAchievementsCoroutine());
    }

    private System.Collections.IEnumerator RefreshAchievementsCoroutine()
    {
        isRefreshing = true;
        if (statusText != null)
            statusText.text = "Refreshing achievements...";
        
        yield return new WaitForSeconds(0.5f);
        
        LoadAchievementData();
        LoadPlayerProgress();
        RefreshAchievements();
        
        isRefreshing = false;
        if (statusText != null)
            statusText.text = "Achievements refreshed!";
        
        yield return new WaitForSeconds(2f);
        ClearStatusText();
    }

    private void OnCloseAchievements()
    {
        if (MasterGameManager.Instance != null)
        {
            MasterGameManager.Instance.OnBack();
        }
    }
}

[System.Serializable]
public class AchievementCategory
{
    public string name;
    public string description;
    public Sprite icon;
    public Color color = Color.white;
}

[System.Serializable]
public class UIAchievementData
{
    public string id;
    public string title;
    public string description;
    public string category;
    public int targetProgress;
    public int rewardAmount;
    public bool isUnlocked;
    public System.DateTime unlockDate;
    public Sprite icon;
}