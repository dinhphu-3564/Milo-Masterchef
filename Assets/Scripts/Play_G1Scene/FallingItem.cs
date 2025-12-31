using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public int goldValue = 5;

    public float minTorque = -15f;
    public float maxTorque = 15f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddTorque(Random.Range(minTorque, maxTorque));       // tạo lực xoay ngẫu nhiên cho vật phẩm khi rơi
    }

    private void OnTriggerEnter2D(Collider2D other)     // khi vật phẩm chạm vào giỏ
    {
        // Destroy(gameObject);
        if (!other.CompareTag("Basket")) return;        // chỉ xử lý khi chạm vào giỏ

        // Item thường
        if (CompareTag("Item"))                         // nếu là trái cây
        {
            GameManager.Instance.AddGold(goldValue);    // cộng vàng cho người chơi
            GameManager.Instance.PlayEatSound();        // phát âm thanh ăn vật phẩm
        }

        // Bomb
        if (CompareTag("Bomb"))                         // nếu là bom
        {
            GameManager.Instance.MinusGold(10);         // trừ vàng người chơi
            GameManager.Instance.PlayBombSound();       // phát âm thanh bom nổ
        }

        // Rock
        if (CompareTag("Rock"))                         // nếu là đá     
        {
            GameManager.Instance.MinusGold(10);         // trừ vàng người chơi  
            GameManager.Instance.PlayBombSound();       // phát âm thanh bom nổ
        }   

        Destroy(gameObject);
    }
}
