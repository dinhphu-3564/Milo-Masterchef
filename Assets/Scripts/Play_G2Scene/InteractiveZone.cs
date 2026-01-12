using UnityEngine;

public class InteractiveZone : MonoBehaviour
{
    // Chỉ định loại khu vực
    public enum ZoneType { CowBarn, FishPond, Shop }

    [Header("Cài đặt khu vực")]
    public ZoneType thisZoneType;
    public GameObject uiPanelToOpen; // Dùng cho Shop/Kho (nếu có)
    public GameObject outlineObject; // Viền sáng khi lại gần

    [Header("Kết nối Script Logic")]
    // ĐÂY LÀ Ô ĐỂ BẠN KÉO SCRIPT VÀO
    public CowBarn cowBarnScript;

    [Header("Trạng thái")]
    public bool isPlayerNearby = false;

    private float lastInteractTime = -10f;
    private float cooldownDuration = 0.3f;
    private PlayerG2 currentPlayer;

    void Start()
    {
        if (outlineObject != null) outlineObject.SetActive(false);
        if (uiPanelToOpen != null) uiPanelToOpen.SetActive(false);
    }

    void Update()
    {
        // Chỉ xử lý khi có người chơi ở gần
        if (isPlayerNearby && currentPlayer != null)
        {
            // 1. CHẶN SPACE KHI BẬN:
            // Nếu Player đang bị khóa (đang mở túi đồ, đang câu cá...), thì KHÔNG nhận nút Space.
            if (currentPlayer.isLocked) return;

            // 2. XỬ LÝ NÚT SPACE:
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Chống spam nút (Cooldown)
                if (Time.time - lastInteractTime > cooldownDuration)
                {
                    HandleInteraction();
                    lastInteractTime = Time.time;
                }
            }
        }
    }

    void HandleInteraction()
    {
        // TRƯỜNG HỢP 1: LÀ CHUỒNG BÒ -> GỌI SCRIPT COWBARN
        if (thisZoneType == ZoneType.CowBarn)
        {
            if (cowBarnScript != null)
            {
                cowBarnScript.Interact(); // Gọi hàm Interact bên CowBarn
            }
            else
            {
                Debug.LogWarning("QUÊN KÉO SCRIPT: Hãy kéo script CowBarn vào ô Cow Barn Script trong InteractiveZone!");
            }
        }
        // TRƯỜNG HỢP 2: CÁC KHU VỰC KHÁC (Shop...) -> MỞ UI PANEL
        else
        {
            ToggleUIPanel();
        }
    }

    // Hàm bật/tắt UI (Dùng cho Shop)
    void ToggleUIPanel()
    {
        if (uiPanelToOpen == null) return;

        bool newState = !uiPanelToOpen.activeSelf;
        uiPanelToOpen.SetActive(newState);

        if (currentPlayer != null)
        {
            currentPlayer.isLocked = newState;
        }
    }

    // --- XỬ LÝ VA CHẠM ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (outlineObject != null) outlineObject.SetActive(true);
            currentPlayer = collision.GetComponent<PlayerG2>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (outlineObject != null) outlineObject.SetActive(false);

            // Tự đóng UI nếu đi xa
            if (uiPanelToOpen != null && uiPanelToOpen.activeSelf)
            {
                uiPanelToOpen.SetActive(false);
                if (currentPlayer != null) currentPlayer.isLocked = false;
            }

            currentPlayer = null;
        }
    }
}