using UnityEngine;
using Platformer.Mechanics;
using LakbayTala.Lore.Data;

namespace LakbayTala.Lore
{
    /// <summary>
    /// Trigger that shows a Lore Card when the player enters. Place in the game world so players can find lore.
    /// Uses LoreCardController — assign your card panel and text/close button in the Inspector.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LoreMarker : MonoBehaviour
    {
        [Header("Lore content")]
        [Tooltip("ID matching a LoreData in LoreManager.allLoreData for full card UI; otherwise uses title/shortText below.")]
        public string loreId = "placeholder_lore";
        public string title = "Laguna Myth";
        [TextArea(2, 4)]
        public string shortText = "Add short cultural description here.";
        public Sprite image;

        [Tooltip("Only trigger once per game session.")]
        public bool triggerOnce = true;
        private bool triggered;

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null) return;
            if (triggerOnce && triggered) return;
            triggered = true;

            LoreData data = null;
            if (LoreManager.Instance != null && LoreManager.Instance.allLoreData != null)
            {
                foreach (var d in LoreManager.Instance.allLoreData)
                {
                    if (d != null && d.id == loreId) { data = d; break; }
                }
            }

            if (data != null && LoreCardController.Instance != null)
            {
                LoreCardController.Instance.Show(loreId, data.title, data.shortDescription ?? data.fullDescription ?? shortText, data.illustration ?? image);
                return;
            }

            if (LoreCardController.Instance != null)
                LoreCardController.Instance.Show(loreId, title, shortText, image);
        }
    }
}
