using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    Transform cam;
    Vector3 lastCamPos;

    /// <summary>1 = moves with camera (far), 0 = no movement (foreground).</summary>
    public float parallaxFactor;
    public float destroyDistance = 30f;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
        if (cam != null)
            lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        float deltaX = cam.position.x - lastCamPos.x;
        transform.position += new Vector3(deltaX * parallaxFactor, 0, 0);
        lastCamPos = cam.position;
        if (transform.position.x < cam.position.x - destroyDistance)
            Destroy(gameObject);
    }
}
