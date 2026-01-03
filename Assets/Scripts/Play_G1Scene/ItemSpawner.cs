//tạo ra các vật phẩm (vàng, bom, đá) một cách ngẫu nhiên tại các vị trí khác nhau phía trên màn hìnhvà với một tần suất nhất định
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Items")]
    public FallingItem[] items;          // Danh sách vật phẩm có thể spawn

    [Header("Spawn Timing")]
    public float spawnRate = 1f;         // Thời gian giữa các lần spawn

    [Header("Spawn Area")]
    public float minX = -7f;             // Giới hạn trái
    public float maxX = 7f;              // Giới hạn phải
    public float spawnY = 6f;            // Vị trí spawn theo trục Y

    // ===================== START =====================
    void Start()
    {
        // Spawn lặp lại theo chu kỳ
        InvokeRepeating(nameof(Spawn), 1f, spawnRate);
    }

    // ===================== SPAWN =====================
    void Spawn()
    {
        // Không có item để spawn
        if (items == null || items.Length == 0) return;

        // Chọn ngẫu nhiên item
        int randomIndex = Random.Range(0, items.Length);

        // Tạo vị trí spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0f
        );

        // Tạo item
        Instantiate(
            items[randomIndex],
            spawnPos,
            Quaternion.identity
        );
    }
}
