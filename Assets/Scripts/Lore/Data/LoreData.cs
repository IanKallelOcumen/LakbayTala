using UnityEngine;

namespace LakbayTala.Lore.Data
{
    [CreateAssetMenu(fileName = "New Lore Data", menuName = "LakbayTala/Lore/Lore Data")]
    public class LoreData : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public LoreCategory category;

        [Header("Content")]
        public string title;
        [TextArea(3, 5)]
        public string shortDescription;
        [TextArea(5, 10)]
        public string fullDescription;
        public Sprite illustration;
        public Sprite background; // Optional specific background

        [Header("Unlock Condition")]
        public bool unlockedByDefault = false;
        // Could add requirements here, but logic is better in LoreManager or GameController
    }
}
