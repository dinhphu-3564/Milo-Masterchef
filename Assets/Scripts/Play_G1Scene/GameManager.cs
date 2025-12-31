using UnityEngine;
using TMPro;
// public TextMeshProUGUI finalGoldText;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;     

    [Header("Score")]
    public int gold = 0;                    // số vàng hiện tại
    public TextMeshProUGUI goldText;        // text hiển thị vàng

    [Header("Time")]
    public float gameTime = 90f;            // thời gian chơi
    private float currentTime;              // thời gian hiện tại
    public TextMeshProUGUI timeText;        // text hiển thị thời gian

    [Header("UI")]
    public GameObject startPanel;           // panel có nút PLAY
    public GameObject gameOverPanel;        // panel hết giờ
    public TextMeshProUGUI finalGoldText;   // text hiển thị vàng cuối cùng

    //Âm thanh Scene_G1
    [Header("Audio")]
    public AudioSource bgmSource;           // nhạc nền

    [Header("SFX")]
    public AudioSource eatSource;           // âm thanh ăn trái cây
    public AudioSource bombSource;          // âm thanh bom nổ


    private bool isPlaying = false;         // trạng thái chơi game


    void Awake()                            
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentTime = gameTime;
        UpdateGoldUI();
        UpdateTimeUI();

        Time.timeScale = 0f;

        // ÁP DỤNG MUTE THEO SETTING
        if (AudioManager.Instance != null && bgmSource != null)
        {
            bgmSource.mute = AudioManager.Instance.isMuted;
        }
    }


    void Update()
    {
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


    // ===== PLAY GAME =====
    public void PlayGame()
    {
        gold = 0;
        currentTime = gameTime;
        isPlaying = true;

        UpdateGoldUI();
        UpdateTimeUI();

        startPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f; // bắt đầu game

        if (bgmSource != null && AudioManager.Instance != null && !AudioManager.Instance.isMuted)
        {
            bgmSource.Play();
        }
    }

    // Chơi lại 
    public void RestartGame()
    {
        // 1. Reset các thông số về ban đầu
        gold = 0;
        currentTime = gameTime;
        isPlaying = true;

        // 2. Cập nhật lại giao diện (UI)
        UpdateGoldUI();
        UpdateTimeUI();

        // 3. Ẩn bảng GameOver
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 4. Chạy lại thời gian trong game
        Time.timeScale = 1f;

        // 5. Phát lại nhạc nền
        if (bgmSource != null)
        {
            bgmSource.Stop();
            if (AudioManager.Instance != null && !AudioManager.Instance.isMuted)
                bgmSource.Play();
        }
    }

    // ===== END GAME =====
    void EndGame()
    {
        isPlaying = false;                      // dừng trạng thái chơi
        Time.timeScale = 0f;                    // dừng thời gian trong game

        if (gameOverPanel != null)              // hiển thị bảng GameOver
            gameOverPanel.SetActive(true);      
    }

    // ===== ADD GOLD =====
    public void AddGold(int amount)             
    {
        if (!isPlaying) return;                 // hết giờ không cộng vàng

        gold += amount;                         // cộng vàng
        UpdateGoldUI();                         // cập nhật giao diện
    }

    // trừ vàng bomb và rock
    public void MinusGold(int amount)
    {
        if (!isPlaying) return;

        gold -= amount;
        if (gold < 0) gold = 0;

        UpdateGoldUI();
    }

    // CẬP NHẬT GIAO DIỆN
    void UpdateGoldUI()
    {
        if (goldText != null)                       // cập nhật text vàng
            goldText.text = gold.ToString();        // hiển thị số vàng hiện tại
    }

    // CẬP NHẬT THỜI GIAN
    void UpdateTimeUI()
    {
        if (timeText != null)                                              // cập nhật text thời gian    
            timeText.text = Mathf.CeilToInt(currentTime).ToString();       // hiển thị thời gian hiện tại
    }

    // HIỂN THỊ GIAO DIỆN KẾT THÚC GAME
    void EndGameUI()
    {
        isPlaying = false;                      // dừng trạng thái chơi
        Time.timeScale = 0f;                    // dừng thời gian trong game

        // Dừng nhạc nền
        if (bgmSource != null)
        bgmSource.Stop();

        // LƯU GOLD SAU KHI HẾT GAME
        GoldData.AddGold(gold);

        // Hiển thị bảng GameOver
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // hiển thị số điểm lên bảng GameOver
            if (finalGoldText != null)
            {
                finalGoldText.text = gold.ToString() + "$"; 
            }
        }
    }

    // âm thanh ăn trái cây
    public void PlayEatSound()
    {
        if (AudioManager.Instance != null && !AudioManager.Instance.isMuted && eatSource != null)
            eatSource.Play();
    }

    // âm thanh bomb và rock
    public void PlayBombSound()
    {
        if (AudioManager.Instance != null && !AudioManager.Instance.isMuted && bombSource != null)
            bombSource.Play();
    }

    
}
