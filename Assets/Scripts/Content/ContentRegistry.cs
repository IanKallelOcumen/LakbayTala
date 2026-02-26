using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LakbayTala.Content
{
    public class ContentRegistry : MonoBehaviour
    {
        public static ContentRegistry Instance { get; private set; }

        public List<CharacterData> allCharacters;
        
        private Dictionary<string, CharacterData> characterLookup;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            BuildLookup();
        }

        private void BuildLookup()
        {
            characterLookup = new Dictionary<string, CharacterData>();
            foreach (var charData in allCharacters)
            {
                if (!characterLookup.ContainsKey(charData.id))
                {
                    characterLookup.Add(charData.id, charData);
                }
            }
        }

        public CharacterData GetCharacter(string id)
        {
            if (characterLookup.TryGetValue(id, out var data))
            {
                return data;
            }
            return null;
        }

        public List<CharacterData> GetCharactersByType(CharacterType type)
        {
            return allCharacters.Where(c => c.type == type).ToList();
        }
    }
}
