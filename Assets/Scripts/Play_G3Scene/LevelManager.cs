using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Bắt buộc có để dùng TextMeshPro

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
}