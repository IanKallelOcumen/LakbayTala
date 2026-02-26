using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LakbayTala.Leaderboard;

/// <summary>
/// Enhanced LakbayTala Leaderboard Entry UI with Filipino cultural theming
/// Displays player rankings with mythological creature associations and cultural elements
/// </summary>
public class LakbayTalaLeaderboardEntryUI : MonoBehaviour
{
    [Header("Cultural UI References")]
    public Text rankText;
    public Text rankTitleText;
    public Text nameText;
    public Text scoreText;
    public Text timeText;
    public Text levelText;
    public Text talaText;
    public Text loreText;
    public Text culturalText;
    public Image backgroundImage;
    public Image rankBadgeImage;
    public Image culturalIconImage;
    public Image decorativePatternImage;
    
    [Header("Cultural Styling")]
    public Color defaultBackgroundColor = Color.white;
    public Color alternateBackgroundColor = new Color(0.95f, 0.95f, 0.9f);
    public Color goldRankColor = new Color(1f, 0.84f, 0f);
    public Color silverRankColor = new Color(0.75f, 0.75f, 0.75f);
    public Color bronzeRankColor = new Color(0.8f, 0.5f, 0.2f);
    public Color culturalTextColor = new Color(0.2f, 0.15f, 0.1f);
    
    [Header("Cultural Elements")]
    public Sprite[] mythologicalBadges;
    public Sprite[] culturalIcons;
    public Sprite[] decorativePatterns;
    public Font primaryFont;
    public Font secondaryFont;
    
    private bool isAlternate = false;
    private LeaderboardEntry currentEntry;
    private int currentRank;
    private string culturalRank;
    private Color rankColor;
    private Sprite rankBadge;

    /// <summary>
    /// Set up the leaderboard entry with cultural theming
    /// </summary>
    public void SetupEntry(LeaderboardEntry entry, int rank, string culturalRankName, Color culturalRankColor, Sprite culturalBadge)
    {
        currentEntry = entry;
        currentRank = rank;
        culturalRank = culturalRankName;
        rankColor = culturalRankColor;
        rankBadge = culturalBadge;
        
        UpdateDisplay();
        ApplyCulturalStyling();
    }

    /// <summary>
    /// Set whether this entry should use alternate background styling
    /// </summary>
    public void SetAlternateBackground(bool alternate)
    {
        isAlternate = alternate;
        UpdateBackgroundColor();
    }

    private static string GetPlayerName(LeaderboardEntry e) => e?.user?.displayName ?? e?.user?.username ?? "";
    private static float GetCompletionTime(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("completionTime", out var t) ? System.Convert.ToSingle(t) : 0f;
    private static string GetLevelName(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("levelName", out var l) ? l?.ToString() ?? "" : e?.user?.culturalLevel ?? "";
    private static int GetTalaCollected(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("talaCollected", out var v) ? System.Convert.ToInt32(v) : 0;
    private static int GetLoreCardsFound(LeaderboardEntry e) => e?.additionalData != null && e.additionalData.TryGetValue("loreCardsFound", out var v) ? System.Convert.ToInt32(v) : 0;
    private static int GetCulturalKnowledgeScore(LeaderboardEntry e) => e?.user?.culturalScores != null && e.user.culturalScores.Count > 0 ? e.user.culturalScores.Values.Max() : (e?.additionalData != null && e.additionalData.TryGetValue("culturalKnowledgeScore", out var v) ? System.Convert.ToInt32(v) : 0);

    private void UpdateDisplay()
    {
        if (currentEntry == null) return;
        
        // Set rank information with cultural title
        if (rankText != null)
        {
            rankText.text = $"#{currentRank}";
            rankText.color = rankColor;
            rankText.font = primaryFont;
        }
        
        if (rankTitleText != null)
        {
            rankTitleText.text = culturalRank;
            rankTitleText.color = rankColor;
            rankTitleText.font = primaryFont;
        }
        
        // Set player information
        if (nameText != null)
        {
            nameText.text = TruncateString(GetPlayerName(currentEntry), 15);
            nameText.color = culturalTextColor;
            nameText.font = primaryFont;
        }
        
        if (scoreText != null)
        {
            scoreText.text = FormatScore(currentEntry.score);
            scoreText.color = culturalTextColor;
            scoreText.font = secondaryFont;
        }
        
        if (timeText != null)
        {
            timeText.text = FormatTime(GetCompletionTime(currentEntry));
            timeText.color = culturalTextColor;
            timeText.font = secondaryFont;
        }
        
        if (levelText != null)
        {
            string levelName = GetLevelName(currentEntry);
            levelText.text = GetCulturalLocationName(levelName);
            levelText.color = GetLocationColor(levelName);
            levelText.font = secondaryFont;
        }
        
        // Set cultural elements
        if (talaText != null)
        {
            talaText.text = $"Tala: {GetTalaCollected(currentEntry)}";
            talaText.color = new Color(0.9f, 0.7f, 0.3f); // Golden yellow for Tala
            talaText.font = secondaryFont;
        }
        
        if (loreText != null)
        {
            loreText.text = $"Alamat: {GetLoreCardsFound(currentEntry)}";
            loreText.color = new Color(0.2f, 0.4f, 0.6f); // Deep blue for lore
            loreText.font = secondaryFont;
        }
        
        if (culturalText != null)
        {
            culturalText.text = $"Karunungan: {GetCulturalKnowledgeScore(currentEntry)}";
            culturalText.color = new Color(0.8f, 0.3f, 0.2f); // Warm red for cultural knowledge
            culturalText.font = secondaryFont;
        }
        
        // Set cultural badges and icons
        if (rankBadgeImage != null && rankBadge != null)
        {
            rankBadgeImage.sprite = rankBadge;
            rankBadgeImage.color = Color.white;
            rankBadgeImage.gameObject.SetActive(true);
        }
        
        if (culturalIconImage != null && culturalIcons.Length > 0)
        {
            string levelName = GetLevelName(currentEntry);
            culturalIconImage.sprite = GetCulturalIcon(levelName);
            culturalIconImage.color = GetLocationColor(levelName);
            culturalIconImage.gameObject.SetActive(true);
        }
        
        // Add decorative pattern
        if (decorativePatternImage != null && decorativePatterns.Length > 0)
        {
            decorativePatternImage.sprite = GetDecorativePattern(currentRank);
            decorativePatternImage.color = new Color(1f, 1f, 1f, 0.1f); // Subtle pattern
            decorativePatternImage.gameObject.SetActive(true);
        }
        
        // Update background color
        UpdateBackgroundColor();
    }

    private void ApplyCulturalStyling()
    {
        // Add traditional Filipino styling elements
        if (backgroundImage != null)
        {
            // Add subtle traditional pattern overlay
            AddTraditionalPatternOverlay();
        }
        
        // Add entrance animation
        AnimateEntry(0f);
    }

    private void AddTraditionalPatternOverlay()
    {
        if (decorativePatternImage == null || decorativePatterns.Length == 0) return;
        
        // Create subtle pattern overlay
        decorativePatternImage.color = new Color(rankColor.r, rankColor.g, rankColor.b, 0.05f);
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            Color baseColor = isAlternate ? alternateBackgroundColor : defaultBackgroundColor;
            backgroundImage.color = baseColor;
        }
    }

    /// <summary>
    /// Get culturally appropriate location name
    /// </summary>
    private string GetCulturalLocationName(string levelName)
    {
        switch (levelName.ToLower())
        {
            case "mount makiling":
                return "Bundok Makiling";
            case "lake mohikap":
                return "Lawa ng Mohikap";
            case "sampaloc lake":
                return "Lawa ng Sampalok";
            case "botocan falls":
                return "Talon ng Botocan";
            default:
                return levelName;
        }
    }

    /// <summary>
    /// Get location-specific color based on Filipino cultural associations
    /// </summary>
    private Color GetLocationColor(string levelName)
    {
        switch (levelName.ToLower())
        {
            case "mount makiling":
                return new Color(0.4f, 0.3f, 0.2f); // Earth brown for mountains
            case "lake mohikap":
                return new Color(0.1f, 0.3f, 0.5f); // Deep blue for lakes
            case "sampaloc lake":
                return new Color(0.2f, 0.5f, 0.7f); // Sky blue for crater lakes
            case "botocan falls":
                return new Color(0.3f, 0.6f, 0.8f); // Misty blue for waterfalls
            default:
                return culturalTextColor;
        }
    }

    /// <summary>
    /// Get appropriate cultural icon for the location
    /// </summary>
    private Sprite GetCulturalIcon(string levelName)
    {
        if (culturalIcons.Length == 0) return null;
        
        // Simple mapping - in real implementation, use more sophisticated matching
        int iconIndex = Mathf.Abs(levelName.GetHashCode()) % culturalIcons.Length;
        return culturalIcons[iconIndex];
    }

    /// <summary>
    /// Get decorative pattern based on rank
    /// </summary>
    private Sprite GetDecorativePattern(int rank)
    {
        if (decorativePatterns.Length == 0) return null;
        
        int patternIndex = (rank - 1) % decorativePatterns.Length;
        return decorativePatterns[patternIndex];
    }

    /// <summary>
    /// Format score with Filipino number formatting
    /// </summary>
    private string FormatScore(int score)
    {
        return score.ToString("N0");
    }

    /// <summary>
    /// Format time in culturally appropriate way
    /// </summary>
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        
        if (minutes > 0)
        {
            return $"{minutes}m {secs}s";
        }
        else
        {
            return $"{secs}s";
        }
    }

    /// <summary>
    /// Truncate string to specified length with ellipsis
    /// </summary>
    private string TruncateString(string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str)) return "";
        if (str.Length <= maxLength) return str;
        return str.Substring(0, maxLength - 3) + "...";
    }

    /// <summary>
    /// Animate the entry when it appears with cultural flair
    /// </summary>
    public void AnimateEntry(float delay = 0f)
    {
        StartCoroutine(CulturalEntranceAnimation(delay));
    }

    private System.Collections.IEnumerator CulturalEntranceAnimation(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        // Initial state
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        Vector3 originalScale = transform.localScale;
        Vector3 startPosition = transform.position;
        
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * 0.8f;
        transform.position = startPosition + Vector3.right * 50f;
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        // Entrance animation with cultural timing
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, originalScale, t);
            transform.position = Vector3.Lerp(startPosition + Vector3.right * 50f, startPosition, t);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;
        transform.position = startPosition;
    }
}