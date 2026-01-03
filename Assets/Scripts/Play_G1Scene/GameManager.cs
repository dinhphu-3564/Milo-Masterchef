using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ===================== SCORE =====================
    [Header("Score")]
    public int gold = 0;                          // Số vàng hiện tại
    public TextMeshProUGUI goldText;              // Text hiển thị vàng

    // ===================== TIME ======================
    [Header("Time")]
    public float gameTime = 90f;                  // Tổng thời gian chơi
    private float currentTime;                    // Thời gian còn lại
    public TextMeshProUGUI timeText;              // Text hiển thị thời gian

    // ===================== UI ========================
    [Header("UI")]
    public GameObject startPanel;                 // Panel bắt đầu
    public GameObject gameOverPanel;              // Panel GameOver
    public TextMeshProUGUI finalGoldText;         // Text vàng cuối game

    // ===================== AUDIO =====================
    [Header("Audio")]
    public AudioSource bgmSource;                 // Nhạc nền

    [Header("SFX")]
    public AudioSource eatSource;                 // Âm thanh ăn
    public AudioSource bombSource;                // Âm thanh bom / rock

    // ================= FLOATING TEXT =================
    [Header("Floating Text")]
    public GameObject floatingTextPrefab;         // Prefab chữ bay
    public Transform canvasTransform;             // Canvas chứa chữ bay

    // ===================== STATE =====================
    private bool isPlaying = false;               // Trạng thái game


    // ===================== AWAKE =====================
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ===================== START =====================
    void Start()
    {
        currentTime = gameTime;

        UpdateGoldUI();
        UpdateTimeUI();

        Time.timeScale = 0f; // Tạm dừng game khi mới vào

        // Đồng bộ mute với AudioManager
        if (AudioManager.Instance != null && bgmSource != null)
        {
            bgmSource.mute = AudioManager.Instance.isMuted;
        }
    }

    // ===================== UPDATE ====================
    void Update()
    {
        // Đồng bộ mute liên tục
        if (AudioManager.Instance != null && bgmSource != null)
        {
            bgmSource.mute = AudioManager.Instance.isMuted;
        }

        if (!isPlaying) return;

        currentTime -= Time.deltaTime;
        UpdateTimeUI();

        if (currentTime <= 0)
        {
            EndGameUI();
        }
    }

    // ===================== PLAY GAME =================
    public void PlayGame()
    {
        gold = 0;
        currentTime = gameTime;
        isPlaying = true;

        UpdateGoldUI();
        UpdateTimeUI();

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        if (bgmSource != null && AudioManager.Instance != null && !AudioManager.Instance.isMuted)
        {
            bgmSource.Play();
        }
    }

    // ===================== RESTART ===================
    public void RestartGame()
    {
        gold = 0;
        currentTime = gameTime;
        isPlaying = true;

        UpdateGoldUI();
        UpdateTimeUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        if (bgmSource != null)
        {
            bgmSource.Stop();
            if (AudioManager.Instance != null && !AudioManager.Instance.isMuted)
                bgmSource.Play();
        }
    }

    // ===================== END GAME ==================
    void EndGame()
    {
        isPlaying = false;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void EndGameUI()
    {
        isPlaying = false;
        Time.timeScale = 0f;

        if (bgmSource != null)
            bgmSource.Stop();

        // Lưu vàng sau khi kết thúc
        GoldData.AddGold(gold);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalGoldText != null)
                finalGoldText.text = gold + "$";
        }
    }

    // ===================== GOLD ======================
    public void AddGold(int amount)
    {
        if (!isPlaying) return;

        gold += amount;
        UpdateGoldUI();
    }

    public void MinusGold(int amount)
    {
        if (!isPlaying) return;

        gold -= amount;
        if (gold < 0) gold = 0;

        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = gold + "$";
    }

    // ===================== TIME UI ===================
    void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = Mathf.CeilToInt(currentTime) + "s";
    }

    // ===================== SFX =======================
    public void PlayEatSound()
    {
        if (AudioManager.Instance != null && !AudioManager.Instance.isMuted && eatSource != null)
            eatSource.Play();
    }

    public void PlayBombSound()
    {
        if (AudioManager.Instance != null && !AudioManager.Instance.isMuted && bombSource != null)
            bombSource.Play();
    }

    // ================= FLOATING TEXT ================
    public void ShowFloatingText(Vector3 worldPosition, string message, Color color)
    {
        if (floatingTextPrefab == null || canvasTransform == null) return;

        GameObject textObj = Instantiate(floatingTextPrefab, canvasTransform);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        textObj.transform.position = screenPos;

        FloatingText ft = textObj.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetText(message, color);
        }
    }
}
