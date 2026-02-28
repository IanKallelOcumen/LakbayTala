# GDD-Aligned Scripts (Chapter III)

Normal scripts you can apply later: data models, save/progress, level registry, and game flow. No dependency on removed UI; wire your own scenes and buttons.

## Data (`Assets/Scripts/Data/`)

| Script | Purpose (GDD 3.6) |
|--------|-------------------|
| **LevelData** | Level definition: `levelId`, `locationId`, `requiredTala`, `sceneName`, `hasQuiz`. Use in LevelRegistry. |
| **LocationIds** | Constants: `Makiling`, `Mohikap`, `Sampaloc`, `Botocan`. |
| **LoreEntry** | Lore content: `loreId`, `locationId`, `levelId`, `title`, `shortText`, `imageReference`. For JSON or mapping to LoreData. |
| **QuizItem** | Quiz content: `quizId`, `questionText`, `options[]`, `correctIndex`, `feedbackText`. Use `ToQuizQuestion()` for Quiz.Data.QuizQuestion. |
| **LakbayTalaSaveData** | Save structure: unlocked locations/levels, collected lore, gallery unlocks, sound/music volume, difficulty override. |
| **LakbayTalaProgress** | MonoBehaviour: loads/saves `LakbayTalaSaveData` as JSON. `UnlockLocation`, `CompleteLevel`, `CollectLore`, `UnlockGallery`, `IsLocationUnlocked`, etc. Put on a persistent GameObject. |
| **LevelRegistry** | MonoBehaviour: list of `LevelData`. `GetLevel(levelId)`, `GetLevelsForLocation(locationId)`. Put on a persistent GameObject. |

## Core (`Assets/Scripts/Core/`)

| Script | Purpose (GDD 3.5) |
|--------|-------------------|
| **LakbayTalaGameFlow** | Central flow: `GoToTitle()`, `GoToWorldMap()`, `StartLevel(levelId)`, `OnLevelComplete(levelId, talaCollected)`, `ReturnToMap()`, `RestartCurrentLevel()`, `HasEnoughTala(levelId, talaCollected)`. Wire menu/level-end buttons to these. |

## Existing (unchanged)

- **MasterGameManager** – Loads `Resources/config/settings.json` (movementSpeed, jumpForce, encounterRate, etc.), scene load/back, menu screens. Keep one in the first scene; DontDestroyOnLoad.
- **GameSettings** – Config fields used by GameManager and PlayerController.
- **PlayerController** – Movement/jump; reads `MasterGameManager.Instance.Settings` for speed/jump.
- **EncounterSystem** – Optional encounters; uses `Settings.encounterRate`.
- **LoreData / LoreMarker / LoreCardController** – Lore cards in levels.
- **Quiz.Data.QuizQuestion, QuizController, QuizManager** – Micro-quizzes; use QuizItem.ToQuizQuestion() if loading from JSON.

## How to apply

1. **Persistent object**  
   In your first scene (e.g. menu), create a GameObject (e.g. "GameSystems") and add:
   - `MasterGameManager` (already used)
   - `LakbayTalaProgress`
   - `LevelRegistry` – in Inspector add `LevelData` entries (levelId, locationId, requiredTala, sceneName, hasQuiz).
   - `LakbayTalaGameFlow` – assign references to GameManager, LevelRegistry, Progress if you want; they are auto-resolved if null.

2. **World map**  
   For each location button: call `LakbayTalaGameFlow.Instance.StartLevel(levelId)`.  
   Optionally show level intro (LevelData.introText) then call `StartLevel`.

3. **Level end (exit zone)**  
   When player reaches exit with enough Tala: get `levelId` (e.g. from a script on the exit), current Tala count, then:
   - `LakbayTalaGameFlow.Instance.OnLevelComplete(levelId, talaCollected)`  
   That records completion and, if level has quiz, you show quiz then call `OnQuizComplete(levelId)`; otherwise it goes to post-level summary. Your summary UI then calls `GoToWorldMap()`.

4. **Pause menu**  
   Resume / Restart / Return to Map → call `LakbayTalaGameFlow.Instance.RestartCurrentLevel()` or `ReturnToMap()`.

5. **Progress & gallery**  
   When player collects lore: `LakbayTalaProgress.Instance.CollectLore(loreId)` and optionally `UnlockGallery(entryId)`. Use `IsLocationUnlocked`, `HasLore`, etc. to drive map and gallery UI.

Config stays in `Resources/config/settings.json`; save file is written to `Application.persistentDataPath/lakbaytala_save.json`.
