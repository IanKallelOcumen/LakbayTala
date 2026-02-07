using UnityEngine;
using Platformer.Mechanics;

namespace LakbayTala.Lore
{
    /// <summary>
    /// Placeholder: Trigger that shows a Lore Card when the player enters.
    /// Assign loreId, title, shortText (and optional sprite) and ensure the player has a Collider2D (trigger).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LoreMarker : MonoBehaviour
    {
        [Header("Lore content (placeholder)")]
        public string loreId = "placeholder_lore";
        public string title = "Laguna Myth";
        [TextArea(2, 4)]
        public string shortText = "Add short cultural description here.";
        public Sprite image;

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null) return;
            if (LoreCardController.Instance != null)
                LoreCardController.Instance.Show(loreId, title, shortText, image);
        }
    }
}
