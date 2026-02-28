using UnityEngine;
using UnityEngine.SceneManagement;
using LakbayTala.Data;

namespace LakbayTala.Core
{
    /// <summary>
    /// GDD 3.5: Central game flow — Title, World Map, Level Intro, Gameplay, Level Complete, Quiz, Post-Level Summary, Gallery, Settings.
    /// Wire your UI buttons to these methods; they use MasterGameManager for scenes and LakbayTalaProgress for save data.
    /// Apply to a persistent GameObject (e.g. with GameManager) or call statically after resolving references.
    /// </summary>
    public class LakbayTalaGameFlow : MonoBehaviour
    {
        public static LakbayTalaGameFlow Instance { get; private set; }

        [Header("Scene names (GDD 3.5)")]
        public string titleSceneName = "MenuScene";
        public string worldMapSceneName = "MenuScene";
        [Tooltip("If world map is same as title, use same scene and switch screens via UI.")]
        public bool mapIsSameAsTitle = true;

        [Header("Optional references (resolved at runtime if null)")]
        public MasterGameManager gameManager;
        public LevelRegistry levelRegistry;
        public LakbayTalaProgress progress;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResolveReferences();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void ResolveReferences()
        {
            if (gameManager == null) gameManager = FindFirstObjectByType<MasterGameManager>();
            if (levelRegistry == null) levelRegistry = FindFirstObjectByType<LevelRegistry>();
            if (progress == null) progress = FindFirstObjectByType<LakbayTalaProgress>();
        }

        /// <summary>GDD: Title screen — Play, Gallery, Settings, Exit.</summary>
        public void GoToTitle()
        {
            ResolveReferences();
            if (gameManager != null)
                gameManager.BackToMainMenu();
            else
                SceneManager.LoadScene(titleSceneName);
        }

        /// <summary>GDD: World map — select location/level.</summary>
        public void GoToWorldMap()
        {
            ResolveReferences();
            if (gameManager != null && mapIsSameAsTitle)
                gameManager.OnMap();
            else if (gameManager != null)
                gameManager.LoadLevel(worldMapSceneName);
            else
                SceneManager.LoadScene(worldMapSceneName);
        }

        /// <summary>GDD: Start level by levelId. Loads LevelData.sceneName; optionally show intro in UI first.</summary>
        public void StartLevel(string levelId)
        {
            ResolveReferences();
            LevelData level = levelRegistry != null ? levelRegistry.GetLevel(levelId) : null;
            string sceneName = level != null ? level.sceneName : levelId;
            if (gameManager != null)
                gameManager.LoadLevel(sceneName);
            else
                SceneManager.LoadScene(sceneName);
        }

        /// <summary>GDD: Level complete — record completion, optionally show quiz then post-level summary. Call when player reaches exit with enough Tala.</summary>
        public void OnLevelComplete(string levelId, int talaCollected)
        {
            ResolveReferences();
            if (progress != null)
            {
                progress.CompleteLevel(levelId);
                progress.AddTala(talaCollected);
            }
            LevelData level = levelRegistry != null ? levelRegistry.GetLevel(levelId) : null;
            if (level != null && level.hasQuiz)
            {
                // Wire your quiz UI here: show quiz for level, then OnQuizComplete();
                Debug.Log("[LakbayTala] Level complete. Quiz required — wire your quiz flow then call OnQuizComplete or GoToWorldMap.");
                return;
            }
            GoToPostLevelSummary(levelId);
        }

        /// <summary>After quiz (or when no quiz). Return to map from your summary UI by calling GoToWorldMap().</summary>
        public void OnQuizComplete(string levelId)
        {
            GoToPostLevelSummary(levelId);
        }

        /// <summary>GDD: Post-level summary — show score/unlocks; UI should call GoToWorldMap() when done.</summary>
        public void GoToPostLevelSummary(string levelId)
        {
            // No scene change; your summary panel shows in current scene or you load a summary scene.
            Debug.Log("[LakbayTala] Post-level summary for " + levelId + " — show UI then call GoToWorldMap().");
        }

        /// <summary>GDD: Return to map from gameplay (pause menu or level end).</summary>
        public void ReturnToMap()
        {
            Time.timeScale = 1f;
            GoToWorldMap();
        }

        /// <summary>GDD: Restart current level.</summary>
        public void RestartCurrentLevel()
        {
            ResolveReferences();
            if (gameManager != null)
                gameManager.ReloadCurrentLevel();
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>Check if player has enough Tala to complete level (for exit zone).</summary>
        public bool HasEnoughTala(string levelId, int talaCollected)
        {
            LevelData level = levelRegistry != null ? levelRegistry.GetLevel(levelId) : null;
            if (level == null) return true;
            return talaCollected >= level.requiredTala;
        }
    }
}
