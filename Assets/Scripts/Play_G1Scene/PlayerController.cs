using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;                        // tốc độ di chuyển của player

    [Header("Player size")]
    public float playerHalfWidth = 0.5f;                // chỉnh cho khớp sprite

    private Animator anim;                              // tham chiếu đến Animator
    private Camera cam;                                 // tham chiếu đến Camera chính

    void Start()
    {
        anim = GetComponent<Animator>();                // lấy thành phần Animator
        cam = Camera.main;                              // lấy tham chiếu đến Camera chính
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");    // nhận input từ bàn phím (A/D hoặc mũi tên)

        Vector3 pos = transform.position;                               // vị trí hiện tại
        pos += Vector3.right * moveInput * moveSpeed * Time.deltaTime;  

        float camHalfWidth = cam.orthographicSize * cam.aspect;         // nửa chiều rộng của camera

        float leftLimit  = cam.transform.position.x - camHalfWidth + playerHalfWidth;   // giới hạn bên trái
        float rightLimit = cam.transform.position.x + camHalfWidth - playerHalfWidth;   // giới hạn bên phải

        // Khóa trong màn hình
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);          

        transform.position = pos;       // Cập nhật vị trí player

        // Đổi hướng
        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;                       // lấy scale hiện tại
            scale.x = Mathf.Abs(scale.x) * (moveInput > 0 ? 1 : -1);    // đổi hướng sprite
            transform.localScale = scale;                               // cập nhật scale mới
        }

        // Animation
        anim.SetBool("Walk", moveInput != 0);                           
    }
}
