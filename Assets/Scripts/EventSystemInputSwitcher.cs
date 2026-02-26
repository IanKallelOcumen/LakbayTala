using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Replaces legacy StandaloneInputModule with InputSystemUIInputModule on the EventSystem
/// when the project uses the new Input System. Runs automatically after scene load—no need
/// to add a GameObject to the scene. For first scene we use RuntimeInitializeOnLoadMethod;
/// for DontDestroyOnLoad or additional scenes, add this component to a persistent GameObject
/// and it will run on Enable, or call EventSystemInputSwitcher.SwitchAllEventSystems() from your bootstrap.
/// </summary>
public static class EventSystemInputSwitcher
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoad()
    {
        SwitchAllEventSystems();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SwitchAllEventSystems();
    }

    /// <summary>Call this to replace StandaloneInputModule with InputSystemUIInputModule on all EventSystems in the scene.</summary>
    public static void SwitchAllEventSystems()
    {
        var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var es in eventSystems)
        {
            if (es == null || es.gameObject == null) continue;

            var standalone = es.GetComponent<StandaloneInputModule>();
            if (standalone != null)
            {
                Object.Destroy(standalone);
                es.gameObject.AddComponent<InputSystemUIInputModule>();
                Debug.Log("[EventSystemInputSwitcher] Replaced StandaloneInputModule with InputSystemUIInputModule.");
            }
        }
    }
}
