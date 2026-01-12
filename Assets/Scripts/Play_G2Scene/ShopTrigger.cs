using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("UI Shop")]
    public GameObject shopPanel; // Kéo cái UI Shop Panel vào đây

    private bool isPlayerClose = false; // Biến kiểm tra xem nhân vật có đang đứng gần không

    void Start()
    {
        // Đảm bảo lúc đầu game shop ẩn đi
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    void Update()
    {
        // Nếu nhân vật đang đứng gần VÀ ấn phím Space VÀ Shop chưa mở
        if (isPlayerClose && Input.GetKeyDown(KeyCode.Space))
        {
            if (!shopPanel.activeSelf)
            {
                OpenShop();
            }
            else
            {
                CloseShop(); // Nếu shop đang mở thì ấn Space lần nữa để đóng
            }
        }
    }

    // Khi nhân vật bước vào vùng Collider của Shop
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nhớ đặt Tag cho nhân vật của bạn là "Player" nhé!
        if (other.CompareTag("Player"))
        {
            isPlayerClose = true;
            Debug.Log("Đã vào vùng mua hàng, ấn Space để mở!");
        }
    }

    // Khi nhân vật đi ra khỏi vùng Collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerClose = false;
            CloseShop(); // Đi ra xa thì tự đóng shop luôn cho tiện
        }
    }

    void OpenShop()
    {
        shopPanel.SetActive(true);
        // Có thể thêm code dừng thời gian hoặc khóa di chuyển nhân vật tại đây
        // Time.timeScale = 0; 
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);
        // Time.timeScale = 1;
    }
}