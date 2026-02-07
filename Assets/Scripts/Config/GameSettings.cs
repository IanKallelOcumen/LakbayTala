using System;
using UnityEngine;

namespace LakbayTala.Config
{
    /// <summary>
    /// Runtime settings loaded from Resources/config/settings.json.
    /// Used by GameManager at boot for movement, jump, and encounter tuning.
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        public string version = "1.0";
        public float movementSpeed = 7f;
        public float jumpForce = 7f;
        public float encounterRate = 0.15f;
        public float resourceSpawnRate = 1f;
        public string difficulty = "normal";
        public int seed = 0;
    }
}
