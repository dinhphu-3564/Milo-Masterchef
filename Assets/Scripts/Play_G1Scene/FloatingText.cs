using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatSpeed = 150f;     // Tốc độ bay lên (UI pixel / giây)
    public float lifeTime = 0.8f;       // Thời gian tồn tại

    private TextMeshProUGUI textComp;

    // ===================== AWAKE =====================
    void Awake()
    {
        textComp = GetComponent<TextMeshProUGUI>();

        if (textComp == null)
        {
            Debug.LogError("❌ FloatingText phải được gắn lên TextMeshProUGUI");
            return;
        }

        // Tự hủy sau lifeTime
        Destroy(gameObject, lifeTime);
    }

    // ===================== UPDATE ====================
    void Update()
    {
        // Di chuyển chữ bay lên theo trục Y
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    // ===================== SET TEXT ==================
    // Được gọi từ GameManager
    public void SetText(string value, Color color)
    {
        if (textComp == null) return;

        textComp.text = value;
        textComp.color = color;
    }
}
