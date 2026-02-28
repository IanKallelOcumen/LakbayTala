#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LakbayTala.Editor
{
    /// <summary>
    /// Fixes canvas scaling and stretches root UI so content fills the screen (no white border).
    /// Run: LakbayTala → Fix canvas full-screen scaling
    /// Also adds a runtime component so stretch is re-applied when entering Play (overrides Figma Converter).
    /// </summary>
    public static class FixCanvasFullScreen
    {
        [MenuItem("LakbayTala/Fix canvas full-screen scaling", false, 110)]
        public static void FixScaling()
        {
            Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            if (allCanvases == null || allCanvases.Length == 0)
            {
                EditorUtility.DisplayDialog("Fix canvas", "No Canvas found in the scene.", "OK");
                return;
            }

            int scalerCount = 0;
            int stretchCount = 0;

            foreach (Canvas canvas in allCanvases)
            {
                var scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    Undo.RecordObject(scaler, "Fix canvas full-screen scaling");
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    scaler.matchWidthOrHeight = 0.5f;
                    scaler.referencePixelsPerUnit = 100f;
                    EditorUtility.SetDirty(scaler);
                    scalerCount++;
                }

                var rect = canvas.GetComponent<RectTransform>();
                if (rect != null && rect.childCount > 0)
                {
                    for (int i = 0; i < rect.childCount; i++)
                    {
                        Transform child = rect.GetChild(i);
                        var childRect = child.GetComponent<RectTransform>();
                        if (childRect == null) continue;
                        StretchRecursive(childRect, ref stretchCount);
                    }
                }
            }

            // Ensure runtime re-stretch runs when entering Play (overrides Figma Converter layout)
            EnsureRuntimeStretchInScene();

            EditorUtility.DisplayDialog("Fix canvas",
                "Updated " + scalerCount + " Canvas Scaler(s) and stretched " + stretchCount + " UI object(s).\n\nA runtime stretcher was added so the fix is re-applied when you press Play (fixes Figma layout overwriting). Save the scene (Ctrl+S) and press Play to test.",
                "OK");
        }

        static void EnsureRuntimeStretchInScene()
        {
            var stretch = Object.FindFirstObjectByType<CanvasFullScreenStretch>();
            if (stretch != null) return;

            // Prefer adding to MasterGameManager so it runs in every scene that has it
            var mgr = Object.FindFirstObjectByType<MasterGameManager>();
            if (mgr != null)
            {
                Undo.AddComponent<CanvasFullScreenStretch>(mgr.gameObject);
                return;
            }
            // Otherwise create a small helper
            var go = new GameObject("CanvasFullScreenStretch");
            Undo.RegisterCreatedObjectUndo(go, "Add Canvas Stretch");
            go.AddComponent<CanvasFullScreenStretch>();
        }

        static void StretchRecursive(RectTransform r, ref int count)
        {
            StretchRect(r, ref count);
            for (int i = 0; i < r.childCount; i++)
            {
                var child = r.GetChild(i);
                var childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                    StretchRecursive(childRect, ref count);
            }
        }

        static void StretchRect(RectTransform r, ref int count)
        {
            Undo.RecordObject(r, "Stretch UI");
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            r.pivot = new Vector2(0.5f, 0.5f);
            r.localScale = Vector3.one;
            EditorUtility.SetDirty(r);
            count++;
        }
    }
}
#endif
