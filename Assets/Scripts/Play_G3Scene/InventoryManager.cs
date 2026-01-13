using UnityEngine;
using TMPro; // Nhớ dòng này để dùng TextMeshPro

namespace Play_G3Scene
{
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Singleton để gọi từ CookingPan

    [Header("UI Số lượng (Kéo các Text vào đây)")]
    public TMP_Text miText;
    public TMP_Text caText;
    public TMP_Text gaText;
    public TMP_Text thitText;
    public TMP_Text rauText;
    public TMP_Text caChuaText;

    [Header("Kho hàng (Số lượng ban đầu)")]
    public int miCount = 10;
    public int caCount = 10;
    public int gaCount = 10;
    public int thitCount = 10;
    public int rauCount = 10;
    public int caChuaCount = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Chuyển đổi dữ liệu từ màn 2 sang màn 3
        LoadDataFromG2();
        UpdateAllUI();
    }

    // Hàm chuyển đổi dữ liệu từ màn 2 (Play_G2Scene) sang màn 3
    void LoadDataFromG2()
    {
        // Lấy dữ liệu từ PlayerPrefs (đã được lưu bởi màn 2)
        int veggieCount = PlayerPrefs.GetInt("VeggieSaved", 0);
        int riceCount = PlayerPrefs.GetInt("RiceSaved", 0);
        int meatCount = PlayerPrefs.GetInt("MeatSaved", 0);
        int chickenMeatCount = PlayerPrefs.GetInt("ChickenSaved", 0);
        int fishMeatCount = PlayerPrefs.GetInt("FishMeatSaved", 0);
        int tomatoCount = PlayerPrefs.GetInt("TomatoSaved", 0);

        // Chuyển đổi dữ liệu từ màn 2 sang màn 3:
        // Màn 2 -> Màn 3
        miCount = riceCount;           // Gạo/Lúa -> Mì
        caCount = fishMeatCount;        // Cá -> Cá
        gaCount = chickenMeatCount;     // Gà -> Gà
        thitCount = meatCount;          // Thịt Bò -> Thịt Heo
        rauCount = veggieCount;         // Rau Cải -> Rau
        caChuaCount = tomatoCount;      // Cà Chua -> Cà Chua

        Debug.Log($"Đã chuyển đổi dữ liệu từ màn 2: Mì={miCount}, Cá={caCount}, Gà={gaCount}, Thịt={thitCount}, Rau={rauCount}, Cà Chua={caChuaCount}");
    }

    // Cập nhật toàn bộ số hiển thị
    void UpdateAllUI()
    {
        if (miText) miText.text = miCount.ToString();
        if (caText) caText.text = caCount.ToString();
        if (gaText) gaText.text = gaCount.ToString();
        if (thitText) thitText.text = thitCount.ToString();
        if (rauText) rauText.text = rauCount.ToString();
        if (caChuaText) caChuaText.text = caChuaCount.ToString();
    }

    // Hàm kiểm tra và trừ nguyên liệu (CookingPan sẽ gọi hàm này)
    public bool TryUseIngredient(string name)
    {
        bool isSuccess = false;

        switch (name)
        {
            case "Mi":
                if (miCount > 0) { miCount--; isSuccess = true; }
                break;
            case "Ca":
                if (caCount > 0) { caCount--; isSuccess = true; }
                break;
            case "Ga":
                if (gaCount > 0) { gaCount--; isSuccess = true; }
                break;
            case "ThitHeo":
                if (thitCount > 0) { thitCount--; isSuccess = true; }
                break;
            case "Rau":
                if (rauCount > 0) { rauCount--; isSuccess = true; }
                break;
            case "CaChua":
                if (caChuaCount > 0) { caChuaCount--; isSuccess = true; }
                break;
        }

        if (isSuccess)
        {
            UpdateAllUI(); // Cập nhật lại số trên màn hình
            return true;   // Báo là lấy thành công
        }
        else
        {
            Debug.Log("Hết nguyên liệu: " + name);
            return false;  // Báo là hết hàng
        }
    }
}
}