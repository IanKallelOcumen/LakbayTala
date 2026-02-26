using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls background cloud animations and positioning for atmospheric effects.
/// Manages cloud spawning, movement, and cleanup for optimal performance.
/// </summary>
public class BackgroundCloudController : MonoBehaviour
{
    [Header("Cloud Prefabs")]
    [Tooltip("Array of cloud prefabs to spawn randomly")]
    public GameObject[] cloudPrefabs;
    
    [Header("Spawn Settings")]
    [Tooltip("Minimum time between cloud spawns")]
    public float minSpawnInterval = 3f;
    
    [Tooltip("Maximum time between cloud spawns")]
    public float maxSpawnInterval = 8f;
    
    [Tooltip("Height range for cloud spawning (world coordinates)")]
    public Vector2 spawnHeightRange = new Vector2(10f, 25f);
    
    [Tooltip("Horizontal spawn position offset from camera")]
    public float spawnOffsetX = 30f;
    
    [Header("Movement Settings")]
    [Tooltip("Base movement speed for clouds")]
    public float baseMoveSpeed = 2f;
    
    [Tooltip("Speed variation range (± from base speed)")]
    public float speedVariation = 1f;
    
    [Tooltip("Direction of cloud movement (typically -1 for left, 1 for right)")]
    public float moveDirection = -1f;
    
    [Header("Scale Settings")]
    [Tooltip("Minimum cloud scale multiplier")]
    public float minScale = 0.5f;
    
    [Tooltip("Maximum cloud scale multiplier")]
    public float maxScale = 1.5f;
    
    [Header("Cleanup Settings")]
    [Tooltip("Distance from camera to destroy clouds")]
    public float cleanupDistance = 40f;
    
    [Header("Performance")]
    [Tooltip("Maximum number of active clouds")]
    public int maxActiveClouds = 10;
    
    [Tooltip("Use object pooling for better performance")]
    public bool usePooling = true;

    private Camera mainCamera;
    private List<GameObject> activeClouds = new List<GameObject>();
    private Queue<GameObject> cloudPool = new Queue<GameObject>();
    private float nextSpawnTime;
    private float currentSpawnInterval;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = Object.FindFirstObjectByType<Camera>();
            
        SetNextSpawnTime();
        
        // Pre-populate pool if using pooling
        if (usePooling)
        {
            PrePopulatePool();
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime && activeClouds.Count < maxActiveClouds)
        {
            SpawnCloud();
            SetNextSpawnTime();
        }
        
        UpdateCloudPositions();
        CleanupDistantClouds();
    }

    private void SetNextSpawnTime()
    {
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    private void SpawnCloud()
    {
        if (cloudPrefabs == null || cloudPrefabs.Length == 0)
            return;

        GameObject cloud = GetCloudFromPool();
        if (cloud == null)
            return;

        // Set random position
        float spawnY = Random.Range(spawnHeightRange.x, spawnHeightRange.y);
        Vector3 spawnPos = mainCamera.transform.position;
        spawnPos.x += spawnOffsetX * moveDirection;
        spawnPos.y += spawnY;
        spawnPos.z = 10f; // Behind gameplay elements
        
        cloud.transform.position = spawnPos;
        
        // Set random scale
        float scale = Random.Range(minScale, maxScale);
        cloud.transform.localScale = Vector3.one * scale;
        
        // Set random movement speed
        float speed = baseMoveSpeed + Random.Range(-speedVariation, speedVariation);
        CloudMover mover = cloud.GetComponent<CloudMover>();
        if (mover == null)
            mover = cloud.AddComponent<CloudMover>();
            
        mover.Initialize(speed * moveDirection, cleanupDistance);
        
        cloud.SetActive(true);
        activeClouds.Add(cloud);
    }

    private void UpdateCloudPositions()
    {
        for (int i = activeClouds.Count - 1; i >= 0; i--)
        {
            GameObject cloud = activeClouds[i];
            if (cloud == null)
            {
                activeClouds.RemoveAt(i);
                continue;
            }

            CloudMover mover = cloud.GetComponent<CloudMover>();
            if (mover != null)
                mover.UpdateMovement();
        }
    }

    private void CleanupDistantClouds()
    {
        for (int i = activeClouds.Count - 1; i >= 0; i--)
        {
            GameObject cloud = activeClouds[i];
            if (cloud == null)
            {
                activeClouds.RemoveAt(i);
                continue;
            }

            float distance = Mathf.Abs(cloud.transform.position.x - mainCamera.transform.position.x);
            if (distance > cleanupDistance)
            {
                ReturnCloudToPool(cloud);
                activeClouds.RemoveAt(i);
            }
        }
    }

    private GameObject GetCloudFromPool()
    {
        if (usePooling && cloudPool.Count > 0)
        {
            return cloudPool.Dequeue();
        }
        
        // Create new cloud if pool is empty or not using pooling
        if (cloudPrefabs.Length > 0)
        {
            GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
            GameObject cloud = Instantiate(prefab);
            return cloud;
        }
        
        return null;
    }

    private void ReturnCloudToPool(GameObject cloud)
    {
        if (cloud == null) return;
        
        cloud.SetActive(false);
        
        if (usePooling)
        {
            cloudPool.Enqueue(cloud);
        }
        else
        {
            Destroy(cloud);
        }
    }

    private void PrePopulatePool()
    {
        int poolSize = Mathf.Min(maxActiveClouds, 5); // Start with smaller pool
        
        for (int i = 0; i < poolSize; i++)
        {
            if (cloudPrefabs.Length > 0)
            {
                GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
                GameObject cloud = Instantiate(prefab);
                cloud.SetActive(false);
                cloudPool.Enqueue(cloud);
            }
        }
    }

    void OnDisable()
    {
        // Clean up all active clouds
        foreach (GameObject cloud in activeClouds)
        {
            if (cloud != null)
            {
                if (usePooling)
                    ReturnCloudToPool(cloud);
                else
                    Destroy(cloud);
            }
        }
        activeClouds.Clear();
    }

    /// <summary>
    /// Force spawn a cloud immediately (useful for testing or scripted events).
    /// </summary>
    public void ForceSpawnCloud()
    {
        if (activeClouds.Count < maxActiveClouds)
        {
            SpawnCloud();
        }
    }

    /// <summary>
    /// Clear all active clouds.
    /// </summary>
    public void ClearAllClouds()
    {
        foreach (GameObject cloud in activeClouds)
        {
            if (cloud != null)
            {
                ReturnCloudToPool(cloud);
            }
        }
        activeClouds.Clear();
    }
}