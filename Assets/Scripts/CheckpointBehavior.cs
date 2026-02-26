using UnityEngine;

/// <summary>
/// Put on the checkpoint GameObject (with trigger collider). When the player touches it,
/// moves the scene's SpawnPoint here so respawn uses this position.
/// </summary>
public class CheckpointBehavior : MonoBehaviour
{
    [Tooltip("If set, use this transform as the spawn position. Otherwise uses this GameObject.")]
    public Transform spawnAnchor;
    Transform cachedSpawnPoint;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || !collision.CompareTag("Player")) return;
        Transform target = spawnAnchor != null ? spawnAnchor : transform;
        if (cachedSpawnPoint == null)
            cachedSpawnPoint = GameObject.Find("SpawnPoint")?.transform;
        if (cachedSpawnPoint != null)
        {
            cachedSpawnPoint.position = target.position;
        }
    }
}