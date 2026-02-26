using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;
using LakbayTala.UI.Core;
using LakbayTala.UI.Menu;
using LakbayTala.Lore;
using LakbayTala.Quiz;
using LakbayTala.Editor.UI;

namespace LakbayTala.Editor
{
    /// <summary>
    /// One-click setup for Menu or Game scene. No manual wiring needed.
    /// </summary>
    public static class LakbayTalaSceneSetup
    {
        // ---- Menu Scene ----
        /// <summary>Detects if current scene is Menu or Game and runs the right setup.</summary>
        [MenuItem("LakbayTala/Menu Scene/Setup Current Scene (Menu or Game)", false, 0)]
        public static void OneClickSetupCurrentScene()
        {
            string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            bool isMenu = sceneName.IndexOf("Menu", System.StringComparison.OrdinalIgnoreCase) >= 0 || sceneName == "MainMenu";
            bool isGame = sceneName.IndexOf("Game", System.StringComparison.OrdinalIgnoreCase) >= 0 || sceneName == "GameScene";

            if (isMenu)
            {
                AddMenuWiringToExistingScene();
                Debug.Log("[LakbayTala] Menu wiring added. Save scene (Ctrl+S).");
                return;
            }

            if (isGame)
            {
                SetupGameScene();
                Debug.Log("[LakbayTala] Game scene setup complete. Save scene (Ctrl+S).");
                return;
            }

            int choice = EditorUtility.DisplayDialogComplex(
                "LakbayTala Setup",
                "Current scene doesn't match Menu or Game.\nWhich setup do you want?",
                "Menu",
                "Cancel",
                "Game");
            if (choice == 0) { AddMenuWiringToExistingScene(); }
            else if (choice == 2) { SetupGameScene(); }
        }

        /// <summary>Add UIManager, menu controller, and MenuSceneSetup so your existing buttons/panels get wired at Play.</summary>
        [MenuItem("LakbayTala/Menu Scene/Add Wiring (Existing UI) _%#m", false, 1)]
        public static void AddMenuWiringToExistingScene()
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<InputSystemUIInputModule>();
            }

            UIManager uiManager = canvasObj.GetComponentInChildren<UIManager>(true);
            if (uiManager == null)
            {
                var mgrObj = new GameObject("UIManager");
                mgrObj.transform.SetParent(canvasObj.transform, false);
                uiManager = mgrObj.AddComponent<UIManager>();
            }

            LakbayTalaGameMenuController menuController = Object.FindFirstObjectByType<LakbayTalaGameMenuController>();
            if (menuController == null)
            {
                var menuGo = new GameObject("LakbayTalaGameMenuController");
                menuGo.transform.SetParent(canvasObj.transform, false);
                menuController = menuGo.AddComponent<LakbayTalaGameMenuController>();
            }

            MenuSceneSetup menuSetup = canvasObj.GetComponentInChildren<MenuSceneSetup>(true);
            if (menuSetup == null)
            {
                var setupGo = new GameObject("MenuSceneSetup");
                setupGo.transform.SetParent(canvasObj.transform, false);
                menuSetup = setupGo.AddComponent<MenuSceneSetup>();
                menuSetup.autoFindReferences = true;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        /// <summary>Create a full menu from scratch: Canvas, buttons, and panels (use when you have no menu UI yet).</summary>
        [MenuItem("LakbayTala/Menu Scene/Build Full Menu (Buttons + Panels)", false, 2)]
        public static void BuildMenuSceneFromSetup()
        {
            UISceneBuilder.BuildMenuScene();
        }

        // ---- Game Scene ----
        /// <summary>Add Canvas, EventSystem, UIManager, LoreManager, LoreCardController, QuizController to the current scene.</summary>
        [MenuItem("LakbayTala/Game Scene/Setup Game Scene", false, 10)]
        public static void SetupGameSceneMenuItem()
        {
            SetupGameScene();
            Debug.Log("[LakbayTala] Game scene setup complete. Save scene (Ctrl+S).");
        }

        /// <summary>Run setup on both Menu and Game scenes (from Build Settings). Do Build Settings first, then this.</summary>
        [MenuItem("LakbayTala/Setup Both Scenes (Menu + Game)", false, 20)]
        public static void RunFullSetupBothScenes()
        {
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            string menuPath = null;
            string gamePath = null;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (!scenes[i].enabled) continue;
                string name = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                if (menuPath == null && (name.IndexOf("Menu", System.StringComparison.OrdinalIgnoreCase) >= 0 || name == "MainMenu"))
                    menuPath = scenes[i].path;
                if (gamePath == null && (name.IndexOf("Game", System.StringComparison.OrdinalIgnoreCase) >= 0 || name == "GameScene"))
                    gamePath = scenes[i].path;
            }
            string currentPath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            bool saved = true;
            if (!string.IsNullOrEmpty(menuPath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(menuPath);
                AddMenuWiringToExistingScene();
                saved = UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            }
            if (saved && !string.IsNullOrEmpty(gamePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(gamePath);
                SetupGameScene();
                UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            }
            if (!string.IsNullOrEmpty(currentPath) && currentPath != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path)
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentPath);
            Debug.Log("[LakbayTala] Full setup done. Menu and Game scenes had wiring/components added. Save any open scene (Ctrl+S).");
        }

        public static void SetupGameScene()
        {
            // 1. Canvas + EventSystem
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<InputSystemUIInputModule>();
            }

            // 2. UIManager
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>();
            if (uiManager == null)
            {
                var mgrObj = new GameObject("UIManager");
                mgrObj.transform.SetParent(canvasObj.transform, false);
                uiManager = mgrObj.AddComponent<UIManager>();
            }

            // 3. LoreManager
            if (Object.FindFirstObjectByType<LoreManager>() == null)
            {
                var loreGo = new GameObject("LoreManager");
                loreGo.AddComponent<LoreManager>();
            }

            // 4. LoreCardController + simple card panel (for LoreMarker fallback when no LoreData)
            if (Object.FindFirstObjectByType<LoreCardController>() == null)
            {
                CreateLoreCardControllerSetup(canvasObj.transform);
            }

            // 5. QuizController + Quiz panel
            if (Object.FindFirstObjectByType<QuizController>() == null)
            {
                CreateQuizControllerSetup(canvasObj.transform);
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        private static void CreateLoreCardControllerSetup(Transform parent)
        {
            // Panel GameObject (the card that shows/hides)
            GameObject panel = new GameObject("LoreCardPanel");
            panel.transform.SetParent(parent, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.25f, 0.25f);
            panelRect.anchorMax = new Vector2(0.75f, 0.75f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.98f);

            // Title
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panel.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(0, 60);
            var titleText = titleGo.AddComponent<TextMeshProUGUI>();
            titleText.text = "Lore";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;

            // Body
            var bodyGo = new GameObject("Body");
            bodyGo.transform.SetParent(panel.transform, false);
            var bodyRect = bodyGo.AddComponent<RectTransform>();
            bodyRect.anchorMin = Vector2.zero;
            bodyRect.anchorMax = new Vector2(1, 1);
            bodyRect.offsetMin = new Vector2(20, 80);
            bodyRect.offsetMax = new Vector2(-20, -80);
            var bodyText = bodyGo.AddComponent<TextMeshProUGUI>();
            bodyText.text = "";
            bodyText.fontSize = 24;
            bodyText.color = Color.white;

            // Illustration (optional)
            var illGo = new GameObject("Illustration");
            illGo.transform.SetParent(panel.transform, false);
            var illRect = illGo.AddComponent<RectTransform>();
            illRect.anchorMin = new Vector2(1, 1);
            illRect.anchorMax = new Vector2(1, 1);
            illRect.pivot = new Vector2(1, 1);
            illRect.anchoredPosition = new Vector2(-20, -20);
            illRect.sizeDelta = new Vector2(120, 120);
            var illustration = illGo.AddComponent<Image>();
            illustration.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            // Close button
            var closeGo = new GameObject("CloseButton");
            closeGo.transform.SetParent(panel.transform, false);
            var closeRect = closeGo.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0);
            closeRect.anchorMax = new Vector2(0.5f, 0);
            closeRect.pivot = new Vector2(0.5f, 0);
            closeRect.anchoredPosition = new Vector2(0, 20);
            closeRect.sizeDelta = new Vector2(160, 50);
            var closeImg = closeGo.AddComponent<Image>();
            closeImg.color = new Color(0.6f, 0.2f, 0.2f, 1f);
            var closeBtn = closeGo.AddComponent<Button>();
            var closeTextGo = new GameObject("Text");
            closeTextGo.transform.SetParent(closeGo.transform, false);
            var closeTextRect = closeTextGo.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            var closeText = closeTextGo.AddComponent<TextMeshProUGUI>();
            closeText.text = "Close";
            closeText.fontSize = 24;
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;

            // Controller GameObject with refs
            var controllerGo = new GameObject("LoreCardController");
            controllerGo.transform.SetParent(parent, false);
            var controller = controllerGo.AddComponent<LoreCardController>();
            controller.cardPanel = panel;
            controller.illustration = illustration;
            controller.titleText = titleText;
            controller.bodyText = bodyText;
            controller.closeButton = closeBtn;
            closeBtn.onClick.AddListener(controller.Hide);

            panel.SetActive(false);
        }

        private static void CreateQuizControllerSetup(Transform parent)
        {
            // Quiz panel
            GameObject panel = new GameObject("QuizPanel");
            panel.transform.SetParent(parent, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(panel.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = new Vector2(400, 80);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "Quiz – Add your questions here";
            label.fontSize = 32;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;

            var closeGo = new GameObject("CloseButton");
            closeGo.transform.SetParent(panel.transform, false);
            var closeRect = closeGo.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0.2f);
            closeRect.anchorMax = new Vector2(0.5f, 0.2f);
            closeRect.sizeDelta = new Vector2(200, 50);
            closeGo.AddComponent<Image>().color = new Color(0.4f, 0.2f, 0.2f, 1f);
            var closeBtn = closeGo.AddComponent<Button>();
            var closeTextGo = new GameObject("Text");
            closeTextGo.transform.SetParent(closeGo.transform, false);
            var closeTextRect = closeTextGo.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            var closeText = closeTextGo.AddComponent<TextMeshProUGUI>();
            closeText.text = "Close";
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;

            var quizControllerGo = new GameObject("QuizController");
            quizControllerGo.transform.SetParent(parent, false);
            var quizController = quizControllerGo.AddComponent<QuizController>();
            quizController.quizPanel = panel;
            closeBtn.onClick.AddListener(() => { if (panel != null) panel.SetActive(false); });

            panel.SetActive(false);
        }
    }
}
