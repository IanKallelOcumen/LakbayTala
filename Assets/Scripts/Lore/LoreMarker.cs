using UnityEngine;
using Platformer.Mechanics;
using LakbayTala.Lore.Data;
using LakbayTala.UI.Lore;

namespace LakbayTala.Lore
{
    /// <summary>
    /// Trigger that shows a Lore Card when the player enters. Place in the game world so players can find lore.
    /// If LoreManager has LoreData for loreId, uses LoreCardUI (full card with reveal). Otherwise uses LoreCardController (simple panel).
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

            if (data != null)
            {
                var cardUI = Object.FindFirstObjectByType<LoreCardUI>();
                if (cardUI != null)
                {
                    cardUI.DisplayLore(data);
                    return;
                }
            }

            if (LoreCardController.Instance != null)
                LoreCardController.Instance.Show(loreId, title, shortText, image);
        }
    }
}
