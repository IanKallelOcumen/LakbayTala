using UnityEngine;

/// <summary>
/// Ensures the game uses the LakbayTala background assets (BGSprite + CloudSpawner) at runtime.
/// Add this to any GameObject in the scene (e.g. GameController); it runs in Awake and forces
/// the correct Background to be active and populated, or creates one from Resources.
/// </summary>
public class BackgroundBootstrap : MonoBehaviour
{
    [Header("Resources fallback (if scene has no BGSprite/CloudSpawner)")]
    public string bgSpriteResourcePath = "Environment/BG_5";
    public string cloudPrefabsFolder = "Environment";
    public float bgSortingOrder = -30f;
    public float bgScale = 50f;

    void Awake()
    {
        EnsureBackgroundInUse();
    }

    /// <summary>Call from GameController or menu to force the scene to use BGSprite + CloudSpawner.</summary>
    public static void EnsureBackgroundInUse()
    {
        // 1) Find the Background that already has CloudSpawner (the one with the real assets)
        CloudSpawner spawner = Object.FindFirstObjectByType<CloudSpawner>();
        if (spawner != null)
        {
            Transform bgRoot = spawner.transform.parent;
            if (bgRoot != null)
            {
                bgRoot.gameObject.SetActive(true);
                spawner.gameObject.SetActive(true);
                if (spawner.cloudPrefabs == null || spawner.cloudPrefabs.Length == 0)
                    AssignCloudPrefabsFromResources(spawner);
            }
            EnsureBGSprite(spawner.transform.parent);
            return;
        }

        // 2) No CloudSpawner in scene - create Background from Resources
        CreateBackgroundFromResources();
    }

    static void EnsureBGSprite(Transform backgroundRoot)
    {
        if (backgroundRoot == null) return;
        Transform bgSpriteT = backgroundRoot.Find("BGSprite");
        if (bgSpriteT == null)
        {
            for (int i = 0; i < backgroundRoot.childCount; i++)
            {
                if (backgroundRoot.GetChild(i).name == "BGSprite")
                {
                    bgSpriteT = backgroundRoot.GetChild(i);
                    break;
                }
            }
        }
        if (bgSpriteT == null) return;
        bgSpriteT.gameObject.SetActive(true);
        var sr = bgSpriteT.GetComponent<SpriteRenderer>();
        if (sr == null) sr = bgSpriteT.gameObject.AddComponent<SpriteRenderer>();
        const string path = "Environment/BG_5";
        const float order = -30f;
        const float scale = 50f;
        if (sr.sprite == null)
        {
            var loaded = Resources.Load<Sprite>(path);
            if (loaded != null)
            {
                sr.sprite = loaded;
                sr.sortingOrder = (int)order;
                sr.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }
        else
        {
            sr.sortingOrder = (int)order;
        }
    }

    static void AssignCloudPrefabsFromResources(CloudSpawner spawner)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Environment");
        if (prefabs == null || prefabs.Length == 0) return;
        System.Collections.Generic.List<GameObject> clouds = new System.Collections.Generic.List<GameObject>();
        foreach (var p in prefabs)
        {
            if (p != null && p.name.Contains("Cloud"))
                clouds.Add(p);
        }
        if (clouds.Count > 0)
            spawner.cloudPrefabs = clouds.ToArray();
    }

    static void CreateBackgroundFromResources()
    {
        const string path = "Environment/BG_5";
        const float order = -30f;
        const float scale = 50f;
        var root = new GameObject("Background");
        root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        var bgGo = new GameObject("BGSprite");
        bgGo.transform.SetParent(root.transform, false);
        bgGo.transform.localPosition = Vector3.zero;
        bgGo.transform.localScale = new Vector3(scale, scale, 1f);
        var sr = bgGo.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>(path);
        sr.sortingOrder = (int)order;

        // CloudSpawner child
        var cloudGo = new GameObject("CloudSpawner");
        cloudGo.transform.SetParent(root.transform, false);
        var spawner = cloudGo.AddComponent<CloudSpawner>();
        AssignCloudPrefabsFromResources(spawner);
    }
}
