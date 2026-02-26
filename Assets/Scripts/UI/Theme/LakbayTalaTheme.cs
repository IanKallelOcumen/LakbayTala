using UnityEngine;
using TMPro;

namespace LakbayTala.UI.Theme
{
    [CreateAssetMenu(fileName = "LakbayTalaTheme", menuName = "LakbayTala/UI/LakbayTala Theme")]
    public class LakbayTalaTheme : UITheme
    {
        // Lakbay Tala specific overrides or additional settings
        // Unique Color Palette (inspired by Philippine colors / modern minimal)
        
        public LakbayTalaTheme()
        {
            // Override defaults in constructor or via Inspector
            // Deep Blue / Gold / White theme
            primaryColor = new Color32(0, 56, 168, 255); // Philippine Blue
            secondaryColor = new Color32(252, 209, 22, 255); // Philippine Sun Yellow
            accentColor = new Color32(206, 17, 38, 255); // Philippine Red
            backgroundColor = new Color32(240, 240, 245, 240); // Off-white/Light Gray for clean look
            textColor = new Color32(30, 30, 30, 255); // Dark text for readability on light bg
            cornerRadius = 15f;
        }
    }
}
