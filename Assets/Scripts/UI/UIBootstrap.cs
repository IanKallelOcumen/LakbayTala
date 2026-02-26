using UnityEngine;
using UnityEngine.UI;
using LakbayTala.UI.Core;
using LakbayTala.UI.Menu;
using LakbayTala.UI.Gameplay;
using LakbayTala.UI.Lore;
using LakbayTala.UI.Components;
using LakbayTala.Quiz.UI;
using LakbayTala.Quiz;
using LakbayTala.Core;

namespace LakbayTala.UI
{
    /// <summary>
    /// Automatically instantiates the entire UI hierarchy if it doesn't exist.
    /// Ensures the game has a functional UI immediately upon Play.
    /// </summary>
    public class UIBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapUI()
        {
            // 0. Ensure Core Systems exist
            EnsureCoreSystems();

            // 1. Ensure UIManager exists
            if (UIManager.Instance == null)
            {
                CreateUIManager();
            }

            // 2. Ensure Main Canvas exists (UIManager creates layers, but we might need a root if UIManager is strictly logic)
            
            // 3. Create Panels
            CreatePanelIfMissing<StatsPanel>(UIPanelType.Stats, "StatsPanel");
            CreatePanelIfMissing<AchievementsPanel>(UIPanelType.Achievements, "AchievementsPanel");
            CreatePanelIfMissing<UIPanel>(UIPanelType.Settings, "SettingsPanel"); // Generic panel for now
            CreatePanelIfMissing<LoreCardUI>(UIPanelType.Lore, "LoreCardPanel");
            CreatePanelIfMissing<QuizPanel>(UIPanelType.Popup, "QuizPanel");
            
            // 4. Create Controllers based on Scene
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.Contains("Menu") || sceneName == "MainMenu") // Adjust logic as needed
            {
                CreateMenuController();
            }
            else
            {
                CreateHUD();
            }
        }

        private static void EnsureCoreSystems()
        {
            if (DifficultyManager.Instance == null)
            {
                GameObject go = new GameObject("DifficultyManager");
                go.AddComponent<DifficultyManager>();
            }
            
            if (GameStateManager.Instance == null)
            {
                GameObject go = new GameObject("GameStateManager");
                go.AddComponent<GameStateManager>();
            }

            if (QuizManager.Instance == null)
            {
                GameObject go = new GameObject("QuizManager");
                go.AddComponent<QuizManager>();
            }

            if (LakbayTala.UI.Theme.UIThemeManager.Instance == null)
            {
                GameObject go = new GameObject("UIThemeManager");
                go.AddComponent<LakbayTala.UI.Theme.UIThemeManager>();
            }
        }

        private static void CreateUIManager()
        {
            GameObject uiGO = new GameObject("UIManager");
            uiGO.AddComponent<UIManager>();
            
            // Add a root Canvas to UIManager if it's meant to be the root renderer
            Canvas canvas = uiGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = uiGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            uiGO.AddComponent<GraphicRaycaster>();
            
            Debug.Log("[UIBootstrap] Created UIManager");
        }

        private static void CreatePanelIfMissing<T>(UIPanelType type, string name) where T : UIPanel
        {
            // First try to find existing panel of this type in scene
            T existingPanel = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (existingPanel != null)
            {
                if (existingPanel.panelType == UIPanelType.None) existingPanel.panelType = type;
                if (UIManager.Instance != null) UIManager.Instance.RegisterPanel(existingPanel);
                return;
            }

            // Fallback: Check if UIManager already has this panel registered or GameObject exists by name
            if (GameObject.Find(name) != null) return;

            // Only create if we are in "Prototype Mode" or user hasn't set up the scene
            // For now, we still create to ensure functionality, but we log it.
            Debug.Log($"[UIBootstrap] Auto-creating {name} (Prefer setting up Prefabs in scene for production)");

            GameObject panelGO = new GameObject(name);
            
            // Setup full screen rect
            RectTransform rect = panelGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            // Add background
            Image bg = panelGO.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.9f); // Dark semi-transparent background

            // Add Component
            T panel = panelGO.AddComponent<T>();
            panel.panelType = type;
            panel.hideOnStart = true;
            panel.showOnStart = false;

            // Add a close button if it's a generic panel
            if (typeof(T) == typeof(UIPanel))
            {
                ButtonFactory.CreateStandardButton("CloseButton", panelGO.transform, "Close", () => panel.Hide());
                // Position close button at bottom
                Transform btn = panelGO.transform.Find("CloseButton");
                if (btn)
                {
                    RectTransform btnRect = btn.GetComponent<RectTransform>();
                    btnRect.anchorMin = new Vector2(0.5f, 0.1f);
                    btnRect.anchorMax = new Vector2(0.5f, 0.1f);
                    btnRect.anchoredPosition = Vector2.zero;
                }
            }

            // Register with UIManager
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RegisterPanel(panel);
            }
            
            // Initially hide
            panelGO.SetActive(false);
        }

        private static void CreateMenuController()
        {
            if (Object.FindFirstObjectByType<LakbayTalaGameMenuController>() == null)
            {
                GameObject menuGO = new GameObject("LakbayTalaGameMenuController");
                LakbayTalaGameMenuController controller = menuGO.AddComponent<LakbayTalaGameMenuController>();
                
                // Assign found panels
                controller.statsPanel = Object.FindFirstObjectByType<StatsPanel>();
                controller.achievementsPanel = Object.FindFirstObjectByType<AchievementsPanel>();
                
                // Find Settings panel (generic UIPanel with type Settings)
                var panels = Object.FindObjectsByType<UIPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach(var p in panels)
                {
                    if (p.panelType == UIPanelType.Settings)
                    {
                        controller.settingsPanel = p;
                        break;
                    }
                }
                
                // We assume Leaderboard is manually handled or complex, skipping auto-create for now if not found
                
                Debug.Log("[UIBootstrap] Created LakbayTalaGameMenuController");
            }
        }

        private static void CreateHUD()
        {
            if (Object.FindFirstObjectByType<HUDController>() == null)
            {
                GameObject hudGO = new GameObject("HUDController");
                // HUD needs to be full screen to position anchors correctly
                RectTransform rect = hudGO.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                hudGO.AddComponent<CanvasGroup>();
                hudGO.AddComponent<HUDController>();
                
                // Register HUD
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.RegisterPanel(hudGO.GetComponent<HUDController>());
                }
                
                Debug.Log("[UIBootstrap] Created HUDController");
            }
        }
    }
}
