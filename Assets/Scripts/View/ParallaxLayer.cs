using UnityEngine;

namespace Platformer.View
{
    /// <summary>
    /// Moves this transform so it follows the camera at a fraction of the speed (parallax).
    /// movementScale (e.g. 0.25) = layer moves at 25% of camera = distant background look.
    /// Use use2DMode to keep Z unchanged (recommended for 2D).
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        /// <summary>Layer position = camera position * movementScale. X/Y &lt; 1 = parallax behind.</summary>
        public Vector3 movementScale = new Vector3(0.25f, 0.25f, 0f);
        /// <summary>If true, Z is not scaled (keeps layer at current depth for 2D).</summary>
        public bool use2DMode = true;

        Transform _camera;

        void Awake()
        {
            if (Camera.main != null)
                _camera = Camera.main.transform;
        }

        void LateUpdate()
        {
            if (_camera == null)
            {
                if (Camera.main != null)
                    _camera = Camera.main.transform;
                if (_camera == null) return;
            }
            if (use2DMode)
            {
                Vector3 cam = _camera.position;
                transform.position = new Vector3(
                    cam.x * movementScale.x,
                    cam.y * movementScale.y,
                    transform.position.z
                );
            }
            else
            {
                transform.position = Vector3.Scale(_camera.position, movementScale);
            }
        }
    }
}