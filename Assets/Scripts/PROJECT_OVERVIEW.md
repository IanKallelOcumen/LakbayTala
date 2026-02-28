# LakbayTala Unity Project – How It Works

**UI is built manually.** There is no `Assets/Scripts/UI` folder. Use the scripts below and wire them to your own Canvas, buttons, and panels in the Inspector.

---

## 1. High-level flow

- **Menu scene** → Your own UI (Play, Leaderboard, Settings, etc.). Use **LakbayTalaIntegration** if you want; assign your Canvas groups and buttons there.
- **Game scene** → Player, **LoreMarker** (shows lore via **LoreCardController**), **MiniQuizTrigger** (shows quiz via **QuizController** + **QuizPanel**). Progress/unlocks can be saved.

---

## 2. Core systems (standalone – apply to your UI later)

### **Lore (Assets/Scripts/Lore/)**

- **LoreManager**  
  Singleton. Holds **LoreData** list and unlocked IDs. Saves to PlayerPrefs. Fires `OnLoreUnlocked`. Assign **allLoreData** in Inspector.

- **LoreData** (ScriptableObject)  
  Create via **LakbayTala/Lore/Lore Data**. Fields: id, category, title, shortDescription, fullDescription, illustration.

- **LoreMarker** (in game world)  
  Collider2D trigger. On player enter: if **LoreManager** has **LoreData** for `loreId`, calls **LoreCardController.Instance.Show(...)** with that data; else uses the marker’s **title** / **shortText** / **image**. No dependency on deleted UI.

- **LoreCardController**  
  Placeholder panel controller. Assign **cardPanel**, **titleText**, **bodyText**, **illustration** (Image), **closeButton** in Inspector. **LoreMarker** calls `Show(loreId, title, body, sprite)`.

### **Quiz (Assets/Scripts/Quiz/)**

- **QuizManager**  
  Singleton. Holds **QuizCollection** list, score, events. **SubmitAnswer(question, choiceIndex)** updates score and PlayerPrefs.

- **QuizController**  
  Assign **quizPanel** (GameObject) in Inspector. **ShowPostLevelQuiz(sceneName)** sets **quizPanel** active. Your panel can use **QuizPanel** (Quiz/UI) to show questions and choices.

- **QuizPanel** (Quiz/UI/QuizPanel.cs)  
  Standalone MonoBehaviour (no UIPanel). Assign **questionText**, **choicesContainer**, **feedbackText**, **closeButton**. Call **ShowQuiz(question, onComplete)** from your flow. Builds choice buttons at runtime if **choiceButtonPrefab** is null.

- **MiniQuizTrigger** (in game world)  
  Collider2D trigger. On player enter, calls **QuizController.Instance.ShowPostLevelQuiz(forLevelScene)**.

### **Leaderboard (Assets/Scripts/Leaderboard/)**

- **LeaderboardService**  
  Load/save leaderboard data. Use from your own UI; no built-in panel.

- **LeaderboardModels**  
  **LeaderboardEntry**, **LeaderboardUser** for the service and your UI.

### **Config (Assets/Scripts/Config/)**

- **GameBalanceConfig**, **DifficultyConfig**  
  ScriptableObjects for tuning. Use from your own systems.

### **Content (Assets/Scripts/Content/)**

- **CharacterData**, **ContentRegistry**  
  Content definitions. Wire to your own UI or gameplay.

### **LakbayTalaIntegration**

- Optional. Ties **MasterGameManager**, clouds, and **CanvasGroup**/Button refs for lobby, map, leaderboard, settings, achievements. **settingsController**, **leaderboardController**, **achievementsController** are now **MonoBehaviour** — assign your own panel scripts; integration calls their methods via reflection if present.

---

## 3. Scenes

| Scene        | Role |
|-------------|------|
| **MenuScene** | Your menu UI. Entry point. |
| **GameScene** | Player, LoreMarkers, MiniQuizTriggers, LoreManager, LoreCardController, QuizController, QuizPanel (or your quiz UI). |

Add both to **File → Build Settings** and set the first scene to your menu.

---

## 4. Applying scripts (manual UI)

1. **Lore**  
   Add **LoreManager** to a GameObject; assign **allLoreData**. Add **LoreCardController**; assign its panel, title/body text, image, close button. Place **LoreMarker** in the level and set **loreId** / **title** / **shortText** / **image**.

2. **Quiz**  
   Add **QuizManager** and **QuizController**. Create a quiz panel GameObject and add **QuizPanel**; assign question text, choices container, feedback text, close button. Assign that panel to **QuizController.quizPanel**. Place **MiniQuizTrigger** in the level.

3. **Leaderboard / Settings / Achievements**  
   Build your own panels and scripts. Optionally assign them to **LakbayTalaIntegration** as **MonoBehaviour** (your controller); ensure they have methods like **RefreshLeaderboard**, **LoadSettings**, **SaveAchievementData**, etc. if you want integration to call them.

---

## 5. License and contact

- **License:** MIT — see [LICENSE](LICENSE).
- **Contact:** IanKallelOcumen
