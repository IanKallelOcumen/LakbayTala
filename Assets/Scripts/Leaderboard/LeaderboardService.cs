using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Newtonsoft.Json;

/// <summary>
/// Comprehensive leaderboard service managing data operations, real-time updates, and offline persistence.
/// Handles Firebase integration, caching, and synchronization for LakbayTala leaderboard system.
/// </summary>
namespace LakbayTala.Leaderboard
{
    public class LeaderboardService : MonoBehaviour
    {
        [Header("Service Configuration")]
        public LeaderboardConfig config;
        public bool enableLogging = true;
        public bool enableOfflineMode = true;
        
        [Header("Firebase Configuration")]
        public string firebaseDatabaseUrl;
        public string leaderboardCollection = "leaderboards";
        public string userScoresCollection = "userScores";
        public string updatesCollection = "updates";
        
        private static LeaderboardService instance;
        public static LeaderboardService Instance
        {
            get
            {
                if (instance == null)
                    instance = FindFirstObjectByType<LeaderboardService>();
                return instance;
            }
        }
        
        // Data storage
        private Dictionary<string, LeaderboardUser> userCache;
        private Dictionary<string, LeaderboardData> leaderboardCache;
        private List<PendingUpdate> pendingUpdates;
        private OfflineData offlineData;
        private PerformanceMetrics currentMetrics;
        
        // State management
        private bool isOnline = true;
        private bool isSyncing = false;
        private DateTime lastSyncTime;
        private string currentUserId;
        
        // Event system
        public event Action<LeaderboardUpdate> OnLeaderboardUpdate;
#pragma warning disable CS0067
        public event Action<List<LeaderboardEntry>> OnLeaderboardLoaded;
        public event Action<string> OnError;
#pragma warning restore CS0067
        public event Action<bool> OnConnectionStatusChanged;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeService();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            StartCoroutine(InitializeFirebaseConnection());
        }
        
        /// <summary>
        /// Initialize the leaderboard service with configuration and data structures.
        /// </summary>
        internal void InitializeService()
        {
            // Initialize configuration
            if (config == null)
            {
                config = new LeaderboardConfig();
            }
            
            // Initialize data storage
            userCache = new Dictionary<string, LeaderboardUser>();
            leaderboardCache = new Dictionary<string, LeaderboardData>();
            pendingUpdates = new List<PendingUpdate>();
            currentMetrics = new PerformanceMetrics();
            
            // Load offline data
            if (enableOfflineMode)
            {
                LoadOfflineData();
            }
            
            // Setup connection monitoring
            StartCoroutine(MonitorConnectionStatus());
            
            Log("Leaderboard Service initialized successfully");
        }
        
        /// <summary>
        /// Initialize Firebase connection and setup real-time listeners.
        /// </summary>
        private IEnumerator InitializeFirebaseConnection()
        {
            if (string.IsNullOrEmpty(firebaseDatabaseUrl))
            {
                LogWarning("Firebase URL not configured. Set firebaseDatabaseUrl when Firebase is implemented.");
                isOnline = false;
                yield break;
            }
            
            // TODO: Initialize Firebase SDK when implemented. No simulated delay or test behavior.
            // Setup real-time listeners
            if (config.enableRealTimeUpdates)
            {
                StartCoroutine(SetupRealtimeListeners());
            }
            
            Log("Firebase connection initialized");
        }
        
        /// <summary>
        /// Setup real-time listeners for leaderboard updates.
        /// </summary>
        private IEnumerator SetupRealtimeListeners()
        {
            while (true)
            {
                if (isOnline && !isSyncing)
                {
                    yield return StartCoroutine(ListenForUpdates());
                }
                yield return new WaitForSeconds(config.updateInterval);
            }
        }
        
        /// <summary>
        /// Listen for real-time updates from Firebase. Implement with Firebase SDK when ready.
        /// </summary>
        private IEnumerator ListenForUpdates()
        {
            // TODO: Replace with Firebase SDK real-time listener when implemented.
            ProcessPendingUpdates();
            yield break;
        }
        
        /// <summary>
        /// Monitor connection status and handle online/offline transitions.
        /// </summary>
        private IEnumerator MonitorConnectionStatus()
        {
            while (true)
            {
                bool previousStatus = isOnline;
                isOnline = CheckInternetConnection();
                
                if (previousStatus != isOnline)
                {
                    OnConnectionStatusChanged?.Invoke(isOnline);
                    
                    if (isOnline)
                    {
                        StartCoroutine(SyncOfflineData());
                    }
                }
                
                yield return new WaitForSeconds(5f);
            }
        }
        
        /// <summary>
        /// Check internet connection availability.
        /// </summary>
        private bool CheckInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        
        /// <summary>
        /// Load leaderboard data with filtering, sorting, and pagination.
        /// </summary>
        public IEnumerator LoadLeaderboard(SearchQuery query, Action<LeaderboardResponse> callback)
        {
            var startTime = Time.realtimeSinceStartup;
            string cacheKey = null;
            LeaderboardResponse cachedResponse = null;
            bool useCache = false;
            Exception caught = null;
            
            try
            {
                // Validate query
                if (query == null)
                {
                    query = new SearchQuery
                    {
                        page = 1,
                        pageSize = config.pageSize,
                        sortBy = SortCriteria.Score,
                        sortDescending = true
                    };
                }
                
                cacheKey = GenerateCacheKey(query);
                if (leaderboardCache.ContainsKey(cacheKey) && IsCacheValid(leaderboardCache[cacheKey]))
                {
                    cachedResponse = CreateResponseFromCache(leaderboardCache[cacheKey], query);
                    currentMetrics.cacheHits++;
                    currentMetrics.loadTime = Time.realtimeSinceStartup - startTime;
                    useCache = true;
                }
            }
            catch (Exception ex)
            {
                caught = ex;
            }
            
            if (caught != null)
            {
                LogError($"Error loading leaderboard: {caught.Message}");
                callback?.Invoke(LeaderboardResponse.CreateError(caught.Message));
                yield break;
            }
            if (useCache)
            {
                callback?.Invoke(cachedResponse);
                yield break;
            }
            
            yield return StartCoroutine(LoadLeaderboardFromSource(query, (response) =>
            {
                currentMetrics.loadTime = Time.realtimeSinceStartup - startTime;
                currentMetrics.cacheMisses++;
                if (response.success && cacheKey != null)
                    leaderboardCache[cacheKey] = response.data;
                callback?.Invoke(response);
            }));
        }
        
        /// <summary>
        /// Load leaderboard data from the appropriate source (Firebase or offline).
        /// </summary>
        private IEnumerator LoadLeaderboardFromSource(SearchQuery query, Action<LeaderboardResponse> callback)
        {
            if (isOnline)
            {
                yield return StartCoroutine(LoadFromFirebase(query, callback));
            }
            else if (enableOfflineMode)
            {
                yield return StartCoroutine(LoadFromOfflineData(query, callback));
            }
            else
            {
                callback?.Invoke(LeaderboardResponse.CreateError("No data source available"));
            }
        }
        
        /// <summary>
        /// Load leaderboard data from Firebase. No test/sample data — implement Firebase SDK later.
        /// </summary>
        private IEnumerator LoadFromFirebase(SearchQuery query, Action<LeaderboardResponse> callback)
        {
            // TODO: Replace with actual Firebase SDK calls when implemented.
            // Do not use test or sample data — Firebase only.
            var emptyData = new LeaderboardData();
            emptyData.totalUsers = 0;
            emptyData.lastUpdated = DateTime.Now;
            emptyData.leaderboardId = "firebase_leaderboard";
            callback?.Invoke(LeaderboardResponse.CreateSuccess(emptyData, 0, query.page, query.pageSize));
            yield break;
        }
        
        /// <summary>
        /// Load leaderboard data from offline storage.
        /// </summary>
        private IEnumerator LoadFromOfflineData(SearchQuery query, Action<LeaderboardResponse> callback)
        {
            if (offlineData == null || offlineData.cachedEntries == null)
            {
                callback?.Invoke(LeaderboardResponse.CreateError("No offline data available"));
                yield break;
            }
            
            // Apply filtering and sorting to offline data
            var filteredEntries = ApplyFilters(offlineData.cachedEntries, query);
            var sortedEntries = ApplySorting(filteredEntries, query);
            var paginatedEntries = ApplyPagination(sortedEntries, query);
            
            var leaderboardData = new LeaderboardData
            {
                entries = paginatedEntries,
                totalUsers = filteredEntries.Count,
                lastUpdated = offlineData.lastSync,
                leaderboardId = "offline_leaderboard"
            };
            
            callback?.Invoke(LeaderboardResponse.CreateSuccess(leaderboardData, leaderboardData.totalUsers, query.page, query.pageSize));
        }
        
        /// <summary>
        /// Update user score and trigger leaderboard recalculation.
        /// </summary>
        public IEnumerator UpdateUserScore(string userId, int newScore, Dictionary<string, object> additionalData = null)
        {
            var update = new LeaderboardUpdate
            {
                userId = userId,
                newScore = newScore,
                additionalData = additionalData ?? new Dictionary<string, object>()
            };
            
            if (isOnline)
            {
                yield return StartCoroutine(UpdateScoreOnline(update));
            }
            else
            {
                QueueOfflineUpdate(update);
            }
        }
        
        /// <summary>
        /// Update score through online service.
        /// </summary>
        private IEnumerator UpdateScoreOnline(LeaderboardUpdate update)
        {
            yield return new WaitForSeconds(0.3f);
            try
            {
                if (userCache.ContainsKey(update.userId))
                {
                    var user = userCache[update.userId];
                    update.oldScore = user.totalScore;
                    user.totalScore = update.newScore;
                    user.lastActive = DateTime.Now;
                }
                OnLeaderboardUpdate?.Invoke(update);
                Log($"Updated score for user {update.userId}: {update.oldScore} -> {update.newScore}");
            }
            catch (Exception ex)
            {
                LogError($"Error updating score online: {ex.Message}");
                QueueOfflineUpdate(update);
            }
        }
        
        /// <summary>
        /// Queue update for offline processing.
        /// </summary>
        private void QueueOfflineUpdate(LeaderboardUpdate update)
        {
            if (!enableOfflineMode) return;
            
            var pendingUpdate = new PendingUpdate
            {
                userId = update.userId,
                newScore = update.newScore,
                type = UpdateType.ScoreUpdate
            };
            
            pendingUpdates.Add(pendingUpdate);
            SaveOfflineData();
            
            Log($"Queued offline update for user {update.userId}");
        }
        
        /// <summary>
        /// Process pending offline updates.
        /// </summary>
        private void ProcessPendingUpdates()
        {
            if (!isOnline || pendingUpdates.Count == 0) return;
            
            Log($"Processing {pendingUpdates.Count} pending updates");
            
            var updatesToProcess = new List<PendingUpdate>(pendingUpdates);
            pendingUpdates.Clear();
            
            foreach (var update in updatesToProcess)
            {
                StartCoroutine(UpdateScoreOnline(new LeaderboardUpdate
                {
                    userId = update.userId,
                    newScore = update.newScore
                }));
            }
        }
        
        /// <summary>
        /// Sync offline data with online service.
        /// </summary>
        private IEnumerator SyncOfflineData()
        {
            if (!enableOfflineMode || !isOnline || isSyncing) yield break;
            
            isSyncing = true;
            Log("Starting offline data sync");
            try
            {
                ProcessPendingUpdates();
            }
            catch (Exception ex)
            {
                LogError($"Error during offline sync: {ex.Message}");
            }
            yield return StartCoroutine(SyncUserCache());
            try
            {
                lastSyncTime = DateTime.Now;
                if (offlineData != null)
                {
                    offlineData.lastSync = lastSyncTime;
                    SaveOfflineData();
                }
                Log("Offline data sync completed");
            }
            catch (Exception ex)
            {
                LogError($"Error during offline sync: {ex.Message}");
            }
            finally
            {
                isSyncing = false;
            }
        }
        
        /// <summary>
        /// Sync user cache with online data. Implement with Firebase SDK when ready.
        /// </summary>
        private IEnumerator SyncUserCache()
        {
            // TODO: Replace with actual Firebase sync when implemented.
            Log($"Synced {userCache.Count} users");
            yield break;
        }
        
        // Data processing methods
        
        /// <summary>
        /// Apply filters to leaderboard entries.
        /// </summary>
        private List<LeaderboardEntry> ApplyFilters(List<LeaderboardEntry> entries, SearchQuery query)
        {
            var filtered = entries.AsEnumerable();
            
            // Apply search filter
            if (!string.IsNullOrEmpty(query.searchTerm))
            {
                filtered = filtered.Where(e => 
                    e.user.username.ToLower().Contains(query.searchTerm.ToLower()) ||
                    e.user.displayName.ToLower().Contains(query.searchTerm.ToLower()) ||
                    e.user.country.ToLower().Contains(query.searchTerm.ToLower()));
            }
            
            // Apply additional filters
            if (query.additionalFilters != null)
            {
                foreach (var filter in query.additionalFilters)
                {
                    // Apply custom filter logic based on filter type
                    switch (filter.Key)
                    {
                        case "country":
                            filtered = filtered.Where(e => e.user.country == filter.Value.ToString());
                            break;
                        case "culturalLevel":
                            filtered = filtered.Where(e => e.user.culturalLevel == filter.Value.ToString());
                            break;
                        case "onlineOnly":
                            if ((bool)filter.Value)
                                filtered = filtered.Where(e => e.user.isOnline);
                            break;
                    }
                }
            }
            
            return filtered.ToList();
        }
        
        /// <summary>
        /// Apply sorting to leaderboard entries.
        /// </summary>
        private List<LeaderboardEntry> ApplySorting(List<LeaderboardEntry> entries, SearchQuery query)
        {
            var sorted = entries.AsEnumerable();
            
            switch (query.sortBy)
            {
                case SortCriteria.Score:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.score) : 
                        sorted.OrderBy(e => e.score);
                    break;
                case SortCriteria.Rank:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.rank) : 
                        sorted.OrderBy(e => e.rank);
                    break;
                case SortCriteria.Name:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.user.username) : 
                        sorted.OrderBy(e => e.user.username);
                    break;
                case SortCriteria.Country:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.user.country) : 
                        sorted.OrderBy(e => e.user.country);
                    break;
                case SortCriteria.CulturalLevel:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.user.culturalLevel) : 
                        sorted.OrderBy(e => e.user.culturalLevel);
                    break;
                case SortCriteria.RecentActivity:
                    sorted = query.sortDescending ? 
                        sorted.OrderByDescending(e => e.user.lastActive) : 
                        sorted.OrderBy(e => e.user.lastActive);
                    break;
            }
            
            return sorted.ToList();
        }
        
        /// <summary>
        /// Apply pagination to leaderboard entries.
        /// </summary>
        private List<LeaderboardEntry> ApplyPagination(List<LeaderboardEntry> entries, SearchQuery query)
        {
            int skip = (query.page - 1) * query.pageSize;
            return entries.Skip(skip).Take(query.pageSize).ToList();
        }
        
        // Utility methods
        
        /// <summary>
        /// Generate cache key for search query.
        /// </summary>
        private string GenerateCacheKey(SearchQuery query)
        {
            return $"{query.searchTerm}_{query.filter}_{query.sortBy}_{query.sortDescending}_{query.page}_{query.pageSize}";
        }
        
        /// <summary>
        /// Check if cached data is still valid.
        /// </summary>
        private bool IsCacheValid(LeaderboardData cachedData)
        {
            return (DateTime.Now - cachedData.lastUpdated).TotalSeconds < config.cacheExpiry;
        }
        
        /// <summary>
        /// Create response from cached data.
        /// </summary>
        private LeaderboardResponse CreateResponseFromCache(LeaderboardData cachedData, SearchQuery query)
        {
            var filtered = ApplyFilters(cachedData.entries, query);
            var sorted = ApplySorting(filtered, query);
            var paginated = ApplyPagination(sorted, query);
            
            var responseData = new LeaderboardData
            {
                entries = paginated,
                totalUsers = filtered.Count,
                lastUpdated = cachedData.lastUpdated,
                leaderboardId = cachedData.leaderboardId
            };
            
            return LeaderboardResponse.CreateSuccess(responseData, responseData.totalUsers, query.page, query.pageSize);
        }
        
        /// <summary>
        /// Generate sample leaderboard data for demonstration.
        /// </summary>
        private LeaderboardData GenerateSampleLeaderboardData(SearchQuery query)
        {
            var entries = new List<LeaderboardEntry>();
            var random = new System.Random();
            
            // Generate sample users
            string[] sampleNames = { "Maria", "Jose", "Ana", "Carlos", "Liza", "Rizal", "Luna", "Andres", "Gabriela", "Emilio" };
            string[] countries = { "Philippines", "USA", "Canada", "Australia", "UK", "Japan", "South Korea", "Singapore" };
            string[] culturalLevels = { "Novice", "Apprentice", "Explorer", "Scholar", "Master", "Legend" };
            
            int totalUsers = 100; // Simulate large dataset
            
            for (int i = 0; i < totalUsers; i++)
            {
                var user = new LeaderboardUser
                {
                    userId = $"user_{i}",
                    username = sampleNames[i % sampleNames.Length] + $"_{i}",
                    displayName = sampleNames[i % sampleNames.Length],
                    totalScore = random.Next(1000, 50000),
                    currentRank = i + 1,
                    previousRank = i + 1 + random.Next(-5, 6),
                    isOnline = random.Next(0, 100) < 30,
                    lastActive = DateTime.Now.AddMinutes(-random.Next(0, 1440)),
                    country = countries[i % countries.Length],
                    culturalLevel = culturalLevels[random.Next(0, culturalLevels.Length)]
                };
                
                var entry = new LeaderboardEntry
                {
                    user = user,
                    rank = user.currentRank,
                    score = user.totalScore,
                    lastUpdated = DateTime.Now,
                    isCurrentUser = user.userId == currentUserId
                };
                
                entries.Add(entry);
            }
            
            // Apply filtering and sorting
            var filtered = ApplyFilters(entries, query);
            var sorted = ApplySorting(filtered, query);
            
            // Update ranks
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].rank = i + 1;
            }
            
            var leaderboardData = new LeaderboardData
            {
                entries = sorted,
                totalUsers = totalUsers,
                lastUpdated = DateTime.Now,
                leaderboardId = "sample_leaderboard",
                type = LeaderboardType.Global,
                timeFrame = LeaderboardTimeFrame.AllTime
            };
            
            return leaderboardData;
        }
        
        // Data persistence methods
        
        /// <summary>
        /// Load offline data from PlayerPrefs.
        /// </summary>
        private void LoadOfflineData()
        {
            try
            {
                string offlineDataJson = PlayerPrefs.GetString("LeaderboardOfflineData", "");
                if (!string.IsNullOrEmpty(offlineDataJson))
                {
                    offlineData = JsonConvert.DeserializeObject<OfflineData>(offlineDataJson);
                    Log("Loaded offline data");
                }
                else
                {
                    offlineData = new OfflineData();
                    Log("Created new offline data");
                }
            }
            catch (Exception ex)
            {
                LogError($"Error loading offline data: {ex.Message}");
                offlineData = new OfflineData();
            }
        }
        
        /// <summary>
        /// Save offline data to PlayerPrefs.
        /// </summary>
        private void SaveOfflineData()
        {
            if (!enableOfflineMode || offlineData == null) return;
            
            try
            {
                string offlineDataJson = JsonConvert.SerializeObject(offlineData);
                PlayerPrefs.SetString("LeaderboardOfflineData", offlineDataJson);
                PlayerPrefs.Save();
                Log("Saved offline data");
            }
            catch (Exception ex)
            {
                LogError($"Error saving offline data: {ex.Message}");
            }
        }
        
        // Logging methods
        
        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[LeaderboardService] {message}");
        }
        
        private void LogWarning(string message)
        {
            if (enableLogging)
                Debug.LogWarning($"[LeaderboardService] {message}");
        }
        
        private void LogError(string message)
        {
            if (enableLogging)
                Debug.LogError($"[LeaderboardService] {message}");
        }
        
        // Public API methods
        
        /// <summary>
        /// Get current performance metrics.
        /// </summary>
        public PerformanceMetrics GetPerformanceMetrics()
        {
            return currentMetrics;
        }
        
        /// <summary>
        /// Get current connection status.
        /// </summary>
        public bool IsOnline()
        {
            return isOnline;
        }
        
        /// <summary>
        /// Set current user ID.
        /// </summary>
        public void SetCurrentUser(string userId)
        {
            currentUserId = userId;
            Log($"Current user set: {userId}");
        }
        
        /// <summary>
        /// Get current user ID (for profile comparison).
        /// </summary>
        public string GetCurrentUserId() => currentUserId ?? "";
        
        /// <summary>
        /// Force sync offline data.
        /// </summary>
        public void ForceSync()
        {
            StartCoroutine(SyncOfflineData());
        }
        
        /// <summary>
        /// Clear all cached data.
        /// </summary>
        public void ClearCache()
        {
            userCache.Clear();
            leaderboardCache.Clear();
            Log("Cache cleared");
        }
        
        void OnDestroy()
        {
            // Save offline data before destruction
            if (enableOfflineMode)
            {
                SaveOfflineData();
            }
            
            Log("Leaderboard Service destroyed");
        }
    }
}