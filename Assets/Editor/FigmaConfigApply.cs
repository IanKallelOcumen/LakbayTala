#if UNITY_EDITOR
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using DA_Assets.FCU;
using DA_Assets.FCU.Model;

namespace LakbayTala.Editor
{
    /// <summary>
    /// One-click Figma sign-in and project link from figma_local.json (gitignored).
    /// Run: LakbayTala → Figma → Sign in and set project from figma_local.json
    /// </summary>
    public static class FigmaConfigApply
    {
        const string ConfigPath = "Assets/Resources/config/figma_local.json";

        [MenuItem("LakbayTala/Figma/Sign in and set project from figma_local.json", false, 50)]
        public static void ApplyFigmaConfig()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", ConfigPath);
            fullPath = Path.GetFullPath(fullPath);

            if (!File.Exists(fullPath))
            {
                EditorUtility.DisplayDialog("Figma config",
                    "File not found:\n" + ConfigPath + "\n\nCopy figma_local.json.example to figma_local.json and add your token and project URL.",
                    "OK");
                return;
            }

            string json = File.ReadAllText(fullPath);
            FigmaLocalConfig config = JsonUtility.FromJson<FigmaLocalConfig>(json);
            if (config == null || string.IsNullOrEmpty(config.accessToken))
            {
                EditorUtility.DisplayDialog("Figma config", "figma_local.json must contain \"accessToken\" and optionally \"projectUrl\".", "OK");
                return;
            }

            var fcu = Object.FindFirstObjectByType<FigmaConverterUnity>();
            if (fcu == null)
            {
                bool create = EditorUtility.DisplayDialog("Figma Converter",
                    "No FigmaConverterUnity in scene. Create one now? (Tools → D.A. Assets → FCU: Create Figma Converter for Unity)",
                    "Create", "Cancel");
                if (create)
                {
                    CreateFcuAndApply(config, fullPath);
                    return;
                }
                return;
            }

            ApplyConfigAndSignIn(fcu, config);
        }

        static void CreateFcuAndApply(FigmaLocalConfig config, string _)
        {
            var go = new GameObject("FigmaConverterUnity");
            var fcu = go.AddComponent<FigmaConverterUnity>();
            ApplyConfigAndSignIn(fcu, config);
            Selection.activeGameObject = go;
            EditorUtility.DisplayDialog("Figma", "Figma Converter created and config applied. Check the Inspector.", "OK");
        }

        static void ApplyConfigAndSignIn(FigmaConverterUnity fcu, FigmaLocalConfig config)
        {
            if (!string.IsNullOrEmpty(config.projectUrl))
            {
                fcu.Settings.MainSettings.ProjectUrl = config.projectUrl;
                Debug.Log("[LakbayTala] Figma project URL set: " + config.projectUrl);
            }

            var authResult = new AuthResult { AccessToken = config.accessToken };
            _ = SignInAsync(fcu, authResult);
        }

        static async Task SignInAsync(FigmaConverterUnity fcu, AuthResult authResult)
        {
            await fcu.Authorizer.AddNew(authResult, AuthType.Manual);
            EditorUtility.DisplayDialog("Figma", "Signed in. You can now use Download and Import in the Figma Converter.", "OK");
        }

        [System.Serializable]
        class FigmaLocalConfig
        {
            public string projectUrl;
            public string accessToken;
        }
    }
}
#endif
