using UnityEngine;

/// <summary>
/// Simple movement component for background clouds.
/// Handles horizontal movement and cleanup when off-screen.
/// </summary>
public class CloudMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float windSpeed = 1f;
    public float parallaxEffect = 0.8f;
    
    private float moveSpeed;
    private float cleanupDistance;
    private Camera mainCamera;

    public void Initialize(float speed, float cleanupDist)
    {
        moveSpeed = speed;
        cleanupDistance = cleanupDist;
        
        if (mainCamera == null)
            mainCamera = Object.FindFirstObjectByType<Camera>();
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Object.FindFirstObjectByType<Camera>();
    }

    public void UpdateMovement()
    {
        if (mainCamera == null)
        {
            mainCamera = Object.FindFirstObjectByType<Camera>();
            if (mainCamera == null)
                return;
        }

        // Move cloud horizontally
        Vector3 newPosition = transform.position;
        newPosition.x += moveSpeed * Time.deltaTime;
        transform.position = newPosition;

        // Check if cloud is too far from camera and should be cleaned up
        float distanceFromCamera = Mathf.Abs(transform.position.x - mainCamera.transform.position.x);
        if (distanceFromCamera > cleanupDistance)
        {
            // Return to pool or destroy - handled by BackgroundCloudController
            gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        // Additional cleanup when cloud becomes invisible to camera
        if (mainCamera != null)
        {
            float distanceFromCamera = Mathf.Abs(transform.position.x - mainCamera.transform.position.x);
            if (distanceFromCamera > cleanupDistance)
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Reset cloud for reuse from object pool.
    /// </summary>
    public void ResetCloud()
    {
        // Reset any visual effects or animations if needed
        // This can be extended based on specific cloud requirements
    }
}