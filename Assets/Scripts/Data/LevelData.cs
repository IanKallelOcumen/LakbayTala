using UnityEngine;

namespace LakbayTala.Data
{
    /// <summary>
    /// GDD 3.6: Level definition for Laguna locations.
    /// Use in LevelRegistry or load from JSON; wire scene name and requirements to your level flow.
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public string levelId;
        /// <summary>e.g. Makiling, Mohikap, Sampaloc, Botocan</summary>
        public string locationId;
        /// <summary>Minimum Tala required to complete the level.</summary>
        public int requiredTala;
        /// <summary>Unity scene name to load.</summary>
        public string sceneName;
        /// <summary>If true, show a micro-quiz after level complete.</summary>
        public bool hasQuiz;

        /// <summary>Optional display name for UI.</summary>
        public string displayName;
        /// <summary>Optional intro text shown before starting the level.</summary>
        [TextArea(2, 4)]
        public string introText;
    }

    /// <summary>GDD 3.2.3: Laguna location identifiers.</summary>
    public static class LocationIds
    {
        public const string Makiling = "Makiling";
        public const string Mohikap = "Mohikap";
        public const string Sampaloc = "Sampaloc";
        public const string Botocan = "Botocan";
    }
}
