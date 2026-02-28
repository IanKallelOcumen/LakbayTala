using UnityEngine;

namespace LakbayTala.Data
{
    /// <summary>
    /// GDD 3.6: Quiz item for post-level micro-quizzes.
    /// QuestionText, Options, CorrectIndex, FeedbackText. Can be loaded from JSON or ScriptableObjects.
    /// Map to LakbayTala.Quiz.Data.QuizQuestion when using QuizManager/QuizController.
    /// </summary>
    [System.Serializable]
    public class QuizItem
    {
        public string quizId;
        public string questionText;
        public string[] options;
        /// <summary>0-based index of the correct option.</summary>
        public int correctIndex;
        /// <summary>Shown after answering (explanation).</summary>
        public string feedbackText;

        /// <summary>Convert to Quiz.Data.QuizQuestion for use with quiz UI.</summary>
        public LakbayTala.Quiz.Data.QuizQuestion ToQuizQuestion(int scoreValue = 1)
        {
            return new LakbayTala.Quiz.Data.QuizQuestion
            {
                questionText = questionText,
                choices = options ?? System.Array.Empty<string>(),
                correctChoiceIndex = Mathf.Clamp(correctIndex, 0, (options != null ? options.Length : 0) - 1),
                scoreValue = scoreValue,
                explanation = feedbackText ?? ""
            };
        }
    }
}
