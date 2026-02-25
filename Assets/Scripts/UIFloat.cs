using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UIFloat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle Animations")]
    [Tooltip("If checked, it waves up and down. (WARNING: Can break Layout Groups!)")]
    public bool enableIdleFloat = false; 
    public float amplitudeY = 10f;
    public float speedY = 0.6f;

    [Tooltip("If checked, it spins slightly around your current rotation.")]
    public bool enableIdleRotate = false;
    public float rotAmplitude = 1.5f;
    public float rotSpeed = 0.35f;

    [Header("Scale Pulse (Safe for Layouts)")]
    public bool enablePulse = true;
    public float scaleAmplitude = 0.02f; 
    public float scaleSpeed = 1.0f;

    [Header("Interactive Juice")]
    public bool enableHover = true;
    public float hoverScaleAmount = 1.1f;
    public float hoverAnimSpeed = 15f;

    [Header("Click Bump")]
    public bool enableBump = true;
    public float bumpScale = 1.2f; 
    public float bumpDuration = 0.2f;

    // Internal State
    private RectTransform _rt;
    private Vector3 _baseScale;
    private Vector2 _basePos;
    private Vector3 _baseRotation; // NEW: Stores your custom rotation
    
    private bool _hasBasePos = false;
    private bool _isHovering;
    private bool _isBumping;
    private float _currentHoverScale = 1f;
    private float _bumpT = 0f;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        // Reset state
        _isHovering = false;
        _isBumping = false;
        _currentHoverScale = 1f;
        
        // 1. Capture the Scale you set in Editor
        _baseScale = _rt.localScale;
        
        // 2. Capture the Rotation you set in Editor (Crucial Fix)
        _baseRotation = _rt.localEulerAngles;

        // 3. Capture Position (only if needed, to avoid fighting Layout Groups)
        if (enableIdleFloat)
        {
            _basePos = _rt.anchoredPosition;
            _hasBasePos = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => _isHovering = true;
    public void OnPointerExit(PointerEventData eventData) => _isHovering = false;

    void Update()
    {
        float time = Time.unscaledTime;
        float dt = Time.unscaledDeltaTime;

        // --- 1. Scale Calculation (Pulse + Hover + Bump) ---
        float pulse = enablePulse ? Mathf.Sin(time * scaleSpeed) * scaleAmplitude : 0f;
        float targetHover = (_isHovering && enableHover) ? hoverScaleAmount : 1f;
        
        _currentHoverScale = Mathf.Lerp(_currentHoverScale, targetHover, dt * hoverAnimSpeed);

        float finalScaleMult = (1f + pulse) * _currentHoverScale;

        // Add Bump logic
        if (_isBumping)
        {
            float bumpCurve = Mathf.Sin((_bumpT / bumpDuration) * Mathf.PI); 
            float bumpScl = (bumpScale - 1f) * bumpCurve;
            finalScaleMult += bumpScl;

            _bumpT += dt;
            if (_bumpT >= bumpDuration) _isBumping = false;
        }

        _rt.localScale = _baseScale * finalScaleMult;

        // --- 2. Rotation Calculation (Relative to YOUR setting) ---
        // Start with the rotation you set in the Inspector
        Vector3 finalRot = _baseRotation;

        if (enableIdleRotate)
        {
            // Add the wobble to the Z axis
            float wobble = Mathf.Sin(time * rotSpeed) * rotAmplitude;
            finalRot.z += wobble;
        }
        
        // Apply it
        _rt.localEulerAngles = finalRot;

        // --- 3. Position Calculation (Optional) ---
        if (enableIdleFloat && _hasBasePos)
        {
            float y = Mathf.Sin(time * speedY) * amplitudeY;
            _rt.anchoredPosition = _basePos + new Vector2(0, y);
        }
    }

    public void TriggerBump()
    {
        if (!enabled || !gameObject.activeInHierarchy) return;
        _isBumping = true;
        _bumpT = 0f;
    }
}