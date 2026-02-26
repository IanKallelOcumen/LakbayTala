using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LakbayTala.Lore.Data;

namespace LakbayTala.Lore
{
    public class LoreManager : MonoBehaviour
    {
        public static LoreManager Instance { get; private set; }

        [Header("Configuration")]
        public List<LoreData> allLoreData;

        private HashSet<string> unlockedLoreIds = new HashSet<string>();
        private const string PREFS_KEY_PREFIX = "LORE_UNLOCKED_";

        public event System.Action<LoreData> OnLoreUnlocked;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadProgress();
        }

        private void LoadProgress()
        {
            unlockedLoreIds.Clear();
            foreach (var lore in allLoreData)
            {
                if (lore.unlockedByDefault)
                {
                    unlockedLoreIds.Add(lore.id);
                }
                else if (PlayerPrefs.GetInt(PREFS_KEY_PREFIX + lore.id, 0) == 1)
                {
                    unlockedLoreIds.Add(lore.id);
                }
            }
        }

        public void UnlockLore(string loreId)
        {
            if (string.IsNullOrEmpty(loreId)) return;

            var lore = allLoreData.FirstOrDefault(l => l.id == loreId);
            if (lore == null)
            {
                Debug.LogWarning($"Lore ID {loreId} not found in database.");
                return;
            }

            if (!unlockedLoreIds.Contains(loreId))
            {
                unlockedLoreIds.Add(loreId);
                PlayerPrefs.SetInt(PREFS_KEY_PREFIX + loreId, 1);
                PlayerPrefs.Save();
                
                OnLoreUnlocked?.Invoke(lore);
                Debug.Log($"Lore unlocked: {lore.title}");
                
                // Show notification if UI is ready
                // UIManager.Instance?.ShowNotification(...) 
            }
        }

        public bool IsLoreUnlocked(string loreId)
        {
            return unlockedLoreIds.Contains(loreId);
        }

        public List<LoreData> GetUnlockedLore()
        {
            return allLoreData.Where(l => unlockedByDefault(l) || unlockedLoreIds.Contains(l.id)).ToList();
        }
        
        public List<LoreData> GetLoreByCategory(LoreCategory category)
        {
            return allLoreData.Where(l => l.category == category).ToList();
        }

        private bool unlockedByDefault(LoreData lore)
        {
            return lore.unlockedByDefault;
        }

        public void ResetProgress()
        {
            foreach (var lore in allLoreData)
            {
                PlayerPrefs.DeleteKey(PREFS_KEY_PREFIX + lore.id);
            }
            unlockedLoreIds.Clear();
            LoadProgress();
        }
    }
}
