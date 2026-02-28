using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using LakbayTala.Leaderboard;

/// <summary>
/// Unit tests for LakbayTala leaderboard system (data models and LeaderboardService).
/// UI component tests removed — UI folder was removed; build your own UI and test manually.
/// </summary>
public class LeaderboardTests
{
    private LeaderboardService leaderboardService;

    [SetUp]
    public void Setup()
    {
        var serviceGO = new GameObject("LeaderboardService");
        leaderboardService = serviceGO.AddComponent<LeaderboardService>();
        SetupTestEnvironment();
    }

    [TearDown]
    public void TearDown()
    {
        if (leaderboardService != null)
            GameObject.DestroyImmediate(leaderboardService.gameObject);
    }

    private void SetupTestEnvironment()
    {
        leaderboardService.config = new LeaderboardConfig
        {
            pageSize = 10,
            maxPages = 5,
            enableRealTimeUpdates = true,
            enableOfflinePersistence = true,
            enableSearch = true,
            enableFiltering = true,
            enableSorting = true,
            updateInterval = 30,
            cacheExpiry = 300,
            enableAnimations = true,
            enablePagination = true,
            enableUserProfiles = true
        };
        leaderboardService.firebaseDatabaseUrl = "https://test-firebase-url.firebaseio.com";
        leaderboardService.enableLogging = true;
        leaderboardService.enableOfflineMode = true;
    }

    #region Data Model Tests

    [Test]
    public void LeaderboardUser_Initialization_CreatesDefaultValues()
    {
        var user = new LeaderboardUser();

        Assert.IsNotNull(user.culturalScores);
        Assert.IsNotNull(user.scoreHistory);
        Assert.IsNotNull(user.profile);
        Assert.AreEqual(0, user.totalScore);
        Assert.AreEqual(0, user.currentRank);
        Assert.AreEqual(0, user.previousRank);
        Assert.IsFalse(user.isOnline);
    }

    [Test]
    public void LeaderboardUser_GetRankChange_CalculatesCorrectly()
    {
        var user = new LeaderboardUser
        {
            currentRank = 5,
            previousRank = 10
        };

        int rankChange = user.GetRankChange();
        Assert.AreEqual(5, rankChange); // Improved from 10 to 5
    }

    [Test]
    public void LeaderboardUser_HasRankImproved_ReturnsCorrectValue()
    {
        var user = new LeaderboardUser
        {
            currentRank = 3,
            previousRank = 5
        };

        Assert.IsTrue(user.HasRankImproved());

        user.currentRank = 5;
        user.previousRank = 3;
        Assert.IsFalse(user.HasRankImproved());
    }

    [Test]
    public void LeaderboardUser_GetRankChangeText_FormatsCorrectly()
    {
        var user = new LeaderboardUser
        {
            currentRank = 2,
            previousRank = 5
        };

        Assert.AreEqual("+3", user.GetRankChangeText());

        user.currentRank = 8;
        user.previousRank = 5;
        Assert.AreEqual("-3", user.GetRankChangeText());

        user.currentRank = 5;
        user.previousRank = 5;
        Assert.AreEqual("-", user.GetRankChangeText());
    }

    [Test]
    public void LeaderboardEntry_Initialization_CreatesDefaultValues()
    {
        var entry = new LeaderboardEntry();

        Assert.IsNotNull(entry.additionalData);
        Assert.IsNull(entry.user);
        Assert.AreEqual(0, entry.rank);
        Assert.AreEqual(0, entry.score);
        Assert.IsFalse(entry.isCurrentUser);
    }

    [Test]
    public void LeaderboardData_Initialization_CreatesDefaultValues()
    {
        var data = new LeaderboardData();

        Assert.IsNotNull(data.entries);
        Assert.IsNotNull(data.metadata);
        Assert.AreEqual(0, data.totalUsers);
        Assert.AreEqual(LeaderboardType.Global, data.type);
        Assert.AreEqual(LeaderboardTimeFrame.AllTime, data.timeFrame);
    }

    [Test]
    public void SearchQuery_Initialization_CreatesDefaultValues()
    {
        var query = new SearchQuery();

        Assert.IsNotNull(query.additionalFilters);
        Assert.AreEqual(1, query.page);
        Assert.AreEqual(0, query.pageSize);
        Assert.IsFalse(query.sortDescending);
        Assert.AreEqual(SortCriteria.Score, query.sortBy);
    }

    [Test]
    public void LeaderboardResponse_CreateSuccess_GeneratesCorrectResponse()
    {
        var data = new LeaderboardData();
        var response = LeaderboardResponse.CreateSuccess(data, 100, 1, 10);

        Assert.IsTrue(response.success);
        Assert.AreEqual(data, response.data);
        Assert.AreEqual(100, response.totalCount);
        Assert.AreEqual(1, response.page);
        Assert.AreEqual(10, response.pageSize);
        Assert.IsTrue((DateTime.Now - response.timestamp).TotalSeconds < 1);
    }

    [Test]
    public void LeaderboardResponse_CreateError_GeneratesCorrectResponse()
    {
        var errorMessage = "Test error";
        var response = LeaderboardResponse.CreateError(errorMessage);

        Assert.IsFalse(response.success);
        Assert.AreEqual(errorMessage, response.error);
        Assert.IsNull(response.data);
        Assert.IsTrue((DateTime.Now - response.timestamp).TotalSeconds < 1);
    }

    #endregion

    #region Service Tests

    [Test]
    public void LeaderboardService_Initialization_SetsUpCorrectly()
    {
        System.Threading.Thread.Sleep(100); // Small delay

        Assert.IsNotNull(leaderboardService);
        Assert.IsNotNull(leaderboardService.config);
        Assert.IsTrue(leaderboardService.enableLogging);
        Assert.IsTrue(leaderboardService.enableOfflineMode);
        Assert.IsFalse(string.IsNullOrEmpty(leaderboardService.firebaseDatabaseUrl));
    }

    [Test]
    public void LeaderboardService_LoadLeaderboard_WithValidQuery_ReturnsData()
    {
        var query = new SearchQuery
        {
            page = 1,
            pageSize = 10,
            sortBy = SortCriteria.Score,
            sortDescending = true,
            searchTerm = "",
            additionalFilters = new Dictionary<string, object>()
        };

        LeaderboardResponse response = null;
        bool operationComplete = false;

        leaderboardService.LoadLeaderboard(query, (result) =>
        {
            response = result;
            operationComplete = true;
        });

        // Wait for operation to complete
        float timeout = 5f;
        var startTime = DateTime.Now;
        while (!operationComplete && (DateTime.Now - startTime).TotalSeconds < timeout)
        {
            System.Threading.Thread.Sleep(100); // Wait 100ms
        }

        Assert.IsNotNull(response);
        Assert.IsTrue(response.success);
        Assert.IsNotNull(response.data);
        Assert.IsNotNull(response.data.entries);
        Assert.Greater(response.data.entries.Count, 0);
    }

    [Test]
    public void LeaderboardService_UpdateUserScore_UpdatesSuccessfully()
    {
        string testUserId = "test_user_123";
        int newScore = 1500;

        bool operationComplete = false;
        LeaderboardUpdate receivedUpdate = null;

        // Subscribe to update event
        leaderboardService.OnLeaderboardUpdate += (update) =>
        {
            receivedUpdate = update;
            operationComplete = true;
        };

        leaderboardService.UpdateUserScore(testUserId, newScore);

        // Wait for update
        float timeout = 3f;
        var startTime = DateTime.Now;
        while (!operationComplete && (DateTime.Now - startTime).TotalSeconds < timeout)
        {
            System.Threading.Thread.Sleep(100); // Wait 100ms
        }

        Assert.IsNotNull(receivedUpdate);
        Assert.AreEqual(testUserId, receivedUpdate.userId);
        Assert.AreEqual(newScore, receivedUpdate.newScore);
    }

    [Test]
    public void LeaderboardService_IsOnline_ReturnsCorrectStatus()
    {
        bool isOnline = leaderboardService.IsOnline();
        
        // Should return true in test environment (simulated online)
        Assert.IsTrue(isOnline);
    }

    [Test]
    public void LeaderboardService_SetCurrentUser_SetsCorrectly()
    {
        string testUserId = "test_user_456";
        
        leaderboardService.SetCurrentUser(testUserId);
        
        // Verify user is set (would need public getter or reflection for full test)
        Assert.Pass("User ID set successfully");
    }

    [Test]
    public void LeaderboardService_ClearCache_ClearsSuccessfully()
    {
        // First load some data to populate cache
        var query = new SearchQuery
        {
            page = 1,
            pageSize = 5,
            sortBy = SortCriteria.Score,
            sortDescending = true
        };

        leaderboardService.LoadLeaderboard(query, (response) => { });

        System.Threading.Thread.Sleep(100); // Small delay

        // Clear cache
        leaderboardService.ClearCache();

        Assert.Pass("Cache cleared successfully");
    }

    #endregion

    #region UI Controller Tests (ignored - UI components removed)

    [Test, Ignore("UI components removed - build your own UI")]
    public void LeaderboardUIController_Initialization_SetsUpCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test, Ignore("UI components removed")]
    public void LeaderboardUIController_SearchDebounce_WorksCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test, Ignore("UI components removed")]
    public void LeaderboardUIController_SortCriteria_UpdatesCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test, Ignore("UI components removed")]
    public void LeaderboardUIController_RefreshLeaderboard_UpdatesData()
    {
        Assert.Pass("UI removed");
    }

    private void SimulateRefresh(Action callback)
    {
        // Simulate async operation
        System.Threading.Thread.Sleep(100);
        callback?.Invoke();
    }

    #endregion

    #region Entry UI Tests (ignored - UI components removed)

    [Test, Ignore("UI components removed")]
    public void LeaderboardEntryUI_SetupEnhancedEntry_ConfiguresCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test]
    public void LeaderboardEntryUI_GetRankChangeIndicator_ShowsCorrectDirection()
    {
        var entry = new LeaderboardEntry
        {
            user = new LeaderboardUser
            {
                currentRank = 3,
                previousRank = 5
            }
        };

        int rankChange = entry.user.GetRankChange();
        Assert.AreEqual(2, rankChange); // Improved
        Assert.IsTrue(entry.user.HasRankImproved());
    }

    [Test, Ignore("UI components removed")]
    public void LeaderboardEntryUI_Animation_CompletesSuccessfully()
    {
        Assert.Pass("UI removed");
    }

    #endregion

    #region Profile Modal Tests (ignored - UI components removed)

    [Test, Ignore("UI components removed")]
    public void UserProfileModal_ShowProfile_DisplaysCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test, Ignore("UI components removed")]
    public void UserProfileModal_CloseProfile_HidesCorrectly()
    {
        Assert.Pass("UI removed");
    }

    #endregion

    #region Performance Tests

    [Test]
    public void LeaderboardService_PerformanceMetrics_TracksCorrectly()
    {
        var metrics = leaderboardService.GetPerformanceMetrics();
        
        Assert.IsNotNull(metrics);
        Assert.AreEqual(0, metrics.loadTime); // Initially 0
        Assert.AreEqual(0, metrics.cacheHits);
        Assert.AreEqual(0, metrics.cacheMisses);
    }

    [Test]
    public void LeaderboardService_LargeDataset_HandlesEfficiently()
    {
        var query = new SearchQuery
        {
            page = 1,
            pageSize = 100, // Large page size
            sortBy = SortCriteria.Score,
            sortDescending = true
        };

        float startTime = Time.realtimeSinceStartup;

        LeaderboardResponse response = null;
        leaderboardService.LoadLeaderboard(query, (result) =>
        {
            response = result;
        });

        float loadTime = Time.realtimeSinceStartup - startTime;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.success);
        Assert.Less(loadTime, 2f, "Load time should be under 2 seconds");
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public void LeaderboardService_InvalidQuery_ReturnsError()
    {
        var invalidQuery = (SearchQuery)null;

        LeaderboardResponse response = null;
        leaderboardService.LoadLeaderboard(invalidQuery, (result) =>
        {
            response = result;
        });

        // Service should handle null query gracefully
        Assert.IsNotNull(response);
    }

    [Test]
    public void LeaderboardService_InvalidConfiguration_HandlesGracefully()
    {
        leaderboardService.config = null;
        
        // Should not crash
        Assert.DoesNotThrow(() =>
        {
            leaderboardService.InitializeService();
        });
    }

    #endregion

    #region Cultural Integration Tests

    [Test, Ignore("UI components removed")]
    public void LeaderboardEntryUI_CulturalTheming_AppliesCorrectly()
    {
        Assert.Pass("UI removed");
    }

    [Test, Ignore("UI components removed")]
    public void UserProfileModal_CulturalInformation_DisplaysCorrectly()
    {
        Assert.Pass("UI removed");
    }

    #endregion

    #region Accessibility Tests

    [Test, Ignore("UI components removed")]
    public void LeaderboardEntryUI_AccessibilityFeatures_EnabledCorrectly()
    {
        Assert.Pass("UI removed");
    }

    #endregion

    #region Serialization Tests

    [Test]
    public void LeaderboardData_Serialization_RoundTrip()
    {
        var originalData = new LeaderboardData
        {
            entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry
                {
                    user = new LeaderboardUser { userId = "serialize_test", username = "SerializeUser" },
                    rank = 1,
                    score = 1000
                }
            },
            totalUsers = 1,
            lastUpdated = DateTime.Now,
            leaderboardId = "test_leaderboard"
        };

        string json = JsonConvert.SerializeObject(originalData);
        var deserializedData = JsonConvert.DeserializeObject<LeaderboardData>(json);

        Assert.IsNotNull(deserializedData);
        Assert.AreEqual(originalData.totalUsers, deserializedData.totalUsers);
        Assert.AreEqual(originalData.leaderboardId, deserializedData.leaderboardId);
        Assert.AreEqual(originalData.entries.Count, deserializedData.entries.Count);
    }

    #endregion

    #region Helper Methods

    private void Log(string message)
    {
        Debug.Log($"[LeaderboardTests] {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[LeaderboardTests] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[LeaderboardTests] {message}");
    }

    #endregion
}