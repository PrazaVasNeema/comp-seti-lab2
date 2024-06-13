using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float baseDragSpeed = 2;
    public float zoomSpeed = 2;
    public float minZoom = 5;
    public float maxZoom = 20;

    [SerializeField] private Camera m_camera;
    public ParabolaAndPointsVisualizer visualizer; // Ссылка на визуализатор

    private Vector3 dragOrigin;

    void Update()
    {
        HandleMouseDrag();
        HandleMouseScroll();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = m_camera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        float currentZoom = m_camera.orthographicSize;
        Vector3 move = new Vector3(pos.x * baseDragSpeed * currentZoom, pos.y * baseDragSpeed * currentZoom, 0);

        transform.Translate(-move, Space.World);

        dragOrigin = Input.mousePosition;
    }

    void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float newZoom = Mathf.Clamp(m_camera.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            m_camera.orthographicSize = newZoom;
            visualizer.UpdatePointSizes(newZoom); // Обновить размеры точек
        }
    }
}
