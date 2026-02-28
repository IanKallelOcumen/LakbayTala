# Function Verification – LakbayTala

**UI folder and editor tools have been removed.** All UI is built manually. This doc describes the **standalone scripts** (Lore, Quiz, Leaderboard, Config, etc.) and how they work without the old UI code.

---

## 1. Lore

| Function | Location | Notes |
|----------|----------|------|
| **LoreManager** | LoreManager.cs | Singleton; **allLoreData** in Inspector. **UnlockLore**, **GetUnlockedLore**, **GetLoreByCategory**. Persists via PlayerPrefs. |
| **LoreMarker.OnTriggerEnter2D** | LoreMarker.cs | Uses **LoreCardController.Instance.Show(...)** only (no LoreCardUI). Passes LoreData or marker title/shortText/image. |
| **LoreCardController.Show** | LoreCardController.cs | Sets title/body/illustration, shows **cardPanel**. Wire **cardPanel**, **titleText**, **bodyText**, **illustration**, **closeButton** in Inspector. |

---

## 2. Quiz

| Function | Location | Notes |
|----------|----------|------|
| **QuizManager.SubmitAnswer** | QuizManager.cs | Updates score, PlayerPrefs, events. No UI dependency. |
| **QuizController.ShowPostLevelQuiz** | QuizController.cs | Sets **quizPanel** active. Assign **quizPanel** in Inspector. |
| **QuizPanel.ShowQuiz** | Quiz/UI/QuizPanel.cs | Standalone MonoBehaviour. Assign questionText, choicesContainer, feedbackText, closeButton. Builds choice buttons if no prefab. |
| **MiniQuizTrigger** | MiniQuizTrigger.cs | Calls **QuizController.Instance.ShowPostLevelQuiz(forLevelScene)**. |

---

## 3. Leaderboard

| Function | Location | Notes |
|----------|----------|------|
| **LeaderboardService** | Leaderboard/LeaderboardService.cs | Load/save entries. Use from your own UI. |
| **LeaderboardModels** | LeaderboardModels.cs | **LeaderboardEntry**, **LeaderboardUser**. |

---

## 4. Integration (optional)

| Function | Location | Notes |
|----------|----------|------|
| **LakbayTalaIntegration** | LakbayTalaIntegration.cs | Buttons and CanvasGroups for lobby, map, leaderboard, settings, achievements. **settingsController**, **leaderboardController**, **achievementsController** are **MonoBehaviour**; assign your own scripts. Integration calls methods via reflection (e.g. RefreshLeaderboard, SaveSettings) if present. |

---

## 5. Summary

- **Lore**: LoreManager + LoreCardController + LoreMarker — no UI folder.
- **Quiz**: QuizManager + QuizController + QuizPanel + MiniQuizTrigger — QuizPanel is standalone.
- **Leaderboard / Config / Content / Core / Utils / View**: Standalone; wire to your own UI when ready.
- **Editor tools and Assets/Scripts/UI** have been removed; build all UI manually.
