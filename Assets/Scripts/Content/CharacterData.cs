using UnityEngine;
using System.Collections.Generic;

namespace LakbayTala.Content
{
    public enum CharacterType
    {
        Hero,
        Ally,
        Enemy,
        Boss
    }

    [CreateAssetMenu(fileName = "New Character Data", menuName = "LakbayTala/Content/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string id;
        public string characterName;
        public CharacterType type;
        public GameObject prefab;
        public Sprite portrait;
        
        [Header("Stats")]
        public float baseHealth;
        public float baseDamage;
        public float moveSpeed;
        
        [Header("Abilities")]
        public List<AbilityData> abilities;
    }

    [System.Serializable]
    public class AbilityData
    {
        public string name;
        public float cooldown;
        public float damage;
        public Sprite icon;
    }
}
