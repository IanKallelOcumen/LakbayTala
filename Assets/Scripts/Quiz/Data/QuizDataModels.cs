namespace LakbayTala.Quiz.Data
{
    public enum QuizDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public string[] choices; // 4 choices typically
        public int correctChoiceIndex;
        public int scoreValue;
        public string explanation; // Shown after answering
    }
}
