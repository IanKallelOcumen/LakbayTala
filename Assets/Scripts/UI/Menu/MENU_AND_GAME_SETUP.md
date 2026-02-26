# Menu & In-Game Setup: Leaderboard, Lore Cards, Mini Quiz

## Menu scene: Wire existing UI and panels (one-time setup)

**Auto-wiring (recommended):** Add **MenuSceneSetup** to your Menu scene (e.g. on the Canvas) and leave **Auto Find References** on. It will:

- Find **buttons** by name or label (Play, Leaderboard, Stats, Achievements, Settings, Quit) and assign them to **LakbayTalaGameMenuController**.
- Find **panels** by name (e.g. LeaderboardPanel, StatsPanel) or by component (StatsPanel, AchievementsPanel, UIPanel) and wire them to the menu controller and **UIManager**.
- If **BrawlStarsMenuController** is in the scene, it is assigned so Leaderboard/Settings/Achievements use your Figma/Brawl Stars panels when you click those buttons.
- **Figma panel prefabs:** In MenuSceneSetup you can assign **Leaderboard Panel Prefab**, **Stats Panel Prefab**, **Achievements Panel Prefab**, **Settings Panel Prefab** (Figma-imported prefabs from the Unity Figma Converter). If a panel is not found in the scene, it will be instantiated from the prefab.

So: keep your existing main menu UI; add **LakbayTalaGameMenuController** and **MenuSceneSetup** (and **UIManager** if missing). Use Figma-imported prefabs for the panels in MenuSceneSetup if you want panel UI from Figma.

---

## Menu scene: Leaderboard button shows the leaderboard

### Option A: Using the Brawl Stars asset pack (recommended)

1. **Brawl Stars UI in scene**
   - Have **BrawlStarsMenuController** in the scene (from the Brawl Stars UI / Figma integration).
   - Assign its **leaderboardPanel** (Transform) to the Brawl Stars–styled leaderboard container, and wire **leaderboardButton** on BrawlStarsMenuController to your Leaderboard button if you use its own button wiring.
   - Or use **GameMenuController** (see below) and point it at Brawl Stars.

2. **GameMenuController + Brawl Stars**
   - Add **GameMenuController** to the Canvas.
   - Assign **Leaderboard Button** (and Play, Settings, Achievements, Quit as needed).
   - Assign **Brawl Stars Menu Controller** to the **Brawl Stars Menu Controller** field.
   - When you press Leaderboard, **GameMenuController** will call **BrawlStarsMenuController.ShowLeaderboard()**, so the Brawl Stars leaderboard panel and styling are used. Same for Settings and Achievements.

3. **Optional: MenuSceneSetup**
   - Add **MenuSceneSetup** with **Auto Find References** enabled. It will find **BrawlStarsMenuController** in the scene and assign it to **GameMenuController** so the pack is used automatically.

### Option B: Without Brawl Stars (generic panels)

1. **Canvas with UIManager**
   - In your **Menu** scene, have a GameObject with **UIManager** (e.g. on the main Canvas or a "Managers" object).
   - UIManager creates layers automatically if you don’t assign them.

2. **GameMenuController**
   - Add **GameMenuController** to the same Canvas (or the object that has your menu buttons).
   - Leave **Brawl Stars Menu Controller** empty.
   - Assign:
     - **Play Button** → your Play button
     - **Leaderboard Button** → your Leaderboard button
     - **Leaderboard Panel** OR **Leaderboard Panel Object**:
       - **Leaderboard Panel**: assign the GameObject that has **LakbayTalaLeaderboardUIController** (UIPanel).
       - **Leaderboard Panel Object**: if you only have **LakbayTalaLeaderboardPanel** (cultural leaderboard), assign that GameObject here.
     - **Main Menu Root** (optional): the GameObject that contains the main menu. Hidden when leaderboard opens, shown when it closes.

3. **Leaderboard panel starts hidden**
   - The leaderboard panel GameObject should start **inactive**, OR the script will hide it at start.

4. **Optional: MenuSceneSetup**
   - Add **MenuSceneSetup** with **Auto Find References** so it can assign the leaderboard panel if you didn’t.

---

## Game scene: Lore cards you can find

1. **LoreCardController or LoreCardUI**
   - In the **Game** scene Canvas, have either:
     - **LoreCardController** with **Card Panel**, **Title**, **Body**, **Close** assigned (simple panel), or
     - **LoreCardUI** (from `LakbayTala.UI.Lore`) for the full card with reveal animation.
   - If you use **LoreManager** and **LoreData** assets, add **LoreManager** to the scene and assign **All Lore Data**. **LoreMarker** will then use **LoreCardUI** when the `loreId` matches a LoreData.

2. **LoreMarker in the world**
   - In the game world, create a GameObject (e.g. empty or sprite).
   - Add a **Collider 2D** (e.g. Box Collider 2D) and check **Is Trigger**.
   - Add **LoreMarker** and set:
     - **Lore Id**: e.g. `"mount_makiling"` (must match a LoreData id if you use LoreManager), or leave as placeholder.
     - **Title** / **Short Text** / **Image** for the simple card.
   - When the player walks into the trigger, the lore card appears.

---

## Game scene: Mini quiz you can find

1. **QuizController**
   - In the Game scene, have a GameObject with **QuizController**.
   - Assign **Quiz Panel**: the GameObject that contains your quiz UI (questions, answer buttons, close).
   - The panel should start **inactive**; QuizController will show it when the quiz is opened.

2. **MiniQuizTrigger in the world**
   - Create a GameObject in the level (e.g. “Quiz Zone”).
   - Add a **Collider 2D** and check **Is Trigger**.
   - Add **MiniQuizTrigger** and set **For Level Scene** (e.g. `"GameScene"`) if needed.
   - When the player walks into the trigger, the quiz panel opens.

---

## Checklist

- [ ] Menu scene: UIManager present
- [ ] Menu scene: GameMenuController with Leaderboard Button and Leaderboard Panel (or Leaderboard Panel Object) assigned
- [ ] Menu scene: Leaderboard panel GameObject starts hidden or has Hide On Start
- [ ] Game scene: LoreCardController or LoreCardUI (+ LoreManager if using LoreData)
- [ ] Game scene: At least one LoreMarker with trigger collider
- [ ] Game scene: QuizController with Quiz Panel assigned
- [ ] Game scene: At least one MiniQuizTrigger with trigger collider
