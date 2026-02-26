using UnityEngine;
using System;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Centralized error handling for UI: logging, optional fallback behavior, and graceful degradation.
    /// </summary>
    public static class UIErrorHandler
    {
        public static bool logErrors = true;
        public static bool logWarnings = true;

        public static event Action<string, Exception> OnUIError;

        /// <summary>Execute an action with try/catch; on exception log and optionally invoke fallback.</summary>
        public static bool Try(Action action, Action<Exception> fallback = null)
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                if (logErrors)
                    Debug.LogError($"[UI] {e.Message}\n{e.StackTrace}");
                OnUIError?.Invoke(e.Message, e);
                fallback?.Invoke(e);
                return false;
            }
        }

        /// <summary>Execute a function with try/catch; on exception return default and optionally run fallback.</summary>
        public static T TryGet<T>(Func<T> getter, T defaultValue = default, Action<Exception> fallback = null)
        {
            try
            {
                return getter != null ? getter() : defaultValue;
            }
            catch (Exception e)
            {
                if (logErrors)
                    Debug.LogError($"[UI] {e.Message}\n{e.StackTrace}");
                OnUIError?.Invoke(e.Message, e);
                fallback?.Invoke(e);
                return defaultValue;
            }
        }

        public static void LogWarning(string message)
        {
            if (logWarnings)
                Debug.LogWarning($"[UI] {message}");
        }
    }
}
