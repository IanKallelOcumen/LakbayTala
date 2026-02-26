using UnityEngine;
using TMPro;

namespace LakbayTala.UI.Theme
{
    public class UIThemeManager : MonoBehaviour
    {
        public static UIThemeManager Instance { get; private set; }

        public UITheme currentTheme;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ApplyThemeToText(TextMeshProUGUI text, bool isTitle = false)
        {
            if (currentTheme == null) return;
            
            if (currentTheme.mainFont != null)
                text.font = currentTheme.mainFont;
                
            text.color = currentTheme.textColor;
            
            if (isTitle)
            {
                text.fontSizeMax = 64;
                text.enableVertexGradient = true;
                text.colorGradient = new VertexGradient(currentTheme.primaryColor, currentTheme.primaryColor, currentTheme.secondaryColor, currentTheme.secondaryColor);
                // Underlay via material instance (TMP underlay is on the shader, not the component in newer versions)
                if (text.fontMaterial != null && text.fontMaterial.HasProperty("_UnderlayColor"))
                {
                    text.fontMaterial.EnableKeyword("UNDERLAY_ON");
                    text.fontMaterial.SetColor("_UnderlayColor", Color.black);
                    text.fontMaterial.SetFloat("_UnderlayDilate", 0.5f);
                }
            }
        }

        public void ApplyThemeToPanel(UnityEngine.UI.Image panelImage)
        {
            if (currentTheme == null) return;
            
            if (currentTheme.panelBackgroundSprite != null)
                panelImage.sprite = currentTheme.panelBackgroundSprite;
            else
                panelImage.color = currentTheme.backgroundColor;
        }
    }
}
