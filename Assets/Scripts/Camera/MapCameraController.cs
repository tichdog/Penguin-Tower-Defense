using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer mapRenderer;

    [Header("Movement")]
    [SerializeField] private float dragSensitivity = 1f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Zoom")]
    [SerializeField] private float zoomSensitivity = 0.01f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Bounds bounds;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector2 lastPanPosition;
    private bool wasZoomingLastFrame;
    private float lastPinchDistance;

    private void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        RefreshBounds();
        targetPosition = transform.position;
        targetZoom = cam != null ? cam.orthographicSize : targetZoom;
        CalculateMaxZoom();
    }

    public void SetMapRenderer(SpriteRenderer renderer)
    {
        if (renderer == null)
        {
            Debug.LogWarning("[MapCameraController] Map renderer is empty.");
            return;
        }

        mapRenderer = renderer;
        RefreshBounds();
        CalculateMaxZoom();

        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        targetPosition = transform.position;
        ClampCamera();
        transform.position = targetPosition;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
        ClampCamera();

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetZoom,
                zoomLerpSpeed * Time.deltaTime
            );
        }
    }

    private void HandleTouch()
    {
        if (cam == null || Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;
        int activeTouches = 0;

        foreach (var touch in touches)
        {
            if (touch.press.isPressed)
                activeTouches++;
        }

        if (activeTouches == 1)
        {
            wasZoomingLastFrame = false;

            foreach (var touch in touches)
            {
                if (!touch.press.isPressed)
                    continue;

                Vector2 currentPos = touch.position.ReadValue();

                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    lastPanPosition = currentPos;
                    return;
                }

                Vector2 delta = cam.ScreenToViewportPoint(lastPanPosition - currentPos);
                Move(delta);
                lastPanPosition = currentPos;
            }
        }
        else if (activeTouches >= 2)
        {
            TouchControl touch0 = null;
            TouchControl touch1 = null;

            foreach (var touch in touches)
            {
                if (!touch.press.isPressed)
                    continue;

                if (touch0 == null)
                    touch0 = touch;
                else if (touch1 == null)
                {
                    touch1 = touch;
                    break;
                }
            }

            if (touch0 == null || touch1 == null)
                return;

            float currentDistance = Vector2.Distance(
                touch0.position.ReadValue(),
                touch1.position.ReadValue()
            );

            if (!wasZoomingLastFrame)
            {
                lastPinchDistance = currentDistance;
                wasZoomingLastFrame = true;
            }
            else
            {
                float delta = currentDistance - lastPinchDistance;
                targetZoom -= delta * zoomSensitivity;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
                lastPinchDistance = currentDistance;
            }
        }
    }

    private void HandleMouse()
    {
        if (cam == null || Mouse.current == null)
            return;

        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            delta *= 0.001f;
            Move(delta);
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            targetZoom -= scroll * 0.05f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    private void Move(Vector2 delta)
    {
        if (cam == null)
            return;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        Vector3 move = new Vector3(
            delta.x * camWidth * dragSensitivity,
            delta.y * camHeight * dragSensitivity,
            0f
        );

        targetPosition += move;
    }

    private void ClampCamera()
    {
        if (cam == null || mapRenderer == null)
            return;

        float camHeight = targetZoom;
        float camWidth = camHeight * cam.aspect;

        float minX = bounds.min.x + camWidth;
        float maxX = bounds.max.x - camWidth;
        float minY = bounds.min.y + camHeight;
        float maxY = bounds.max.y - camHeight;

        targetPosition.x = minX <= maxX ? Mathf.Clamp(targetPosition.x, minX, maxX) : bounds.center.x;
        targetPosition.y = minY <= maxY ? Mathf.Clamp(targetPosition.y, minY, maxY) : bounds.center.y;
        targetPosition.z = transform.position.z;
    }

    private void CalculateMaxZoom()
    {
        if (cam == null || mapRenderer == null)
            return;

        float verticalSize = bounds.size.y / 2f;
        float horizontalSize = (bounds.size.x / cam.aspect) / 2f;

        maxZoom = Mathf.Min(verticalSize, horizontalSize);
        maxZoom = Mathf.Max(minZoom, maxZoom);
    }

    private void RefreshBounds()
    {
        if (mapRenderer == null)
            return;

        bounds = mapRenderer.bounds;
    }
}
