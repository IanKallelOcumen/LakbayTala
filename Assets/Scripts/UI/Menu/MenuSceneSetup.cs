using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LakbayTala.UI.Core;

namespace LakbayTala.UI.Menu
{
    /// <summary>
    /// Add to Menu scene to wire existing main menu UI to panels.
    /// Auto-finds buttons by name (Play, Leaderboard, Stats, Achievements, Settings, Quit),
    /// finds or creates panels (including from Figma-imported prefabs), and assigns everything
    /// so no manual Inspector wiring is needed.
    /// </summary>
    public class MenuSceneSetup : MonoBehaviour
    {
        [Tooltip("If true, find and wire buttons and panels at Start.")]
        public bool autoFindReferences = true;

        [Header("Optional overrides (leave empty to use auto-find)")]
        public LakbayTalaGameMenuController menuController;
        public UIManager uiManager;
        public Transform menuRoot;

        [Header("Figma panel prefabs (optional)")]
        [Tooltip("Assign Figma-imported prefabs from Unity Figma Converter. Used when panel is not found in scene.")]
        public GameObject leaderboardPanelPrefab;
        public GameObject statsPanelPrefab;
        public GameObject achievementsPanelPrefab;
        public GameObject settingsPanelPrefab;

        [Header("Button and panel name hints (for auto-find)")]
        [Tooltip("Matches button GameObject name or button label text. Add your menu names (e.g. LOBBY SCREEN, PLAY).")]
        public string[] playButtonNames = new[] { "Play", "PLAY", "PlayButton", "BtnPlay" };
        public string[] leaderboardButtonNames = new[] { "Leaderboard", "LeaderboardButton", "BtnLeaderboard", "LOBBY", "Lobby", "RANK", "Rank" };
        public string[] statsButtonNames = new[] { "Stats", "StatsButton", "BtnStats", "MAP CHOICES", "Map" };
        public string[] achievementsButtonNames = new[] { "Achievements", "AchievementsButton", "BtnAchievements", "FOLKLORE BESTI", "Bestiary", "FOLK LORE" };
        public string[] settingsButtonNames = new[] { "Settings", "SettingsButton", "BtnSettings", "Menu", "MENU" };
        public string[] quitButtonNames = new[] { "Quit", "QuitButton", "BtnQuit", "Exit", "EXIT" };

        private void Awake()
        {
            if (!autoFindReferences) return;

            if (uiManager == null)
                uiManager = Object.FindFirstObjectByType<UIManager>();
            if (menuController == null)
                menuController = Object.FindFirstObjectByType<LakbayTalaGameMenuController>();

            if (menuController == null)
            {
                Debug.LogWarning("[MenuSceneSetup] LakbayTalaGameMenuController not found. Add it to the Canvas or create it.");
                return;
            }

            Transform searchRoot = menuRoot != null ? menuRoot : GetMenuSearchRoot();
            if (searchRoot == null) { Debug.LogWarning("[MenuSceneSetup] No Canvas found; create a Canvas or assign Menu Root."); return; }
            WireButtons(searchRoot);
            WirePanels(searchRoot);
            EnsurePanelsFromFigmaPrefabs();
            RegisterPanelsWithUIManager();
            WireBrawlStarsIfPresent();
            LogWiringSummary();
        }

        private void LogWiringSummary()
        {
            int buttons = (menuController.playButton != null ? 1 : 0) + (menuController.leaderboardButton != null ? 1 : 0) + (menuController.statsButton != null ? 1 : 0) + (menuController.achievementsButton != null ? 1 : 0) + (menuController.settingsButton != null ? 1 : 0) + (menuController.quitButton != null ? 1 : 0);
            int panels = (menuController.leaderboardPanel != null ? 1 : 0) + (menuController.statsPanel != null ? 1 : 0) + (menuController.achievementsPanel != null ? 1 : 0) + (menuController.settingsPanel != null ? 1 : 0);
            Debug.Log($"[MenuSceneSetup] Wired {buttons} buttons, {panels} panels. UIManager: {(uiManager != null ? "yes" : "no")}.");
            if (buttons == 0)
                Debug.LogWarning("[MenuSceneSetup] No buttons wired. If your menu is under a different Canvas, set Menu Root on MenuSceneSetup to that Canvas.");
        }

        private void WireBrawlStarsIfPresent()
        {
            var brawl = Object.FindFirstObjectByType<BrawlStarsMenuController>();
            if (brawl != null && menuController.brawlStarsMenuController == null)
                menuController.brawlStarsMenuController = brawl;
        }

        private Transform GetMenuSearchRoot()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            return canvas != null ? canvas.transform : transform;
        }

        private void WireButtons(Transform root)
        {
            if (root == null) return;
            var allButtons = root.GetComponentsInChildren<Button>(true);
            foreach (var btn in allButtons)
            {
                if (btn == null) continue;
                string name = btn.gameObject.name;
                string label = GetButtonLabel(btn);
                if (MatchNameOrLabel(name, label, playButtonNames) && menuController.playButton == null)
                    menuController.playButton = btn;
                else if (MatchNameOrLabel(name, label, leaderboardButtonNames) && menuController.leaderboardButton == null)
                    menuController.leaderboardButton = btn;
                else if (MatchNameOrLabel(name, label, statsButtonNames) && menuController.statsButton == null)
                    menuController.statsButton = btn;
                else if (MatchNameOrLabel(name, label, achievementsButtonNames) && menuController.achievementsButton == null)
                    menuController.achievementsButton = btn;
                else if (MatchNameOrLabel(name, label, settingsButtonNames) && menuController.settingsButton == null)
                    menuController.settingsButton = btn;
                else if (MatchNameOrLabel(name, label, quitButtonNames) && menuController.quitButton == null)
                    menuController.quitButton = btn;
            }
        }

        private static string GetButtonLabel(Button btn)
        {
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null && !string.IsNullOrEmpty(tmp.text)) return tmp.text.Trim();
            var legacy = btn.GetComponentInChildren<UnityEngine.UI.Text>(true);
            if (legacy != null && !string.IsNullOrEmpty(legacy.text)) return legacy.text.Trim();
            return "";
        }

        private static bool MatchNameOrLabel(string name, string label, string[] hints)
        {
            if (hints == null) return false;
            string n = (name ?? "").Trim();
            string l = (label ?? "").Trim();
            for (int i = 0; i < hints.Length; i++)
            {
                string h = hints[i];
                if (string.IsNullOrWhiteSpace(h)) continue;
                string hint = h.Trim();
                if (n.IndexOf(hint, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
                if (l.IndexOf(hint, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
            }
            return false;
        }

        private void WirePanels(Transform root)
        {
            if (root == null || menuController == null) return;
            var stats = root.GetComponentInChildren<StatsPanel>(true);
            if (stats != null && menuController.statsPanel == null) menuController.statsPanel = stats;
            var achievements = root.GetComponentInChildren<AchievementsPanel>(true);
            if (achievements != null && menuController.achievementsPanel == null) menuController.achievementsPanel = achievements;
            var lb = root.GetComponentInChildren<LakbayTalaLeaderboardUIController>(true);
            if (lb != null && menuController.leaderboardPanel == null) menuController.leaderboardPanel = lb;
            foreach (var p in root.GetComponentsInChildren<UIPanel>(true))
            {
                if (p == null) continue;
                if (p.panelType == UIPanelType.Leaderboard && menuController.leaderboardPanel == null)
                    menuController.leaderboardPanel = p;
                else if (p.panelType == UIPanelType.Settings && menuController.settingsPanel == null)
                    menuController.settingsPanel = p;
            }
            FindPanelsByName(root);
        }

        private void FindPanelsByName(Transform root)
        {
            string[] leaderboardNames = { "LeaderboardPanel", "Leaderboard", "PanelLeaderboard", "LOBBY SCREEN", "LOBBY SCREEN -", "Lobby Screen", "Rank", "RANK" };
            string[] statsNames = { "StatsPanel", "Stats", "PanelStats", "MAP CHOICES", "Map Choices" };
            string[] achievementsNames = { "AchievementsPanel", "Achievements", "PanelAchievements", "FOLKLORE BESTI", "FOLK LORE INFO", "Folklore Bestiary", "Folk Lore Info" };
            string[] settingsNames = { "SettingsPanel", "Settings", "PanelSettings", "Menu", "MENU" };
            TryAssignUIPanelByName(root, leaderboardNames, UIPanelType.Leaderboard, ref menuController.leaderboardPanel);
            TryAssignStatsPanelByName(root, statsNames);
            TryAssignAchievementsPanelByName(root, achievementsNames);
            TryAssignUIPanelByName(root, settingsNames, UIPanelType.Settings, ref menuController.settingsPanel);
        }

        private void TryAssignUIPanelByName(Transform root, string[] names, UIPanelType type, ref UIPanel outPanel)
        {
            if (outPanel != null) return;
            foreach (var n in names)
            {
                var t = FindChildRecursive(root, n);
                if (t == null) continue;
                var p = t.GetComponent<UIPanel>();
                if (p == null) p = t.gameObject.AddComponent<UIPanel>();
                if (t.GetComponent<CanvasGroup>() == null) t.gameObject.AddComponent<CanvasGroup>();
                p.panelType = type;
                outPanel = p;
                return;
            }
        }

        private void TryAssignStatsPanelByName(Transform root, string[] names)
        {
            if (menuController.statsPanel != null) return;
            foreach (var n in names)
            {
                var t = FindChildRecursive(root, n);
                if (t == null) continue;
                var p = t.GetComponent<StatsPanel>();
                if (p != null) { menuController.statsPanel = p; return; }
            }
        }

        private void TryAssignAchievementsPanelByName(Transform root, string[] names)
        {
            if (menuController.achievementsPanel != null) return;
            foreach (var n in names)
            {
                var t = FindChildRecursive(root, n);
                if (t == null) continue;
                var p = t.GetComponent<AchievementsPanel>();
                if (p != null) { menuController.achievementsPanel = p; return; }
            }
        }

        private static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent == null || string.IsNullOrWhiteSpace(name)) return null;
            string search = name.Trim();
            string parentName = (parent.name ?? "").Trim();
            if (parentName.Equals(search, System.StringComparison.OrdinalIgnoreCase)) return parent;
            if (parentName.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0) return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindChildRecursive(parent.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        private void EnsurePanelsFromFigmaPrefabs()
        {
            Transform parent = (uiManager != null && uiManager.transform != null) ? uiManager.transform : GetMenuSearchRoot();
            if (parent == null) return;
            if (menuController.leaderboardPanel == null && leaderboardPanelPrefab != null)
                menuController.leaderboardPanel = InstantiatePanelPrefab(leaderboardPanelPrefab, parent, UIPanelType.Leaderboard);
            if (menuController.statsPanel == null && statsPanelPrefab != null)
            {
                var go = Instantiate(statsPanelPrefab, parent);
                go.name = statsPanelPrefab.name;
                var sp = go.GetComponent<StatsPanel>();
                if (sp == null) sp = go.AddComponent<StatsPanel>();
                if (go.GetComponent<CanvasGroup>() == null) go.AddComponent<CanvasGroup>();
                if (go.GetComponent<UIPanel>() != null) go.GetComponent<UIPanel>().panelType = UIPanelType.Stats;
                else { var uip = go.AddComponent<UIPanel>(); uip.panelType = UIPanelType.Stats; }
                go.SetActive(false);
                menuController.statsPanel = sp;
            }
            if (menuController.achievementsPanel == null && achievementsPanelPrefab != null)
            {
                var go = Instantiate(achievementsPanelPrefab, parent);
                go.name = achievementsPanelPrefab.name;
                var ap = go.GetComponent<AchievementsPanel>();
                if (ap == null) ap = go.AddComponent<AchievementsPanel>();
                if (go.GetComponent<CanvasGroup>() == null) go.AddComponent<CanvasGroup>();
                if (go.GetComponent<UIPanel>() != null) go.GetComponent<UIPanel>().panelType = UIPanelType.Achievements;
                else { var uip = go.AddComponent<UIPanel>(); uip.panelType = UIPanelType.Achievements; }
                go.SetActive(false);
                menuController.achievementsPanel = ap;
            }
            if (menuController.settingsPanel == null && settingsPanelPrefab != null)
                menuController.settingsPanel = InstantiatePanelPrefab(settingsPanelPrefab, parent, UIPanelType.Settings);
            if (menuController.leaderboardPanel == null)
                menuController.leaderboardPanel = CreateFallbackLeaderboardPanel(parent);
        }

        private UIPanel CreateFallbackLeaderboardPanel(Transform parent)
        {
            GameObject go = new GameObject("LeaderboardPanel");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image bg = go.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.2f, 0.98f);
            CanvasGroup cg = go.AddComponent<CanvasGroup>();
            UIPanel p = go.AddComponent<UIPanel>();
            p.panelType = UIPanelType.Leaderboard;
            p.hideOnStart = true;
            p.showOnStart = false;

            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(go.transform, false);
            RectTransform titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -40);
            titleRect.sizeDelta = new Vector2(400, 60);
            TextMeshProUGUI titleText = titleGo.AddComponent<TextMeshProUGUI>();
            titleText.text = "Leaderboard";
            titleText.fontSize = 42;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;

            GameObject closeGo = new GameObject("CloseButton");
            closeGo.transform.SetParent(go.transform, false);
            RectTransform closeRect = closeGo.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0.08f);
            closeRect.anchorMax = new Vector2(0.5f, 0.08f);
            closeRect.pivot = new Vector2(0.5f, 0.5f);
            closeRect.sizeDelta = new Vector2(180, 50);
            Image closeImg = closeGo.AddComponent<Image>();
            closeImg.color = new Color(0.5f, 0.2f, 0.2f, 1f);
            Button closeBtn = closeGo.AddComponent<Button>();
            GameObject closeTextGo = new GameObject("Text");
            closeTextGo.transform.SetParent(closeGo.transform, false);
            RectTransform closeTextRect = closeTextGo.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            TextMeshProUGUI closeText = closeTextGo.AddComponent<TextMeshProUGUI>();
            closeText.text = "Close";
            closeText.fontSize = 28;
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;
            closeBtn.onClick.AddListener(() => p.Hide());

            go.SetActive(false);
            return p;
        }

        private UIPanel InstantiatePanelPrefab(GameObject prefab, Transform parent, UIPanelType type)
        {
            var go = Instantiate(prefab, parent);
            go.name = prefab.name;
            var p = go.GetComponent<UIPanel>();
            if (p == null) p = go.AddComponent<UIPanel>();
            if (go.GetComponent<CanvasGroup>() == null) go.AddComponent<CanvasGroup>();
            p.panelType = type;
            p.hideOnStart = true;
            p.showOnStart = false;
            go.SetActive(false);
            return p;
        }

        private void RegisterPanelsWithUIManager()
        {
            if (uiManager == null) return;
            if (menuController.leaderboardPanel != null) uiManager.RegisterPanel(menuController.leaderboardPanel);
            if (menuController.statsPanel != null) uiManager.RegisterPanel(menuController.statsPanel);
            if (menuController.achievementsPanel != null) uiManager.RegisterPanel(menuController.achievementsPanel);
            if (menuController.settingsPanel != null) uiManager.RegisterPanel(menuController.settingsPanel);
        }
    }
}
