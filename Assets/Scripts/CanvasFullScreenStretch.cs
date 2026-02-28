using UnityEngine;

namespace LakbayTala
{
    /// <summary>
    /// Applies full-screen stretch to all RectTransforms under every Canvas when the scene loads.
    /// Add this to any GameObject in the scene (e.g. MasterGameManager). Runs once in Start so it
    /// overrides Figma Converter layout and fixes the small-box UI issue.
    /// </summary>
    public class CanvasFullScreenStretch : MonoBehaviour
    {
        [Tooltip("If set, only stretch UI under this Canvas. Otherwise all Canvases in the scene.")]
        [SerializeField] Canvas targetCanvas;

        void Start()
        {
            ApplyStretch();
        }

        public void ApplyStretch()
        {
            Canvas[] canvases = targetCanvas != null
                ? new[] { targetCanvas }
                : FindObjectsByType<Canvas>(FindObjectsSortMode.None);

            foreach (Canvas canvas in canvases)
            {
                if (canvas == null) continue;

                var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (scaler != null)
                {
                    scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    scaler.referencePixelsPerUnit = 100f;
                }

                RectTransform root = canvas.GetComponent<RectTransform>();
                if (root == null || root.childCount == 0) continue;

                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    var childRect = child.GetComponent<RectTransform>();
                    if (childRect != null)
                        StretchRecursive(childRect);
                }
            }
        }

        static void StretchRecursive(RectTransform r)
        {
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            r.pivot = new Vector2(0.5f, 0.5f);
            r.localScale = Vector3.one;

            for (int i = 0; i < r.childCount; i++)
            {
                var child = r.GetChild(i);
                var childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                    StretchRecursive(childRect);
            }
        }
    }
}
