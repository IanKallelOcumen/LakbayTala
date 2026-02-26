using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LakbayTala.UI.Core;
using LakbayTala.UI.Theme;

namespace LakbayTala.UI.Menu
{
    public class StatsPanel : UIPanel
    {
        [Header("Stats UI")]
        public TextMeshProUGUI totalScoreText;
        public TextMeshProUGUI totalPlayTimeText;
        public TextMeshProUGUI gamesPlayedText;
        public TextMeshProUGUI unlockedLoreText;
        public Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            if (closeButton != null)
                closeButton.onClick.AddListener(() => Hide());
            
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (UIThemeManager.Instance == null) return;

            // Apply theme to texts
            if (totalScoreText) UIThemeManager.Instance.ApplyThemeToText(totalScoreText);
            if (totalPlayTimeText) UIThemeManager.Instance.ApplyThemeToText(totalPlayTimeText);
            if (gamesPlayedText) UIThemeManager.Instance.ApplyThemeToText(gamesPlayedText);
            if (unlockedLoreText) UIThemeManager.Instance.ApplyThemeToText(unlockedLoreText);
            
            // Apply theme to background if Image component exists
            var img = GetComponent<Image>();
            if (img) UIThemeManager.Instance.ApplyThemeToPanel(img);
        }

        public override void Show(bool instant = false)
        {
            UpdateStats();
            base.Show(instant);
        }

        private void UpdateStats()
        {
            // Placeholder for actual data retrieval
            // Should connect to PlayerProfile or GameController
            if (totalScoreText) totalScoreText.text = "Score: " + PlayerPrefs.GetInt("TotalScore", 0);
            if (totalPlayTimeText) totalPlayTimeText.text = "Play Time: " + FormatTime(PlayerPrefs.GetFloat("TotalPlayTime", 0));
            if (gamesPlayedText) gamesPlayedText.text = "Games Played: " + PlayerPrefs.GetInt("GamesPlayed", 0);
            
            // Example of using LoreManager if available
            if (unlockedLoreText && LakbayTala.Lore.LoreManager.Instance != null)
            {
                unlockedLoreText.text = "Lore Unlocked: " + LakbayTala.Lore.LoreManager.Instance.GetUnlockedLore().Count;
            }
        }

        private string FormatTime(float seconds)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
        }
    }
}
