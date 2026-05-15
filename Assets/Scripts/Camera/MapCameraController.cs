using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MapCameraController : MonoBehaviour
{
    [Header("References")]
    // Ссылка на камеру
    [SerializeField] private Camera cam;
    // SpriteRenderer карты
    [SerializeField] private SpriteRenderer mapRenderer;

    [Header("Movement")]
    // Чувствительность перемещения карты
    // Чем больше число — тем быстрее двигается камера
    [SerializeField] private float dragSensitivity = 1f;

    // Плавность движения камеры
    // Чем больше число — тем быстрее камера догоняет targetPosition
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Zoom")]
    // Скорость приближения/отдаления
    [SerializeField] private float zoomSensitivity = 0.01f;
    // Плавность zoom
    [SerializeField] private float zoomLerpSpeed = 10f;
    // Минимальный zoom
    // Чем меньше число — тем ближе можно приблизить карту
    [SerializeField] private float minZoom = 3f;
    // Максимальный zoom
    // Обычно вычисляется автоматически от размера карты
    [SerializeField] private float maxZoom = 10f;


    // Границы карты
    private Bounds bounds;
    // Позиция, к которой должна плавно двигаться камера
    private Vector3 targetPosition;
    // Целевой zoom камеры
    private float targetZoom;
    // Последняя позиция пальца при drag
    private Vector2 lastPanPosition;
    // Был ли pinch zoom на прошлом кадре
    private bool wasZoomingLastFrame;
    // Последнее расстояние между двумя пальцами
    private float lastPinchDistance;


    private void Start()
    {
        // Получаем границы карты
        bounds = mapRenderer.bounds;
        // Запоминаем стартовую позицию камеры
        targetPosition = transform.position;
        // Запоминаем стартовый zoom
        targetZoom = cam.orthographicSize;
        // Автоматически вычисляем максимальный zoom (чтобы не выйти за пределы карты)
        CalculateMaxZoom();
    }


    private void Update()
    {
        // В редакторе и на ПК используем мышь
        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
        // На телефоне используем touch
        #else
        HandleTouch();
        #endif
        // Ограничиваем камеру границами карты
        ClampCamera();

        transform.position = Vector3.Lerp(
            transform.position,     // текущая позиция
            targetPosition,         // куда хотим прийти
            smoothSpeed * Time.deltaTime
        );

        // плавный зум
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,   // текущий zoom
            targetZoom,             // целевой zoom
            zoomLerpSpeed * Time.deltaTime
        );
    }


    // Android
    private void HandleTouch()
    {
        // Если touch экрана нет — выходим
        if (Touchscreen.current == null)
            return;

        // Получаем список всех touch
        var touches = Touchscreen.current.touches;

        // Считаем количество активных касаний
        int activeTouches = 0;

        foreach (var touch in touches)
        {
            if (touch.press.isPressed)
                activeTouches++;
        }

        // перемещение при 1 касании
        if (activeTouches == 1)
        {
            // Если раньше был zoom то сбрасываем флаг
            wasZoomingLastFrame = false;

            foreach (var touch in touches)
            {
                // Пропускаем неактивные касания
                if (!touch.press.isPressed)
                    continue;

                // Текущая позиция пальца
                Vector2 currentPos = touch.position.ReadValue();

                // Если палец только коснулся экрана
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    // Запоминаем стартовую позицию
                    lastPanPosition = currentPos;
                    return;
                }

                // Вычисляем разницу движения пальца
                Vector2 delta = cam.ScreenToViewportPoint(lastPanPosition - currentPos);

                // Двигаем камеру
                Move(delta);

                // Запоминаем текущую позицию
                lastPanPosition = currentPos;
            }
        }
        // zoom
        else if (activeTouches >= 2)
        {
            TouchControl touch0 = null;
            TouchControl touch1 = null;

            // Находим два активных касания
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

            // Если два касания не нашли, то выходим
            if (touch0 == null || touch1 == null)
                return;

            // Текущее расстояние между пальцами
            float currentDistance = Vector2.Distance(
                touch0.position.ReadValue(),
                touch1.position.ReadValue()
            );

            // Если zoom только начался
            if (!wasZoomingLastFrame)
            {
                // Запоминаем стартовое расстояние
                lastPinchDistance = currentDistance;

                // Включаем флаг zoom
                wasZoomingLastFrame = true;
            }
            else
            {
                // Разница расстояний между кадрами
                float delta = currentDistance - lastPinchDistance;
                // Меняем zoom
                targetZoom -= delta * zoomSensitivity;

                // Ограничиваем zoom
                targetZoom = Mathf.Clamp(
                    targetZoom,
                    minZoom,
                    maxZoom
                );

                // Сохраняем новое расстояние
                lastPinchDistance = currentDistance;
            }
        }
    }

    // ПК
    private void HandleMouse()
    {
        // Если мыши нет, то выходим
        if (Mouse.current == null)
            return;

        // Если зажата ЛКМ
        if (Mouse.current.leftButton.isPressed)
        {
            // Получаем delta мыши
            Vector2 delta = Mouse.current.delta.ReadValue();
            // Немного уменьшаем значение
            delta *= 0.001f;
            // Двигаем камеру
            Move(delta);
        }

        // zoom
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            // Меняем target zoom
            targetZoom -= scroll * 0.05f;

            // Ограничиваем zoom
            targetZoom = Mathf.Clamp(
                targetZoom,
                minZoom,
                maxZoom
            );
        }
    }


    private void Move(Vector2 delta)
    {
        // Высота камеры в world units
        float camHeight = cam.orthographicSize * 2f;

        // Ширина камеры
        float camWidth = camHeight * cam.aspect;

        // Вычисляем movement
        Vector3 move = new Vector3(
            delta.x * camWidth * dragSensitivity,
            delta.y * camHeight * dragSensitivity,
            0f
        );

        // Двигаем target position
        targetPosition += move;
    }

    // Ограничение камеры
    private void ClampCamera()
    {
        // Размеры камеры
        float camHeight = targetZoom;
        float camWidth = camHeight * cam.aspect;

        // Границы X
        float minX = bounds.min.x + camWidth;
        float maxX = bounds.max.x - camWidth;

        // Границы Y
        float minY = bounds.min.y + camHeight;
        float maxY = bounds.max.y - camHeight;

        // Ограничиваем позицию камеры
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        // Z не трогаем
        targetPosition.z = transform.position.z;
    }

    private void CalculateMaxZoom()
    {
        // Максимальный zoom по вертикали
        float verticalSize = bounds.size.y / 2f;

        // Максимальный zoom по горизонтали
        float horizontalSize = (bounds.size.x / cam.aspect) / 2f;

        // Берем минимальный размер, чтобы камера не вылезала за карту
        maxZoom = Mathf.Min(
            verticalSize,
            horizontalSize
        );
    }
}