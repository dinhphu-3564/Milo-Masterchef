using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance;

    [Header("Kho Nguyên Liệu (Từ Giai đoạn 1)")]
    public int miCount;        // Mì
    public int thitHeoCount;   // Thịt heo (làm thịt bằm)
    public int gaCount;        // Gà (làm gà chiên)
    public int caCount;        // Cá (làm cá chiên)
    public int rauCount;       // Rau (ăn kèm)
    public int caChuaCount;    // Cà chua
    public int hanhCount;      // Hành (cho mì)
    public int tuongOtCount;   // Tương ớt (cho gà)

    void Awake()
    {
        // Giữ GameManager tồn tại xuyên suốt các màn chơi
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CheatData(); // Tự động nạp nguyên liệu để test ngay
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm này giúp bạn có sẵn 100 nguyên liệu mỗi loại để test nấu nướng
    public void CheatData()
    {
        miCount = 100;
        thitHeoCount = 100;
        gaCount = 100;
        caCount = 100;
        rauCount = 100;
        caChuaCount = 100;
        hanhCount = 100;
        tuongOtCount = 100;
        Debug.Log("Đã nạp dữ liệu test thành công!");
    }
}