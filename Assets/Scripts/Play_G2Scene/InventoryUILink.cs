using UnityEngine;
using TMPro;
using Play_G2Scene;

public class InventoryUILink : MonoBehaviour
{
    [Header("Kéo các thành phần UI MỚI vào đây")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI veggieText;
    public TextMeshProUGUI riceText;
    public TextMeshProUGUI meatText;
    public TextMeshProUGUI fishText;
    public TextMeshProUGUI chickenText; // Nếu có
    public TextMeshProUGUI tomatoText; // Thêm dòng này cho cà chua

    void Start()
    {
        // Khi Scene vừa load xong, script này sẽ chạy.
        // Nó sẽ tìm ông trùm InventoryManager và đưa hàng mới cho ổng giữ.
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.inventoryPanel = this.inventoryPanel;

            InventoryManager.instance.veggieAmountText = this.veggieText;
            InventoryManager.instance.riceAmountText = this.riceText;
            InventoryManager.instance.meatAmountText = this.meatText;
            InventoryManager.instance.fishAmountText = this.fishText;
            InventoryManager.instance.chickenAmountText = this.chickenText;
            InventoryManager.instance.tomatoAmountText = this.tomatoText; // Thêm dòng này

            // Bắt ông trùm cập nhật lại số liệu lên bảng mới ngay lập tức
            InventoryManager.instance.UpdateUI();

            // Đảm bảo bảng kho ẩn đi lúc đầu
            if (this.inventoryPanel != null)
                this.inventoryPanel.SetActive(false);
        }
    }
}