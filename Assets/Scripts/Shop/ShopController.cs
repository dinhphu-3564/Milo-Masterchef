using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopController : MonoBehaviour
{
    [Header("Gold")]
    public TextMeshProUGUI goldText;

    [Header("Inventory UI")]
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
    
    // Lưu trữ vị trí gốc của các nút ngay khi vào game
    private Dictionary<Transform, Vector2> buttonOriginalPositions = new Dictionary<Transform, Vector2>();
    // Đánh dấu nút nào đang trong quá trình rung
    private HashSet<Transform> buttonsCurrentlyShaking = new HashSet<Transform>();

    void Awake()
    {
        // Lưu vị trí chuẩn của tất cả các nút để dùng mãi mãi
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

    void UpdateUI()
    {
        int gold = GoldData.GetGold();
        goldText.text = gold + "$";
        waterCountText.text = "x " + ItemData.GetItem("ITEM_WATER");
        grainCountText.text = "x " + ItemData.GetItem("ITEM_GRAIN");
        grassCountText.text = "x " + ItemData.GetItem("ITEM_GRASS");

        UpdateButtonColor(buyWaterBtn, gold >= 20);
        UpdateButtonColor(buyGrainBtn, gold >= 30);
        UpdateButtonColor(buyGrassBtn, gold >= 15);
    }

    public void BuyWater() { BuyItem(20, "ITEM_WATER", buyWaterBtn); }
    public void BuyGrain() { BuyItem(30, "ITEM_GRAIN", buyGrainBtn); }
    public void BuyGrass() { BuyItem(15, "ITEM_GRASS", buyGrassBtn); }

    void BuyItem(int price, string key, Button btn)
    {
        int gold = GoldData.GetGold();

        if (gold < price)
        {
            ShowNotEnoughGold();
            
            // Nếu nút này chưa rung thì mới cho rung
            if (!buttonsCurrentlyShaking.Contains(btn.transform))
            {
                StartCoroutine(SafeShake(btn.transform));
            }
            return;
        }

        GoldData.SetGold(gold - price);
        ItemData.AddItem(key, 1);
        UpdateUI();
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

    // HÀM RUNG AN TOÀN TUYỆT ĐỐI
    IEnumerator SafeShake(Transform btnTransform)
    {
        buttonsCurrentlyShaking.Add(btnTransform);
        RectTransform rect = btnTransform as RectTransform;
        Vector2 originalAnchoredPos = buttonOriginalPositions[btnTransform];

        float duration = 0.2f; // Rung trong 0.2 giây
        float elapsed = 0f;
        float strength = 12f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để mượt hơn
            
            // Tạo độ lệch ngẫu nhiên
            float offsetX = Random.Range(-1f, 1f) * strength;
            
            // Áp dụng độ lệch dựa trên VỊ TRÍ GỐC ĐÃ LƯU (Không lấy vị trí hiện tại)
            rect.anchoredPosition = new Vector2(originalAnchoredPos.x + offsetX, originalAnchoredPos.y);
            
            yield return null;
        }

        // KẾT THÚC: Ép nút về đúng vị trí gốc ban đầu
        rect.anchoredPosition = originalAnchoredPos;
        buttonsCurrentlyShaking.Remove(btnTransform);
    }

    void UpdateButtonColor(Button btn, bool canBuy)
    {
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = canBuy ? normalColor : notEnoughColor;
    }
}