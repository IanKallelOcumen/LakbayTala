using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationManager : MonoBehaviour
{
    [Header("Screen Setup")]
    public List<GameObject> allScreens;
    public GameObject startScreen;

    [Header("Transition Setup")]
    public CanvasGroup fadeOverlay; // Drag 'TransitionFadeToBlack' here
    public float fadeDuration = 0.5f; // How fast the fade is (0.5 seconds)

    private bool isTransitioning = false;

    void Start()
    {
        // 1. Setup the fade overlay to be invisible at start
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0;
            fadeOverlay.blocksRaycasts = false;
        }

        // 2. Show the start screen immediately without fade
        ShowScreenImmediate(startScreen);
    }

    // Connect your Buttons to THIS function
    public void GoToScreen(GameObject screenToOpen)
    {
        // If we are already fading, ignore clicks (prevents bugs)
        if (isTransitioning) return;

        StartCoroutine(FadeAndSwitchSequence(screenToOpen));
    }

    // The logic that handles the animation
    IEnumerator FadeAndSwitchSequence(GameObject screenToOpen)
    {
        isTransitioning = true;

        // PHASE 1: Fade to Black
        if (fadeOverlay != null)
        {
            fadeOverlay.blocksRaycasts = true; // Block clicks while fading
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                // Smoothly increase Alpha from 0 to 1
                fadeOverlay.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null; // Wait for next frame
            }
            fadeOverlay.alpha = 1; // Ensure it's fully black
        }

        // PHASE 2: Swap the Screens (Hidden behind the black screen)
        ShowScreenImmediate(screenToOpen);
        
        // Optional: Wait a tiny moment in the dark (feels smoother)
        yield return new WaitForSeconds(0.1f);

        // PHASE 3: Fade IN (Reveal new screen)
        if (fadeOverlay != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                // Smoothly decrease Alpha from 1 to 0
                fadeOverlay.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                yield return null;
            }
            fadeOverlay.alpha = 0; // Ensure it's invisible
            fadeOverlay.blocksRaycasts = false; // Allow clicks again
        }

        isTransitioning = false;
    }

    // Helper function to just swap objects
    void ShowScreenImmediate(GameObject screenToShow)
    {
        // Turn everyone OFF
        foreach (GameObject screen in allScreens)
        {
            if (screen != null) screen.SetActive(false);
        }

        // Turn target ON
        if (screenToShow != null)
        {
            screenToShow.SetActive(true);
        }
    }
}