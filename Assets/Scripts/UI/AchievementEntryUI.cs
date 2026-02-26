using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component for individual achievement entries.
/// Handles display of achievement information, progress, and unlock status.
/// </summary>
public class AchievementEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public Text titleText;
    public Text descriptionText;
    public Text progressText;
    public Slider progressSlider;
    public Image iconImage;
    public Image backgroundImage;
    public Image lockImage;
    public Text rewardText;
    public GameObject unlockedIndicator;
    public GameObject lockedIndicator;
    
    [Header("Styling")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    public Color unlockedTextColor = Color.black;
    public Color lockedTextColor = new Color(0.3f, 0.3f, 0.3f);
    public Sprite unlockedIcon;
    public Sprite lockedIcon;
    
    private UIAchievementData currentAchievement;
    private int currentProgress;
    private bool isAlternate = false;

    /// <summary>
    /// Set up the achievement entry with data and styling.
    /// </summary>
    /// <param name="achievement">The achievement data</param>
    /// <param name="progress">Current progress toward the achievement</param>
    public void SetupAchievement(UIAchievementData achievement, int progress)
    {
        currentAchievement = achievement;
        currentProgress = progress;
        
        UpdateDisplay();
    }

    /// <summary>
    /// Set whether this entry should use alternate background styling.
    /// </summary>
    /// <param name="alternate">True for alternate styling</param>
    public void SetAlternateBackground(bool alternate)
    {
        isAlternate = alternate;
        UpdateBackgroundColor();
    }

    private void UpdateDisplay()
    {
        if (currentAchievement == null) return;
        
        // Update text content
        if (titleText != null)
        {
            titleText.text = currentAchievement.title;
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = currentAchievement.description;
        }
        
        // Update progress
        UpdateProgressDisplay();
        
        // Update unlock status styling
        UpdateUnlockDisplay();
        
        // Update reward display
        UpdateRewardDisplay();
        
        // Update background color
        UpdateBackgroundColor();
    }

    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            progressText.text = $"{currentProgress}/{currentAchievement.targetProgress}";
        }
        
        if (progressSlider != null)
        {
            progressSlider.maxValue = currentAchievement.targetProgress;
            progressSlider.value = currentProgress;
        }
    }

    private void UpdateUnlockDisplay()
    {
        bool isUnlocked = currentAchievement.isUnlocked;
        
        // Update text colors
        if (titleText != null)
        {
            titleText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }
        
        if (descriptionText != null)
        {
            descriptionText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }
        
        if (progressText != null)
        {
            progressText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }
        
        // Update icon
        if (iconImage != null)
        {
            iconImage.sprite = isUnlocked ? unlockedIcon : lockedIcon;
            iconImage.color = isUnlocked ? unlockedColor : lockedColor;
        }
        
        // Update lock indicator
        if (lockImage != null)
        {
            lockImage.gameObject.SetActive(!isUnlocked);
        }
        
        // Update unlocked/locked indicators
        if (unlockedIndicator != null)
        {
            unlockedIndicator.SetActive(isUnlocked);
        }
        
        if (lockedIndicator != null)
        {
            lockedIndicator.SetActive(!isUnlocked);
        }
    }

    private void UpdateRewardDisplay()
    {
        if (rewardText != null && currentAchievement.rewardAmount > 0)
        {
            rewardText.text = $"+{currentAchievement.rewardAmount} Tala";
            rewardText.gameObject.SetActive(true);
        }
        else if (rewardText != null)
        {
            rewardText.gameObject.SetActive(false);
        }
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            Color baseColor = isAlternate ? 
                new Color(0.95f, 0.95f, 0.95f) : 
                new Color(1f, 1f, 1f);
                
            backgroundImage.color = currentAchievement.isUnlocked ? baseColor : 
                Color.Lerp(baseColor, lockedColor, 0.3f);
        }
    }

    /// <summary>
    /// Update the progress for this achievement.
    /// </summary>
    /// <param name="progress">New progress value</param>
    public void UpdateProgress(int progress)
    {
        currentProgress = progress;
        UpdateProgressDisplay();
        
        // Check if achievement was unlocked
        if (progress >= currentAchievement.targetProgress && !currentAchievement.isUnlocked)
        {
            PlayUnlockAnimation();
        }
    }

    /// <summary>
    /// Play unlock animation for this achievement.
    /// </summary>
    private void PlayUnlockAnimation()
    {
        StartCoroutine(UnlockAnimationCoroutine());
    }

    private System.Collections.IEnumerator UnlockAnimationCoroutine()
    {
        // Simple unlock animation - can be expanded with more effects
        float duration = 0.5f;
        float elapsed = 0f;
        
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        
        // Scale up
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        // Update display to unlocked state
        currentAchievement.isUnlocked = true;
        UpdateUnlockDisplay();
        
        // Scale back down
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }

    /// <summary>
    /// Animate the entry when it appears.
    /// </summary>
    public void AnimateEntry(float delay = 0f)
    {
        StartCoroutine(EntryAnimation(delay));
    }

    private System.Collections.IEnumerator EntryAnimation(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        
        // Fade in animation
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        canvasGroup.alpha = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
}