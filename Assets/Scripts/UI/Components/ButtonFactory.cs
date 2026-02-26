using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LakbayTala.UI.Theme;
using TMPro;

namespace LakbayTala.UI.Components
{
    public static class ButtonFactory
    {
        public static GameObject CreateButton(string name, Transform parent, string text, UnityAction onClick, Vector2 size, Vector2 position, Color normalColor, Color highlightedColor, Color pressedColor)
        {
            // Create Button Object
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            // Add RectTransform
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            // Add Image (Background)
            Image image = buttonObj.AddComponent<Image>();
            
            // Apply Theme Sprite if available
            if (UIThemeManager.Instance != null && UIThemeManager.Instance.currentTheme != null)
            {
                if (UIThemeManager.Instance.currentTheme.primaryButtonSprite != null)
                {
                    image.sprite = UIThemeManager.Instance.currentTheme.primaryButtonSprite;
                    image.type = Image.Type.Sliced; // Ensure slicing for UI scaling
                }
                else
                {
                    image.color = normalColor;
                }
            }
            else
            {
                image.color = normalColor;
            }

            // Add Button Component
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            // Configure Button Colors
            ColorBlock colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = highlightedColor;
            colors.pressedColor = pressedColor;
            colors.selectedColor = highlightedColor;
            colors.disabledColor = Color.gray;
            colors.colorMultiplier = 1;
            colors.fadeDuration = 0.1f;
            button.colors = colors;
            
            // Add Shadow/Outline for polish
            Shadow shadow = buttonObj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(2, -2);

            // Add OnClick Listener
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            // Create Text Object (TMP)
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            // Add RectTransform for Text
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = new Vector2(10, 5); // Padding
            textRect.offsetMax = new Vector2(-10, -5);

            // Add Text Component (TextMeshProUGUI)
            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.fontSize = 24;
            buttonText.textWrappingMode = TextWrappingModes.NoWrap;
            
            // Apply Theme Font
            if (UIThemeManager.Instance != null && UIThemeManager.Instance.currentTheme != null)
            {
                UIThemeManager.Instance.ApplyThemeToText(buttonText);
            }
            else
            {
                buttonText.color = Color.black;
            }

            return buttonObj;
        }

        // Overload for simpler creation with defaults
        public static GameObject CreateStandardButton(string name, Transform parent, string text, UnityAction onClick)
        {
            Color primary = Color.white;
            if (UIThemeManager.Instance != null && UIThemeManager.Instance.currentTheme != null)
            {
                // Ensure we use the theme's primary color, which is now distinctly Lakbay Tala (Blue/Gold)
                primary = UIThemeManager.Instance.currentTheme.primaryColor;
            }

            return CreateButton(
                name, 
                parent, 
                text, 
                onClick, 
                new Vector2(250, 70), // Larger, more prominent buttons
                Vector2.zero, 
                primary, 
                Color.Lerp(primary, Color.white, 0.2f), // Auto-highlight
                Color.Lerp(primary, Color.black, 0.2f)  // Auto-press
            );
        }
    }
}
