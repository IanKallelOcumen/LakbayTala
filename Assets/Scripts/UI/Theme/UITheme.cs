using UnityEngine;
using TMPro;

namespace LakbayTala.UI.Theme
{
    [CreateAssetMenu(fileName = "New UI Theme", menuName = "LakbayTala/UI/Theme")]
    public class UITheme : ScriptableObject
    {
        [Header("Fonts")]
        public TMP_FontAsset mainFont; // BrawlStars Font
        public TMP_FontAsset secondaryFont;
        
        [Header("Colors")]
        public Color primaryColor = new Color(1f, 0.8f, 0f); // Yellow/Gold
        public Color secondaryColor = new Color(0f, 0.6f, 1f); // Blue
        public Color accentColor = new Color(1f, 0.2f, 0.2f); // Red
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.9f); // Dark Blue Overlay
        public Color textColor = Color.white;
        
        [Header("Button Styles")]
        public Sprite primaryButtonSprite;
        public Sprite secondaryButtonSprite;
        public Sprite closeButtonSprite;
        
        [Header("Panel Styles")]
        public Sprite panelBackgroundSprite;
        public float cornerRadius = 20f;
    }
}
