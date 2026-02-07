using UnityEngine;

namespace LakbayTala.Quiz
{
    /// <summary>
    /// Placeholder: Post-level micro-quiz (3–5 items). Wire to a UI with questions and answer buttons.
    /// Show after level complete when HasQuiz is true for the level.
    /// </summary>
    public class QuizController : MonoBehaviour
    {
        [Header("Placeholder – wire in Inspector")]
        public GameObject quizPanel;
        public string levelSceneName;

        public static QuizController Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (quizPanel != null) quizPanel.SetActive(false);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Placeholder: open quiz UI for the given level. Implement question/answer flow later.</summary>
        public void ShowPostLevelQuiz(string forLevelScene)
        {
            levelSceneName = forLevelScene;
            if (quizPanel != null) quizPanel.SetActive(true);
            Debug.Log("[Lakbay Tala] Quiz placeholder for level: " + forLevelScene);
        }

        public void HideAndReturnToMap()
        {
            if (quizPanel != null) quizPanel.SetActive(false);
            if (MasterGameManager.Instance != null)
                MasterGameManager.Instance.BackToMainMenu();
        }
    }
}
