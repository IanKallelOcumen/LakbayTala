using System;
using System.Collections.Generic;
using UnityEngine;

namespace LakbayTala.Data
{
    /// <summary>
    /// GDD 3.6: Save data for player progress and preferences.
    /// Unlocked locations/levels, collected lore, gallery unlocks, and basic settings.
    /// Stored as JSON in persistentDataPath; use LakbayTalaProgress to load/save.
    /// </summary>
    [Serializable]
    public class LakbayTalaSaveData
    {
        public List<string> unlockedLocationIds = new List<string>();
        public List<string> completedLevelIds = new List<string>();
        public List<string> collectedLoreIds = new List<string>();
        public List<string> galleryUnlockedIds = new List<string>();

        [Range(0f, 1f)] public float soundVolume = 0.8f;
        [Range(0f, 1f)] public float musicVolume = 0.7f;
        /// <summary>Override difficulty from config (e.g. "easy", "normal", "hard"). Empty = use config.</summary>
        public string difficultyOverride = "";

        /// <summary>Total Tala collected across all runs (optional).</summary>
        public int totalTalaCollected;
        /// <summary>Last played level id for "continue" or analytics.</summary>
        public string lastPlayedLevelId = "";
    }

    /// <summary>
    /// Loads and saves LakbayTalaSaveData to a JSON file. Apply to a persistent GameObject (e.g. with GameManager).
    /// Call Save() after unlocking, completing levels, or collecting lore so progress persists.
    /// </summary>
    public class LakbayTalaProgress : MonoBehaviour
    {
        public static LakbayTalaProgress Instance { get; private set; }

        const string SaveFileName = "lakbaytala_save.json";

        [Header("Optional: locations/levels unlocked by default (e.g. first location)")]
        public string[] defaultUnlockedLocationIds = new[] { LocationIds.Makiling };

        LakbayTalaSaveData _data;
        string _savePath;

        public LakbayTalaSaveData Data => _data ??= LoadFromDisk();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _savePath = System.IO.Path.Combine(Application.persistentDataPath, SaveFileName);
            _data = LoadFromDisk();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        LakbayTalaSaveData LoadFromDisk()
        {
            if (string.IsNullOrEmpty(_savePath))
                _savePath = System.IO.Path.Combine(Application.persistentDataPath, SaveFileName);

            if (System.IO.File.Exists(_savePath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(_savePath);
                    var loaded = JsonUtility.FromJson<LakbayTalaSaveData>(json);
                    if (loaded != null) return loaded;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("LakbayTala: Could not load save: " + e.Message);
                }
            }

            var fresh = new LakbayTalaSaveData();
            foreach (string id in defaultUnlockedLocationIds)
            {
                if (!string.IsNullOrEmpty(id) && !fresh.unlockedLocationIds.Contains(id))
                    fresh.unlockedLocationIds.Add(id);
            }
            return fresh;
        }

        /// <summary>Persist current data to disk. Call after progress changes.</summary>
        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(_data, true);
                System.IO.File.WriteAllText(_savePath, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("LakbayTala: Could not save: " + e.Message);
            }
        }

        public void UnlockLocation(string locationId)
        {
            if (string.IsNullOrEmpty(locationId)) return;
            if (!_data.unlockedLocationIds.Contains(locationId))
                _data.unlockedLocationIds.Add(locationId);
            Save();
        }

        public void CompleteLevel(string levelId)
        {
            if (string.IsNullOrEmpty(levelId)) return;
            if (!_data.completedLevelIds.Contains(levelId))
                _data.completedLevelIds.Add(levelId);
            _data.lastPlayedLevelId = levelId;
            Save();
        }

        public void CollectLore(string loreId)
        {
            if (string.IsNullOrEmpty(loreId)) return;
            if (!_data.collectedLoreIds.Contains(loreId))
                _data.collectedLoreIds.Add(loreId);
            Save();
        }

        public void UnlockGallery(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return;
            if (!_data.galleryUnlockedIds.Contains(entryId))
                _data.galleryUnlockedIds.Add(entryId);
            Save();
        }

        public void AddTala(int count)
        {
            _data.totalTalaCollected += count;
            Save();
        }

        public bool IsLocationUnlocked(string locationId) => _data.unlockedLocationIds.Contains(locationId);
        public bool IsLevelCompleted(string levelId) => _data.completedLevelIds.Contains(levelId);
        public bool HasLore(string loreId) => _data.collectedLoreIds.Contains(loreId);
        public bool IsGalleryUnlocked(string entryId) => _data.galleryUnlockedIds.Contains(entryId);
    }
}
