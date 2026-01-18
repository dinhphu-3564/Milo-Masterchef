using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Play_G3Scene;

public class CookingPan : MonoBehaviour
{
    [Header("Cấu hình")]
    public Transform[] ingredientSlots; // Kéo 4 cái Slot_1, Slot_2... vào đây
    public GameObject finishedDishPrefab; // Prefab món ăn đã nấu chín (làm sau)
    public Sprite cookingSprite;    // Kéo hình "Chảo đang nấu" vào đây
    public GameObject timerCanvas; // <--- MỚI: Kéo cái Canvas chứa Slider vào đây
    public Slider progressSlider;  // <--- MỚI: Kéo cái Slider vào đây

    [Header("Thành phẩm (Kéo Prefab món chín vào)")]
    public GameObject miYPrefab;
    public GameObject gaChienPrefab;
    public GameObject caChienPrefab;

    [Header("Audio")]
    public AudioSource cookingAudioSource; // AudioSource riêng cho âm thanh nấu ăn (loop)

    // Dữ liệu nội bộ
    private List<string> addedIngredients = new List<string>();
    private int currentSlotIndex = 0;
    private SpriteRenderer mySpriteRenderer; // <--- MỚI: Để điều khiển hình ảnh chảo
    private Sprite defaultSprite;            // <--- MỚI: Để lưu lại hình chảo rỗng ban đầu
    private Vector3 originalScale;
    private Vector3 originalPos;
    public float dishScale = 1.0f;

    private bool isCooking = false; // Đánh dấu trạng thái đang nấu
    private GameObject currentDish; // Biến để "nhớ" món ăn vừa nấu xong
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        if (mySpriteRenderer != null) defaultSprite = mySpriteRenderer.sprite;
        // Lưu lại vị trí/kích thước ban đầu của cái chảo rỗng
        originalScale = transform.localScale;
        originalPos = transform.localPosition;

        if (timerCanvas != null) timerCanvas.SetActive(false);
    }
    void Update()
    {
        // Logic mới: Kiểm tra dựa trên biến currentDish

        if (currentDish != null)
        {
            // TRƯỜNG HỢP 1: Món ăn VẪN CÒN (đang nằm trên bếp hoặc đang bị người chơi kéo đi)
            // -> Giữ cho cái chảo gốc TÀNG HÌNH để không bị lộ
            mySpriteRenderer.enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
        else if (!isCooking)
        {
            // TRƯỜNG HỢP 2: Món ăn ĐÃ BIẾN MẤT (Khách ăn xong -> Destroy -> null)
            // VÀ không phải đang nấu
            // -> Hiện lại cái chảo rỗng để nấu món tiếp theo

            if (mySpriteRenderer.enabled == false)
            {
                // Trả về hình chảo rỗng
                mySpriteRenderer.sprite = defaultSprite;
                mySpriteRenderer.enabled = true;
                mySpriteRenderer.color = Color.white;

                // Trả về kích thước/vị trí gốc
                transform.localScale = originalScale;
                transform.localPosition = originalPos;

                // Bật lại Collider để nhận nguyên liệu mới
                GetComponent<Collider2D>().enabled = true;
            }
        }
    }


    private void OnMouseDown()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.isGameOver) return;
        // Chỉ cho phép đổ rác khi:
        // 1. Không phải đang nấu (isCooking == false)
        // 2. Trong chảo đang có nguyên liệu (addedIngredients.Count > 0)
        // 3. Chảo đang hiện (enabled == true) - tránh click nhầm khi chảo đã ẩn sau khi nấu

        if (!isCooking && addedIngredients.Count > 0 && mySpriteRenderer.enabled)
        {
            Debug.Log(">>> ĐÃ ĐỔ BỎ NGUYÊN LIỆU SAI!");
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.AddScore(-10);
            }
            ClearPan(); // Gọi hàm dọn dẹp
        }
    }
    // -----------------------------------
   
    void ClearPan()
    {
        // Dừng âm thanh nấu ăn nếu đang phát
        if (cookingAudioSource != null && cookingAudioSource.isPlaying)
        {
            cookingAudioSource.Stop();
        }

        // Xóa hình ảnh nguyên liệu trên chảo
        foreach (Transform slot in ingredientSlots)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }
        // Reset dữ liệu
        currentSlotIndex = 0;
        addedIngredients.Clear();
        // Trả chảo về hình dáng cũ
        mySpriteRenderer.sprite = defaultSprite;
        transform.localScale = originalScale;
        transform.localPosition = originalPos;
        mySpriteRenderer.color = Color.white;
        Debug.Log("Chảo đã được dọn sạch!");
    }

    public void AddIngredient(string name, Sprite spriteHienThi)
    {
        if (LevelManager.Instance != null && LevelManager.Instance.isGameOver) return;
        // 1. Kiểm tra: Đang nấu HOẶC Chảo đầy thì dừng lại
        if (isCooking || currentSlotIndex >= ingredientSlots.Length)
        {
            Debug.Log("Không thể thêm: Chảo đầy hoặc đang nấu!");
            return;
        }

        // --- ĐOẠN MỚI: KIỂM TRA KHO HÀNG ---
        // Gọi sang InventoryManager để trừ nguyên liệu
        if (InventoryManager.Instance != null)
        {
            // Nếu dùng nguyên liệu thất bại (do hết hàng) -> Dừng ngay, không thêm vào chảo
            bool success = InventoryManager.Instance.TryUseIngredient(name);
            if (!success)
            {
                Debug.Log("Hết nguyên liệu " + name + " trong kho!");
                return;
            }
        }
        // ------------------------------------

        // 2. Thêm tên nguyên liệu vào danh sách để check công thức
        addedIngredients.Add(name);

        // 3. Hiển thị hình ảnh nguyên liệu lên chảo (Thức ăn sống)
        GameObject visual = new GameObject("Raw_" + name);
        visual.transform.SetParent(ingredientSlots[currentSlotIndex]);

        // Chỉnh vị trí Z = -0.1f để nó nổi lên trên đáy chảo
        visual.transform.localPosition = new Vector3(0, 0, -0.1f);

        // Mẹo: Nếu hình nguyên liệu bị to quá, hãy giảm số này xuống (VD: 0.45f)
        visual.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = spriteHienThi;

        sr.sortingLayerName = "Item"; // Đảm bảo bạn đã tạo Layer "Item" hoặc dùng "Default"
        sr.sortingOrder = 10 + currentSlotIndex;

        // Tăng chỉ số slot lên
        currentSlotIndex++;

        CheckRecipe();
    }

    void CheckRecipe()
    {
        // 1. MÓN MÌ Ý (Công thức: Mì + Cà chua + Thịt heo -> Tổng 3 món)
        // Thêm điều kiện: addedIngredients.Count == 3
        if (addedIngredients.Count == 3 &&
            addedIngredients.Contains("Mi") &&
            addedIngredients.Contains("CaChua") &&
            addedIngredients.Contains("ThitHeo"))
        {
            Debug.Log(">>> ĐỦ NGUYÊN LIỆU: Đang nấu Mì Ý!");
            StartCooking("MiY");
        }

        // 2. MÓN CÁ CHIÊN (Công thức: Cá + Rau -> Tổng 2 món)
        // Thêm điều kiện: addedIngredients.Count == 2
        else if (addedIngredients.Count == 2 &&
                 addedIngredients.Contains("Ca") &&
                 addedIngredients.Contains("Rau"))
        {
            Debug.Log(">>> ĐỦ NGUYÊN LIỆU: Đang nấu Cá chiên!");
            StartCooking("CaChien");
        }

        // 3. MÓN GÀ CHIÊN (Công thức: Gà + Rau -> Tổng 2 món)
        // Thêm điều kiện: addedIngredients.Count == 2
        else if (addedIngredients.Count == 2 &&
                 addedIngredients.Contains("Ga") &&
                 addedIngredients.Contains("Rau"))
        {
            Debug.Log(">>> ĐỦ NGUYÊN LIỆU: Đang nấu Gà chiên!");
            StartCooking("GaChien");
        }

        // Nếu code chạy đến đây mà không vào cái if nào ở trên
        // nghĩa là SAI CÔNG THỨC (hoặc chưa đủ nguyên liệu) -> Không làm gì cả.
    }

    void StartCooking(string dishName)
    {
        isCooking = true;
        // 1. Trả về kích thước gốc trước (QUAN TRỌNG)
        // Nếu không có dòng này, chảo sẽ bị méo nếu lần trước đã chỉnh sửa
        transform.localScale = originalScale;
        transform.localPosition = originalPos;
        mySpriteRenderer.color = Color.white;

        // Xóa nguyên liệu cũ
        foreach (Transform slot in ingredientSlots)
        {
            if (slot.childCount > 0) Destroy(slot.GetChild(0).gameObject);
        }

        // Reset dữ liệu
        currentSlotIndex = 0;
        addedIngredients.Clear();

        // 2. Đổi hình và Chỉnh lại kích thước phù hợp
        if (cookingSprite != null)
        {
            mySpriteRenderer.sprite = cookingSprite;

            // --- SỬA ĐOẠN NÀY ---
            // Thay vì gán số cứng (1.2f), hãy gán bằng đúng kích thước gốc (originalScale)
            // Nếu hình mới vẫn to/nhỏ hơn một chút, bạn có thể nhân thêm hệ số.
            // Ví dụ: originalScale * 1.0f (giữ nguyên) hoặc * 1.1f (to hơn xíu)
            transform.localPosition = originalPos + new Vector3(0, 0.25f, 0);
            transform.localScale = originalScale * 1.15f;
            // --------------------

        }
        float cookingTime = 4f; // Thời gian mặc định

        switch (dishName)
        {
            case "MiY":
                cookingTime = 3f;  // Mì Ý nấu 5 giây
                break;
            case "GaChien":
                cookingTime = 3.5f;  // Gà chiên nấu 8 giây
                break;
            case "CaChien":
                cookingTime = 3.5f;  // Cá chiên nấu 7 giây
                break;
            default:
                cookingTime = 2.5f;  // Món lạ nào đó
                break;
        }

        Debug.Log("Đang nấu món " + dishName + " trong " + cookingTime + "s...");

        // Phát âm thanh nấu ăn loop khi bắt đầu nấu
        if (cookingAudioSource != null && LevelManager.Instance != null && LevelManager.Instance.soundCooking != null)
        {
            cookingAudioSource.clip = LevelManager.Instance.soundCooking;
            cookingAudioSource.loop = true;
            cookingAudioSource.volume = LevelManager.Instance.sfxVolume;
            cookingAudioSource.Play();
        }

        StartCoroutine(CookingProcess(dishName, cookingTime)); // Test 5 giây cho nhanh
    }

    IEnumerator CookingProcess(string dishName, float duration)
    {
        // Hiện thanh slider
        if (timerCanvas != null) timerCanvas.SetActive(true);
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            // Cập nhật thanh trượt (từ 0 đến 1)
            if (progressSlider != null)
                progressSlider.value = timeElapsed / duration;

            yield return null; // Chờ đến frame tiếp theo
        }

        // --- KHI NẤU XONG ---
        Debug.Log(">>> NẤU XONG MÓN: " + dishName);

        // Dừng âm thanh nấu ăn khi nấu xong
        if (cookingAudioSource != null && cookingAudioSource.isPlaying)
        {
            cookingAudioSource.Stop();
        }

        // Ẩn thanh slider
        if (timerCanvas != null) timerCanvas.SetActive(false);

        // Trả chảo về hình dáng cũ (hoặc biến mất để hiện món ăn - tùy bạn chọn)
        // Ở đây tôi sẽ reset chảo và sinh ra món ăn chín
        FinishCooking(dishName);
    }

    void FinishCooking(string dishName)
    {
        isCooking = false;
        // Reset chảo về rỗng
        mySpriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        transform.localPosition = originalPos;
        transform.localScale = originalScale;
        mySpriteRenderer.color = Color.white;

        GameObject dishToSpawn = null;

        if (dishName == "MiY")
        {
            dishToSpawn = miYPrefab;
        }
        else if (dishName == "GaChien")
        {
            dishToSpawn = gaChienPrefab;
        }
        else if (dishName == "CaChien")
        {
            dishToSpawn = caChienPrefab;
        }

        // 3. Sinh ra món ăn ngay tại vị trí cái bếp
        if (dishToSpawn != null)
        {
            // Instantiate(Mẫu, Vị trí, Góc quay)
            GameObject cookedDish = Instantiate(dishToSpawn, transform.position, Quaternion.identity);

            // Đẩy món ăn nhích lên một chút cho đẹp (tùy chỉnh)
            cookedDish.transform.position += new Vector3(0, 0.2f, -1f);

            cookedDish.transform.localScale = new Vector3(dishScale, dishScale, 1f);

            currentDish = cookedDish;
            // (Tùy chọn) Gán script kéo thả cho món ăn này để bưng cho khách
            // cookedDish.AddComponent<DishDrag>(); 
        }

        Debug.Log(">>> ĐÃ DỌN MÓN: " + dishName);
    }
}