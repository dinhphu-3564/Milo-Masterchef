using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressSceneManager : MonoBehaviour
{
    [Header("UI Slider và Text")]
    public Slider proficiencySlider;        // Kéo Slider vào đây
    public TMP_Text proficiencyText;        // Text hiển thị "ĐIỂM TINH THÔNG: X/1000"
    public TMP_Text percentageText;         // Text hiển thị phần trăm (tùy chọn)

    [Header("UI Win Game")]
    public GameObject winPanel;             // Panel thông báo Win Game (tùy chọn)
    public Button resetButton;              // Nút Reset sau khi win (tùy chọn)

    private const int MAX_PROFICIENCY = 1000;

    void Start()
    {
        // Tự động cấu hình slider để có góc bo tròn (nếu dùng sprite 9-slice)
        ConfigureSliderForRoundedCorners();
        
        UpdateProgressUI();
        CheckWinCondition();

        // Nếu có nút Reset, gán sự kiện
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetClicked);
        }
    }

    // Hàm cấu hình slider để có góc bo tròn
    // Yêu cầu: Sprite của Fill Image phải được set thành Sprite với Border (9-slice)
    // Trong Unity: Chọn sprite > Sprite Editor > Set border > Apply
    void ConfigureSliderForRoundedCorners()
    {
        if (proficiencySlider != null)
        {
            // Tìm Fill Area > Fill Image
            Transform fillArea = proficiencySlider.transform.Find("Fill Area");
            if (fillArea != null)
            {
                Transform fillTransform = fillArea.Find("Fill");
                if (fillTransform != null)
                {
                    Image fillImage = fillTransform.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        // Set Image Type thành Sliced để sử dụng border của sprite (tạo góc bo tròn)
                        fillImage.type = Image.Type.Sliced;
                        
                        // Đảm bảo Fill Method là Horizontal (từ trái sang phải)
                        fillImage.fillMethod = Image.FillMethod.Horizontal;
                        fillImage.fillOrigin = 0; // Fill từ trái
                        
                        Debug.Log("Đã cấu hình Slider Fill Image thành Sliced. Lưu ý: Sprite cần có border (9-slice) để bo tròn hoạt động.");
                    }
                }
            }
        }
    }

    void UpdateProgressUI()
    {
        // Lấy điểm tinh thông hiện tại
        int currentProficiency = ProficiencyData.GetProficiency();
        float percentage = ProficiencyData.GetPercentage();

        // Cập nhật Slider (từ 0 đến 1)
        if (proficiencySlider != null)
        {
            proficiencySlider.value = percentage;
            proficiencySlider.maxValue = 1f; // Slider Unity dùng 0-1 cho phần trăm
        }

        // Cập nhật Text điểm
        if (proficiencyText != null)
        {
            proficiencyText.text = $" {currentProficiency}/{MAX_PROFICIENCY}";
        }

        // Cập nhật Text phần trăm (nếu có)
        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.RoundToInt(percentage * 100)}%";
        }

        Debug.Log($"Progress Scene - Điểm hiện tại: {currentProficiency}/{MAX_PROFICIENCY} ({percentage * 100}%)");
    }

    void CheckWinCondition()
    {
        // Kiểm tra xem đã đủ 1000 điểm chưa
        if (ProficiencyData.IsGameWon())
        {
            Debug.Log(">>> WIN GAME! Đã đạt 1000 điểm tinh thông!");
            
            // Hiển thị panel Win Game (nếu có)
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }

            // Tự động reset (nếu muốn) hoặc để người chơi bấm nút
            // AutoReset(); // Bỏ comment nếu muốn tự động reset
        }
    }

    // Hàm reset thanh progress về 0
    public void OnResetClicked()
    {
        ProficiencyData.ResetProficiency();
        UpdateProgressUI();
        
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        Debug.Log(">>> Đã reset điểm tinh thông về 0");
    }

    // Tự động reset (gọi khi win game)
    void AutoReset()
    {
        ProficiencyData.ResetProficiency();
        UpdateProgressUI();
        Debug.Log(">>> Tự động reset điểm tinh thông sau khi win game");
    }
}
