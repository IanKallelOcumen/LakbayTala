using UnityEngine;
using UnityEditor;

public static class LakbayTalaBackgroundTool
{
    [MenuItem("LakbayTala/Game Scene/Force BGSprite + CloudSpawner (this scene)", false, 11)]
    public static void ForceUseBackgroundAssets()
    {
        CloudSpawner spawner = Object.FindFirstObjectByType<CloudSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("LakbayTala: No CloudSpawner in scene. Add a Background with CloudSpawner, or run the game (BackgroundBootstrap will create one from Resources).");
            return;
        }
        Transform root = spawner.transform.parent;
        if (root != null)
        {
            root.gameObject.SetActive(true);
            spawner.gameObject.SetActive(true);
        }
        EnsureBGSpriteInEditor(spawner.transform.parent);
        if (spawner.cloudPrefabs == null || spawner.cloudPrefabs.Length == 0)
        {
            var prefabs = Resources.LoadAll<GameObject>("Environment");
            if (prefabs != null && prefabs.Length > 0)
            {
                var list = new System.Collections.Generic.List<GameObject>();
                foreach (var p in prefabs)
                    if (p != null && p.name.Contains("Cloud")) list.Add(p);
                if (list.Count > 0)
                {
                    spawner.cloudPrefabs = list.ToArray();
                    EditorUtility.SetDirty(spawner);
                }
            }
        }
        Debug.Log("LakbayTala: Background with BGSprite + CloudSpawner enabled and assigned. Save the scene to keep changes.");
    }

    static void EnsureBGSpriteInEditor(Transform backgroundRoot)
    {
        if (backgroundRoot == null) return;
        Transform bgT = null;
        for (int i = 0; i < backgroundRoot.childCount; i++)
        {
            if (backgroundRoot.GetChild(i).name == "BGSprite") { bgT = backgroundRoot.GetChild(i); break; }
        }
        if (bgT == null) return;
        bgT.gameObject.SetActive(true);
        var sr = bgT.GetComponent<SpriteRenderer>();
        if (sr == null) sr = bgT.gameObject.AddComponent<SpriteRenderer>();
        if (sr.sprite == null)
        {
            var loaded = Resources.Load<Sprite>("Environment/BG_5");
            if (loaded != null)
            {
                sr.sprite = loaded;
                sr.sortingOrder = -30;
                bgT.localScale = new Vector3(50, 50, 1f);
                EditorUtility.SetDirty(sr);
            }
        }
        else
        {
            sr.sortingOrder = -30;
            EditorUtility.SetDirty(sr);
        }
    }
}
