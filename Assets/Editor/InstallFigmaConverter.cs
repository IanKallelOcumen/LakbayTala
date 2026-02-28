#if UNITY_EDITOR
using UnityEditor;

namespace LakbayTala.Editor
{
    /// <summary>
    /// Menu item to install Figma Converter for Unity v5.5.2 from the package stored in the project.
    /// Run once: LakbayTala → Install Figma Converter (v5.5.2)
    /// </summary>
    public static class InstallFigmaConverter
    {
        private const string PackagePath = "Assets/Plugins/FigmaConverter_Unity_v5.5.2.unitypackage";

        [MenuItem("LakbayTala/Install Figma Converter (v5.5.2)", false, 0)]
        public static void Install()
        {
            string path = System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", PackagePath);
            path = System.IO.Path.GetFullPath(path);
            if (!System.IO.File.Exists(path))
            {
                UnityEditor.EditorUtility.DisplayDialog("Figma Converter", 
                    "Package not found at:\n" + PackagePath + "\n\nEnsure the .unitypackage is in Assets/Plugins/.", "OK");
                return;
            }
            AssetDatabase.ImportPackage(PackagePath, true);
        }
    }
}
#endif
