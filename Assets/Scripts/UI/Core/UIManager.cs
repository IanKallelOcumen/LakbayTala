using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LakbayTala.UI.Core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Layer Containers")]
        public Transform backgroundLayer;
        public Transform gameplayLayer;
        public Transform hudLayer;
        public Transform menuLayer;
        public Transform loreLayer;
        public Transform popupLayer;
        public Transform overlayLayer;

        private Dictionary<UIPanelType, UIPanel> registeredPanels = new Dictionary<UIPanelType, UIPanel>();
        private Stack<UIPanel> panelStack = new Stack<UIPanel>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeLayers();
        }

        private void InitializeLayers()
        {
            // Create layers if not assigned
            if (backgroundLayer == null) backgroundLayer = CreateLayer("BackgroundLayer", 0);
            if (gameplayLayer == null) gameplayLayer = CreateLayer("GameplayLayer", 100);
            if (hudLayer == null) hudLayer = CreateLayer("HUDLayer", 200);
            if (menuLayer == null) menuLayer = CreateLayer("MenuLayer", 300);
            if (loreLayer == null) loreLayer = CreateLayer("LoreLayer", 400);
            if (popupLayer == null) popupLayer = CreateLayer("PopupLayer", 500);
            if (overlayLayer == null) overlayLayer = CreateLayer("OverlayLayer", 600);
        }

        private Transform CreateLayer(string name, int sortOrder)
        {
            GameObject layerObj = new GameObject(name);
            layerObj.transform.SetParent(transform, false);
            
            Canvas canvas = layerObj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortOrder;
            layerObj.AddComponent<GraphicRaycaster>();
            
            return layerObj.transform;
        }

        public void RegisterPanel(UIPanel panel)
        {
            if (!registeredPanels.ContainsKey(panel.panelType))
            {
                registeredPanels.Add(panel.panelType, panel);
                
                // Reparent to correct layer
                Transform targetLayer = GetLayerForType(panel.panelType);
                if (targetLayer != null)
                {
                    panel.transform.SetParent(targetLayer, false);
                }
            }
        }

        public void UnregisterPanel(UIPanel panel)
        {
            if (registeredPanels.ContainsKey(panel.panelType))
            {
                registeredPanels.Remove(panel.panelType);
            }
        }

        public void ShowPanel(UIPanelType type)
        {
            if (registeredPanels.TryGetValue(type, out UIPanel panel))
            {
                panel.Show();
                panelStack.Push(panel);
            }
            else
            {
                Debug.LogWarning($"Panel {type} not registered!");
            }
        }

        public void HidePanel(UIPanelType type)
        {
            if (registeredPanels.TryGetValue(type, out UIPanel panel))
                panel.Hide();
        }

        public void HideAll()
        {
            foreach (var panel in registeredPanels.Values)
            {
                panel.Hide();
            }
            panelStack.Clear();
        }

        private Transform GetLayerForType(UIPanelType type)
        {
            switch (type)
            {
                case UIPanelType.HUD: return hudLayer;
                case UIPanelType.Menu:
                case UIPanelType.Leaderboard:
                case UIPanelType.Stats:
                case UIPanelType.Achievements:
                case UIPanelType.Settings: return menuLayer;
                case UIPanelType.Lore: return loreLayer;
                case UIPanelType.Popup: return popupLayer;
                case UIPanelType.Overlay: return overlayLayer;
                default: return menuLayer;
            }
        }
    }
}
