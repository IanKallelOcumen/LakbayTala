using UnityEditor;
using UnityEngine;

/// <summary>
/// Removes NOVA_UI_EXISTS from scripting define symbols when Nova UI is not in the project,
/// so Figma Converter compiles without the Nova package. Runs on script load.
/// </summary>
[InitializeOnLoad]
public static class StripNovaDefineIfMissing
{
    const string NovaDefine = "NOVA_UI_EXISTS";
    const string NovaTypeName = "Nova.UIBlock2D, Nova";

    static StripNovaDefineIfMissing()
    {
        EditorApplication.delayCall += RunOnce;
    }

    static void RunOnce()
    {
        if (System.Type.GetType(NovaTypeName) != null)
            return; // Nova is present, leave defines as-is.

        RemoveDefineFromAllTargets(NovaDefine);
    }

    static void RemoveDefineFromAllTargets(string define)
    {
#if UNITY_2022_3_OR_NEWER
        var allTargets = new[]
        {
            UnityEditor.Build.NamedBuildTarget.Standalone,
            UnityEditor.Build.NamedBuildTarget.Android,
            UnityEditor.Build.NamedBuildTarget.iOS,
            UnityEditor.Build.NamedBuildTarget.WebGL
        };
        foreach (var target in allTargets)
        {
            string raw = PlayerSettings.GetScriptingDefineSymbols(target);
            if (string.IsNullOrEmpty(raw) || !raw.Contains(define)) continue;
            var list = new System.Collections.Generic.List<string>(raw.Split(';'));
            list.RemoveAll(x => x == define);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", list));
        }
#else
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        string raw = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        if (!string.IsNullOrEmpty(raw) && raw.Contains(define))
        {
            var list = new System.Collections.Generic.List<string>(raw.Split(';'));
            list.RemoveAll(x => x == define);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", list));
        }
#endif
    }
}
