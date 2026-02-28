using UnityEngine;

namespace LakbayTala.Data
{
    /// <summary>
    /// GDD 3.6: Lore entry for game content data.
    /// Can be loaded from JSON or mapped from LoreData ScriptableObjects.
    /// </summary>
    [System.Serializable]
    public class LoreEntry
    {
        public string loreId;
        /// <summary>e.g. Makiling, Mohikap, Sampaloc, Botocan</summary>
        public string locationId;
        /// <summary>Optional: specific level this lore appears in.</summary>
        public string levelId;
        public string title;
        [TextArea(2, 5)]
        public string shortText;
        /// <summary>Sprite name or path (e.g. Resources path or addressable key).</summary>
        public string imageReference;
    }
}
