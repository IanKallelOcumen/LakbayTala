using UnityEngine;
using Platformer.Mechanics;

namespace LakbayTala.Quiz
{
    /// <summary>
    /// Place this in the game scene with a Collider2D (trigger). When the player enters, the mini quiz panel opens.
    /// Assign quizPanel on QuizController in the scene (or it will use QuizController.Instance.quizPanel).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MiniQuizTrigger : MonoBehaviour
    {
        [Tooltip("Scene name passed to quiz (e.g. GameScene).")]
        public string forLevelScene = "GameScene";

        [Tooltip("Only trigger once per session.")]
        public bool triggerOnce = true;
        private bool triggered;

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null) return;
            if (triggerOnce && triggered) return;
            triggered = true;

            if (QuizController.Instance != null)
            {
                QuizController.Instance.ShowPostLevelQuiz(forLevelScene);
            }
            else
            {
                Debug.LogWarning("[MiniQuizTrigger] QuizController not in scene. Add QuizController to the Game scene and assign its quizPanel.");
            }
        }
    }
}
