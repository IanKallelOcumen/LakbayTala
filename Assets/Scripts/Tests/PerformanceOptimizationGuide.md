# LakbayTala Leaderboard System - Performance Tests & Optimization Guide

## Overview
This document provides comprehensive performance testing procedures and optimization strategies for the LakbayTala leaderboard system, ensuring optimal performance with large datasets and Firebase Spark plan limitations.

## Performance Test Suite

### 1. Load Testing
Tests system behavior under various load conditions:

```csharp
[Test]
public void PerformanceTest_LargeDataset_10KUsers()
{
    // Test with 10,000 users
    var largeDataset = GenerateTestUsers(10000);
    var stopwatch = Stopwatch.StartNew();
    
    var result = ProcessLeaderboardData(largeDataset);
    
    stopwatch.Stop();
    Assert.Less(stopwatch.ElapsedMilliseconds, 2000, 
        "10K users processed in under 2 seconds");
}

[Test]
public void PerformanceTest_MemoryUsage_StayWithinLimits()
{
    var initialMemory = GC.GetTotalMemory(false);
    
    // Process large dataset
    ProcessLargeLeaderboard();
    
    var finalMemory = GC.GetTotalMemory(true);
    var memoryIncrease = finalMemory - initialMemory;
    
    Assert.Less(memoryIncrease, 100 * 1024 * 1024, 
        "Memory increase under 100MB");
}
```

### 2. Real-time Update Performance
Tests real-time update handling efficiency:

```csharp
[UnityTest]
public IEnumerator PerformanceTest_RealTimeUpdates_100PerSecond()
{
    int updatesProcessed = 0;
    int targetUpdates = 100;
    float testDuration = 1f;
    
    // Setup update handler
    leaderboardService.OnLeaderboardUpdate += (update) =>
    {
        updatesProcessed++;
    };
    
    // Generate updates
    float startTime = Time.time;
    while (Time.time - startTime < testDuration)
    {
        yield return leaderboardService.UpdateUserScore($"user_{updatesProcessed}", 1000);
        yield return null; // Process frame
    }
    
    Assert.GreaterOrEqual(updatesProcessed, targetUpdates * 0.9f, 
        "90% of target updates processed");
}
```

### 3. Search Performance
Tests search functionality with various query complexities:

```csharp
[Test]
public void PerformanceTest_ComplexSearch_1000QueriesPerSecond()
{
    var queries = GenerateComplexSearchQueries(1000);
    var stopwatch = Stopwatch.StartNew();
    
    foreach (var query in queries)
    {
        var result = ExecuteSearch(query);
    }
    
    stopwatch.Stop();
    float queriesPerSecond = 1000f / (stopwatch.ElapsedMilliseconds / 1000f);
    
    Assert.Greater(queriesPerSecond, 800f, 
        "Search performance over 800 queries/second");
}
```

## Firebase Spark Plan Optimization

### 1. Concurrent Connections Limit (100)
```csharp
public class ConnectionPoolManager
{
    private const int MAX_CONNECTIONS = 90; // 90% of limit for safety
    private int activeConnections = 0;
    
    public bool TryAcquireConnection()
    {
        if (activeConnections < MAX_CONNECTIONS)
        {
            activeConnections++;
            return true;
        }
        return false;
    }
    
    public void ReleaseConnection()
    {
        activeConnections = Math.Max(0, activeConnections - 1);
    }
}
```

### 2. Daily Limits Optimization

#### Database Read/Write Limits (50,000/day)
```csharp
public class FirebaseUsageTracker
{
    private int dailyReads = 0;
    private int dailyWrites = 0;
    private DateTime lastReset = DateTime.Now.Date;
    
    public bool CanPerformRead()
    {
        ResetIfNewDay();
        return dailyReads < 45000; // 90% of limit
    }
    
    public bool CanPerformWrite()
    {
        ResetIfNewDay();
        return dailyWrites < 45000; // 90% of limit
    }
    
    private void ResetIfNewDay()
    {
        if (DateTime.Now.Date > lastReset)
        {
            dailyReads = 0;
            dailyWrites = 0;
            lastReset = DateTime.Now.Date;
        }
    }
}
```

#### Storage Quota (1GB)
```csharp
public class StorageOptimizer
{
    private const long MAX_STORAGE_BYTES = 1024L * 1024L * 1024L; // 1GB
    private const long SAFETY_LIMIT = 900L * 1024L * 1024L; // 900MB
    
    public bool HasStorageSpace(long requiredBytes)
    {
        var currentUsage = GetCurrentStorageUsage();
        return currentUsage + requiredBytes < SAFETY_LIMIT;
    }
    
    private long GetCurrentStorageUsage()
    {
        // Implementation to check actual storage usage
        return 0; // Placeholder
    }
}
```

### 3. Bandwidth Optimization (10GB/month)
```csharp
public class BandwidthOptimizer
{
    private const long MONTHLY_BANDWIDTH_LIMIT = 10L * 1024L * 1024L * 1024L; // 10GB
    private const long DAILY_BANDWIDTH_LIMIT = MONTHLY_BANDWIDTH_LIMIT / 30; // ~341MB/day
    private long dailyBandwidthUsed = 0;
    
    public bool CanTransferData(long bytes)
    {
        return dailyBandwidthUsed + bytes < DAILY_BANDWIDTH_LIMIT * 0.9f; // 90% safety
    }
    
    public void RecordDataTransfer(long bytes)
    {
        dailyBandwidthUsed += bytes;
    }
}
```

## Caching Strategies

### 1. Multi-level Caching
```csharp
public class LeaderboardCacheManager
{
    // Level 1: Memory cache (fastest)
    private readonly Dictionary<string, LeaderboardData> memoryCache = new Dictionary<string, LeaderboardData>();
    
    // Level 2: PlayerPrefs cache (persistent)
    private const string PLAYERPREFS_CACHE_KEY = "LeaderboardCache_";
    
    // Level 3: Firebase cache (cloud)
    private const string FIREBASE_CACHE_KEY = "cached_leaderboard_";
    
    public LeaderboardData GetCachedData(string cacheKey)
    {
        // Check memory cache first
        if (memoryCache.TryGetValue(cacheKey, out var data))
        {
            if (IsCacheValid(data))
                return data;
        }
        
        // Check PlayerPrefs cache
        data = GetPlayerPrefsCache(cacheKey);
        if (data != null && IsCacheValid(data))
        {
            memoryCache[cacheKey] = data; // Promote to memory cache
            return data;
        }
        
        return null;
    }
    
    private bool IsCacheValid(LeaderboardData data)
    {
        return (DateTime.Now - data.lastUpdated).TotalMinutes < 5; // 5-minute cache
    }
}
```

### 2. Smart Cache Invalidation
```csharp
public class CacheInvalidationManager
{
    private readonly HashSet<string> invalidatedKeys = new HashSet<string>();
    
    public void InvalidateUserScore(string userId)
    {
        // Invalidate user-specific caches
        invalidatedKeys.Add($"user_{userId}");
        invalidatedKeys.Add($"leaderboard_global");
        invalidatedKeys.Add($"leaderboard_friends_{userId}");
    }
    
    public bool ShouldUseCache(string cacheKey)
    {
        if (invalidatedKeys.Contains(cacheKey))
        {
            invalidatedKeys.Remove(cacheKey); // Remove after checking
            return false;
        }
        return true;
    }
}
```

## Data Compression Techniques

### 1. JSON Compression
```csharp
public class DataCompressor
{
    public static string CompressLeaderboardData(LeaderboardData data)
    {
        var json = JsonConvert.SerializeObject(data);
        return CompressString(json);
    }
    
    private static string CompressString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
    
    public static LeaderboardData DecompressLeaderboardData(string compressedData)
    {
        var json = DecompressString(compressedData);
        return JsonConvert.DeserializeObject<LeaderboardData>(json);
    }
}
```

### 2. Delta Updates
```csharp
public class DeltaUpdateManager
{
    public class LeaderboardDelta
    {
        public List<ScoreChange> ScoreChanges { get; set; } = new List<ScoreChange>();
        public List<RankChange> RankChanges { get; set; } = new List<RankChange>();
        public DateTime Timestamp { get; set; }
    }
    
    public LeaderboardDelta CalculateDelta(LeaderboardData previous, LeaderboardData current)
    {
        var delta = new LeaderboardDelta
        {
            Timestamp = DateTime.Now
        };
        
        // Calculate score changes
        foreach (var currentEntry in current.entries)
        {
            var previousEntry = previous.entries.FirstOrDefault(e => e.user.userId == currentEntry.user.userId);
            if (previousEntry != null && previousEntry.score != currentEntry.score)
            {
                delta.ScoreChanges.Add(new ScoreChange
                {
                    UserId = currentEntry.user.userId,
                    OldScore = previousEntry.score,
                    NewScore = currentEntry.score
                });
            }
        }
        
        return delta;
    }
}
```

## Virtual Scrolling Implementation

### 1. Virtual Scrolling Manager
```csharp
public class VirtualScrollingManager
{
    private const int VISIBLE_ITEM_COUNT = 10;
    private const int BUFFER_ITEM_COUNT = 5;
    private readonly List<LeaderboardEntry> allData = new List<LeaderboardEntry>();
    private readonly List<LeaderboardEntry> visibleData = new List<LeaderboardEntry>();
    
    public void UpdateVisibleItems(float scrollPosition)
    {
        int totalItems = allData.Count;
        int itemHeight = 100; // pixels per item
        
        int firstVisibleIndex = Mathf.FloorToInt(scrollPosition / itemHeight);
        int lastVisibleIndex = firstVisibleIndex + VISIBLE_ITEM_COUNT;
        
        // Add buffer items
        firstVisibleIndex = Mathf.Max(0, firstVisibleIndex - BUFFER_ITEM_COUNT);
        lastVisibleIndex = Mathf.Min(totalItems - 1, lastVisibleIndex + BUFFER_ITEM_COUNT);
        
        // Update visible data
        visibleData.Clear();
        for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
        {
            visibleData.Add(allData[i]);
        }
    }
    
    public List<LeaderboardEntry> GetVisibleItems()
    {
        return new List<LeaderboardEntry>(visibleData);
    }
}
```

## Memory Management

### 1. Object Pooling
```csharp
public class LeaderboardEntryPool
{
    private readonly Queue<GameObject> availableEntries = new Queue<GameObject>();
    private readonly List<GameObject> activeEntries = new List<GameObject>();
    private readonly GameObject prefab;
    private readonly Transform parent;
    
    public LeaderboardEntryPool(GameObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }
    
    public GameObject GetEntry()
    {
        GameObject entry;
        
        if (availableEntries.Count > 0)
        {
            entry = availableEntries.Dequeue();
        }
        else
        {
            entry = GameObject.Instantiate(prefab, parent);
        }
        
        entry.SetActive(true);
        activeEntries.Add(entry);
        return entry;
    }
    
    public void ReturnEntry(GameObject entry)
    {
        entry.SetActive(false);
        activeEntries.Remove(entry);
        availableEntries.Enqueue(entry);
    }
    
    public void ClearPool()
    {
        foreach (var entry in activeEntries)
        {
            entry.SetActive(false);
            availableEntries.Enqueue(entry);
        }
        activeEntries.Clear();
    }
}
```

### 2. Memory Monitoring
```csharp
public class MemoryMonitor
{
    private long lastMemoryUsage = 0;
    private readonly Queue<long> memoryHistory = new Queue<long>();
    private const int HISTORY_SIZE = 10;
    
    public void LogMemoryUsage()
    {
        var currentMemory = GC.GetTotalMemory(false);
        memoryHistory.Enqueue(currentMemory);
        
        if (memoryHistory.Count > HISTORY_SIZE)
            memoryHistory.Dequeue();
        
        var averageMemory = memoryHistory.Average();
        
        if (currentMemory > averageMemory * 1.5f) // 50% spike
        {
            Debug.LogWarning($"Memory spike detected: {currentMemory / 1024 / 1024}MB");
            TriggerGarbageCollection();
        }
        
        lastMemoryUsage = currentMemory;
    }
    
    private void TriggerGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
```

## Load Balancing Strategies

### 1. Request Batching
```csharp
public class RequestBatcher
{
    private readonly Queue<PendingUpdate> pendingUpdates = new Queue<PendingUpdate>();
    private const int BATCH_SIZE = 10;
    private const float BATCH_INTERVAL = 1f; // seconds
    
    public void AddUpdate(PendingUpdate update)
    {
        pendingUpdates.Enqueue(update);
        
        if (pendingUpdates.Count >= BATCH_SIZE)
        {
            ProcessBatch();
        }
    }
    
    private void ProcessBatch()
    {
        var batch = new List<PendingUpdate>();
        
        while (pendingUpdates.Count > 0 && batch.Count < BATCH_SIZE)
        {
            batch.Add(pendingUpdates.Dequeue());
        }
        
        SendBatchToFirebase(batch);
    }
    
    private void SendBatchToFirebase(List<PendingUpdate> batch)
    {
        // Implement batch API call to Firebase
        // This reduces network overhead significantly
    }
}
```

## Monitoring and Analytics

### 1. Performance Metrics Collection
```csharp
public class PerformanceAnalytics
{
    private readonly Dictionary<string, List<float>> metrics = new Dictionary<string, List<float>>();
    
    public void RecordMetric(string metricName, float value)
    {
        if (!metrics.ContainsKey(metricName))
            metrics[metricName] = new List<float>();
        
        metrics[metricName].Add(value);
        
        // Keep only last 100 values
        if (metrics[metricName].Count > 100)
            metrics[metricName].RemoveAt(0);
    }
    
    public PerformanceReport GenerateReport()
    {
        var report = new PerformanceReport();
        
        foreach (var metric in metrics)
        {
            var values = metric.Value;
            if (values.Count > 0)
            {
                report.Metrics[metric.Key] = new MetricData
                {
                    Average = values.Average(),
                    Min = values.Min(),
                    Max = values.Max(),
                    Count = values.Count
                };
            }
        }
        
        return report;
    }
}
```

### 2. Firebase Analytics Integration
```csharp
public class FirebaseAnalyticsIntegration
{
    public void TrackLeaderboardEvent(string eventName, Dictionary<string, object> parameters)
    {
        // Firebase Analytics event tracking
        // This helps monitor real-world usage patterns
        
        var analyticsEvent = new Dictionary<string, object>
        {
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["user_count"] = GetActiveUserCount(),
            ["performance_score"] = GetPerformanceScore()
        };
        
        // Merge with provided parameters
        foreach (var param in parameters)
        {
            analyticsEvent[param.Key] = param.Value;
        }
        
        // Send to Firebase Analytics
        // FirebaseAnalytics.LogEvent(eventName, analyticsEvent);
    }
}
```

## Best Practices Summary

### 1. **Caching Strategy**
- Implement 3-level caching (Memory → PlayerPrefs → Firebase)
- Use cache invalidation for data consistency
- Set appropriate cache expiry times

### 2. **Firebase Optimization**
- Stay within Spark plan limits (90% safety margin)
- Use connection pooling and request batching
- Implement delta updates to reduce data transfer

### 3. **Performance Monitoring**
- Track key metrics (load time, memory usage, network requests)
- Set up alerts for performance degradation
- Regular performance testing with realistic data volumes

### 4. **Memory Management**
- Use object pooling for UI elements
- Implement proper cleanup in OnDestroy()
- Monitor memory usage and trigger garbage collection when needed

### 5. **Error Handling**
- Implement retry mechanisms with exponential backoff
- Handle offline scenarios gracefully
- Provide meaningful error messages to users

This optimization guide ensures the LakbayTala leaderboard system performs efficiently while staying within Firebase Spark plan limitations and providing an excellent user experience.