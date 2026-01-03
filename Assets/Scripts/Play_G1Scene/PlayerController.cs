using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;                 // Tốc độ di chuyển

    [Header("Player Size")]
    public float playerHalfWidth = 0.5f;         // Nửa chiều rộng sprite (để giới hạn màn hình)

    private Animator anim;                       // Animator của player
    private Camera mainCam;                      // Camera chính

    // ===================== START =====================
    void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;
    }

    // ===================== UPDATE ====================
    void Update()
    {
        // ===== INPUT =====
        float moveInput = Input.GetAxisRaw("Horizontal");

        // ===== MOVE =====
        Vector3 position = transform.position;
        position += Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        // ===== CAMERA LIMIT =====
        float camHalfWidth = mainCam.orthographicSize * mainCam.aspect;

        float leftLimit  = mainCam.transform.position.x - camHalfWidth + playerHalfWidth;
        float rightLimit = mainCam.transform.position.x + camHalfWidth - playerHalfWidth;

        position.x = Mathf.Clamp(position.x, leftLimit, rightLimit);
        transform.position = position;

        // ===== FLIP SPRITE =====
        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (moveInput > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        // ===== ANIMATION =====
        if (anim != null)
            anim.SetBool("Walk", moveInput != 0);
    }
}
