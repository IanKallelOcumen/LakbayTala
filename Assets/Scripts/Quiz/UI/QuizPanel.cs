using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LakbayTala.UI.Core;
using LakbayTala.UI.Components;
using LakbayTala.Quiz.Data;
using System.Collections;

namespace LakbayTala.Quiz.UI
{
    public class QuizPanel : UIPanel
    {
        [Header("UI Elements")]
        public TextMeshProUGUI questionText;
        public Transform choicesContainer;
        public TextMeshProUGUI feedbackText;
        public Button closeButton;

        private LakbayTala.Quiz.Data.QuizQuestion currentQuestion;
        private System.Action onComplete;

        protected override void Awake()
        {
            panelType = UIPanelType.Popup; // Treat quiz as a popup
            base.Awake();
            
            if (closeButton) closeButton.onClick.AddListener(() => Hide());
        }

        public void ShowQuiz(LakbayTala.Quiz.Data.QuizQuestion question, System.Action onCompleteCallback = null)
        {
            currentQuestion = question;
            onComplete = onCompleteCallback;
            
            RenderQuestion();
            Show();
        }

        private void RenderQuestion()
        {
            if (currentQuestion == null) return;
            if (questionText) questionText.text = currentQuestion.questionText ?? "";
            if (feedbackText) feedbackText.text = "";

            if (choicesContainer == null) return;
            // Clear choices
            foreach (Transform child in choicesContainer)
            {
                Destroy(child.gameObject);
            }

            if (currentQuestion.choices == null) return;
            // Create buttons for choices
            for (int i = 0; i < currentQuestion.choices.Length; i++)
            {
                int index = i;
                ButtonFactory.CreateStandardButton(
                    $"Choice_{i}", 
                    choicesContainer, 
                    currentQuestion.choices[i], 
                    () => OnChoiceSelected(index)
                );
            }
        }

        private void OnChoiceSelected(int index)
        {
            bool isCorrect = (index == currentQuestion.correctChoiceIndex);
            
            if (feedbackText)
            {
                feedbackText.text = isCorrect ? "Correct! " : "Incorrect. ";
                feedbackText.text += currentQuestion.explanation;
                feedbackText.color = isCorrect ? Color.green : Color.red;
            }

            // Disable buttons
            foreach (Button btn in choicesContainer.GetComponentsInChildren<Button>())
            {
                btn.interactable = false;
            }

            QuizManager.Instance.SubmitAnswer(currentQuestion, index);

            StartCoroutine(CloseDelay());
        }

        private IEnumerator CloseDelay()
        {
            yield return new WaitForSeconds(2.0f);
            Hide();
            onComplete?.Invoke();
        }
    }
}
