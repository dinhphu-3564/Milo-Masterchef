using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerG2 : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public Rigidbody2D rb;
    public float speed = 5f;
    public Animator animator;

    [Header("QUẢN LÝ UI (Kéo các Panel vào đây)")]
    public GameObject fishingPanel;
    public GameObject shopPanel;

    // Tham chiếu Inventory lấy từ Singleton, không cần kéo

    private Vector2 movement;
    private SoilSlot currentSlot;

    // Biến chặn di chuyển
    public bool isLocked = false;

    void Start()
    {
        isLocked = false; // Luôn mở khóa khi bắt đầu
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Mở khóa biến di chuyển
        isLocked = false;

        // 2. [QUAN TRỌNG NHẤT] KHÔI PHỤC THỜI GIAN
        // Nếu thiếu dòng này, FixedUpdate sẽ không chạy -> Nhân vật không đi được
        Time.timeScale = 1f;

        // 3. Reset tốc độ vật lý
        if (rb != null) rb.velocity = Vector2.zero;

        // 4. Reset hoạt ảnh
        if (animator != null) animator.SetFloat("Speed", 0);
    }

    void Update()
    {
        // --- 1. XỬ LÝ NÚT ESC (THOÁT TOÀN BỘ) ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
            return;
        }

        if (isLocked)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        // --- 2. XỬ LÝ DI CHUYỂN ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        // --- 3. XỬ LÝ TƯƠNG TÁC (NÔNG TRẠI) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSlot != null)
            {
                currentSlot.Interact();
            }
        }
    }

    // Hàm xử lý logic khi bấm ESC
    void HandleEscapeKey()
    {
        // Ưu tiên 1: Đang câu cá -> Tắt ngay
        if (fishingPanel != null && fishingPanel.activeSelf)
        {
            fishingPanel.SetActive(false);
            isLocked = false;
            Debug.Log("Đã hủy câu cá bằng ESC");
            return;
        }

        // Ưu tiên 2: Đang mở Shop -> Tắt ngay
        if (shopPanel != null && shopPanel.activeSelf)
        {
            shopPanel.SetActive(false);
            isLocked = false;
            return;
        }

        // Ưu tiên 3: Đang mở Túi đồ -> Tắt ngay
        if (InventoryManager.instance != null && InventoryManager.instance.inventoryPanel.activeSelf)
        {
            InventoryManager.instance.ToggleInventory();
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!isLocked)
        {
            // Hàm này sẽ KHÔNG CHẠY nếu Time.timeScale = 0
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Soil"))
        {
            SoilSlot newSlot = collision.GetComponent<SoilSlot>();
            if (newSlot != null)
            {
                if (currentSlot != null && currentSlot != newSlot)
                {
                    currentSlot.SetHighlight(false);
                }
                currentSlot = newSlot;
                currentSlot.SetHighlight(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Soil"))
        {
            SoilSlot exitingSlot = collision.GetComponent<SoilSlot>();
            if (exitingSlot == currentSlot)
            {
                currentSlot.SetHighlight(false);
                currentSlot = null;
            }
        }
    }
}