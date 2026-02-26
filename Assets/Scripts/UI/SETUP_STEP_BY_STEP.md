# LakbayTala – Setup Step by Step

Do each step in order. When a step says “add” or “assign,” do it in the Unity Editor, then move to the next step.

---

## Step 1: Build settings (you do this)

1. Open **File → Build Settings**.
2. Ensure **MenuScene** (or your main menu scene) and **GameScene** are in **Scenes In Build**.
3. Set the **first** scene in the list to your menu scene (index 0).
4. Close Build Settings.

---

## Step 2: Run the setup tools (menu + game in one go)

1. In the Unity menu bar, click **LakbayTala → Setup Both Scenes (Menu + Game)**.
2. This opens your Menu scene, adds **UIManager**, **LakbayTalaGameMenuController**, **MenuSceneSetup**, and **EventSystem** (if missing), then opens your Game scene and adds **Canvas**, **EventSystem**, **UIManager**, **LoreManager**, **LoreCardController** + card panel, **QuizController** + quiz panel.
3. Save when prompted or press **Ctrl+S**. You can then do Steps 3–7 to assign Figma panels or tweak button names if needed.

**Or do scenes separately:** Open Menu scene → **LakbayTala → Menu Scene → Add Wiring (Existing UI)**. Then open Game scene → **LakbayTala → Game Scene → Setup Game Scene**. Save each.

---

## Step 3: Open the Menu scene (optional from here)

1. Open your **Menu** scene (the one with your existing main menu UI).
2. Leave it open for Steps 4–8 if you want to assign Figma prefabs or check button names.

---

## Step 4: Add or find UIManager (Menu scene)

1. In the **Hierarchy**, look for a GameObject that has the **UIManager** component (often on **Canvas** or a child).
2. **If UIManager exists:** note which GameObject it’s on. Go to Step 5.
3. **If UIManager does not exist:**
   - Right‑click in Hierarchy → **Create Empty**. Name it **UIManager**.
   - With **UIManager** selected, **Add Component** → search **UIManager** → add **UIManager (Script)**.
   - If your menu is under a Canvas, drag **UIManager** as a **child of the Canvas** (optional but keeps hierarchy tidy).
4. Do not remove or rename the GameObject. Leave the UIManager component defaults.

---

## Step 4: Add LakbayTalaGameMenuController (Menu scene)

1. In the **Hierarchy**, select the **Canvas** (or the root object that holds your menu buttons).
2. **Add Component** → search **LakbayTala** → add **Lakbay Tala Game Menu Controller (Script)**.
3. Leave all fields **empty** for now. Wiring will be done in Step 5.
4. Save the scene (Ctrl+S).

---

## Step 6: Add MenuSceneSetup and enable auto‑wiring (Menu scene)

1. In the **Hierarchy**, select the **same Canvas** (or the same root as in Step 4).
2. **Add Component** → search **MenuSceneSetup** → add **Menu Scene Setup (Script)**.
3. In the Inspector, ensure **Auto Find References** is **checked**.
4. **(Optional)** If your menu is inside a specific child (e.g. “MainMenuPanel”), drag that Transform into **Menu Root**; otherwise leave **Menu Root** empty so it uses the whole Canvas.
5. Save the scene (Ctrl+S).

**What this does:** When you press Play, MenuSceneSetup will find your Play, Leaderboard, Stats, Achievements, Settings, and Quit buttons by name/label and assign them to LakbayTalaGameMenuController. It will also find or create panels and register them with UIManager.

---

## Step 6: Panels – choose one path

You only need **one** of the options below.

### Option A: Panels already in the scene (e.g. from Figma)

1. Ensure you have GameObjects that will act as panels (Leaderboard, Stats, Achievements, Settings).
2. Name them (or their parent) so they can be found, e.g. **LeaderboardPanel**, **StatsPanel**, **AchievementsPanel**, **SettingsPanel**.
3. Add **UIPanel** (and **Canvas Group**) to each panel GameObject if not already present:
   - Select the panel GameObject → **Add Component** → **UIPanel**.
   - Set **Panel Type** (Leaderboard, Stats, Achievements, Settings).
   - Ensure **Hide On Start** is checked so they don’t show at start.
4. Save the scene. MenuSceneSetup will find them in Step 6.

### Option B: Use Figma‑imported panel prefabs

1. Import your panel designs from Figma (D.A. Figma Converter for Unity) so you have **prefabs** for Leaderboard, Stats, Achievements, Settings.
2. In the Menu scene, select the GameObject that has **Menu Scene Setup**.
3. In the Inspector, under **Figma Panel Prefabs** on MenuSceneSetup, assign:
   - **Leaderboard Panel Prefab**
   - **Stats Panel Prefab**
   - **Achievements Panel Prefab**
   - **Settings Panel Prefab**
4. Save the scene. If a panel is not found in the scene, it will be created from the prefab at runtime.

### Option C: Use Brawl Stars / Figma menu controller

1. Ensure **BrawlStarsMenuController** is in the scene and its panels (Leaderboard, Settings, Achievements) are assigned in its Inspector.
2. Do **not** assign Figma panel prefabs in MenuSceneSetup unless you also want fallback panels.
3. MenuSceneSetup will find BrawlStarsMenuController and assign it to LakbayTalaGameMenuController. Clicks on Leaderboard / Settings / Achievements will then use the Brawl Stars panels.

---

## Step 8: Check menu button names (Menu scene)

1. In the Hierarchy, locate your **Play**, **Leaderboard**, **Stats**, **Achievements**, **Settings**, and **Quit** buttons.
2. Each button’s **GameObject name** or the **text on the button** (e.g. “Play”, “Leaderboard”) should match what MenuSceneSetup looks for.
3. Default names/labels already include your hierarchy: **PLAY**, **LOBBY**, **RANK** (leaderboard), **MAP CHOICES** (stats), **FOLKLORE BESTI**, **FOLK LORE** (achievements), **Menu** (settings), **Exit** (quit). Panels can be matched by name too (e.g. **LOBBY SCREEN**, **FOLK LORE INFO**, **FOLKLORE BESTI**, **MAP CHOICES**).
4. If the wrong panel opens for a button, assign the correct panel on **LakbayTalaGameMenuController** in the Inspector, or edit **Button and Panel Name Hints** on **MenuSceneSetup**.

**Optional – easiest matching:** You can rename GameObjects so wiring finds them with no extra hints. For buttons, use **GameObject name** or **button label** like: **Play** / **PLAY**, **Leaderboard** / **LOBBY**, **Stats** / **MAP CHOICES**, **Achievements** / **FOLKLORE BESTI**, **Settings** / **Menu**, **Quit** / **Exit**. For panels, name the panel GameObject e.g. **LeaderboardPanel**, **StatsPanel**, **LOBBY SCREEN**, **MAP CHOICES**, **FOLK LORE INFO** (partial names like "LOBBY SCREEN" also match "LOBBY SCREEN -").

---

## Step 9: Test the Menu scene

1. Press **Play** in the Menu scene.
2. Click **Play** → the game should load the Game scene (if Step 1 is correct).
3. Click **Leaderboard**, **Stats**, **Achievements**, **Settings** → the corresponding panel should open.
4. Click **Quit** → the application should quit (or exit Play mode in the Editor).
5. Stop Play mode and save the scene if you changed anything.

---

## Step 10: Open the Game scene

1. Open your **Game** scene (the one where the player plays the level).
2. Leave it open for Steps 11–16.

---

## Step 11: Canvas and UIManager (Game scene)

1. In the Hierarchy, check if there is a **Canvas** and, on it or a child, a **UIManager**.
2. **If both exist:** go to Step 12.
3. **If missing:** use **LakbayTala → Game Scene → Setup Game Scene** (or **Menu Scene → Setup Current Scene** when in the Game scene). This will add Canvas, EventSystem, and UIManager. Save the scene.

---

## Step 12: LoreManager (Game scene)

1. In the Hierarchy, look for a GameObject with **Lore Manager (Script)**.
2. **If it exists:** ensure **All Lore Data** is assigned (list of LoreData assets). Go to Step 13.
3. **If it does not exist:**  
   - Right‑click in Hierarchy → **Create Empty** → name it **LoreManager**.  
   - Add Component → **Lore Manager (Script)**.  
   - Assign your **Lore Data** assets to **All Lore Data** (can be empty at first).  
4. Save the scene.

---

## Step 13: Lore card UI (Game scene)

1. Decide how you want lore to appear:
   - **Simple:** use **LoreCardController** with a panel (title, body, image, close button).
   - **Full:** use **LoreCardUI** (with LoreData and optional reveal).
2. **If you already have a lore panel in the scene:** add **Lore Card Controller (Script)** or **Lore Card UI (Script)** to it and assign its fields (Card Panel, Title, Body, Close, etc.).
3. **If you have nothing:** run **LakbayTala → Game Scene → Setup Game Scene** once; it creates a simple LoreCardController + panel. You can replace the panel with your own design later.
4. Save the scene.

---

## Step 14: LoreMarker in the level (Game scene)

1. In the **Hierarchy** or in your level layout, create or select a GameObject where the player should discover lore (e.g. “Lore_MountMakiling”).
2. Add a **Collider 2D** (e.g. Box Collider 2D). Check **Is Trigger**.
3. Add Component → **Lore Marker (Script)**.
4. Set **Lore Id** to match a LoreData id (if using LoreManager), or leave as placeholder.
5. Set **Title** and **Short Text** for the simple card. Optionally assign **Image**.
6. Save the scene. Repeat for other lore points.

---

## Step 15: QuizController and Quiz panel (Game scene)

1. In the Hierarchy, look for a GameObject with **Quiz Controller (Script)**.
2. **If it exists:** assign its **Quiz Panel** to the GameObject that holds your quiz UI (questions, answers, close). Go to Step 16.
3. **If it does not exist:**  
   - Right‑click in Hierarchy → **Create Empty** → name it **QuizController**.  
   - Add Component → **Quiz Controller (Script)**.  
   - Create or use an existing panel for the quiz UI. Assign it to **Quiz Panel**.  
   - Ensure the quiz panel starts **inactive** (unchecked in Hierarchy).  
4. Save the scene.

---

## Step 16: MiniQuizTrigger in the level (Game scene)

1. Create or select a GameObject in the level where the player should trigger a quiz (e.g. “QuizZone_1”).
2. Add a **Collider 2D** and check **Is Trigger**.
3. Add Component → **Mini Quiz Trigger (Script)**.
4. Set **For Level Scene** to your game scene name (e.g. **GameScene**).
5. Save the scene. Repeat for other quiz zones if needed.

---

## Step 17: Test the Game scene

1. Press **Play** in the Game scene.
2. Move the player into a **LoreMarker** trigger → a lore card should appear.
3. Move the player into a **MiniQuizTrigger** trigger → the quiz panel should open.
4. Stop Play mode and save.

---

## Checklist (quick reference)

- [ ] **Step 1:** You did Build Settings (Menu + Game in build, menu first).
- [ ] **Step 2:** You ran **LakbayTala → Setup Both Scenes (Menu + Game)** and saved.
- [ ] **Steps 4–6:** Menu scene has UIManager, LakbayTalaGameMenuController, MenuSceneSetup (done by tool if you ran Step 2).
- [ ] **Step 7:** Panels set up (in scene, or Figma prefabs, or BrawlStarsMenuController).
- [ ] **Steps 8–9:** Button names/labels correct; menu test (Play, Leaderboard, Stats, etc.) works.
- [ ] **Steps 11–13:** Game scene has Canvas, UIManager, LoreManager, LoreCardController/LoreCardUI (done by tool if you ran Step 2).
- [ ] **Step 14:** At least one LoreMarker with trigger collider.
- [ ] **Steps 15–16:** QuizController with Quiz Panel; at least one MiniQuizTrigger with trigger collider.
- [ ] **Step 17:** Game scene test: lore and quiz trigger correctly.

If something doesn’t work, re-check the step that covers that part (e.g. Step 5 for button wiring, Step 6 for panels, Step 13 for lore triggers).
