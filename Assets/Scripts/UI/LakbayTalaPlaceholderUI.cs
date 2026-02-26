using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LakbayTala Placeholder UI Manager - Creates and manages placeholder UI elements
/// with Filipino cultural aesthetic for development and testing
/// </summary>
public class LakbayTalaPlaceholderUI : MonoBehaviour
{
    [Header("Placeholder Sprites")]
    public Sprite[] baybayinPlaceholders;
    public Sprite[] mythologicalPlaceholders;
    public Sprite[] landscapePlaceholders;
    public Sprite[] patternPlaceholders;
    public Sprite[] decorativeBorderPlaceholders;
    
    [Header("UI Element Templates")]
    public GameObject buttonTemplate;
    public GameObject panelTemplate;
    public GameObject textTemplate;
    public GameObject imageTemplate;
    
    [Header("Cultural Colors")]
    public Color primaryColor = new Color(0.9f, 0.7f, 0.3f);      // Golden yellow
    public Color secondaryColor = new Color(0.2f, 0.4f, 0.6f);   // Deep blue
    public Color accentColor = new Color(0.8f, 0.3f, 0.2f);     // Warm red
    public Color backgroundColor = new Color(0.95f, 0.92f, 0.88f); // Cream parchment
    public Color textColor = new Color(0.2f, 0.15f, 0.1f);       // Dark brown
    
    [Header("Development Settings")]
    public bool autoGeneratePlaceholders = true;
    public bool showPlaceholderLabels = true;
    public bool useCulturalColors = true;
    
    private static LakbayTalaPlaceholderUI instance;
    public static LakbayTalaPlaceholderUI Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<LakbayTalaPlaceholderUI>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (autoGeneratePlaceholders)
        {
            GeneratePlaceholderSprites();
        }
    }

    /// <summary>
    /// Generate placeholder sprites with Filipino cultural aesthetic
    /// </summary>
    private void GeneratePlaceholderSprites()
    {
        if (baybayinPlaceholders == null || baybayinPlaceholders.Length == 0)
        {
            baybayinPlaceholders = new Sprite[8];
            for (int i = 0; i < 8; i++)
            {
                baybayinPlaceholders[i] = CreateBaybayinPlaceholder($"Baybayin_{i}");
            }
        }
        
        if (mythologicalPlaceholders == null || mythologicalPlaceholders.Length == 0)
        {
            mythologicalPlaceholders = new Sprite[12];
            for (int i = 0; i < 12; i++)
            {
                mythologicalPlaceholders[i] = CreateMythologicalPlaceholder($"Myth_{i}");
            }
        }
        
        if (landscapePlaceholders == null || landscapePlaceholders.Length == 0)
        {
            landscapePlaceholders = new Sprite[6];
            string[] locations = { "Mount Makiling", "Lake Mohikap", "Sampaloc Lake", "Botocan Falls", "Forest", "Sky" };
            for (int i = 0; i < 6; i++)
            {
                landscapePlaceholders[i] = CreateLandscapePlaceholder(locations[i]);
            }
        }
        
        if (patternPlaceholders == null || patternPlaceholders.Length == 0)
        {
            patternPlaceholders = new Sprite[10];
            for (int i = 0; i < 10; i++)
            {
                patternPlaceholders[i] = CreatePatternPlaceholder($"Pattern_{i}");
            }
        }
        
        if (decorativeBorderPlaceholders == null || decorativeBorderPlaceholders.Length == 0)
        {
            decorativeBorderPlaceholders = new Sprite[4];
            for (int i = 0; i < 4; i++)
            {
                decorativeBorderPlaceholders[i] = CreateBorderPlaceholder($"Border_{i}");
            }
        }
    }

    /// <summary>
    /// Create a Baybayin-inspired placeholder sprite
    /// </summary>
    private Sprite CreateBaybayinPlaceholder(string name)
    {
        Texture2D texture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        
        // Create Baybayin-inspired geometric pattern
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                int index = y * 128 + x;
                
                // Create curved lines reminiscent of Baybayin script
                float curve1 = Mathf.Sin(x * 0.1f) * 20f;
                float curve2 = Mathf.Cos(y * 0.08f) * 15f;
                
                if (Mathf.Abs(y - 64 + curve1) < 3f || Mathf.Abs(x - 64 + curve2) < 3f)
                {
                    pixels[index] = primaryColor;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        sprite.name = name;
        return sprite;
    }

    /// <summary>
    /// Create a mythological creature placeholder sprite
    /// </summary>
    private Sprite CreateMythologicalPlaceholder(string name)
    {
        Texture2D texture = new Texture2D(256, 256);
        Color[] pixels = new Color[256 * 256];
        
        // Create stylized creature silhouette
        Color creatureColor = Color.Lerp(primaryColor, secondaryColor, Random.Range(0f, 1f));
        
        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                int index = y * 256 + x;
                
                // Create organic creature shape
                float centerX = 128f;
                float centerY = 128f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                // Add organic variations
                float noise = Mathf.PerlinNoise(x * 0.02f, y * 0.02f) * 30f;
                float radius = 80f + noise;
                
                if (distance < radius && distance > radius * 0.3f)
                {
                    pixels[index] = creatureColor;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        sprite.name = name;
        return sprite;
    }

    /// <summary>
    /// Create a landscape placeholder sprite
    /// </summary>
    private Sprite CreateLandscapePlaceholder(string locationName)
    {
        Texture2D texture = new Texture2D(512, 256);
        Color[] pixels = new Color[512 * 256];
        
        // Create landscape based on location
        Color skyColor = new Color(0.6f, 0.7f, 0.9f, 0.8f);
        Color mountainColor = new Color(0.4f, 0.3f, 0.2f, 0.9f);
        Color waterColor = new Color(0.2f, 0.4f, 0.6f, 0.8f);
        Color forestColor = new Color(0.3f, 0.5f, 0.3f, 0.9f);
        
        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                int index = y * 512 + x;
                
                if (locationName.Contains("Mount"))
                {
                    // Mountain landscape
                    if (y > 128 + Mathf.Sin(x * 0.01f) * 30f)
                        pixels[index] = mountainColor;
                    else if (y > 100)
                        pixels[index] = forestColor;
                    else
                        pixels[index] = skyColor;
                }
                else if (locationName.Contains("Lake") || locationName.Contains("Falls"))
                {
                    // Water landscape
                    if (y > 180)
                        pixels[index] = skyColor;
                    else if (y > 150)
                        pixels[index] = waterColor;
                    else
                        pixels[index] = forestColor;
                }
                else
                {
                    // Forest landscape
                    if (y > 180)
                        pixels[index] = skyColor;
                    else
                        pixels[index] = forestColor;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 512, 256), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        sprite.name = locationName;
        return sprite;
    }

    /// <summary>
    /// Create a traditional pattern placeholder sprite
    /// </summary>
    private Sprite CreatePatternPlaceholder(string name)
    {
        Texture2D texture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        
        // Create repeating traditional pattern
        Color patternColor = Color.Lerp(accentColor, backgroundColor, 0.5f);
        
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                int index = y * 128 + x;
                
                // Create geometric repeating pattern
                float patternX = Mathf.Sin(x * 0.2f) * Mathf.Cos(y * 0.2f);
                float patternY = Mathf.Cos(x * 0.15f) * Mathf.Sin(y * 0.15f);
                
                if (Mathf.Abs(patternX + patternY) > 0.5f)
                {
                    pixels[index] = patternColor;
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        sprite.name = name;
        return sprite;
    }

    /// <summary>
    /// Create a decorative border placeholder sprite
    /// </summary>
    private Sprite CreateBorderPlaceholder(string name)
    {
        Texture2D texture = new Texture2D(256, 64);
        Color[] pixels = new Color[256 * 64];
        
        // Create decorative border pattern
        Color borderColor = primaryColor;
        
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                int index = y * 256 + x;
                
                // Create decorative border elements
                if (y < 8 || y > 56) // Top and bottom borders
                {
                    float decoration = Mathf.Sin(x * 0.1f) * 4f;
                    if (Mathf.Abs(y - 32f + decoration) < 2f)
                    {
                        pixels[index] = borderColor;
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
                else if (x < 8 || x > 248) // Side borders
                {
                    float decoration = Mathf.Cos(y * 0.2f) * 2f;
                    if (Mathf.Abs(x - 128f + decoration) < 1f)
                    {
                        pixels[index] = borderColor;
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
                else
                {
                    pixels[index] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 256, 64), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        sprite.name = name;
        return sprite;
    }

    /// <summary>
    /// Create a culturally-themed button with placeholder elements
    /// </summary>
    public GameObject CreateCulturalButton(string text, Transform parent)
    {
        GameObject button = new GameObject($"CulturalButton_{text}");
        button.transform.SetParent(parent, false);
        
        // Add Button component
        Button buttonComponent = button.AddComponent<Button>();
        
        // Create background image
        GameObject background = new GameObject("Background");
        background.transform.SetParent(button.transform, false);
        Image bgImage = background.AddComponent<Image>();
        
        if (patternPlaceholders.Length > 0)
        {
            bgImage.sprite = patternPlaceholders[Random.Range(0, patternPlaceholders.Length)];
            bgImage.color = new Color(1f, 1f, 1f, 0.3f); // Subtle pattern
        }
        else
        {
            bgImage.color = useCulturalColors ? primaryColor : Color.white;
        }
        
        // Create text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(button.transform, false);
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.color = useCulturalColors ? textColor : Color.black;
        textComponent.fontSize = 24;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Set up RectTransforms
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200f, 60f);
        
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        if (showPlaceholderLabels)
        {
            button.name = $"CulturalButton_{text}_Placeholder";
        }
        
        return button;
    }

    /// <summary>
    /// Create a culturally-themed panel with placeholder elements
    /// </summary>
    public GameObject CreateCulturalPanel(string name, Vector2 size, Transform parent)
    {
        GameObject panel = new GameObject($"CulturalPanel_{name}");
        panel.transform.SetParent(parent, false);
        
        // Add Image component for background
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = useCulturalColors ? backgroundColor : Color.white;
        
        // Add decorative border if available
        if (decorativeBorderPlaceholders.Length > 0)
        {
            GameObject border = new GameObject("DecorativeBorder");
            border.transform.SetParent(panel.transform, false);
            Image borderImage = border.AddComponent<Image>();
            borderImage.sprite = decorativeBorderPlaceholders[Random.Range(0, decorativeBorderPlaceholders.Length)];
            borderImage.color = accentColor;
            
            RectTransform borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.sizeDelta = Vector2.zero;
            borderRect.anchoredPosition = Vector2.zero;
        }
        
        // Set panel size
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = size;
        
        if (showPlaceholderLabels)
        {
            panel.name = $"CulturalPanel_{name}_Placeholder";
        }
        
        return panel;
    }

    /// <summary>
    /// Create a culturally-themed text element
    /// </summary>
    public GameObject CreateCulturalText(string text, Vector2 size, Transform parent)
    {
        GameObject textObject = new GameObject($"CulturalText_{text}");
        textObject.transform.SetParent(parent, false);
        
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = text;
        textComponent.color = useCulturalColors ? textColor : Color.black;
        textComponent.fontSize = 18;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        // Add subtle background pattern if available
        if (patternPlaceholders.Length > 0)
        {
            GameObject bg = new GameObject("TextBackground");
            bg.transform.SetParent(textObject.transform, false);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.sprite = patternPlaceholders[Random.Range(0, patternPlaceholders.Length)];
            bgImage.color = new Color(1f, 1f, 1f, 0.1f);
            
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
        }
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.sizeDelta = size;
        
        if (showPlaceholderLabels)
        {
            textObject.name = $"CulturalText_{text}_Placeholder";
        }
        
        return textObject;
    }

    /// <summary>
    /// Create a culturally-themed image element
    /// </summary>
    public GameObject CreateCulturalImage(Sprite sprite, Vector2 size, Transform parent)
    {
        GameObject imageObject = new GameObject($"CulturalImage_{sprite.name}");
        imageObject.transform.SetParent(parent, false);
        
        Image imageComponent = imageObject.AddComponent<Image>();
        imageComponent.sprite = sprite;
        imageComponent.color = Color.white;
        imageComponent.preserveAspect = true;
        
        // Add decorative frame if available
        if (decorativeBorderPlaceholders.Length > 0)
        {
            GameObject frame = new GameObject("DecorativeFrame");
            frame.transform.SetParent(imageObject.transform, false);
            Image frameImage = frame.AddComponent<Image>();
            frameImage.sprite = decorativeBorderPlaceholders[Random.Range(0, decorativeBorderPlaceholders.Length)];
            frameImage.color = secondaryColor;
            
            RectTransform frameRect = frame.GetComponent<RectTransform>();
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.sizeDelta = Vector2.zero;
            frameRect.anchoredPosition = Vector2.zero;
        }
        
        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.sizeDelta = size;
        
        if (showPlaceholderLabels)
        {
            imageObject.name = $"CulturalImage_{sprite.name}_Placeholder";
        }
        
        return imageObject;
    }

    /// <summary>
    /// Get a random cultural placeholder sprite
    /// </summary>
    public Sprite GetRandomCulturalSprite(string category)
    {
        switch (category.ToLower())
        {
            case "baybayin":
                return baybayinPlaceholders != null && baybayinPlaceholders.Length > 0 ? 
                       baybayinPlaceholders[Random.Range(0, baybayinPlaceholders.Length)] : null;
            case "mythological":
                return mythologicalPlaceholders != null && mythologicalPlaceholders.Length > 0 ? 
                       mythologicalPlaceholders[Random.Range(0, mythologicalPlaceholders.Length)] : null;
            case "landscape":
                return landscapePlaceholders != null && landscapePlaceholders.Length > 0 ? 
                       landscapePlaceholders[Random.Range(0, landscapePlaceholders.Length)] : null;
            case "pattern":
                return patternPlaceholders != null && patternPlaceholders.Length > 0 ? 
                       patternPlaceholders[Random.Range(0, patternPlaceholders.Length)] : null;
            case "border":
                return decorativeBorderPlaceholders != null && decorativeBorderPlaceholders.Length > 0 ? 
                       decorativeBorderPlaceholders[Random.Range(0, decorativeBorderPlaceholders.Length)] : null;
            default:
                return mythologicalPlaceholders != null && mythologicalPlaceholders.Length > 0 ? 
                       mythologicalPlaceholders[Random.Range(0, mythologicalPlaceholders.Length)] : null;
        }
    }

    /// <summary>
    /// Create a complete placeholder UI panel for testing
    /// </summary>
    public GameObject CreateTestPanel(string panelName, Transform parent)
    {
        GameObject panel = CreateCulturalPanel(panelName, new Vector2(400f, 300f), parent);
        
        // Add title
        GameObject title = CreateCulturalText($"{panelName} Panel", new Vector2(300f, 50f), panel.transform);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 120f);
        
        // Add decorative image
        if (mythologicalPlaceholders.Length > 0)
        {
            GameObject decoration = CreateCulturalImage(mythologicalPlaceholders[0], new Vector2(100f, 100f), panel.transform);
            decoration.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 50f);
        }
        
        // Add buttons
        GameObject button1 = CreateCulturalButton("Simulan", panel.transform);
        button1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80f, -50f);
        
        GameObject button2 = CreateCulturalButton("Balik", panel.transform);
        button2.GetComponent<RectTransform>().anchoredPosition = new Vector2(80f, -50f);
        
        // Add status text
        GameObject status = CreateCulturalText("Placeholder UI for Testing", new Vector2(350f, 30f), panel.transform);
        status.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -120f);
        
        return panel;
    }
}