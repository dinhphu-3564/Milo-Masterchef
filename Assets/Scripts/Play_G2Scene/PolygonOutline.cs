using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PolygonOutline : MonoBehaviour
{
    // Kéo cái Polygon Collider dùng để vẽ viền vào đây
    public PolygonCollider2D targetCollider;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Cấu hình nét vẽ
        lineRenderer.useWorldSpace = false; // Vẽ theo tọa độ cục bộ của object
        lineRenderer.loop = true;           // QUAN TRỌNG: Tự động nối điểm cuối về điểm đầu (khép kín vòng)

        // Mẹo: Chỉnh độ dày nét vẽ nhỏ lại một chút cho đẹp (tùy chỉnh nếu muốn)
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        DrawOutline();
    }

    void DrawOutline()
    {
        if (targetCollider == null) return;

        // Lấy danh sách các điểm của Polygon
        Vector2[] points = targetCollider.GetPath(0);

        lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            // Line Renderer cần Vector3 (x, y, z), Polygon chỉ có Vector2 (x, y)
            // z = 0 (hoặc chỉnh z phù hợp nếu bị khuất sau background)
            lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, -0.1f));
        }
    }
}