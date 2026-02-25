using UnityEngine;
using System.Collections.Generic;

namespace LakbayTala.Gallery
{
    /// <summary>
    /// Placeholder: Unlockable gallery of Lore Cards / mythical creatures.
    /// Unlock entries when player collects lore or completes levels.
    /// </summary>
    public class GalleryController : MonoBehaviour
    {
        [Header("Placeholder – wire list in Inspector")]
        public List<string> unlockedLoreIds = new List<string>();

        public static GalleryController Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Unlock(string loreId)
        {
            if (string.IsNullOrEmpty(loreId) || unlockedLoreIds.Contains(loreId)) return;
            unlockedLoreIds.Add(loreId);
            Debug.Log("[Lakbay Tala] Gallery placeholder: unlocked " + loreId);
        }

        public bool IsUnlocked(string loreId)
        {
            return unlockedLoreIds.Contains(loreId);
        }
    }
}
