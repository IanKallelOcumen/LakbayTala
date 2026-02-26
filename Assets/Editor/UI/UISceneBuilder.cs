using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;
using LakbayTala.UI.Menu;
using LakbayTala.UI.Core;
using LakbayTala.UI.Components;
using LakbayTala.UI.Gameplay;
using LakbayTala.UI.Theme;

namespace LakbayTala.Editor.UI
{
    public class UISceneBuilder : EditorWindow
    {
        /// <summary>Called from LakbayTala > Menu Scene > Build Full Menu. Creates Canvas, UIManager, panels, and buttons.</summary>
        public static void BuildMenuScene()
        {
            // 1. Setup Canvas
            GameObject canvasObj = SetupCanvas();

            // 2. Setup UIManager
            UIManager uiManager = SetupUIManager(canvasObj);

            // 3. Create Panels (set panelType so UIManager layers work)
            GameObject statsPanel = CreatePanel<StatsPanel>("StatsPanel", uiManager.transform, new Color(0.1f, 0.1f, 0.2f, 0.95f), UIPanelType.Stats);
            GameObject achievementsPanel = CreatePanel<AchievementsPanel>("AchievementsPanel", uiManager.transform, new Color(0.1f, 0.1f, 0.2f, 0.95f), UIPanelType.Achievements);
            GameObject settingsPanel = CreatePanel<UIPanel>("SettingsPanel", uiManager.transform, new Color(0.1f, 0.1f, 0.2f, 0.95f), UIPanelType.Settings);
            GameObject leaderboardPanel = CreatePanel<LakbayTalaLeaderboardUIController>("LeaderboardPanel", uiManager.transform, new Color(0.1f, 0.1f, 0.2f, 0.95f), UIPanelType.Leaderboard);

            // 4. Create Main Menu Layout (parent under Canvas for clean hierarchy)
            GameObject menuControllerObj = new GameObject("LakbayTalaGameMenuController");
            menuControllerObj.transform.SetParent(canvasObj.transform, false);
            LakbayTalaGameMenuController menuController = menuControllerObj.AddComponent<LakbayTalaGameMenuController>();
            
            // Assign Panels
            menuController.statsPanel = statsPanel.GetComponent<StatsPanel>();
            menuController.achievementsPanel = achievementsPanel.GetComponent<AchievementsPanel>();
            menuController.settingsPanel = settingsPanel.GetComponent<UIPanel>();
            menuController.leaderboardPanel = leaderboardPanel.GetComponent<LakbayTalaLeaderboardUIController>();

            // Create Buttons Layout
            GameObject buttonContainer = new GameObject("MenuButtons");
            buttonContainer.transform.SetParent(canvasObj.transform, false);
            RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(400, 600);
            containerRect.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup layout = buttonContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            // Generate Buttons
            menuController.playButton = CreateStyledButton("PlayButton", buttonContainer.transform, "PLAY", Color.green).GetComponent<Button>();
            menuController.leaderboardButton = CreateStyledButton("LeaderboardButton", buttonContainer.transform, "LEADERBOARD", Color.yellow).GetComponent<Button>();
            menuController.statsButton = CreateStyledButton("StatsButton", buttonContainer.transform, "STATS", Color.cyan).GetComponent<Button>();
            menuController.achievementsButton = CreateStyledButton("AchievementsButton", buttonContainer.transform, "ACHIEVEMENTS", Color.magenta).GetComponent<Button>();
            menuController.settingsButton = CreateStyledButton("SettingsButton", buttonContainer.transform, "SETTINGS", Color.gray).GetComponent<Button>();
            menuController.quitButton = CreateStyledButton("QuitButton", buttonContainer.transform, "QUIT", Color.red).GetComponent<Button>();

            // Create Theme Manager
            if (Object.FindFirstObjectByType<UIThemeManager>() == null)
            {
                GameObject themeMgr = new GameObject("UIThemeManager");
                themeMgr.transform.SetParent(canvasObj.transform, false);
                themeMgr.AddComponent<UIThemeManager>();
            }

            // Ensure EventSystem exists for UI input
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<InputSystemUIInputModule>();
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            Debug.Log("Menu Scene UI Built Successfully! Save the scene (Ctrl+S) to keep changes.");
        }

        private static GameObject SetupCanvas()
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            return canvasObj;
        }

        private static UIManager SetupUIManager(GameObject canvas)
        {
            UIManager mgr = Object.FindFirstObjectByType<UIManager>();
            if (mgr == null)
            {
                GameObject mgrObj = new GameObject("UIManager");
                mgrObj.transform.SetParent(canvas.transform, false);
                mgr = mgrObj.AddComponent<UIManager>();
            }
            return mgr;
        }

        private static GameObject CreatePanel<T>(string name, Transform parent, Color bgColor, UIPanelType panelType = UIPanelType.None) where T : Component
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = bgColor;
            
            // Add script and set panelType for UIManager
            T comp = panel.AddComponent<T>();
            var uip = comp as UIPanel;
            if (uip != null) uip.panelType = panelType;
            
            // Add Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panel.transform, false);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = name.Replace("Panel", "");
            titleText.fontSize = 60;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -50);
            titleRect.sizeDelta = new Vector2(0, 100);

            // Add Close Button
            GameObject closeBtn = CreateStyledButton("CloseButton", panel.transform, "CLOSE", Color.red);
            RectTransform closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.pivot = new Vector2(1, 1);
            closeRect.anchoredPosition = new Vector2(-20, -20);
            closeRect.sizeDelta = new Vector2(100, 50);

            // Hide initially
            panel.SetActive(false);

            return panel;
        }

        private static GameObject CreateStyledButton(string name, Transform parent, string text, Color color)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            
            // Generate a simple rounded rect sprite programmatically
            img.sprite = CreateRoundedRectSprite(128, 128, 20);
            img.type = Image.Type.Sliced;
            img.color = color;
            
            // Add shadow
            Shadow shadow = btnObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0,0,0,0.5f);
            shadow.effectDistance = new Vector2(4, -4);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 80);

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 32;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.textWrappingMode = TextWrappingModes.NoWrap;
            
            // Try to load BrawlStars font if available in project
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts/BrawlStars SDF");
            if (font != null) tmp.font = font;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return btnObj;
        }

        private static Sprite CreateRoundedRectSprite(int width, int height, int radius)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color[] colors = new Color[width * height];
            
            float rSquared = radius * radius;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Simple rounded corner logic
                    bool inside = true;
                    if (x < radius && y < radius) inside = (x - radius)*(x - radius) + (y - radius)*(y - radius) < rSquared;
                    else if (x > width - radius && y < radius) inside = (x - (width - radius))*(x - (width - radius)) + (y - radius)*(y - radius) < rSquared;
                    else if (x < radius && y > height - radius) inside = (x - radius)*(x - radius) + (y - (height - radius))*(y - (height - radius)) < rSquared;
                    else if (x > width - radius && y > height - radius) inside = (x - (width - radius))*(x - (width - radius)) + (y - (height - radius))*(y - (height - radius)) < rSquared;
                    
                    colors[y * width + x] = inside ? Color.white : Color.clear;
                }
            }
            
            tex.SetPixels(colors);
            tex.Apply();
            
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }
    }
}
