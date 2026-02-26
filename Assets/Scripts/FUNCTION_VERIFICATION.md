# Function Verification – LakbayTala

This document verifies that the main systems and their functions are correctly wired and safe to use. **Static/code verification** is done by tracing calls; **runtime verification** requires running the game and using **LakbayTala > Menu Scene > Build Full Menu** or playing Menu/Game scenes.

---

## 1. Menu & UI bootstrap

| Function | Location | Verification |
|----------|----------|--------------|
| **UIBootstrap.BootstrapUI()** | Runs `AfterSceneLoad` | Creates UIManager, panels (Stats, Achievements, Settings, Lore, Quiz), DifficultyManager, GameStateManager, QuizManager, UIThemeManager. In Menu scene creates **LakbayTalaGameMenuController**; in Game scene creates **HUDController**. All use `FindObjectOfType` / create-if-missing. **Works** if scene name contains `"Menu"` or is `"MainMenu"` for menu path. |
| **LakbayTalaGameMenuController.Start()** | GameMenuController.cs | Registers leaderboard/stats/achievements/settings panels with UIManager when refs are set; calls **InitializeMenuButtons()**. **Works** when buttons and panels are assigned (Inspector or Build Menu Scene). |
| **InitializeMenuButtons()** | GameMenuController.cs | Play → **OnPlayClicked** (LoadScene "GameScene"); Leaderboard/Stats/Achievements/Settings → **UIManager.Instance?.ShowPanel(type)**; Quit → **OnQuitClicked**. **Works**; ensure **GameScene** is in Build Settings. |
| **MenuSceneSetup.Start()** | MenuSceneSetup.cs | If `autoFindReferences`: finds UIManager and LakbayTalaGameMenuController; registers leaderboard panel with UIManager if both exist. **Works**. |

**Potential issues:**  
- UIBootstrap uses deprecated `Object.FindObjectOfType` / `FindObjectsOfType` (still functional; can be updated to `FindFirstObjectByType` / `FindObjectsByType` for consistency).  
- If no Canvas/EventSystem exists in scene, UI may not receive input until EventSystemInputSwitcher and Canvas are present (UIManager creation adds a Canvas).

---

## 2. UIManager & panels

| Function | Location | Verification |
|----------|----------|--------------|
| **UIManager.RegisterPanel(UIPanel)** | UIManager.cs | Adds panel to dictionary by `panelType`; reparents to correct layer via **GetLayerForType**. **Works**. |
| **UIManager.ShowPanel(UIPanelType)** | UIManager.cs | Looks up panel, calls **panel.Show()**, pushes to stack. **Works** when panel is registered. |
| **UIManager.HidePanel / HideAll** | UIManager.cs | **Works**. |
| **UIPanel.Show/Hide** | UIPanel.cs | Sets active, alpha, interactable, blocksRaycasts; uses **CanvasGroup** (RequiredComponent). Fade coroutine uses **showCurve** / **hideCurve**. **Works**; panels must have **CanvasGroup** (auto-added by RequireComponent). |
| **UIStateManager.NotifyPanelShown/Hidden** | UIPanel calls these | UIPanel notifies UIStateManager when shown/hidden. **Works** if UIStateManager exists (optional). |

**Potential issues:**  
- **StatsPanel** and **AchievementsPanel** call **base.Awake()** and use **UIThemeManager.Instance**; safe with null check in StatsPanel. **AchievementsPanel**: **achievementsContainer** now guarded with `if (achievementsContainer == null) return` in **LoadAchievements()** to avoid NRE when unset.

---

## 3. Lore flow

| Function | Location | Verification |
|----------|----------|--------------|
| **LoreMarker.OnTriggerEnter2D** | LoreMarker.cs | Requires **PlayerController** on other; checks **LoreManager.Instance** and **allLoreData** for matching **loreId**. If found → **LoreCardUI.DisplayLore(data)**; else → **LoreCardController.Instance.Show(...)**. **Works** when LoreManager and either LoreCardUI or LoreCardController exist in scene. |
| **LoreCardUI.DisplayLore(LoreData)** | LoreCardUI.cs | Updates title/category/description/image, then **Show()**. **Works**. |
| **LoreCardController.Show(...)** | LoreCardController.cs | Sets title/body/illustration, activates **cardPanel**. **Works** when refs assigned. |
| **LoreManager.UnlockLore / GetUnlockedLore** | LoreManager.cs | **Works**; persistence via PlayerPrefs. |

**Potential issues:**  
- **LoreCardUI** uses **UIFeedbackHelper.TriggerButtonPress** / **ApplyPressVisual** on close; static helper, no null ref.  
- **LoreManager.allLoreData** must be assigned in Inspector (list of LoreData assets).

---

## 4. Quiz flow

| Function | Location | Verification |
|----------|----------|--------------|
| **MiniQuizTrigger.OnTriggerEnter2D** | MiniQuizTrigger.cs | Requires **PlayerController**; calls **QuizController.Instance.ShowPostLevelQuiz(forLevelScene)**. **Works** when QuizController is in scene. |
| **QuizController.ShowPostLevelQuiz(string)** | QuizController.cs | Sets **levelSceneName**, sets **quizPanel.SetActive(true)**. **Works** when **quizPanel** is assigned in Inspector. |

**Potential issues:**  
- **QuizController** must be present in Game scene and **quizPanel** assigned; otherwise MiniQuizTrigger logs a warning.

---

## 5. Leaderboard

| Function | Location | Verification |
|----------|----------|--------------|
| **Leaderboard button** | LakbayTalaGameMenuController | Calls **UIManager.Instance?.ShowPanel(UIPanelType.Leaderboard)**. **Works** when a panel with **panelType == Leaderboard** is registered (e.g. **LakbayTalaLeaderboardUIController** extends UIPanel). |
| **LakbayTalaLeaderboardUIController** | LakbayTalaLeaderboardUIController.cs | Extends **UIPanel**; many optional UI refs (container, buttons, etc.). **Works** when used as the registered Leaderboard panel. |
| **LeaderboardTests** | Tests/LeaderboardTests.cs | NUnit tests for **LeaderboardUser**, **LeaderboardService**, UI components. **Run** via Unity Test Runner (Edit Mode) or `Unity -runTests`. |

**Potential issues:**  
- **LeaderboardTests** create **LeaderboardEntryUI** and set **rankText**, **usernameText**, **scoreText** (UnityEngine.UI.Text). If **LeaderboardEntryUI** in the project uses different field names or types (e.g. TMP), tests may need updating.  
- **LeaderboardService** and **LakbayTalaLeaderboardUIController** are in different namespaces; tests use global **LakbayTalaLeaderboardUIController** (no namespace in that file).

---

## 6. Input & editor

| Function | Location | Verification |
|----------|----------|--------------|
| **EventSystemInputSwitcher** | EventSystemInputSwitcher.cs | **AfterSceneLoad** and **sceneLoaded**: calls **SwitchAllEventSystems()**, which finds all **EventSystem**, removes **StandaloneInputModule**, adds **InputSystemUIInputModule**. **Works** when Input System Package is in use. |
| **LakbayTala > Menu Scene > Build Full Menu** | UISceneBuilder.cs (Editor) | Creates Canvas, UIManager, Stats/Achievements/Settings/Leaderboard panels, **LakbayTalaGameMenuController**, buttons, UIThemeManager. **Works**; all references use **LakbayTalaGameMenuController**. |

---

## 7. Summary

- **Menu / Bootstrap / UIManager / Panels**: Wired and function as designed; AchievementsPanel is safe when **achievementsContainer** is unset.  
- **Lore**: LoreMarker → LoreManager + LoreCardUI or LoreCardController works when scene contains those components and LoreData is assigned.  
- **Quiz**: MiniQuizTrigger → QuizController.ShowPostLevelQuiz works when QuizController and quizPanel are in the Game scene.  
- **Leaderboard**: Button shows leaderboard panel when it is registered with UIManager as Leaderboard type.  
- **Input**: EventSystemInputSwitcher replaces legacy input module on scene load.  
- **Build Menu Scene**: Editor tool creates full menu with LakbayTalaGameMenuController.

**How to verify at runtime:**  
1. Open **MenuScene**, press Play. Use Play / Leaderboard / Stats / Achievements / Settings / Quit.  
2. Open **GameScene**, ensure LoreManager, LoreCardUI or LoreCardController, QuizController (+ quizPanel) and optionally LoreMarkers / MiniQuizTriggers are present; trigger them with the player.  
3. Run **Edit Mode** tests: in Unity open **Window > General > Test Runner**, switch to **EditMode**, then **Run All** (or run **LeaderboardTests**). Alternatively: `"C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe" -runTests -testPlatform EditMode -projectPath "C:\Users\kalle\OneDrive\Desktop\LakbayTala"`.
