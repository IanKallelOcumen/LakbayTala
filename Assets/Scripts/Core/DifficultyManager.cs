using UnityEngine;
using LakbayTala.Config;

namespace LakbayTala.Core
{
    public class DifficultyManager : MonoBehaviour
    {
        public static DifficultyManager Instance { get; private set; }

        public DifficultyConfig easyConfig;
        public DifficultyConfig normalConfig;
        public DifficultyConfig hardConfig;

        private GameDifficulty currentDifficulty = GameDifficulty.Normal;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved difficulty
            currentDifficulty = (GameDifficulty)PlayerPrefs.GetInt("GameDifficulty", (int)GameDifficulty.Normal);
        }

        public void SetDifficulty(GameDifficulty difficulty)
        {
            currentDifficulty = difficulty;
            PlayerPrefs.SetInt("GameDifficulty", (int)difficulty);
            PlayerPrefs.Save();
            Debug.Log($"Difficulty set to: {difficulty}");
        }

        public DifficultyConfig GetCurrentConfig()
        {
            switch (currentDifficulty)
            {
                case GameDifficulty.Easy: return easyConfig;
                case GameDifficulty.Hard: return hardConfig;
                default: return normalConfig;
            }
        }
    }
}
