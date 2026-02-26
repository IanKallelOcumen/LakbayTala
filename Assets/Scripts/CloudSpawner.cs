using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject[] cloudPrefabs;
    public float spawnInterval = 1.5f;
    public float spawnDistanceX = 15f;
    public float minHeightFromCam = 3f;
    public float maxHeightFromCam = 8f;
    public float minScale = 3f;
    public float maxScale = 6f;

    private Transform cam;
    private float timer;

    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        if (cam != null && cloudPrefabs != null && cloudPrefabs.Length > 0)
            PrewarmSky();
    }

    void Update()
    {
        if (cam == null || cloudPrefabs == null || cloudPrefabs.Length == 0) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnCloud(spawnDistanceX);
            timer = 0;
        }
    }

    void PrewarmSky()
    {
        for (int i = 0; i < 4; i++)
            SpawnCloud(Random.Range(-10f, 10f));
    }

    void SpawnCloud(float xOffset)
    {
        float spawnY = cam.position.y + Random.Range(minHeightFromCam, maxHeightFromCam);
        Vector3 spawnPos = new Vector3(cam.position.x + xOffset, spawnY, 0);
        GameObject selectedCloud = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        if (selectedCloud == null) return;
        GameObject newCloud = Instantiate(selectedCloud, spawnPos, Quaternion.identity);
        float randomScale = Random.Range(minScale, maxScale);
        newCloud.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        CloudMover mover = newCloud.GetComponent<CloudMover>();
        SpriteRenderer sr = newCloud.GetComponent<SpriteRenderer>();
        if (mover != null && sr != null)
        {
            float t = Mathf.InverseLerp(minScale, maxScale, randomScale);
            mover.windSpeed = Mathf.Lerp(0.2f, 1.0f, t);
            mover.parallaxEffect = Mathf.Lerp(0.95f, 0.5f, t);
            sr.sortingOrder = -20;
        }
    }
}
