# LakbayTala Leaderboard System - Deployment Guide & API Documentation

## Table of Contents
1. [Firebase Setup](#firebase-setup)
2. [Unity Integration](#unity-integration)
3. [API Reference](#api-reference)
4. [Configuration](#configuration)
5. [Deployment Steps](#deployment-steps)
6. [Monitoring & Maintenance](#monitoring--maintenance)
7. [Troubleshooting](#troubleshooting)

## Firebase Setup

### 1. Create Firebase Project
```bash
# Using Firebase CLI
firebase projects:create lakbaytala-leaderboard
firebase use lakbaytala-leaderboard
```

### 2. Enable Authentication Methods
```javascript
// firebase.json configuration
{
  "database": {
    "rules": "database.rules.json"
  },
  "auth": {
    "providers": {
      "email": true,
      "google": true,
      "facebook": false,
      "anonymous": true
    }
  }
}
```

### 3. Database Rules Configuration
```javascript
// database.rules.json
{
  "rules": {
    "leaderboards": {
      ".read": "auth != null",
      ".write": "auth != null",
      "$leaderboardId": {
        ".validate": "newData.hasChildren(['entries', 'lastUpdated'])"
      }
    },
    "userScores": {
      "$userId": {
        ".read": "auth != null && (auth.uid == $userId || auth.token.admin == true)",
        ".write": "auth != null && (auth.uid == $userId || auth.token.admin == true)",
        ".validate": "newData.hasChildren(['score', 'lastUpdated'])"
      }
    },
    "updates": {
      ".read": "auth != null",
      ".write": "auth != null",
      "$updateId": {
        ".validate": "newData.hasChildren(['userId', 'type', 'timestamp'])"
      }
    }
  }
}
```

### 4. Firebase Unity SDK Integration
```csharp
// FirebaseManager.cs
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseApp app;
    private DatabaseReference databaseRef;
    private FirebaseAuth auth;
    
    public async Task InitializeFirebase()
    {
        try
        {
            // Check dependencies
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Initialize Firebase
                app = FirebaseApp.DefaultInstance;
                databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                auth = FirebaseAuth.DefaultInstance;
                
                Debug.Log("Firebase initialized successfully");
            }
            else
            {
                Debug.LogError($"Firebase dependencies not available: {dependencyStatus}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Firebase initialization failed: {ex.Message}");
        }
    }
}
```

## Unity Integration

### 1. Package Dependencies
Add to `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.google.firebase.database": "11.6.0",
    "com.google.firebase.auth": "11.6.0",
    "com.google.firebase.analytics": "11.6.0",
    "com.unity.nuget.newtonsoft-json": "3.2.2"
  }
}
```

### 2. Scene Setup
```csharp
// LeaderboardSceneSetup.cs
using UnityEngine;
using LakbayTala.Leaderboard;

public class LeaderboardSceneSetup : MonoBehaviour
{
    [Header("Leaderboard Configuration")]
    public LeaderboardConfig config;
    public GameObject leaderboardUI;
    public GameObject userProfileModal;
    
    private LeaderboardService leaderboardService;
    private LakbayTalaLeaderboardUIController uiController;
    
    void Start()
    {
        SetupLeaderboardSystem();
        ConfigureFirebase();
    }
    
    private void SetupLeaderboardSystem()
    {
        // Initialize service
        leaderboardService = LeaderboardService.Instance;
        leaderboardService.config = config;
        
        // Setup UI controller
        uiController = leaderboardUI.GetComponent<LakbayTalaLeaderboardUIController>();
        if (uiController != null)
        {
            uiController.userProfileModal = userProfileModal;
            uiController.ApplyCulturalTheme();
        }
        
        // Configure Firebase
        leaderboardService.firebaseDatabaseUrl = "https://your-project.firebaseio.com";
        
        Debug.Log("Leaderboard system setup complete");
    }
    
    private async void ConfigureFirebase()
    {
        var firebaseManager = FindObjectOfType<FirebaseManager>();
        if (firebaseManager != null)
        {
            await firebaseManager.InitializeFirebase();
        }
    }
}
```

### 3. Cultural Integration
```csharp
// LakbayTalaCulturalIntegration.cs
using UnityEngine;
using LakbayTala.Leaderboard;

public class LakbayTalaCulturalIntegration : MonoBehaviour
{
    [Header("Cultural Elements")]
    public string[] rankTitles = {
        "Diwata", "Tikbalang", "Kapre", "Aswang", "Tiyanak",
        "Manananggal", "Wakwak", "Sigbin", "Tigbalang", "Siyokoy"
    };
    
    public Color[] rankColors = {
        Color.yellow, Color.red, Color.green, Color.blue, Color.magenta,
        Color.cyan, Color.orange, Color.grey, Color.white, Color.black
    };
    
    public string[] rankDescriptions = {
        "A divine fairy of the forests",
        "A creature with horse head and human body",
        "A tree-dwelling giant",
        "A shape-shifting creature",
        "A vampiric child creature"
    };
    
    public string[] rankBaybayinNames = {
        "ᜇᜒᜏᜆ", "ᜆᜒᜃ᜔ᜊᜎᜅ᜔", "ᜃᜉ᜔ᜇᜒ", "ᜀᜐ᜔ᜏᜅ᜔", "ᜆᜒᜌᜈᜃ᜔"
    };
    
    void Start()
    {
        var leaderboardPanel = GetComponent<LakbayTalaLeaderboardPanel>();
        if (leaderboardPanel != null)
        {
            leaderboardPanel.rankTitles = rankTitles;
            leaderboardPanel.rankColors = rankColors;
            leaderboardPanel.rankDescriptions = rankDescriptions;
            leaderboardPanel.rankBaybayinNames = rankBaybayinNames;
        }
    }
}
```

## API Reference

### LeaderboardService API

#### Methods

```csharp
// Load leaderboard data
public IEnumerator LoadLeaderboard(SearchQuery query, Action<LeaderboardResponse> callback)

// Update user score
public IEnumerator UpdateUserScore(string userId, int newScore, Dictionary<string, object> additionalData = null)

// Get current connection status
public bool IsOnline()

// Set current user
public void SetCurrentUser(string userId)

// Force sync offline data
public void ForceSync()

// Clear all cached data
public void ClearCache()

// Get performance metrics
public PerformanceMetrics GetPerformanceMetrics()
```

#### Events

```csharp
// Leaderboard update event
public event Action<LeaderboardUpdate> OnLeaderboardUpdate;

// Leaderboard loaded event
public event Action<List<LeaderboardEntry>> OnLeaderboardLoaded;

// Error event
public event Action<string> OnError;

// Connection status changed event
public event Action<bool> OnConnectionStatusChanged;
```

### Data Models

```csharp
// Leaderboard User
[System.Serializable]
public class LeaderboardUser
{
    public string userId;
    public string username;
    public string displayName;
    public string avatarUrl;
    public int totalScore;
    public int currentRank;
    public int previousRank;
    public bool isOnline;
    public DateTime lastActive;
    public string country;
    public string culturalLevel;
    public Dictionary<string, int> culturalScores;
    public List<ScoreHistory> scoreHistory;
    public UserProfile profile;
}

// Search Query
[System.Serializable]
public class SearchQuery
{
    public string searchTerm;
    public FilterCriteria filter;
    public SortCriteria sortBy;
    public bool sortDescending;
    public int page;
    public int pageSize;
    public Dictionary<string, object> additionalFilters;
}

// Leaderboard Response
[System.Serializable]
public class LeaderboardResponse
{
    public bool success;
    public LeaderboardData data;
    public string error;
    public int totalCount;
    public int page;
    public int pageSize;
    public DateTime timestamp;
}
```

## Configuration

### LeaderboardConfig
```csharp
[System.Serializable]
public class LeaderboardConfig
{
    public int pageSize = 25;                    // Items per page
    public int maxPages = 10;                    // Maximum pages to cache
    public bool enableRealTimeUpdates = true;     // Enable Firebase real-time updates
    public bool enableOfflinePersistence = true;  // Enable offline functionality
    public bool enableSearch = true;              // Enable search functionality
    public bool enableFiltering = true;           // Enable filtering options
    public bool enableSorting = true;             // Enable sorting functionality
    public int updateInterval = 30;               // Update interval in seconds
    public int cacheExpiry = 300;                 // Cache expiry in seconds
    public bool enableAnimations = true;          // Enable UI animations
    public bool enablePagination = true;          // Enable pagination
    public bool enableUserProfiles = true;        // Enable user profile modals
}
```

### Firebase Configuration
```csharp
[System.Serializable]
public class FirebaseConfig
{
    public string databaseUrl = "https://your-project.firebaseio.com";
    public string leaderboardCollection = "leaderboards";
    public string userScoresCollection = "userScores";
    public string updatesCollection = "updates";
    public int maxRetries = 3;
    public float retryDelay = 1f;
    public bool enableOfflineSync = true;
}
```

## Deployment Steps

### 1. Pre-deployment Checklist
- [ ] Firebase project created and configured
- [ ] Database rules deployed
- [ ] Authentication methods enabled
- [ ] Unity project configured with Firebase SDK
- [ ] All dependencies installed
- [ ] Configuration files updated
- [ ] Unit tests passing (80%+ coverage)
- [ ] Integration tests passing
- [ ] Performance tests completed

### 2. Build Configuration
```csharp
// BuildSettings.cs
#if UNITY_ANDROID
    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
    PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
#endif

// Optimization settings
PlayerSettings.stripEngineCode = true;
PlayerSettings.stripUnusedMeshComponents = true;
PlayerSettings.mipStripping = true;
PlayerSettings.textureMipmapLimiting = true;
```

### 3. Production Build
```bash
# Build for Android
Unity -quit -batchmode -executeMethod BuildScript.BuildAndroid -logFile build.log

# Build for iOS (if applicable)
Unity -quit -batchmode -executeMethod BuildScript.BuildiOS -logFile build.log
```

### 4. Firebase Deployment
```bash
# Deploy database rules
firebase deploy --only database

# Deploy authentication settings
firebase deploy --only auth

# Deploy cloud functions (if any)
firebase deploy --only functions
```

### 5. Post-deployment Verification
```csharp
// PostDeploymentVerification.cs
using UnityEngine;
using LakbayTala.Leaderboard;

public class PostDeploymentVerification : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RunVerificationTests());
    }
    
    private IEnumerator RunVerificationTests()
    {
        // Test 1: Firebase connection
        yield return TestFirebaseConnection();
        
        // Test 2: Leaderboard loading
        yield return TestLeaderboardLoading();
        
        // Test 3: User authentication
        yield return TestUserAuthentication();
        
        // Test 4: Real-time updates
        yield return TestRealTimeUpdates();
        
        // Test 5: Offline functionality
        yield return TestOfflineFunctionality();
        
        Debug.Log("Post-deployment verification completed");
    }
    
    private IEnumerator TestFirebaseConnection()
    {
        var service = LeaderboardService.Instance;
        bool isOnline = service.IsOnline();
        
        if (isOnline)
        {
            Debug.Log("✓ Firebase connection established");
        }
        else
        {
            Debug.LogError("✗ Firebase connection failed");
        }
        
        yield return null;
    }
    
    private IEnumerator TestLeaderboardLoading()
    {
        var query = new SearchQuery
        {
            page = 1,
            pageSize = 10,
            sortBy = SortCriteria.Score
        };
        
        bool loadSuccess = false;
        yield return LeaderboardService.Instance.LoadLeaderboard(query, (response) =>
        {
            loadSuccess = response.success;
        });
        
        if (loadSuccess)
        {
            Debug.Log("✓ Leaderboard loading successful");
        }
        else
        {
            Debug.LogError("✗ Leaderboard loading failed");
        }
    }
    
    // Additional test methods...
}
```

## Monitoring & Maintenance

### 1. Performance Monitoring
```csharp
public class LeaderboardPerformanceMonitor : MonoBehaviour
{
    private PerformanceAnalytics analytics;
    
    void Start()
    {
        analytics = new PerformanceAnalytics();
        StartCoroutine(CollectMetrics());
    }
    
    private IEnumerator CollectMetrics()
    {
        while (true)
        {
            var metrics = LeaderboardService.Instance.GetPerformanceMetrics();
            
            // Log key metrics
            LogMetric("load_time", metrics.loadTime);
            LogMetric("memory_usage", metrics.memoryUsage);
            LogMetric("cache_hits", metrics.cacheHits);
            LogMetric("cache_misses", metrics.cacheMisses);
            
            yield return new WaitForSeconds(60); // Collect every minute
        }
    }
    
    private void LogMetric(string name, float value)
    {
        analytics.RecordMetric(name, value);
        
        // Send to Firebase Analytics
        // FirebaseAnalytics.LogEvent($"leaderboard_{name}", new Parameter(name, value));
    }
}
```

### 2. Error Tracking
```csharp
public class LeaderboardErrorTracker : MonoBehaviour
{
    private void OnEnable()
    {
        LeaderboardService.Instance.OnError += HandleError;
    }
    
    private void OnDisable()
    {
        LeaderboardService.Instance.OnError -= HandleError;
    }
    
    private void HandleError(string error)
    {
        // Log to console
        Debug.LogError($"Leaderboard Error: {error}");
        
        // Send to Firebase Crashlytics
        // FirebaseCrashlytics.LogException(new Exception(error));
        
        // Send to analytics
        // FirebaseAnalytics.LogEvent("leaderboard_error", new Parameter("error_message", error));
    }
}
```

### 3. Usage Analytics
```csharp
public class LeaderboardUsageAnalytics : MonoBehaviour
{
    public void TrackLeaderboardView()
    {
        // FirebaseAnalytics.LogEvent("leaderboard_view");
    }
    
    public void TrackUserProfileView(string userId)
    {
        // FirebaseAnalytics.LogEvent("user_profile_view", 
        //     new Parameter("user_id", userId));
    }
    
    public void TrackSearchUsage(string searchTerm)
    {
        // FirebaseAnalytics.LogEvent("leaderboard_search", 
        //     new Parameter("search_term", searchTerm));
    }
}
```

## Troubleshooting

### Common Issues and Solutions

#### 1. Firebase Connection Issues
**Problem**: "Failed to connect to Firebase"
**Solution**:
```csharp
// Check Firebase configuration
if (string.IsNullOrEmpty(leaderboardService.firebaseDatabaseUrl))
{
    Debug.LogError("Firebase URL not configured");
    return;
}

// Verify internet connection
if (Application.internetReachability == NetworkReachability.NotReachable)
{
    Debug.LogWarning("No internet connection available");
    // Switch to offline mode
    leaderboardService.enableOfflineMode = true;
}
```

#### 2. Performance Issues
**Problem**: "Leaderboard loading slowly"
**Solution**:
```csharp
// Optimize cache settings
leaderboardService.config.cacheExpiry = 300; // 5 minutes
leaderboardService.config.pageSize = 25; // Reasonable page size
leaderboardService.config.enableVirtualScrolling = true;

// Enable compression
leaderboardService.enableCompression = true;
```

#### 3. Memory Leaks
**Problem**: "Memory usage increasing over time"
**Solution**:
```csharp
// Implement proper cleanup
void OnDestroy()
{
    // Clear all event listeners
    leaderboardService.OnLeaderboardUpdate -= HandleUpdate;
    leaderboardService.OnError -= HandleError;
    
    // Clear caches
    leaderboardService.ClearCache();
    
    // Force garbage collection
    GC.Collect();
}
```

#### 4. Offline Sync Issues
**Problem**: "Offline data not syncing"
**Solution**:
```csharp
// Check sync status
if (!leaderboardService.IsOnline())
{
    Debug.Log("Currently offline - data will sync when connection restored");
    return;
}

// Force sync
leaderboardService.ForceSync();

// Monitor sync progress
leaderboardService.OnConnectionStatusChanged += (isOnline) =>
{
    if (isOnline)
    {
        Debug.Log("Connection restored - syncing offline data");
        leaderboardService.ForceSync();
    }
};
```

### Performance Debugging
```csharp
public class LeaderboardDebugger : MonoBehaviour
{
    [ContextMenu("Debug Performance")]
    private void DebugPerformance()
    {
        var metrics = LeaderboardService.Instance.GetPerformanceMetrics();
        
        Debug.Log($"Load Time: {metrics.loadTime:F2}s");
        Debug.Log($"Memory Usage: {metrics.memoryUsage}MB");
        Debug.Log($"Cache Hit Rate: {(float)metrics.cacheHits / (metrics.cacheHits + metrics.cacheMisses):P2}");
        Debug.Log($"Network Requests: {metrics.networkRequests}");
    }
    
    [ContextMenu("Debug Firebase Usage")]
    private void DebugFirebaseUsage()
    {
        // Check Firebase usage against Spark plan limits
        Debug.Log("Firebase Spark Plan Usage:");
        Debug.Log("- Database Reads: [Check Firebase Console]");
        Debug.Log("- Database Writes: [Check Firebase Console]");
        Debug.Log("- Storage Usage: [Check Firebase Console]");
        Debug.Log("- Bandwidth Usage: [Check Firebase Console]");
    }
}
```

## Support and Maintenance

### Regular Maintenance Tasks
1. **Weekly**: Check Firebase usage against Spark plan limits
2. **Monthly**: Review performance metrics and optimize as needed
3. **Quarterly**: Update Firebase SDK and dependencies
4. **Annually**: Review and update security rules

### Contact Information
- **Technical Support**: support@lakbaytala.com
- **Documentation**: https://docs.lakbaytala.com
- **Issue Tracker**: https://github.com/lakbaytala/leaderboard/issues

---

**Last Updated**: February 2025  
**Version**: 1.0.0  
**Compatibility**: Unity 2022.3 LTS+