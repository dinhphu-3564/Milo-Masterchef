using UnityEngine;

public class FallingItem : MonoBehaviour
{
    [Header("Item Value")]
    public int goldValue;                        // Giá trị vàng (+ hoặc -)

    [Header("Rotation")]
    public float minTorque = -15f;               // Lực xoay tối thiểu
    public float maxTorque = 15f;                // Lực xoay tối đa

    private Rigidbody2D rb;

    // ===================== START =====================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Tạo lực xoay ngẫu nhiên khi vật phẩm rơi
            rb.AddTorque(Random.Range(minTorque, maxTorque));
        }
    }

    // ================= FLOATING TEXT =================
    void ShowGoldText(string text, Color color)
    {
        if (GameManager.Instance == null) return;

        // Gửi vị trí world để GameManager chuyển sang UI
        GameManager.Instance.ShowFloatingText(
            transform.position,
            text,
            color
        );
    }

    // ===================== COLLISION =================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Basket")) return;

        // ----- ITEM (+ GOLD) -----
        if (CompareTag("Item"))
        {
            GameManager.Instance.AddGold(goldValue);
            GameManager.Instance.PlayEatSound();
            ShowGoldText("+" + goldValue + "$", Color.green);
        }

        // ----- BOMB / ROCK (- GOLD) -----
        if (CompareTag("Bomb") || CompareTag("Rock"))
        {
            GameManager.Instance.MinusGold(goldValue);
            GameManager.Instance.PlayBombSound();

            Color textColor = CompareTag("Bomb") ? Color.red : Color.gray;
            ShowGoldText("-" + goldValue + "$", textColor);
        }

        // Xóa vật phẩm sau khi xử lý
        Destroy(gameObject);
    }
}
