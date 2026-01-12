using UnityEngine;
using TMPro;

public class MaterialHUD : MonoBehaviour
{
    [Header("Kéo 3 cái Text bên ngoài màn hình vào đây")]
    public TextMeshProUGUI txtWaterHUD;
    public TextMeshProUGUI txtGrainHUD;
    public TextMeshProUGUI txtGrassHUD;

    // Dùng Update để số nhảy liên tục mỗi khi có thay đổi (mua/bán/dùng)
    void Update()
    {
        // Lấy dữ liệu từ cái kho tổng ItemData
        if (txtWaterHUD != null)
            txtWaterHUD.text = "x " + ItemData.GetItem("ITEM_WATER");

        if (txtGrainHUD != null)
            txtGrainHUD.text = "x " + ItemData.GetItem("ITEM_GRAIN");

        if (txtGrassHUD != null)
            txtGrassHUD.text = "x " + ItemData.GetItem("ITEM_GRASS");
    }
}