using UnityEngine;
using System;
using System.Collections.Generic;

namespace LakbayTala.UI.Core
{
    /// <summary>
    /// Centralized UI state and event bus for consistency across game HUD, lore, and menu.
    /// Use for panel visibility, modal state, and theme so all canvas components stay in sync.
    /// </summary>
    public class UIStateManager : MonoBehaviour
    {
        public static UIStateManager Instance { get; private set; }

        [Header("Initial state")]
        [Tooltip("Current active panel type (for restore after load).")]
        public UIPanelType currentPanelType = UIPanelType.None;

        [Tooltip("Last selected menu tab or sub-panel ID for persistence.")]
        public string lastMenuSubId = "";

        [Tooltip("Whether a modal or blocking overlay is active.")]
        public bool isModalActive;

        /// <summary>Fired when any panel is shown. Arg: panel type.</summary>
        public event Action<UIPanelType> OnPanelShown;

        /// <summary>Fired when any panel is hidden. Arg: panel type.</summary>
        public event Action<UIPanelType> OnPanelHidden;

        /// <summary>Fired when modal state changes. Arg: is modal active.</summary>
        public event Action<bool> OnModalStateChanged;

        /// <summary>Fired when UI theme or settings change (e.g. from settings panel).</summary>
        public event Action OnThemeOrSettingsChanged;

        private readonly Stack<UIPanelType> panelStack = new Stack<UIPanelType>();

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

        public void NotifyPanelShown(UIPanelType type)
        {
            currentPanelType = type;
            panelStack.Push(type);
            OnPanelShown?.Invoke(type);
        }

        public void NotifyPanelHidden(UIPanelType type)
        {
            if (panelStack.Count > 0 && panelStack.Peek() == type)
                panelStack.Pop();
            if (panelStack.Count > 0)
                currentPanelType = panelStack.Peek();
            else
                currentPanelType = UIPanelType.None;
            OnPanelHidden?.Invoke(type);
        }

        public void SetModalActive(bool active)
        {
            if (isModalActive == active) return;
            isModalActive = active;
            OnModalStateChanged?.Invoke(isModalActive);
        }

        public void NotifyThemeOrSettingsChanged()
        {
            OnThemeOrSettingsChanged?.Invoke();
        }

        public void SetLastMenuSubId(string subId)
        {
            lastMenuSubId = subId ?? "";
        }

        public UIPanelType GetCurrentPanelType() => currentPanelType;
        public bool IsModalActive() => isModalActive;
    }
}
