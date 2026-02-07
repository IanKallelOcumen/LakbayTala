using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LakbayTala.Lore
{
    /// <summary>
    /// Placeholder: Controls the Lore Card overlay (myth/location text + close).
    /// Hook up to a Canvas with image, title, body text, and Close button.
    /// </summary>
    public class LoreCardController : MonoBehaviour
    {
        [Header("Placeholder – wire in Inspector")]
        public GameObject cardPanel;
        public Image illustration;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI bodyText;
        public Button closeButton;

        public static LoreCardController Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (cardPanel != null) cardPanel.SetActive(false);
            if (closeButton != null) closeButton.onClick.AddListener(Hide);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Show a lore card. Placeholder: logs and shows panel if assigned.</summary>
        public void Show(string loreId, string title, string shortText, Sprite image = null)
        {
            if (titleText != null) titleText.text = title;
            if (bodyText != null) bodyText.text = shortText;
            if (illustration != null && image != null) illustration.sprite = image;
            if (cardPanel != null) cardPanel.SetActive(true);
            Debug.Log("[Lakbay Tala] Lore Card placeholder: " + loreId + " – " + title);
        }

        public void Hide()
        {
            if (cardPanel != null) cardPanel.SetActive(false);
        }
    }
}
