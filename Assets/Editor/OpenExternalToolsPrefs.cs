#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LakbayTala.Editor
{
    /// <summary>
    /// Fix "External Code Editor path does not exist" (e.g. missing VS Code).
    /// Use: LakbayTala → Fix external editor (use Cursor) or Open External Tools Preferences.
    /// </summary>
    public static class OpenExternalToolsPrefs
    {
        const string CursorPath = "C:\\Users\\kalle\\AppData\\Local\\Programs\\cursor\\Cursor.exe";

        [MenuItem("LakbayTala/Fix external editor (use Cursor)", false, 99)]
        public static void FixUseCursor()
        {
            if (File.Exists(CursorPath))
            {
                Unity.CodeEditor.CodeEditor.SetExternalScriptEditor(CursorPath);
                EditorUtility.DisplayDialog("External editor", "Set to Cursor.\nDouble-click a script to open it in Cursor.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Cursor not found",
                    "Cursor.exe not found at:\n" + CursorPath + "\n\nUse \"Open External Tools Preferences\" and click Browse to select your editor.",
                    "OK");
                Open();
            }
        }

        [MenuItem("LakbayTala/Open External Tools Preferences", false, 100)]
        public static void Open()
        {
            SettingsService.OpenUserPreferences("Preferences/External Tools");
        }
    }
}
#endif
