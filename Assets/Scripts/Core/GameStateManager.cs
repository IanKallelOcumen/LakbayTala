using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LakbayTala.Core
{
    [Serializable]
    public class GameData
    {
        public int totalScore;
        public int currentLevel;
        public float playTime;
        public int lives;
        // Add more fields as needed (inventory, unlocked items, etc.)
    }

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        private GameData currentGameData;
        private string savePath;

        public event Action<GameData> OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/gamesave.dat";
            LoadGame();
        }

        public void SaveGame()
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(savePath);
                bf.Serialize(file, currentGameData);
                file.Close();
                Debug.Log("Game Saved");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save game: " + e.Message);
            }
        }

        public void LoadGame()
        {
            if (File.Exists(savePath))
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(savePath, FileMode.Open);
                    currentGameData = (GameData)bf.Deserialize(file);
                    file.Close();
                    Debug.Log("Game Loaded");
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to load game: " + e.Message);
                    CreateNewGame();
                }
            }
            else
            {
                CreateNewGame();
            }
            
            OnGameStateChanged?.Invoke(currentGameData);
        }

        public void CreateNewGame()
        {
            currentGameData = new GameData
            {
                totalScore = 0,
                currentLevel = 1,
                playTime = 0,
                lives = 3
            };
            SaveGame();
        }

        public GameData GetData()
        {
            return currentGameData;
        }

        public void UpdateScore(int points)
        {
            currentGameData.totalScore += points;
            OnGameStateChanged?.Invoke(currentGameData);
        }
        
        // Auto-save on quit
        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}
