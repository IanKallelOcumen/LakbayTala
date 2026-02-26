using UnityEngine;
using UnityEngine.UI;
using LakbayTala.UI.Core;

namespace LakbayTala.UI.Menu
{
    public class LakbayTalaGameMenuController : MonoBehaviour
    {
        [Header("Menu Buttons (Assign in Inspector)")]
        public Button playButton;
        public Button leaderboardButton;
        public Button statsButton;
        public Button achievementsButton;
        public Button settingsButton;
        public Button quitButton;

        [Header("Panel References (Assign in Inspector or via MenuSceneSetup)")]
        public UIPanel leaderboardPanel;
        public StatsPanel statsPanel;
        public AchievementsPanel achievementsPanel;
        public UIPanel settingsPanel;

        [Header("Optional: Brawl Stars / Figma menu")]
        [Tooltip("If set, Leaderboard/Settings/Achievements buttons will use this controller (e.g. Figma-imported panels).")]
        public BrawlStarsMenuController brawlStarsMenuController;

        [Header("Scene to load on Play")]
        [Tooltip("Name of the game scene in Build Settings. If empty or missing, loads scene index 1.")]
        public string gameSceneName = "GameScene";

        private void Start()
        {
            // MenuSceneSetup runs in Awake so buttons/panels are already wired.
            if (UIManager.Instance == null)
                Debug.LogWarning("[LakbayTalaGameMenuController] UIManager not found. Ensure UIManager is in the scene.");
            RegisterPanels();
            InitializeMenuButtons();
        }

        private void RegisterPanels()
        {
            if (UIManager.Instance == null) return;
            if (leaderboardPanel != null) UIManager.Instance.RegisterPanel(leaderboardPanel);
            if (statsPanel != null) UIManager.Instance.RegisterPanel(statsPanel);
            if (achievementsPanel != null) UIManager.Instance.RegisterPanel(achievementsPanel);
            if (settingsPanel != null) UIManager.Instance.RegisterPanel(settingsPanel);
        }

        private void InitializeMenuButtons()
        {
            if (playButton) playButton.onClick.AddListener(OnPlayClicked);
            if (leaderboardButton) leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
            if (statsButton) statsButton.onClick.AddListener(() => UIManager.Instance?.ShowPanel(UIPanelType.Stats));
            if (achievementsButton) achievementsButton.onClick.AddListener(OnAchievementsClicked);
            if (settingsButton) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnLeaderboardClicked()
        {
            if (brawlStarsMenuController != null)
            {
                brawlStarsMenuController.ShowLeaderboard();
                return;
            }
            if (UIManager.Instance == null) return;
            if (leaderboardPanel != null) UIManager.Instance.RegisterPanel(leaderboardPanel);
            UIManager.Instance.ShowPanel(UIPanelType.Leaderboard);
        }

        private void OnAchievementsClicked()
        {
            if (brawlStarsMenuController != null) brawlStarsMenuController.ShowAchievements();
            else UIManager.Instance?.ShowPanel(UIPanelType.Achievements);
        }

        private void OnSettingsClicked()
        {
            if (brawlStarsMenuController != null) brawlStarsMenuController.ShowSettings();
            else UIManager.Instance?.ShowPanel(UIPanelType.Settings);
        }

        private void OnPlayClicked()
        {
            if (!string.IsNullOrWhiteSpace(gameSceneName) && Application.CanStreamedLevelBeLoaded(gameSceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
                return;
            }
            // Fallback: load scene at index 1 (usually the game scene after menu at 0)
            if (UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings > 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
            else
            {
                Debug.LogWarning("[LakbayTala] No game scene. Add your game scene to File > Build Settings and set Game Scene Name on this controller.");
            }
        }

        private void OnQuitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
