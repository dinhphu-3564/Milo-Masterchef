using UnityEngine;

public class CircularFish : MonoBehaviour
{
    [Header("Cài đặt Tâm hồ")]
    [Tooltip("Kéo cái GameObject 'PondCenterPoint' vào đây")]
    public Transform centerPoint;

    [Header("Cài đặt Hình dáng bơi")]
    [Tooltip("Độ rộng của vòng bơi (Bán kính ngang)")]
    public float radiusX = 1.5f;
    [Tooltip("Độ cao của vòng bơi (Bán kính dọc) - Nên nhỏ hơn X vì góc nhìn 2D")]
    public float radiusY = 0.8f;
    [Tooltip("Tốc độ bơi (Số dương bơi ngược chiều kim đồng hồ, âm thì ngược lại)")]
    public float speed = 1f;

    [Header("Cài đặt Khác")]
    [Tooltip("Có tự động xoay đầu cá theo hướng bơi không?")]
    public bool rotateTowardsDirection = true;
    [Tooltip("Nếu cá bị ngược đầu, chỉnh cái này (thường là -90, 0, hoặc 90)")]
    public float rotationOffset = 0f;

    // Góc hiện tại trên vòng tròn (tính bằng radian)
    private float currentAngle = 0f;

    void Update()
    {
        if (centerPoint == null) return;

        // 1. Tính toán vị trí mới trên hình bầu dục
        // Tăng góc dựa trên thời gian và tốc độ
        currentAngle += speed * Time.deltaTime;

        // Công thức tính tọa độ hình bầu dục
        float x = Mathf.Cos(currentAngle) * radiusX;
        float y = Mathf.Sin(currentAngle) * radiusY;

        // Cập nhật vị trí mới (lấy tâm hồ làm gốc cộng thêm tọa độ vừa tính)
        Vector3 newPos = centerPoint.position + new Vector3(x, y, 0);

        // 2. Xử lý xoay đầu cá (Để nhìn mượt mà nhất)
        if (rotateTowardsDirection)
        {
            // Tính hướng di chuyển bằng cách lấy vị trí mới trừ vị trí cũ
            Vector3 direction = newPos - transform.position;

            // Tính góc xoay dựa trên vector hướng
            float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Áp dụng góc xoay (cộng thêm offset nếu sprite gốc không nằm ngang)
            transform.rotation = Quaternion.Euler(0, 0, angleDeg + rotationOffset);
        }

        // 3. Áp dụng vị trí mới
        transform.position = newPos;
    }

    // Vẽ đường gizmo trong cửa sổ Scene để dễ căn chỉnh (Không hiện khi chơi game)
    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;
        Gizmos.color = Color.cyan;
        // Vẽ mô phỏng đường bơi
        int segments = 36;
        float angleStep = 360f / segments;
        Vector3 prevPos = centerPoint.position + new Vector3(Mathf.Cos(0) * radiusX, Mathf.Sin(0) * radiusY, 0);
        for (int i = 1; i <= segments + 1; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPos = centerPoint.position + new Vector3(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY, 0);
            Gizmos.DrawLine(prevPos, nextPos);
            prevPos = nextPos;
        }
    }
}