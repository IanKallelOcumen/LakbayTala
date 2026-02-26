using UnityEngine;
using System.Collections.Generic;

namespace LakbayTala.Quiz.Data
{
    [CreateAssetMenu(fileName = "New Quiz Collection", menuName = "LakbayTala/Quiz/Quiz Collection")]
    public class QuizCollection : ScriptableObject
    {
        public string topicName;
        public QuizDifficulty difficulty;
        public List<QuizQuestion> questions;
    }
}
