using System.Collections.Generic;
using UnityEngine;

namespace LakbayTala.Data
{
    /// <summary>
    /// GDD 3.6: Registry of levels. Assign levels in Inspector or create via code; use for world map and level flow.
    /// Apply to a persistent GameObject or use as a ScriptableObject asset.
    /// </summary>
    public class LevelRegistry : MonoBehaviour
    {
        public static LevelRegistry Instance { get; private set; }

        [Tooltip("All playable levels. Add LevelData (e.g. from inspector or load from JSON).")]
        public List<LevelData> levels = new List<LevelData>();

        Dictionary<string, LevelData> _byId;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildLookup();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void BuildLookup()
        {
            _byId = new Dictionary<string, LevelData>();
            if (levels == null) return;
            foreach (var level in levels)
            {
                if (level != null && !string.IsNullOrEmpty(level.levelId) && !_byId.ContainsKey(level.levelId))
                    _byId[level.levelId] = level;
            }
        }

        /// <summary>Get level by id. Returns null if not found.</summary>
        public LevelData GetLevel(string levelId)
        {
            if (_byId == null) BuildLookup();
            return _byId != null && _byId.TryGetValue(levelId, out var level) ? level : null;
        }

        /// <summary>All levels for a location (e.g. Makiling, Mohikap).</summary>
        public List<LevelData> GetLevelsForLocation(string locationId)
        {
            if (levels == null) return new List<LevelData>();
            var list = new List<LevelData>();
            foreach (var level in levels)
                if (level != null && level.locationId == locationId)
                    list.Add(level);
            return list;
        }

        /// <summary>All levels. Rebuild lookup if levels list was modified at runtime.</summary>
        public IReadOnlyList<LevelData> GetAllLevels()
        {
            return levels ?? (IReadOnlyList<LevelData>)new List<LevelData>();
        }

        /// <summary>Call after adding/removing levels at runtime.</summary>
        public void RebuildLookup()
        {
            BuildLookup();
        }
    }
}
