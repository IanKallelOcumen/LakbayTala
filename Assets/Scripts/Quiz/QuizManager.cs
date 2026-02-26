using UnityEngine;
using System.Collections.Generic;
using LakbayTala.Quiz.Data;

namespace LakbayTala.Quiz
{
    public class QuizManager : MonoBehaviour
    {
        public static QuizManager Instance { get; private set; }

        public List<QuizCollection> allQuizzes;
        
        public event System.Action<int> OnQuizScoreUpdated;
        public event System.Action<LakbayTala.Quiz.Data.QuizQuestion> OnQuestionAnswered;

        private int currentSessionScore = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public QuizCollection GetRandomQuiz(QuizDifficulty difficulty)
        {
            // Simple random selection
            var matching = allQuizzes.FindAll(q => q.difficulty == difficulty);
            if (matching.Count == 0) return null;
            return matching[Random.Range(0, matching.Count)];
        }

        public void SubmitAnswer(LakbayTala.Quiz.Data.QuizQuestion question, int choiceIndex)
        {
            bool isCorrect = (choiceIndex == question.correctChoiceIndex);
            if (isCorrect)
            {
                currentSessionScore += question.scoreValue;
                // Add to total persistent score
                int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
                PlayerPrefs.SetInt("TotalScore", totalScore + question.scoreValue);
                PlayerPrefs.Save();
            }
            
            OnQuestionAnswered?.Invoke(question);
            OnQuizScoreUpdated?.Invoke(currentSessionScore);
        }
    }
}
