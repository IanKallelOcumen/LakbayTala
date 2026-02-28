#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace LakbayTala.Editor
{
    /// <summary>
    /// Replace Legacy UI Text and TextMeshProUGUI fonts across all scenes and prefabs.
    /// LilitaOne: TTF is in Assets/Fonts. For TMP, create a Font Asset from it
    /// (right-click LilitaOne-Regular → Create → TextMeshPro → Font Asset) and save as "LilitaOne SDF".
    /// </summary>
    public class FontReplacerTool : EditorWindow
    {
        public Font newFont;
        public TMP_FontAsset newTMPFont;

        private const string LilitaOneTTFPath = "Assets/Fonts/LilitaOne-Regular.ttf";
        private const string LilitaOneSDFName = "LilitaOne SDF";

        [MenuItem("LakbayTala/Font Replacer", false, 30)]
        public static void ShowWindow()
        {
            var w = GetWindow<FontReplacerTool>("Font Replacer");
            w.minSize = new Vector2(320, 140);
        }

        private void OnGUI()
        {
            GUILayout.Label("Font consistency (Legacy + TMP)", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            newFont = (Font)EditorGUILayout.ObjectField("Legacy Font (UI.Text)", newFont, typeof(Font), false);
            newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", newTMPFont, typeof(TMP_FontAsset), false);

            EditorGUILayout.Space(6);

            if (GUILayout.Button("Use LilitaOne (find in project)"))
            {
                TryAssignLilitaOne();
            }

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Replace fonts in ALL Scenes & Prefabs"))
            {
                if (newFont == null && newTMPFont == null)
                {
                    EditorUtility.DisplayDialog("Font Replacer", "Assign at least one font above, or click \"Use LilitaOne\" first.", "OK");
                    return;
                }
                if (EditorUtility.DisplayDialog("Confirm", "This will modify all scenes and prefabs. Save your work first!", "Go Ahead", "Cancel"))
                {
                    ReplaceFonts();
                }
            }
        }

        private void TryAssignLilitaOne()
        {
            newFont = AssetDatabase.LoadAssetAtPath<Font>(LilitaOneTTFPath);
            if (newFont == null)
            {
                string[] guids = AssetDatabase.FindAssets("LilitaOne t:Font");
                foreach (string g in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(g);
                    if (path.IndexOf("LilitaOne", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        newFont = AssetDatabase.LoadAssetAtPath<Font>(path);
                        break;
                    }
                }
            }

            string[] tmpGuids = AssetDatabase.FindAssets("LilitaOne t:TMP_FontAsset");
            foreach (string g in tmpGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                if (path.IndexOf("LilitaOne", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    newTMPFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    break;
                }
            }
            if (newTMPFont == null)
            {
                tmpGuids = AssetDatabase.FindAssets(LilitaOneSDFName + " t:TMP_FontAsset");
                foreach (string g in tmpGuids)
                {
                    newTMPFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(g));
                    if (newTMPFont != null) break;
                }
            }

            if (newTMPFont != null || newFont != null)
            {
                Debug.Log("[Font Replacer] LilitaOne assigned. Legacy: " + (newFont != null ? "yes" : "no") + ", TMP: " + (newTMPFont != null ? "yes" : "no") + ". Click \"Replace fonts in ALL Scenes & Prefabs\" to apply.");
            }
            else
            {
                Debug.LogWarning("[Font Replacer] LilitaOne TMP not found. Create it: right-click " + LilitaOneTTFPath + " → Create → TextMeshPro → Font Asset, save as \"" + LilitaOneSDFName + "\".");
            }
        }

        private void ReplaceFonts()
        {
            if (newFont == null && newTMPFont == null)
            {
                Debug.LogError("[Font Replacer] Assign at least one font.");
                return;
            }

            int count = 0;

            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                if (ProcessGameObject(prefab, true))
                {
                    EditorUtility.SetDirty(prefab);
                    count++;
                }
            }

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled) continue;
                Scene s = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                bool sceneModified = false;
                foreach (GameObject root in s.GetRootGameObjects())
                {
                    if (ProcessGameObject(root, false)) sceneModified = true;
                }
                if (sceneModified)
                {
                    EditorSceneManager.MarkSceneDirty(s);
                    EditorSceneManager.SaveScene(s);
                    count++;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Font Replacer] Done. Modified " + count + " assets/scenes.");
        }

        private bool ProcessGameObject(GameObject go, bool isPrefab)
        {
            bool modified = false;

            if (newFont != null)
            {
                Text[] texts = go.GetComponentsInChildren<Text>(true);
                foreach (Text t in texts)
                {
                    if (t.font != newFont)
                    {
                        if (!isPrefab) Undo.RecordObject(t, "Replace Font");
                        t.font = newFont;
                        modified = true;
                    }
                }
            }

            if (newTMPFont != null)
            {
                TextMeshProUGUI[] tmps = go.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI t in tmps)
                {
                    if (t.font != newTMPFont)
                    {
                        if (!isPrefab) Undo.RecordObject(t, "Replace Font TMP");
                        t.font = newTMPFont;
                        modified = true;
                    }
                }
            }

            return modified;
        }
    }
}
#endif
