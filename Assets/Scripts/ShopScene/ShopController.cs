using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    [Header("Gold")]
    public TextMeshProUGUI goldText;

    [Header("Inventory UI (Kéo Text hiển thị số lượng vào đây)")]
    public TextMeshProUGUI waterCountText;
    public TextMeshProUGUI grainCountText;
    public TextMeshProUGUI grassCountText;

    [Header("Buy Buttons")]
    public Button buyWaterBtn;
    public Button buyGrainBtn;
    public Button buyGrassBtn;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color notEnoughColor = Color.red;

    [Header("Notify")]
    public GameObject notEnoughGoldText;

    private Coroutine notifyCoroutine;

    // Lưu trữ vị trí gốc của các nút
    private Dictionary<Transform, Vector2> buttonOriginalPositions = new Dictionary<Transform, Vector2>();
    private HashSet<Transform> buttonsCurrentlyShaking = new HashSet<Transform>();

    void Awake()
    {
        // Lưu vị trí chuẩn của tất cả các nút
        SavePosition(buyWaterBtn.transform);
        SavePosition(buyGrainBtn.transform);
        SavePosition(buyGrassBtn.transform);
    }

    void SavePosition(Transform t)
    {
        RectTransform rt = t as RectTransform;
        if (rt != null) buttonOriginalPositions[t] = rt.anchoredPosition;
    }

    void Start()
    {
        if (notEnoughGoldText != null) notEnoughGoldText.SetActive(false);
        UpdateUI();
    }

    // [QUAN TRỌNG] Chạy mỗi khi Shop được bật lên để cập nhật số liệu mới nhất
    void OnEnable()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // 1. Cập nhật tiền
        int gold = GoldData.GetGold();
        if (goldText != null) goldText.text = gold + "$";

        // 2. Cập nhật số lượng nguyên liệu (Lấy từ ItemData)
        // Lưu ý: Đảm bảo ItemData đã có các key này
        if (waterCountText != null)
            waterCountText.text = "x " + ItemData.GetItem("ITEM_WATER");

        if (grainCountText != null)
            grainCountText.text = "x " + ItemData.GetItem("ITEM_GRAIN");

        if (grassCountText != null)
            grassCountText.text = "x " + ItemData.GetItem("ITEM_GRASS");

        // 3. Đổi màu nút nếu không đủ tiền
        UpdateButtonColor(buyWaterBtn, gold >= 20);
        UpdateButtonColor(buyGrainBtn, gold >= 30);
        UpdateButtonColor(buyGrassBtn, gold >= 15);
    }

    // --- CÁC HÀM MUA HÀNG ---

    public void BuyWater() { BuyItem(20, "ITEM_WATER", buyWaterBtn); }
    public void BuyGrain() { BuyItem(30, "ITEM_GRAIN", buyGrainBtn); }
    public void BuyGrass() { BuyItem(15, "ITEM_GRASS", buyGrassBtn); }

    void BuyItem(int price, string key, Button btn)
    {
        int gold = GoldData.GetGold();

        // KHÔNG ĐỦ VÀNG
        if (gold < price)
        {
            ShowNotEnoughGold();

            // Rung nút cảnh báo
            if (!buttonsCurrentlyShaking.Contains(btn.transform))
            {
                StartCoroutine(SafeShake(btn.transform));
            }
            return;
        }

        // ĐỦ VÀNG → MUA THÀNH CÔNG
        GoldData.SetGold(gold - price);
        ItemData.AddItem(key, 1);

        UpdateUI(); // Cập nhật lại giao diện ngay lập tức
    }

    void ShowNotEnoughGold()
    {
        if (notEnoughGoldText == null) return;
        if (notifyCoroutine != null) StopCoroutine(notifyCoroutine);
        notifyCoroutine = StartCoroutine(HideNotifyAfterDelay());
    }

    IEnumerator HideNotifyAfterDelay()
    {
        notEnoughGoldText.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        notEnoughGoldText.SetActive(false);
        notifyCoroutine = null;
    }

    // HÀM RUNG NÚT
    IEnumerator SafeShake(Transform btnTransform)
    {
        buttonsCurrentlyShaking.Add(btnTransform);
        RectTransform rect = btnTransform as RectTransform;

        if (!buttonOriginalPositions.ContainsKey(btnTransform)) SavePosition(btnTransform);
        Vector2 originalAnchoredPos = buttonOriginalPositions[btnTransform];

        float duration = 0.2f;
        float elapsed = 0f;
        float strength = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float offsetX = Random.Range(-1f, 1f) * strength;
            rect.anchoredPosition = new Vector2(originalAnchoredPos.x + offsetX, originalAnchoredPos.y);
            yield return null;
        }

        rect.anchoredPosition = originalAnchoredPos;
        buttonsCurrentlyShaking.Remove(btnTransform);
    }

    void UpdateButtonColor(Button btn, bool canBuy)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = canBuy ? normalColor : notEnoughColor;
    }
}