using UnityEngine;
using TMPro;
using System; // [BẮT BUỘC] Để xử lý thời gian thực

public class SoilSlot : MonoBehaviour
{
    // [CỰC KỲ QUAN TRỌNG] ID này phải KHÁC NHAU ở mỗi ô đất trong Inspector
    // Ví dụ: Ruộng lúa là Rice_01, Rice_02... Vườn là Garden_01, Garden_02...
    public string slotID = "Slot_01";

    public static SoilSlot activeSlot;

    public enum SoilState { Empty, Seeded, Growing, Ready }
    public enum FieldType { RiceField, GardenField }
    public enum PlantType { Rice, Cabbage, Tomato }

    [Header("--- CẤU HÌNH LOẠI RUỘNG ---")]
    public FieldType fieldType = FieldType.RiceField;

    [Header("--- TRẠNG THÁI (Read Only) ---")]
    public SoilState currentState = SoilState.Empty;
    public PlantType currentPlant = PlantType.Rice;

    [Header("--- UI CHỌN HẠT ---")]
    public GameObject selectionPanel;

    [Header("--- NGUYÊN LIỆU ---")]
    public string seedKey = "ITEM_GRAIN";
    public string waterKey = "ITEM_WATER";

    [Header("--- SẢN PHẨM ---")]
    public string productRiceKey = "ITEM_RICE";
    public string productCabbageKey = "ITEM_VEGGIE";
    public string productTomatoKey = "ITEM_TOMATO";

    [Header("--- HÌNH ẢNH ---")]
    public SpriteRenderer plantRenderer;
    public Sprite imgSeed;
    public Sprite imgGrowing;
    public Sprite imgReadyRice;
    public Sprite imgReadyCabbage;
    public Sprite imgReadyTomato;

    [Header("--- CÀI ĐẶT CHUNG ---")]
    public float timeToGrow = 10f;
    public GameObject highlightObj;
    public GameObject floatingTextPrefab;
    public TextMeshProUGUI timerText;

    private float currentTimer = 0f;

    void Start()
    {
        if (plantRenderer == null) plantRenderer = GetComponent<SpriteRenderer>();
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (selectionPanel != null) selectionPanel.SetActive(false);
        if (highlightObj != null) highlightObj.SetActive(false);

        // Kiểm tra xem người dùng có quên đổi tên ID không
        if (slotID == "Slot_01" && transform.GetSiblingIndex() > 0)
        {
            Debug.LogWarning($"CẢNH BÁO: Ô đất {name} đang dùng ID mặc định 'Slot_01'. Hãy đổi tên ID khác trong Inspector!");
        }

        // Tải dữ liệu ngay khi vào game
        LoadData();
    }

    void OnDisable()
    {
        if (activeSlot == this) activeSlot = null;
        if (selectionPanel != null) selectionPanel.SetActive(false);
        SetPlayerLock(false);

        // Lưu lần cuối khi tắt object
        SaveData();
    }

    void Update()
    {
        if (currentState == SoilState.Growing)
        {
            currentTimer -= Time.deltaTime;

            if (timerText != null)
            {
                if (currentTimer > 0)
                {
                    int minutes = Mathf.FloorToInt(currentTimer / 60F);
                    int seconds = Mathf.FloorToInt(currentTimer - minutes * 60);
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }
                else timerText.text = "00:00";
            }

            if (currentTimer <= 0) FinishGrowing();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectionPanel != null && selectionPanel.activeSelf) ClosePanel();
        }
    }

    public void SetHighlight(bool isOn)
    {
        if (highlightObj != null) highlightObj.SetActive(isOn);
    }

    public void Interact()
    {
        switch (currentState)
        {
            case SoilState.Empty: HandleEmptyInteract(); break;
            case SoilState.Seeded: AttemptWater(); break;
            case SoilState.Growing: ShowFloatingMessage("Cây đang lớn...", Color.yellow); break;
            case SoilState.Ready: Harvest(); break;
        }
    }

    void HandleEmptyInteract()
    {
        if (fieldType == FieldType.RiceField) SelectRiceToPlant();
        else if (fieldType == FieldType.GardenField)
        {
            if (selectionPanel != null && !selectionPanel.activeSelf)
            {
                activeSlot = this;
                selectionPanel.SetActive(true);
                SetPlayerLock(true);
            }
            else SelectCabbageToPlant();
        }
    }

    public void SelectCabbageToPlant() { AttemptSowSeed(PlantType.Cabbage); ClosePanel(); }
    public void SelectTomatoToPlant() { AttemptSowSeed(PlantType.Tomato); ClosePanel(); }
    void SelectRiceToPlant() { AttemptSowSeed(PlantType.Rice); }

    public void ClosePanel()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
        SetPlayerLock(false);
        activeSlot = null;
    }

    // --- LOGIC TRỒNG CÂY ---

    void AttemptSowSeed(PlantType typeToPlant)
    {
        int currentSeeds = ItemData.GetItem(seedKey);
        if (currentSeeds > 0)
        {
            ItemData.AddItem(seedKey, -1);

            currentState = SoilState.Seeded;
            currentPlant = typeToPlant;

            UpdateVisual();
            SaveData(); // [LƯU NGAY LẬP TỨC]

            ShowFloatingMessage("-1 Hạt giống", Color.white);
        }
        else ShowFloatingMessage("Thiếu Hạt giống!", Color.red);
    }

    void AttemptWater()
    {
        int currentWater = ItemData.GetItem(waterKey);
        if (currentWater > 0)
        {
            ItemData.AddItem(waterKey, -1);
            currentState = SoilState.Growing;

            // [REAL-TIME] Lưu thời gian đích (Dùng UtcNow cho chuẩn)
            DateTime finishTime = DateTime.UtcNow.AddSeconds(timeToGrow);
            PlayerPrefs.SetString(slotID + "_FinishTime", finishTime.ToBinary().ToString());

            currentTimer = timeToGrow;
            UpdateVisual();
            SaveData(); // [LƯU NGAY LẬP TỨC]

            if (timerText != null) timerText.gameObject.SetActive(true);
            ShowFloatingMessage("-1 Nước", Color.cyan);
        }
        else ShowFloatingMessage("Cần nước!", Color.blue);
    }

    void FinishGrowing()
    {
        currentState = SoilState.Ready;
        UpdateVisual();
        SaveData(); // [LƯU TRẠNG THÁI CHÍN]

        if (timerText != null) timerText.gameObject.SetActive(false);
        ShowFloatingMessage("Đã chín!", Color.green);
    }

    void Harvest()
    {
        int amount = 2;
        string finalProductKey = productRiceKey;
        string productNameUI = "Lúa";

        if (currentPlant == PlantType.Cabbage) { finalProductKey = productCabbageKey; productNameUI = "Rau Cải"; }
        else if (currentPlant == PlantType.Tomato) { finalProductKey = productTomatoKey; productNameUI = "Cà Chua"; }

        if (InventoryManager.instance != null) InventoryManager.instance.AddItem(finalProductKey, amount);
        ShowFloatingMessage("+" + amount + " " + productNameUI, Color.green);

        currentState = SoilState.Empty;
        UpdateVisual();

        SaveData(); // [LƯU TRẠNG THÁI RỖNG]
    }

    void UpdateVisual()
    {
        if (plantRenderer == null) return;
        switch (currentState)
        {
            case SoilState.Empty: plantRenderer.sprite = null; break;
            case SoilState.Seeded: plantRenderer.sprite = imgSeed; break;
            case SoilState.Growing: plantRenderer.sprite = imgGrowing; break;
            case SoilState.Ready:
                if (currentPlant == PlantType.Rice) plantRenderer.sprite = imgReadyRice;
                else if (currentPlant == PlantType.Cabbage) plantRenderer.sprite = imgReadyCabbage;
                else if (currentPlant == PlantType.Tomato) plantRenderer.sprite = imgReadyTomato;
                break;
        }
    }

    // --- HỆ THỐNG SAVE / LOAD ---

    void SaveData()
    {
        PlayerPrefs.SetInt(slotID + "_State", (int)currentState);
        PlayerPrefs.SetInt(slotID + "_Plant", (int)currentPlant);
        PlayerPrefs.Save();

        // Debug để bạn kiểm tra xem nó lưu đúng ID chưa
        Debug.Log($"[SAVE] {slotID}: State={currentState}");
    }

    void LoadData()
    {
        if (PlayerPrefs.HasKey(slotID + "_State"))
        {
            currentState = (SoilState)PlayerPrefs.GetInt(slotID + "_State");
            currentPlant = (PlantType)PlayerPrefs.GetInt(slotID + "_Plant");

            // Xử lý thời gian thực nếu cây đang lớn
            if (currentState == SoilState.Growing)
            {
                string timeStr = PlayerPrefs.GetString(slotID + "_FinishTime", "");
                if (!string.IsNullOrEmpty(timeStr))
                {
                    long temp = Convert.ToInt64(timeStr);
                    DateTime finishTime = DateTime.FromBinary(temp);

                    // Tính thời gian còn lại
                    double secondsLeft = (finishTime - DateTime.UtcNow).TotalSeconds;

                    if (secondsLeft <= 0)
                    {
                        FinishGrowing(); // Đã chín trong lúc tắt game
                    }
                    else
                    {
                        currentTimer = (float)secondsLeft;
                        if (timerText != null) timerText.gameObject.SetActive(true);
                    }
                }
            }

            UpdateVisual();
            Debug.Log($"[LOAD] {slotID}: State={currentState}");
        }
    }

    // Reset dữ liệu (Dùng để test nếu cần)
    [ContextMenu("Delete Save Data")]
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(slotID + "_State");
        PlayerPrefs.DeleteKey(slotID + "_Plant");
        PlayerPrefs.DeleteKey(slotID + "_FinishTime");
        Debug.Log($"Đã xóa dữ liệu của {slotID}");
    }

    void SetPlayerLock(bool isLocked)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerG2 s = player.GetComponent<PlayerG2>();
            if (s != null) s.isLocked = isLocked;
        }
    }

    void ShowFloatingMessage(string content, Color color)
    {
        if (floatingTextPrefab != null)
        {
            GameObject go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingTextG2 ft = go.GetComponent<FloatingTextG2>();
            if (ft != null) ft.SetText(content, color);
        }
    }
}