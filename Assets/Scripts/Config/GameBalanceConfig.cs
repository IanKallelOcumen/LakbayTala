using UnityEngine;

namespace LakbayTala.Config
{
    [CreateAssetMenu(fileName = "Game Balance Config", menuName = "LakbayTala/Config/Game Balance")]
    public class GameBalanceConfig : ScriptableObject
    {
        [Header("Experience & Leveling")]
        public int[] experienceToNextLevel;
        public float expMultiplier = 1.0f;

        [Header("Scoring")]
        public int pointsPerEnemy = 100;
        public int pointsPerQuiz = 500;
        public int pointsPerCollectible = 50;

        [Header("Drop Rates")]
        [Range(0, 1)] public float healthPotionDropRate = 0.1f;
        [Range(0, 1)] public float loreCardDropRate = 0.05f;
    }
}
