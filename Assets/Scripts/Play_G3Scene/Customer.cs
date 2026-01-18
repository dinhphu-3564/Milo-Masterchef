using UnityEngine;
using UnityEngine.UI; // <--- BẮT BUỘC CÓ để dùng Slider

public class Customer : MonoBehaviour
{
    [Header("Cài đặt Slider Thời Gian")]
    public Slider timeSlider;      // Kéo cái Slider tổng vào đây
    public Image timeSliderFill;   // Kéo cái "Fill" (bên trong Slider) vào đây để đổi màu

    [Header("Cấu hình Bong bóng")]
    public SpriteRenderer bubbleRenderer;
    public GameObject angryIcon;

    [Header("Cấu hình Khuôn mặt")]
    public Sprite normalSprite;
    public Sprite angrySprite;
    private SpriteRenderer myBodyRenderer;

    [Header("Danh sách món")]
    public string[] possibleDishes = { "MiY", "GaChien", "CaChien" };
    public Sprite[] dishIcons;

    [Header("Tính cách")]
    public float patienceTime = 10f; // 10 giây
    private float currentTimer;

    [Header("Cấu hình Hồi phục")]
    public float respawnCooldown = 2.0f;
    public float nextSpawnTime = 0f;

    // Trạng thái
    public string currentRequest;
    private bool isActive = false;
    public bool IsActive => isActive;

    void Awake()
    {
        myBodyRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        HideCustomer();
    }

    void Update()
    {
        if (!isActive) return;

        // 1. Đếm ngược thời gian
        currentTimer -= Time.deltaTime;

        // --- ĐOẠN MỚI: CẬP NHẬT SLIDER ---
        if (timeSlider != null)
        {
            // Tính tỉ lệ phần trăm còn lại (từ 0 đến 1)
            float ratio = currentTimer / patienceTime;

            // Gán vào Slider (nó sẽ tự co ngắn lại)
            timeSlider.value = ratio;

            // Đổi màu từ Xanh sang Đỏ
            if (timeSliderFill != null)
            {
                // Lerp: Pha màu dựa theo ratio (1 là xanh, 0 là đỏ)
                timeSliderFill.color = Color.Lerp(Color.red, Color.green, ratio);
            }
        }
        // ---------------------------------

        // 2. Logic Giận dữ (khi còn dưới 30%)
        if (currentTimer <= patienceTime * 0.3f)
        {
            MakeAngry();
        }

        // 3. Hết giờ -> Bỏ đi
        if (currentTimer <= 0)
        {
            Leave(false);
        }
    }

    public void Appear()
    {
        isActive = true;
        gameObject.SetActive(true);
        currentTimer = patienceTime;

        // --- RESET SLIDER ---
        // Khi khách mới xuất hiện, bơm đầy thanh và cho về màu xanh
        if (timeSlider != null) timeSlider.value = 1;
        if (timeSliderFill != null) timeSliderFill.color = Color.green;
        // --------------------

        // Reset trạng thái
        if (angryIcon != null) angryIcon.SetActive(false);
        if (myBodyRenderer != null && normalSprite != null) myBodyRenderer.sprite = normalSprite;

        int randomIndex = Random.Range(0, possibleDishes.Length);
        currentRequest = possibleDishes[randomIndex];

        if (bubbleRenderer != null && dishIcons.Length > randomIndex)
        {
            bubbleRenderer.gameObject.SetActive(true);
            bubbleRenderer.sprite = dishIcons[randomIndex];
        }

        // Phát âm thanh khách đến
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.PlayCustomerArriveSound();
        }
    }

    public void ReceiveDish(string dishName)
    {
        if (!isActive) return;

        if (dishName == currentRequest)
        {
            int points = (dishName == "MiY") ? 30 : 20;
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.AddScore(points);
                LevelManager.Instance.PlayCorrectSound(); // Phát âm thanh đúng
            }
            Leave(true);
        }
        else
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.AddScore(-5);
                LevelManager.Instance.PlayWrongSound(); // Phát âm thanh sai
            }
            MakeAngry();
            Debug.Log("Sai món! Khách giận.");
            Leave(false);
        }
    }

    void MakeAngry()
    {
        if (angryIcon != null) angryIcon.SetActive(true);
        if (myBodyRenderer != null && angrySprite != null) myBodyRenderer.sprite = angrySprite;
    }

    void Leave(bool isHappy)
    {
        isActive = false;
        HideCustomer(); // Ẩn ngay lập tức
    }

    public void HideCustomer()
    {
        isActive = false;
        gameObject.SetActive(false);
        if (bubbleRenderer != null) bubbleRenderer.sprite = null;
        nextSpawnTime = Time.time + respawnCooldown;
    }
}