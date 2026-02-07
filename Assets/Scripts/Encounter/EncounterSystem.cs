using UnityEngine;
using LakbayTala.Config;

namespace LakbayTala.Encounter
{
    /// <summary>
    /// Placeholder: Optional events / resource spawns using encounterRate from config.
    /// Can trigger lore hints, ambient encounters, or spawns based on encounterRate.
    /// </summary>
    public class EncounterSystem : MonoBehaviour
    {
        [Header("Placeholder – uses Settings.encounterRate when available")]
        public float overrideEncounterRate = -1f;

        float GetEncounterRate()
        {
            if (overrideEncounterRate >= 0f) return overrideEncounterRate;
            if (MasterGameManager.Instance != null && MasterGameManager.Instance.Settings != null)
                return MasterGameManager.Instance.Settings.encounterRate;
            return 0.15f;
        }

        void Start()
        {
            Debug.Log("[Lakbay Tala] EncounterSystem placeholder active. Rate: " + GetEncounterRate());
        }

        /// <summary>Placeholder: call from level logic to roll for an optional encounter.</summary>
        public bool TryTriggerEncounter()
        {
            return Random.value < GetEncounterRate();
        }
    }
}
