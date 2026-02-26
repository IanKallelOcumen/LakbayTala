# LakbayTala Unity Project – How It Works

## Step-by-step setup

**Follow in order:** [SETUP_STEP_BY_STEP.md](UI/SETUP_STEP_BY_STEP.md) (in `Assets/Scripts/UI/`). It walks you through: Build Settings → Menu scene (UIManager, LakbayTalaGameMenuController, MenuSceneSetup, panels, buttons) → Game scene (Lore, Quiz, triggers) → test.

---

## One-click setup (no manual wiring)

- **Menu scene:** Open your **Menu** scene, then use **LakbayTala > Menu Scene > Setup Current Scene**. This builds the full menu (Canvas, UIManager, panels, **LakbayTalaGameMenuController**, all buttons wired, EventSystem). Save the scene (Ctrl+S) when done.
- **Game scene:** Open your **Game** scene, then use **LakbayTala > Menu Scene > Setup Current Scene**. This adds Canvas (if missing), EventSystem, UIManager, **LoreManager**, **LoreCardController** + card panel, **QuizController** + quiz panel. Save the scene when done.

If the current scene name doesn’t contain “Menu” or “Game”, the one-click menu will ask you to choose **Menu Setup** or **Game Setup**.

You can also use:
- **LakbayTala > Menu Scene > Add Wiring (Existing UI)** — wiring only; **Build Full Menu** — full menu from scratch.
- **LakbayTala > Game Scene > Setup Game Scene** — game scene only; **Force BGSprite + CloudSpawner** — background.
- **LakbayTala > Setup Both Scenes (Menu + Game)** — run both from Build Settings.

---

## 1. High-level flow

- **Menu scene** → You see the main menu (Play, Leaderboard, Stats, Achievements, Settings, Quit). Play loads the game scene.
- **Game scene** → You control the player, find **lore markers** (show lore cards) and **quiz triggers** (show mini quiz). Progress and unlocks can be saved.
- **UI** is driven by **UIManager** (layers), **UIPanel** (show/hide/transitions), and optional **Brawl Stars** styling.

---

## 2. Scenes

| Scene        | Role |
|-------------|------|
| **MenuScene** | Main menu: buttons, leaderboard, stats, achievements, settings. Entry point. |
| **GameScene** | In-game: player, map, HUD, lore markers, quiz triggers. |

Build settings define which scene loads first (usually Menu).

---

## 3. Core systems (by folder / concern)

### **UI (Assets/Scripts/UI/)**

- **UIManager** (Core)  
  Singleton. Creates canvas layers (Background, HUD, Menu, Lore, Popup, Overlay) and assigns **sort order** so panels draw in the right order. Panels register with `RegisterPanel()` and are shown with `ShowPanel(type)`.

- **UIPanel** (Core)  
  Base for all panels. Has `panelType`, show/hide with optional **fade** (duration + curve). Fires `OnShowComplete` / `OnHideComplete`. Notifies **UIStateManager** when shown/hidden.

- **LakbayTalaGameMenuController** (Menu)  
  Lives in the **menu scene**. If you use **One-Click Setup** or **Build Menu Scene**, buttons and panels are created and assigned for you. Otherwise assign in the Inspector: buttons (Play, Leaderboard, Stats, Achievements, Settings, Quit) and panels (leaderboard, stats, achievements, settings). In **Start** it registers those panels with UIManager and wires each button (e.g. Leaderboard → `UIManager.ShowPanel(Leaderboard)`).

- **MenuSceneSetup** (Menu)  
  Optional. With “Auto Find References” it finds **UIManager**, **LakbayTalaGameMenuController**, and leaderboard panel. Not required if you used **One-Click Setup** or **Build Menu Scene**.

- **Brawl Stars** (optional)  
  **BrawlStarsMenuController** holds Brawl Stars–style panels. If you assign it on **LakbayTalaGameMenuController**, the menu can use those panels and styling.

- **Leaderboard**  
  **LakbayTalaLeaderboardPanel** or **LakbayTalaLeaderboardUIController** shows the list. Data can come from **LeaderboardService** (e.g. Firebase later). **LeaderboardSortController** can sort by score, time, level, etc.

### **Lore (Assets/Scripts/Lore/)**

- **LoreManager**  
  Holds **LoreData** list and which IDs are unlocked. Saves unlocks (e.g. PlayerPrefs). Fires `OnLoreUnlocked`.

- **LoreData** (ScriptableObject)  
  One asset per entry: id, category, title, short/full description, illustration, unlock condition.

- **LoreMarker** (in game world)  
  Trigger (Collider2D). When the **player** enters, it shows a lore card: if **LoreManager** has **LoreData** for that `loreId`, it uses **LoreCardUI**; otherwise **LoreCardController** (simple panel).

- **LoreCardUI** (UI/Lore)  
  Full card UI (image, title, category, description) with reveal animations. Uses **LoreData**.

### **Quiz (Assets/Scripts/Quiz/)**

- **QuizController**  
  Holds a reference to the **quiz panel** GameObject. `ShowPostLevelQuiz(sceneName)` shows it; you implement questions/answers in your UI.

- **MiniQuizTrigger** (in game world)  
  Trigger. On player enter, calls **QuizController.ShowPostLevelQuiz()** so the player “finds” a quiz in the level.

### **Leaderboard (Assets/Scripts/Leaderboard/)**

- **LeaderboardService**  
  Loads/saves leaderboard data (e.g. Firebase when configured). Exposes events so the UI can refresh without reloading the scene.

- **LeaderboardEntry / LeaderboardUser**  
  Data models used by the service and the leaderboard UI.

### **Game flow (e.g. Assets/Scripts/Mechanics/, Scripts/)**

- **PlayerController** (or similar)  
  Movement, jump, input (e.g. new Input System). Collides with **LoreMarker** and **MiniQuizTrigger**.

- **UIBootstrap** (UI)  
  Runs **AfterSceneLoad**. Ensures **UIManager**, core panels, and in menu scene **LakbayTalaGameMenuController** exist. Creates them if missing. For a fully wired scene with no manual setup, use **LakbayTala > Menu Scene > Setup Current Scene** (or **Setup Both Scenes**) first, then Play.

---

## 4. Data and persistence

- **UISaveLoadBridge**  
  Saves/loads UI-related state (last panel, last menu sub-id, unlocked lore IDs). Uses **IUIProgressProvider** or falls back to PlayerPrefs.

- **LoreManager**  
  Persists unlocked lore IDs (e.g. PlayerPrefs).

- **LeaderboardService**  
  Intended for remote leaderboard; can also cache locally.

---

## 5. Input and editor

- **EventSystemInputSwitcher**  
  Replaces legacy **StandaloneInputModule** with **InputSystemUIInputModule** so UI works when “Input System Package” is active in Player Settings.

- **External editor (VS Code)**  
  The message “External Code Editor application path does not exist (…\Code.exe)” is a **Unity Editor** setting: **Edit → Preferences → External Tools**. Set “External Script Editor” to the path where VS Code (or your editor) is installed, or choose “Open by file extension” and pick the correct .exe.

---

## 6. Summary diagram

```
[Menu Scene]
  UIManager (layers) ← panels register here
  LakbayTalaGameMenuController (buttons + panel refs) → ShowPanel(Leaderboard) etc.
  (Optional) BrawlStarsMenuController, MenuSceneSetup

[Game Scene]
  PlayerController ← collides with →
  LoreMarker → LoreCardUI / LoreCardController
  MiniQuizTrigger → QuizController → quiz panel
  UIManager + HUD panels (if any)
  LoreManager, LeaderboardService (data)
```

The **leaderboard button** works when the leaderboard panel is registered (automatic if you used **One-Click Setup** or **Build Menu Scene** for the menu).
