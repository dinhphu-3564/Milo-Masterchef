using UnityEngine;
using TMPro;

public class FloatingTextG2 : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyTime = 1f;
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();

        // --- KHẮC PHỤC LỖI TÀNG HÌNH ---
        // 1. Ép chữ vẽ đè lên tất cả (Layer 20)
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().sortingOrder = 20;
        }
    }

    void Start()
    {
        Destroy(gameObject, destroyTime);

        // 2. Đảm bảo kích thước không bị về 0
        // Đôi khi Instantiate xong nó bị bé tí xíu, dòng này ép nó to ra
        transform.localScale = new Vector3(1, 1, 1);
    }   

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    public void SetText(string content, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = content;
            textMesh.color = color;
            // Ép cập nhật hiển thị ngay lập tức
            textMesh.ForceMeshUpdate();
        }
    }
}