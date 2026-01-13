using UnityEngine;
using TMPro;

namespace Play_G2Scene
{
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("--- DỮ LIỆU KHO (THÀNH PHẨM) ---")]
    public int veggieCount = 0;
    public int riceCount = 0;
    public int meatCount = 0;     // Thịt Bò
    public int chickenMeatCount = 0; // Thịt Gà (MỚI)
    public int fishMeatCount = 0; // Thịt Cá
    public int tomatoCount = 0; // Cà Chua (MỚI)

    [Header("--- GIAO DIỆN UI ---")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI veggieAmountText;
    public TextMeshProUGUI riceAmountText;
    public TextMeshProUGUI meatAmountText;     // Text số lượng bò
    public TextMeshProUGUI chickenAmountText;  // Text số lượng gà (MỚI - nhớ kéo vào)
    public TextMeshProUGUI fishAmountText;
    public TextMeshProUGUI tomatoAmountText; // Text số lượng cà chua (MỚI - nhớ kéo vào)

    private bool isInventoryOpen = false;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        LoadData();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryPanel != null) inventoryPanel.SetActive(isInventoryOpen);
        if (isInventoryOpen) UpdateUI();
    }

    // --- HÀM TỔNG QUÁT ---
    public void AddItem(string itemKey, int amount)
    {
        switch (itemKey)
        {
            case "ITEM_FISH_MEAT": AddFishMeat(amount); break;
            case "ITEM_MEAT": AddMeat(amount); break;      // Thịt Bò
            case "ITEM_CHICKEN_MEAT": AddChickenMeat(amount); break; // Thịt Gà (MỚI)
            case "ITEM_VEGGIE": AddVeggie(amount); break;
            case "ITEM_RICE": AddRice(amount); break;
            case "ITEM_TOMATO": AddTomato(amount); break; // Cà Chua (MỚI)
            default: Debug.LogWarning("Kho không nhận item này: " + itemKey); break;
        }
    }

    // --- CÁC HÀM CỤ THỂ ---
    public void AddMeat(int amount)
    {
        meatCount += amount;
        Debug.Log($"Kho: +{amount} Thịt Bò. Tổng: {meatCount}");
        UpdateUI();
        SaveData();
    }

    // Hàm thêm gà mới
    public void AddChickenMeat(int amount)
    {
        chickenMeatCount += amount;
        Debug.Log($"Kho: +{amount} Thịt Gà. Tổng: {chickenMeatCount}");
        UpdateUI();
        SaveData();
    }

    public void AddFishMeat(int amount)
    {
        fishMeatCount += amount;
        UpdateUI();
        SaveData();
    }

    public void AddVeggie(int amount)
    {
        veggieCount += amount;
        UpdateUI();
        SaveData();
    }

    public void AddRice(int amount)
    {
        riceCount += amount;
        UpdateUI();
        SaveData();
    }

    // Hàm thêm cà chua mới
    public void AddTomato(int amount)
    {
        tomatoCount += amount;
        Debug.Log($"Kho: +{amount} Cà Chua. Tổng: {tomatoCount}");
        UpdateUI();
        SaveData();
    }

    public void UpdateUI()
    {
        if (veggieAmountText != null) veggieAmountText.text = "x" + veggieCount;
        if (riceAmountText != null) riceAmountText.text = "x" + riceCount;
        if (meatAmountText != null) meatAmountText.text = "x" + meatCount;
        if (fishAmountText != null) fishAmountText.text = "x" + fishMeatCount;
        if (chickenAmountText != null) chickenAmountText.text = "x" + chickenMeatCount;
        if (tomatoAmountText != null) tomatoAmountText.text = "x" + tomatoCount; // Cập nhật UI Cà Chua
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("VeggieSaved", veggieCount);
        PlayerPrefs.SetInt("RiceSaved", riceCount);
        PlayerPrefs.SetInt("MeatSaved", meatCount);
        PlayerPrefs.SetInt("ChickenSaved", chickenMeatCount);
        PlayerPrefs.SetInt("FishMeatSaved", fishMeatCount);
        PlayerPrefs.SetInt("TomatoSaved", tomatoCount); // Lưu cà chua
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("VeggieSaved")) veggieCount = PlayerPrefs.GetInt("VeggieSaved");
        if (PlayerPrefs.HasKey("RiceSaved")) riceCount = PlayerPrefs.GetInt("RiceSaved");
        if (PlayerPrefs.HasKey("MeatSaved")) meatCount = PlayerPrefs.GetInt("MeatSaved");
        if (PlayerPrefs.HasKey("ChickenSaved")) chickenMeatCount = PlayerPrefs.GetInt("ChickenSaved");
        if (PlayerPrefs.HasKey("FishMeatSaved")) fishMeatCount = PlayerPrefs.GetInt("FishMeatSaved");
        if (PlayerPrefs.HasKey("TomatoSaved")) tomatoCount = PlayerPrefs.GetInt("TomatoSaved"); // Tải cà chua
    }
}
}