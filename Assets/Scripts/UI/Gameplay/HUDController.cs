using UnityEngine;
using UnityEngine.UI;
using LakbayTala.UI.Core;

namespace LakbayTala.UI.Gameplay
{
    public class HUDController : UIPanel
    {
        [Header("HUD Controls (Assign in Inspector)")]
        public Button pauseButton;
        public Button settingsButton;
        public Button loreButton;

        protected override void Awake()
        {
            panelType = UIPanelType.HUD;
            base.Awake();
            
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            if (pauseButton) pauseButton.onClick.AddListener(OnPauseClicked);
            if (settingsButton) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (loreButton) loreButton.onClick.AddListener(OnLoreClicked);
        }

        private void OnPauseClicked()
        {
            Debug.Log("Game Paused");
            Time.timeScale = 0;
            UIManager.Instance?.ShowPanel(UIPanelType.Popup); 
        }

        private void OnSettingsClicked()
        {
            UIManager.Instance?.ShowPanel(UIPanelType.Settings);
        }

        private void OnLoreClicked()
        {
            UIManager.Instance?.ShowPanel(UIPanelType.Lore);
        }
    }
}
