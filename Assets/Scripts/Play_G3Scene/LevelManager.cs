using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Bắt buộc có để dùng TextMeshPro
using Play_G3Scene; // Để dùng InventoryManager

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Cài đặt UI trong Game")]
    public TMP_Text scoreText; // Điểm số góc màn hình
    public TMP_Text timeText;  // Đồng hồ góc màn hình

    [Header("Cài đặt UI Game Over")]
    public GameObject gameOverPanel; // Cái bảng Game Over to đùng
    public GameObject gameStartPanel; // <--- MỚI: Bảng hướng dẫn đầu game
    public TMP_Text finalScoreText;  // <--- MỚI: Cái số 0 ở giữa bảng Game Over

    [Header("Thông số Game")]
    public float levelDuration = 60f;
    public int currentScore = 0;

    public static bool isRestart = false;
    private float timeRemaining;
    public bool isGameOver = false;
    public GameObject ingredientsBasket;

    [Header("Audio")]
    public AudioSource bgmSource;              // Nhạc nền
    [Header("SFX - Sound Effects")]
    public AudioSource sfxSource;              // Nguồn âm thanh hiệu ứng (có thể tái sử dụng)
    [Range(0f, 1f)]
    public float sfxVolume = 1f;               // Âm lượng SFX (0-1)
    [Range(0f, 1f)]
    public float bgmVolume = 1f;               // Âm lượng BGM (0-1)
    
    public AudioClip soundPickIngredient;       // Âm thanh khi click lấy nguyên liệu
    public AudioClip soundCooking;             // Âm thanh khi nấu ăn
    public AudioClip soundCorrect;             // Âm thanh khi đúng món
    public AudioClip soundWrong;               // Âm thanh khi sai món
    public AudioClip soundCustomerArrive;      // Âm thanh khi khách đến
    public AudioClip soundGameOver;            // Âm thanh khi game over

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        // --- KIỂM TRA KẾT NỐI UI ---
        if (scoreText == null) Debug.LogError("LỖI: ScoreText đang bị Null (Trống)!");
        if (timeText == null) Debug.LogError("LỖI: TimeText đang bị Null (Trống)!");
        // ---------------------------

        // Debug để kiểm tra xem game có hiểu là đang Restart không
        Debug.Log("Start chạy! isRestart = " + isRestart);

        if (isRestart)
        {
            Debug.Log(">>> Đang Restart -> Bỏ qua bảng hướng dẫn -> Chơi luôn!");
            if (gameStartPanel != null) gameStartPanel.SetActive(false);

            // QUAN TRỌNG: Reset biến này về false ngay lập tức cho lần sau
            isRestart = false;

            StartGameplay();
        }
        else if (gameStartPanel != null)
        {
            Debug.Log(">>> Lần đầu vào game -> Hiện bảng hướng dẫn");
            gameStartPanel.SetActive(true);

            // Đóng băng game
            Time.timeScale = 0;
            ToggleIngredientsInteract(false);
        }
        else
        {
            StartGameplay();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        timeRemaining = levelDuration;
        UpdateUI();
    }
    public void StartGameplay()
    {
        // 1. Ẩn bảng hướng dẫn
        if (gameStartPanel != null) gameStartPanel.SetActive(false);

        // 2. Mở khóa thời gian -> Game bắt đầu chạy
        Time.timeScale = 1;

        // 3. Mở khóa tương tác nguyên liệu
        ToggleIngredientsInteract(true);

        // 4. Phát nhạc nền
        if (bgmSource != null && bgmSource.clip != null)
        {
            bgmSource.volume = bgmVolume; // Áp dụng volume
            bgmSource.loop = true;
            bgmSource.Play();
        }

        Debug.Log(">>> GAME START!");
    }

    void Update()
    {
        if (isGameOver || Time.timeScale == 0) return;
        if (timeRemaining > 0) { timeRemaining -= Time.deltaTime; UpdateTimerUI(); }
        else EndGame();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = currentScore.ToString();
    }

    void UpdateTimerUI()
    {
        if (timeText != null)
        {
            timeText.text = Mathf.RoundToInt(timeRemaining).ToString();
            if (timeRemaining < 10) timeText.color = Color.red;
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        currentScore += amount;
        UpdateUI();
    }
    void ToggleIngredientsInteract(bool state)
    {
        if (ingredientsBasket != null)
        {
            Collider2D[] allColliders = ingredientsBasket.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in allColliders) col.enabled = state;
        }
    }

    // --- HÀM KẾT THÚC GAME (QUAN TRỌNG) ---
    void EndGame()
    {
        isGameOver = true;
        timeRemaining = 0;

        // 1. Hiện bảng Game Over
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 2. Cập nhật điểm tổng kết vào giữa bảng
        if (finalScoreText != null)
        {
            finalScoreText.text = currentScore.ToString();
        }

        // --- Lưu điểm tinh thông vào PlayerPrefs ---
        ProficiencyData.AddProficiency(currentScore);
        Debug.Log($"Đã lưu điểm tinh thông: {currentScore}. Tổng hiện tại: {ProficiencyData.GetProficiency()}/1000");

        // --- Lưu nguyên liệu còn lại ---
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveRemainingIngredients();
        }

        if (ingredientsBasket != null)
        {
            Collider2D[] allColliders = ingredientsBasket.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in allColliders)
            {
                col.enabled = false; // Tắt va chạm -> Chuột sẽ bấm xuyên qua được
            }
        }
        // 3. DỪNG TOÀN BỘ GAME (Đóng băng thời gian)
        // Lệnh này sẽ làm mọi thứ đứng im: Khách không đi, không nấu ăn được nữa
        Time.timeScale = 0;

        // 4. Dừng nhạc nền và phát âm thanh game over
        if (bgmSource != null) bgmSource.Stop();
        PlaySFX(soundGameOver);

        Debug.Log(">>> GAME OVER - GAME PAUSED");
    }

    // --- HÀM NÚT CHƠI LẠI ---
    public void RestartGame()
    {
        isRestart = true;
        // Quan trọng: Phải mở lại thời gian trước khi load màn mới
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- HÀM NÚT MENU (Nếu bạn cần) ---
    public void BackToMenu()
    {
        isRestart = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu"); // Đổi tên "MenuScene" thành tên màn hình menu của bạn
    }

    // ===================== AUDIO =====================
    // Hàm phát âm thanh hiệu ứng (SFX) với volume tùy chỉnh
    public void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("LevelManager.PlaySFX: AudioClip is null!");
            return;
        }

        // Nếu không chỉ định volume, dùng volume mặc định
        float finalVolume = (volume < 0) ? sfxVolume : volume;

        // Nếu có AudioSource riêng cho SFX, dùng nó
        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, finalVolume);
        }
        else
        {
            // Nếu không có, tạo AudioSource tạm thời
            if (Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, finalVolume);
            }
            else
            {
                Debug.LogWarning("LevelManager.PlaySFX: Camera.main is null!");
            }
        }
    }

    // Hàm phát âm thanh nấu ăn
    public void PlayCookingSound()
    {
        PlaySFX(soundCooking);
    }

    // Hàm phát âm thanh đúng món
    public void PlayCorrectSound()
    {
        PlaySFX(soundCorrect);
    }

    // Hàm phát âm thanh sai món
    public void PlayWrongSound()
    {
        PlaySFX(soundWrong);
    }

    // Hàm phát âm thanh khách đến
    public void PlayCustomerArriveSound()
    {
        PlaySFX(soundCustomerArrive);
    }

    // Hàm phát âm thanh khi click lấy nguyên liệu
    public void PlayPickIngredientSound()
    {
        PlaySFX(soundPickIngredient);
    }
}