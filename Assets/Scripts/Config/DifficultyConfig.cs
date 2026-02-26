using UnityEngine;

namespace LakbayTala.Config
{
    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard
    }

    [CreateAssetMenu(fileName = "Game Difficulty Config", menuName = "LakbayTala/Config/Difficulty")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("General")]
        public GameDifficulty difficultyLevel;
        public float scoreMultiplier = 1.0f;

        [Header("Enemies")]
        public float enemyDamageMultiplier = 1.0f;
        public float enemyHealthMultiplier = 1.0f;
        public float enemySpeedMultiplier = 1.0f;

        [Header("Player")]
        public int startingLives = 3;
        public float healthRegenRate = 0f;
    }
}
