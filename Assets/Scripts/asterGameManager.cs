using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using LakbayTala.Config;

public class MasterGameManager : MonoBehaviour
{
    public static MasterGameManager Instance { get; private set; }

    [Header("CONFIG")]
    [Tooltip("Path under Resources to load settings from (e.g. config/settings).")]
    public string configResourcePath = "config/settings";

    /// <summary>Runtime settings loaded at boot from Resources/config/settings.json. Null if load failed.</summary>
    public GameSettings Settings { get; private set; }

    [Header("SCENE SETTINGS")]
    [Tooltip("The exact name of your Main Menu scene (used for the Back button in levels).")]
    public string mainMenuSceneName = "MenuScene";

    [Header("UI PANELS (Menu Scene Only)")]
    public CanvasGroup lobbyScreen;
    public CanvasGroup mapScreen;
    public CanvasGroup bestiaryScreen;
    public CanvasGroup infoScreen;

    [Header("MAP SCREEN ELEMENTS")]
    [Tooltip("The parent object containing Laguna location choices (e.g. Mount Makiling, Lake Mohikap, Sampaloc, Botocan).")]
    public CanvasGroup regionChoicesGroup;

    [Header("TRANSITION SETTINGS")]
    public CanvasGroup blackOverlay;
    public float fadeDuration = 0.5f;

    private CanvasGroup _currentScreen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadConfig();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Load configuration from Resources. Uses defaults if file missing or invalid.</summary>
    private void LoadConfig()
    {
        var textAsset = Resources.Load<TextAsset>(configResourcePath);
        if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
        {
            try
            {
                Settings = JsonUtility.FromJson<GameSettings>(textAsset.text);
                if (Settings != null)
                    return;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Lakbay Tala: Failed to parse config: " + e.Message);
            }
        }
        Settings = new GameSettings();
        if (textAsset == null)
            Debug.Log("Lakbay Tala: No config at Resources/" + configResourcePath + ". Using defaults.");
    }

    // --- SCENE MANAGEMENT ---

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(FadeAndLoadLevel(sceneName));
    }

    private IEnumerator FadeAndLoadLevel(string sceneName)
    {
        if (blackOverlay != null)
        {
            blackOverlay.blocksRaycasts = true;
            yield return StartCoroutine(FadeCanvasGroup(blackOverlay, 1f));
        }
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- MENU UI NAVIGATION ---

    public void OnMenu() => ShowScreen(lobbyScreen);

    public void OnMap()
    {
        ShowScreen(mapScreen);
        if (regionChoicesGroup != null)
        {
            regionChoicesGroup.alpha = 0f;
            regionChoicesGroup.interactable = false;
            regionChoicesGroup.blocksRaycasts = false;
        }
    }

    public void OnBestiary() => ShowScreen(bestiaryScreen);
    public void OnInfo() => ShowScreen(infoScreen);

    public void OnLandOfMaharlika()
    {
        if (regionChoicesGroup != null)
        {
            float targetAlpha = (regionChoicesGroup.alpha > 0.5f) ? 0f : 1f;
            StartCoroutine(FadeCanvasGroup(regionChoicesGroup, targetAlpha));
        }
    }

    public void OnBack()
    {
        if (_currentScreen == infoScreen)
            ShowScreen(bestiaryScreen);
        else if (_currentScreen == mapScreen || _currentScreen == bestiaryScreen)
            ShowScreen(lobbyScreen);
        else
            ShowScreen(lobbyScreen);
    }

    private void ShowScreen(CanvasGroup targetScreen)
    {
        _currentScreen = targetScreen;
        StopAllCoroutines();
        StartCoroutine(TransitionSequence(targetScreen));
    }

    private IEnumerator TransitionSequence(CanvasGroup targetScreen)
    {
        if (blackOverlay != null)
        {
            blackOverlay.blocksRaycasts = true;
            yield return StartCoroutine(FadeCanvasGroup(blackOverlay, 1f));
        }
        CanvasGroup[] allScreens = { lobbyScreen, mapScreen, bestiaryScreen, infoScreen };
        foreach (CanvasGroup screen in allScreens)
        {
            if (screen == null) continue;
            bool isTarget = (screen == targetScreen);
            screen.alpha = isTarget ? 1f : 0f;
            screen.interactable = isTarget;
            screen.blocksRaycasts = isTarget;
        }
        if (blackOverlay != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(blackOverlay, 0f));
            blackOverlay.blocksRaycasts = false;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        float startAlpha = cg.alpha;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        cg.alpha = targetAlpha;
        if (targetAlpha == 1f)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        else if (targetAlpha == 0f)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }
}
