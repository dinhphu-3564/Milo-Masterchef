//tạo ra các vật phẩm (vàng, bom, đá) một cách ngẫu nhiên tại các vị trí khác nhau phía trên màn hìnhvà với một tần suất nhất định
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public FallingItem[] items;             // các item rơi xuống
    public float spawnRate = 1f;            // thời gian giữa các lần spawn
    public float minX = -7f;                // giới hạn vị trí spawn theo trục X
    public float maxX = 7f;                 // giới hạn vị trí spawn theo trục X
    public float spawnY = 6f;               // vị trí spawn theo trục Y

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnRate);
        
    }

    void Spawn()                                        // tạo vật phẩm rơi xuống
    {
        if (items.Length == 0) return;                  // nếu không có vật phẩm nào để spawn thì thoát

        int rand = Random.Range(0, items.Length);       // chọn ngẫu nhiên một vật phẩm từ mảng

        Vector3 pos = new Vector3(                      // xác định vị trí spawn
            Random.Range(minX, maxX),                   // vị trí X ngẫu nhiên trong khoảng minX đến maxX
            spawnY,                                     // vị trí Y cố định        
            0
        );

        Instantiate(items[rand], pos, Quaternion.identity);     // tạo vật phẩm tại vị trí đã xác định với không có xoay
    }
}
