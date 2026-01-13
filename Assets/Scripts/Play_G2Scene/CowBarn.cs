using UnityEngine;
using TMPro;
using System; // [BẮT BUỘC]
using Play_G2Scene;

public class CowBarn : MonoBehaviour
{
    // [QUAN TRỌNG] ID riêng biệt cho từng chuồng (VD: Barn_01)
    public string barnID = "Barn_01";

    public enum AnimalType { None, Cow, Chicken }

    [Header("--- TRẠNG THÁI HIỆN TẠI ---")]
    public AnimalType currentAnimal = AnimalType.None;

    [Header("--- GIAO DIỆN CHỌN VẬT NUÔI ---")]
    public GameObject selectionPanel;

    [Header("--- HIỂN THỊ ---")]
    public GameObject cowBabyObject;
    public GameObject cowAdultObject;
    public GameObject chickenBabyObject;
    public GameObject chickenAdultObject;

    [Header("--- UI THÔNG BÁO ---")]
    public TextMeshProUGUI timerText;
    public GameObject floatingTextPrefab;
    public Transform spawnPoint;

    [Header("--- CẤU HÌNH ---")]
    public float growTime = 10f;

    private string foodNameUI;
    private string productUI;
    private string meatInventoryKey;

    private bool isGrowing = false;
    private bool isReady = false;
    private float currentTimer = 0f;

    void Start()
    {
        ShowIdleState();
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (selectionPanel != null) selectionPanel.SetActive(false);

        // [MỚI] Tải dữ liệu
        LoadData();
    }

    void OnDisable()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
        SetPlayerLock(false);
        SaveData(); // [LƯU]
    }

    void Update()
    {
        if (isGrowing)
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

    public void Interact()
    {
        if (!isGrowing && !isReady)
        {
            if (selectionPanel != null && !selectionPanel.activeSelf)
            {
                selectionPanel.SetActive(true);
                SetPlayerLock(true);
            }
        }
        else if (isGrowing) ShowFloatingText($"Đang ăn {foodNameUI}...", Color.yellow);
        else if (isReady) HarvestAnimal();
    }

    public void SelectCow() { AttemptStartGrowing(AnimalType.Cow); }
    public void SelectChicken() { AttemptStartGrowing(AnimalType.Chicken); }

    public void ClosePanel()
    {
        if (selectionPanel != null) selectionPanel.SetActive(false);
        SetPlayerLock(false);
    }

    void AttemptStartGrowing(AnimalType type)
    {
        string checkFoodKey = (type == AnimalType.Cow) ? "ITEM_GRASS" : "ITEM_GRAIN";
        string checkFoodName = (type == AnimalType.Cow) ? "Cỏ" : "Hạt";

        int currentFood = ItemData.GetItem(checkFoodKey);

        if (currentFood > 0)
        {
            ItemData.AddItem(checkFoodKey, -1);
            currentAnimal = type;
            SetupAnimalData(type);

            isGrowing = true;
            isReady = false;

            // [REAL-TIME] Lưu thời gian hoàn thành
            DateTime finishTime = DateTime.Now.AddSeconds(growTime);
            PlayerPrefs.SetString(barnID + "_FinishTime", finishTime.ToBinary().ToString());

            currentTimer = growTime;
            ShowGrowingVisual(type);
            SaveData(); // [LƯU]

            if (timerText != null) { timerText.gameObject.SetActive(true); timerText.color = Color.white; }
            ClosePanel();
            ShowFloatingText($"-1 {checkFoodName}", Color.red);
        }
        else ShowFloatingText($"Thiếu {checkFoodName}!", Color.red);
    }

    void FinishGrowing()
    {
        isGrowing = false;
        isReady = true;
        if (timerText != null) timerText.gameObject.SetActive(false);

        HideAllAnimals();
        if (currentAnimal == AnimalType.Cow && cowAdultObject != null) cowAdultObject.SetActive(true);
        else if (currentAnimal == AnimalType.Chicken && chickenAdultObject != null) chickenAdultObject.SetActive(true);

        ShowFloatingText($"{productUI} đã lớn!", Color.green);
        SaveData(); // [LƯU]
    }

    void HarvestAnimal()
    {
        int randomValue = UnityEngine.Random.Range(1, 101);
        int meatReceived = (randomValue <= 20) ? 10 : (randomValue <= 50 ? 7 : 5);
        string qualityText = (randomValue <= 20) ? "Hảo hạng!" : (randomValue <= 50 ? "Ngon!" : "Thường");
        Color textColor = (randomValue <= 20) ? new Color(1f, 0.84f, 0f) : (randomValue <= 50 ? Color.cyan : Color.white);

        if (InventoryManager.instance != null) InventoryManager.instance.AddItem(meatInventoryKey, meatReceived);
        ShowFloatingText($"+{meatReceived} {productUI} ({qualityText})", textColor);

        isReady = false;
        isGrowing = false;
        currentAnimal = AnimalType.None;

        ShowIdleState();
        SaveData(); // [LƯU TRẠNG THÁI RỖNG]
    }

    // --- HỆ THỐNG SAVE / LOAD / REAL-TIME ---

    void SaveData()
    {
        PlayerPrefs.SetInt(barnID + "_Type", (int)currentAnimal);
        PlayerPrefs.SetInt(barnID + "_Growing", isGrowing ? 1 : 0);
        PlayerPrefs.SetInt(barnID + "_Ready", isReady ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadData()
    {
        if (PlayerPrefs.HasKey(barnID + "_Type"))
        {
            currentAnimal = (AnimalType)PlayerPrefs.GetInt(barnID + "_Type");
            isGrowing = PlayerPrefs.GetInt(barnID + "_Growing") == 1;
            isReady = PlayerPrefs.GetInt(barnID + "_Ready") == 1;

            SetupAnimalData(currentAnimal);

            if (isReady)
            {
                FinishGrowing();
            }
            else if (isGrowing)
            {
                ShowGrowingVisual(currentAnimal);

                string timeStr = PlayerPrefs.GetString(barnID + "_FinishTime", "");
                if (!string.IsNullOrEmpty(timeStr))
                {
                    long temp = Convert.ToInt64(timeStr);
                    DateTime finishTime = DateTime.FromBinary(temp);
                    double secondsLeft = (finishTime - DateTime.Now).TotalSeconds;

                    if (secondsLeft <= 0) FinishGrowing();
                    else
                    {
                        currentTimer = (float)secondsLeft;
                        if (timerText != null) timerText.gameObject.SetActive(true);
                    }
                }
            }
            else ShowIdleState();
        }
        else ShowIdleState();
    }

    // --- CÁC HÀM PHỤ (Giữ nguyên) ---
    void SetPlayerLock(bool isLocked)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerG2 playerScript = player.GetComponent<PlayerG2>();
            if (playerScript != null) playerScript.isLocked = isLocked;
        }
    }
    void ShowGrowingVisual(AnimalType type) { HideAllAnimals(); if (type == AnimalType.Cow) cowBabyObject.SetActive(true); else chickenBabyObject.SetActive(true); }
    void ShowIdleState() { HideAllAnimals(); cowBabyObject.SetActive(true); chickenBabyObject.SetActive(true); }
    void HideAllAnimals() { cowBabyObject.SetActive(false); cowAdultObject.SetActive(false); chickenBabyObject.SetActive(false); chickenAdultObject.SetActive(false); }
    void SetupAnimalData(AnimalType type) { if (type == AnimalType.Cow) { meatInventoryKey = "ITEM_MEAT"; foodNameUI = "Cỏ"; productUI = "Bò"; } else { meatInventoryKey = "ITEM_CHICKEN_MEAT"; foodNameUI = "Hạt"; productUI = "Gà"; } }
    void ShowFloatingText(string content, Color color) { if (floatingTextPrefab != null) { GameObject obj = Instantiate(floatingTextPrefab, spawnPoint.position, Quaternion.identity); obj.GetComponent<FloatingTextG2>().SetText(content, color); } }
}