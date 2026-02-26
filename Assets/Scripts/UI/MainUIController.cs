using UnityEngine;

namespace Platformer.UI
{
    /// <summary>
    /// Switches between UI panels (e.g. menu vs in-game). If panels is empty, finds InGameControls and uses [menu, InGameControls].
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        public GameObject[] panels;

        void Awake()
        {
            if (panels == null || panels.Length == 0)
            {
                var inGame = FindInChildren(transform, "InGameControls");
                if (inGame != null)
                    panels = new GameObject[] { null, inGame };
            }
            if (GetComponent<InGameControlInput>() == null)
                gameObject.AddComponent<InGameControlInput>();
        }

        static GameObject FindInChildren(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root.gameObject;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindInChildren(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        public void SetActivePanel(int index)
        {
            if (panels == null || panels.Length == 0) return;
            for (var i = 0; i < panels.Length; i++)
            {
                var g = panels[i];
                if (g == null) continue;
                var active = i == index;
                if (g.activeSelf != active) g.SetActive(active);
            }
        }

        void OnEnable()
        {
            if (panels != null && panels.Length > 0)
                SetActivePanel(1);
        }
    }
}