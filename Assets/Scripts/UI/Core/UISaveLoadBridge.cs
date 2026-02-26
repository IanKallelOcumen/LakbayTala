using UnityEngine;
using System;
using System.Collections.Generic;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Bridge for persisting UI-related data: preferences, last menu tab, unlocked lore IDs.
    /// Integrates with game save/load (local or cloud); implement IUIProgressProvider to supply data.
    /// </summary>
    public class UISaveLoadBridge : MonoBehaviour
    {
        public static UISaveLoadBridge Instance { get; private set; }

        [Header("Storage")]
        [Tooltip("Key prefix for UI keys in PlayerPrefs when using local fallback.")]
        public string playerPrefsPrefix = "LakbayTala_UI_";

        private IUIProgressProvider provider;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetProvider(IUIProgressProvider p) => provider = p;

        public void SaveUIProgress()
        {
            try
            {
                if (provider != null)
                {
                    provider.SaveUIProgress(CollectProgress());
                    return;
                }
                SaveToPlayerPrefs(CollectProgress());
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UISaveLoadBridge] Save failed: {e.Message}");
            }
        }

        public void LoadUIProgress()
        {
            try
            {
                UIProgressData data = null;
                if (provider != null)
                    data = provider.LoadUIProgress();
                if (data == null)
                    data = LoadFromPlayerPrefs();
                ApplyProgress(data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UISaveLoadBridge] Load failed: {e.Message}");
            }
        }

        private UIProgressData CollectProgress()
        {
            var data = new UIProgressData();
            if (UIStateManager.Instance != null)
            {
                data.lastPanelType = (int)UIStateManager.Instance.GetCurrentPanelType();
                data.lastMenuSubId = UIStateManager.Instance.lastMenuSubId ?? "";
            }
            data.unlockedLoreIds = GetUnlockedLoreIdsSnapshot();
            return data;
        }

        private void ApplyProgress(UIProgressData data)
        {
            if (data == null) return;
            if (UIStateManager.Instance != null && !string.IsNullOrEmpty(data.lastMenuSubId))
                UIStateManager.Instance.SetLastMenuSubId(data.lastMenuSubId);
            ApplyUnlockedLoreIds(data.unlockedLoreIds);
        }

        private List<string> GetUnlockedLoreIdsSnapshot()
        {
            var list = new List<string>();
            var manager = FindFirstByType<LakbayTala.Lore.LoreManager>();
            if (manager != null)
            {
                var unlocked = manager.GetUnlockedLore();
                if (unlocked != null)
                {
                    foreach (var lore in unlocked)
                        if (lore != null && !string.IsNullOrEmpty(lore.id))
                            list.Add(lore.id);
                }
            }
            return list;
        }

        private void ApplyUnlockedLoreIds(List<string> ids)
        {
            if (ids == null) return;
            var manager = FindFirstByType<LakbayTala.Lore.LoreManager>();
            if (manager != null)
            {
                foreach (var id in ids)
                    if (!string.IsNullOrEmpty(id))
                        manager.UnlockLore(id);
            }
        }

        private void SaveToPlayerPrefs(UIProgressData data)
        {
            if (data == null) return;
            PlayerPrefs.SetInt(playerPrefsPrefix + "LastPanel", data.lastPanelType);
            PlayerPrefs.SetString(playerPrefsPrefix + "LastMenuSubId", data.lastMenuSubId ?? "");
            if (data.unlockedLoreIds != null)
            {
                for (int i = 0; i < data.unlockedLoreIds.Count; i++)
                    PlayerPrefs.SetString(playerPrefsPrefix + "Lore_" + i, data.unlockedLoreIds[i]);
                PlayerPrefs.SetInt(playerPrefsPrefix + "LoreCount", data.unlockedLoreIds.Count);
            }
            PlayerPrefs.Save();
        }

        private UIProgressData LoadFromPlayerPrefs()
        {
            var data = new UIProgressData();
            data.lastPanelType = PlayerPrefs.GetInt(playerPrefsPrefix + "LastPanel", 0);
            data.lastMenuSubId = PlayerPrefs.GetString(playerPrefsPrefix + "LastMenuSubId", "");
            int count = PlayerPrefs.GetInt(playerPrefsPrefix + "LoreCount", 0);
            data.unlockedLoreIds = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string id = PlayerPrefs.GetString(playerPrefsPrefix + "Lore_" + i, "");
                if (!string.IsNullOrEmpty(id)) data.unlockedLoreIds.Add(id);
            }
            return data;
        }

        private static T FindFirstByType<T>() where T : UnityEngine.Object
        {
            return UnityEngine.Object.FindFirstObjectByType<T>();
        }

        [Serializable]
        public class UIProgressData
        {
            public int lastPanelType;
            public string lastMenuSubId;
            public List<string> unlockedLoreIds;
        }
    }

    public interface IUIProgressProvider
    {
        void SaveUIProgress(UISaveLoadBridge.UIProgressData data);
        UISaveLoadBridge.UIProgressData LoadUIProgress();
    }
}
