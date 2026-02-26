using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

/// <summary>
/// Comprehensive leaderboard data models and structures for LakbayTala.
/// Supports real-time updates, offline persistence, and advanced filtering/sorting.
/// </summary>
namespace LakbayTala.Leaderboard
{
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
        
        public LeaderboardUser()
        {
            culturalScores = new Dictionary<string, int>();
            scoreHistory = new List<ScoreHistory>();
            profile = new UserProfile();
        }
        
        public int GetRankChange()
        {
            return previousRank - currentRank;
        }
        
        public bool HasRankImproved()
        {
            return GetRankChange() > 0;
        }
        
        public string GetRankChangeText()
        {
            int change = GetRankChange();
            if (change > 0) return $"+{change}";
            if (change < 0) return change.ToString();
            return "-";
        }
    }
    
    [System.Serializable]
    public class ScoreHistory
    {
        public int score;
        public DateTime timestamp;
        public string gameMode;
        public string levelName;
        public float completionTime;
        public Dictionary<string, object> metadata;
        
        public ScoreHistory()
        {
            metadata = new Dictionary<string, object>();
        }
    }
    
    [System.Serializable]
    public class UserProfile
    {
        public string bio;
        public string favoriteCreature;
        public int gamesPlayed;
        public int totalPlayTime;
        public DateTime joinedDate;
        public List<string> achievements;
        public Dictionary<string, int> culturalKnowledge;
        
        public UserProfile()
        {
            achievements = new List<string>();
            culturalKnowledge = new Dictionary<string, int>();
        }
    }
    
    [System.Serializable]
    public class LeaderboardEntry
    {
        public LeaderboardUser user;
        public int rank;
        public int score;
        public DateTime lastUpdated;
        public bool isCurrentUser;
        public Dictionary<string, object> additionalData;
        
        public LeaderboardEntry()
        {
            additionalData = new Dictionary<string, object>();
        }
    }
    
    [System.Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries;
        public int totalUsers;
        public DateTime lastUpdated;
        public string leaderboardId;
        public LeaderboardType type;
        public LeaderboardTimeFrame timeFrame;
        public Dictionary<string, object> metadata;
        
        public LeaderboardData()
        {
            entries = new List<LeaderboardEntry>();
            metadata = new Dictionary<string, object>();
        }
    }
    
    public enum LeaderboardType
    {
        Global,
        Friends,
        Country,
        Cultural,
        Weekly,
        Monthly,
        AllTime
    }
    
    public enum LeaderboardTimeFrame
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
        AllTime
    }
    
    public enum SortCriteria
    {
        Score,
        Rank,
        Name,
        Country,
        CulturalLevel,
        RecentActivity,
        PlayTime
    }
    
    public enum FilterCriteria
    {
        All,
        Friends,
        Country,
        CulturalLevel,
        OnlineOnly,
        NewPlayers
    }
    
    [System.Serializable]
    public class LeaderboardConfig
    {
        public int pageSize = 25;
        public int maxPages = 10;
        public bool enableRealTimeUpdates = true;
        public bool enableOfflinePersistence = true;
        public bool enableSearch = true;
        public bool enableFiltering = true;
        public bool enableSorting = true;
        public int updateInterval = 30; // seconds
        public int cacheExpiry = 300; // seconds
        public bool enableAnimations = true;
        public bool enablePagination = true;
        public bool enableUserProfiles = true;
    }
    
    [System.Serializable]
    public class LeaderboardUpdate
    {
        public string updateId;
        public UpdateType type;
        public string userId;
        public int oldScore;
        public int newScore;
        public int oldRank;
        public int newRank;
        public DateTime timestamp;
        public Dictionary<string, object> additionalData;
        
        public LeaderboardUpdate()
        {
            updateId = Guid.NewGuid().ToString();
            timestamp = DateTime.Now;
            additionalData = new Dictionary<string, object>();
        }
    }
    
    public enum UpdateType
    {
        ScoreUpdate,
        RankChange,
        NewEntry,
        UserRemoved,
        UserAdded
    }
    
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
        
        public SearchQuery()
        {
            additionalFilters = new Dictionary<string, object>();
        }
    }
    
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
        
        public static LeaderboardResponse CreateSuccess(LeaderboardData data, int totalCount, int page, int pageSize)
        {
            return new LeaderboardResponse
            {
                success = true,
                data = data,
                totalCount = totalCount,
                page = page,
                pageSize = pageSize,
                timestamp = DateTime.Now
            };
        }
        
        public static LeaderboardResponse CreateError(string error)
        {
            return new LeaderboardResponse
            {
                success = false,
                error = error,
                timestamp = DateTime.Now
            };
        }
    }
    
    [System.Serializable]
    public class OfflineData
    {
        public List<LeaderboardEntry> cachedEntries;
        public DateTime lastSync;
        public List<PendingUpdate> pendingUpdates;
        public Dictionary<string, object> metadata;
        
        public OfflineData()
        {
            cachedEntries = new List<LeaderboardEntry>();
            pendingUpdates = new List<PendingUpdate>();
            metadata = new Dictionary<string, object>();
        }
    }
    
    [System.Serializable]
    public class PendingUpdate
    {
        public string updateId;
        public string userId;
        public int newScore;
        public DateTime timestamp;
        public UpdateType type;
        public int retryCount;
        public DateTime lastRetry;
        
        public PendingUpdate()
        {
            updateId = Guid.NewGuid().ToString();
            timestamp = DateTime.Now;
        }
    }
    
    [System.Serializable]
    public class PerformanceMetrics
    {
        public float loadTime;
        public int renderTime;
        public int updateLatency;
        public int memoryUsage;
        public int networkRequests;
        public int cacheHits;
        public int cacheMisses;
        public DateTime timestamp;
        
        public PerformanceMetrics()
        {
            timestamp = DateTime.Now;
        }
    }
    
    [System.Serializable]
    public class ErrorLog
    {
        public string errorId;
        public string errorType;
        public string message;
        public string stackTrace;
        public DateTime timestamp;
        public Dictionary<string, object> context;
        
        public ErrorLog()
        {
            errorId = Guid.NewGuid().ToString();
            timestamp = DateTime.Now;
            context = new Dictionary<string, object>();
        }
    }
}