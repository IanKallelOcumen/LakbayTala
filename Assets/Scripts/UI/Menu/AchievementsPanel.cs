using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LakbayTala.UI.Core;
using TMPro;

namespace LakbayTala.UI.Menu
{
    public class AchievementsPanel : UIPanel
    {
        [Header("Achievements UI")]
        public Transform achievementsContainer;
        public GameObject achievementEntryPrefab;
        public Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            if (closeButton != null)
                closeButton.onClick.AddListener(() => Hide());
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            LoadAchievements();
        }

        private void LoadAchievements()
        {
            if (achievementsContainer == null) return;
            // Clear existing
            foreach (Transform child in achievementsContainer)
            {
                Destroy(child.gameObject);
            }

            // Placeholder logic: iterate through achievements (ScriptableObjects or Enums)
            // For now, I'll simulate some achievements
            List<string> mockAchievements = new List<string> { "First Steps", "Explorer", "Collector", "Master" };

            foreach (var achievement in mockAchievements)
            {
                if (achievementEntryPrefab != null)
                {
                    GameObject entry = Instantiate(achievementEntryPrefab, achievementsContainer);
                    // Setup entry UI (assuming AchievementEntryUI script exists or generic text)
                    // For simplicity, just finding text components
                    var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
                    if (texts.Length > 0) texts[0].text = achievement;
                    
                    // Set unlock status visual
                    bool unlocked = PlayerPrefs.GetInt("Achievement_" + achievement, 0) == 1;
                    // entry.GetComponent<Image>().color = unlocked ? Color.white : Color.gray;
                }
            }
        }
    }
}
