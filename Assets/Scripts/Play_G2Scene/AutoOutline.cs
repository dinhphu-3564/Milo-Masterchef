using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PolygonCollider2D))] // Bắt buộc phải có Collider để lấy mẫu vẽ
public class AutoOutline : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private PolygonCollider2D myCollider;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        myCollider = GetComponent<PolygonCollider2D>();

        // Cấu hình Line Renderer mặc định (nếu bạn lười chỉnh tay)
        lineRenderer.useWorldSpace = false; // Vẽ theo tọa độ cục bộ
        lineRenderer.loop = true;           // Nối liền vòng tròn
    }

    void OnEnable()
    {
        // Mỗi khi Object này được Bật (SetActive true), nó sẽ vẽ lại ngay
        DrawOutline();
    }

    void DrawOutline()
    {
        if (myCollider == null) return;

        // Lấy danh sách điểm từ Polygon Collider của chính nó
        Vector2[] points = myCollider.GetPath(0);

        lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}