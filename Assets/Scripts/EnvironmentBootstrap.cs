using UnityEngine;
using Platformer.View;

/// <summary>
/// Spawns clouds, background, and mountain layers at runtime so they work without scene setup.
/// Loads prefabs and sprites from Resources/Environment if not assigned in the inspector.
/// </summary>
public class EnvironmentBootstrap : MonoBehaviour
{
    [Header("Optional: assign in inspector to avoid Resources")]
    [Tooltip("Leave empty to load from Resources/Environment")]
    public GameObject[] cloudPrefabs;
    [Tooltip("Mountain/parallax prefabs (e.g. 3_0, 5_0, 8_0). Leave empty to load from Resources.")]
    public GameObject[] mountainPrefabs;
    [Tooltip("Background sprite. Leave null to load BG_5 from Resources.")]
    public Sprite backgroundSprite;

    [Header("Settings")]
    public float cloudSpawnInterval = 1.5f;
    public float cloudSpawnDistanceX = 15f;
    public float cloudMinHeight = 3f;
    public float cloudMaxHeight = 8f;
    public float cloudMinScale = 4f;
    public float cloudMaxScale = 12f;
    public float mountainParallaxFactor = 0.5f;
    public int mountainSortingOrder = -15;
    public float mountainY = -3f;
    public float backgroundParallaxScale = 0.2f;
    public int backgroundSortingOrder = -30;

    const string ResourcesFolder = "Environment";

    void Awake()
    {
        Transform root = new GameObject("Environment").transform;
        root.SetParent(transform);

        SetupClouds(root);
        SetupBackground(root);
        SetupMountains(root);
    }

    void SetupClouds(Transform root)
    {
        GameObject[] prefabs = cloudPrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            prefabs = LoadCloudPrefabs();
            if (prefabs == null || prefabs.Length == 0) return;
        }

        GameObject spawnerGo = new GameObject("CloudSpawner");
        spawnerGo.transform.SetParent(root);
        spawnerGo.transform.localPosition = Vector3.zero;

        CloudSpawner spawner = spawnerGo.AddComponent<CloudSpawner>();
        spawner.cloudPrefabs = prefabs;
        spawner.spawnInterval = cloudSpawnInterval;
        spawner.spawnDistanceX = cloudSpawnDistanceX;
        spawner.minHeightFromCam = cloudMinHeight;
        spawner.maxHeightFromCam = cloudMaxHeight;
        spawner.minScale = cloudMinScale;
        spawner.maxScale = cloudMaxScale;
    }

    GameObject[] LoadCloudPrefabs()
    {
        string[] names = { "Clouds V2_0", "Clouds V2_1", "Clouds V2_2", "Clouds V2_3", "Clouds V2_4", "Clouds V2_5" };
        var list = new System.Collections.Generic.List<GameObject>();
        foreach (string n in names)
        {
            var prefab = Resources.Load<GameObject>(ResourcesFolder + "/" + n);
            if (prefab != null) list.Add(prefab);
        }
        return list.Count > 0 ? list.ToArray() : null;
    }

    void SetupBackground(Transform root)
    {
        Sprite sprite = backgroundSprite;
        if (sprite == null)
        {
            sprite = Resources.Load<Sprite>(ResourcesFolder + "/BG_5");
            if (sprite == null)
            {
                var tex = Resources.Load<Texture2D>(ResourcesFolder + "/BG_5");
                if (tex != null)
                    sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        if (sprite == null) return;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root);
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale = new Vector3(50f, 50f, 1f);

        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = backgroundSortingOrder;
        sr.drawMode = SpriteDrawMode.Simple;

        ParallaxLayer pl = bg.AddComponent<ParallaxLayer>();
        pl.movementScale = new Vector3(backgroundParallaxScale, backgroundParallaxScale * 0.5f, 1f);
    }

    void SetupMountains(Transform root)
    {
        GameObject[] prefabs = mountainPrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            prefabs = new[]
            {
                Resources.Load<GameObject>(ResourcesFolder + "/3_0"),
                Resources.Load<GameObject>(ResourcesFolder + "/5_0"),
                Resources.Load<GameObject>(ResourcesFolder + "/8_0")
            };
            if (prefabs[0] == null && prefabs[1] == null && prefabs[2] == null) return;
        }

        GameObject mountainsRoot = new GameObject("Mountains");
        mountainsRoot.transform.SetParent(root);
        mountainsRoot.transform.localPosition = Vector3.zero;

        float[] xPositions = { -25f, 0f, 25f };
        int count = 0;
        for (int i = 0; i < prefabs.Length && count < 3; i++)
        {
            if (prefabs[i] == null) continue;
            float x = xPositions[count];
            GameObject instance = Instantiate(prefabs[i], new Vector3(x, mountainY, 0f), Quaternion.identity, mountainsRoot.transform);
            instance.name = prefabs[i].name + "_" + count;

            ParallaxObject po = instance.GetComponent<ParallaxObject>();
            if (po != null)
                po.parallaxFactor = mountainParallaxFactor;

            SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = mountainSortingOrder;
            count++;
        }
    }
}
