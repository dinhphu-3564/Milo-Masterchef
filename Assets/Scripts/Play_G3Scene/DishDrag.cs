using UnityEngine;

public class DishDrag : MonoBehaviour
{
    private Vector3 startPosition; // Lưu vị trí ban đầu để nếu thả sai thì bay về
    private bool isDragging = false;
    private Vector3 offset;

    void OnMouseDown()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.isGameOver)
        {
            return;
        }
        isDragging = true;
        startPosition = transform.position; // Nhớ vị trí trên bếp

        // Tính khoảng cách giữa chuột và vật
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        // (Tùy chọn) Tắt Collider để không bị vướng khi kéo qua vật khác
        GetComponent<Collider2D>().enabled = false;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            transform.position = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // --- SỬA ĐỔI: Dùng OverlapPointAll để tìm tất cả vật va chạm ---
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        bool givenToCustomer = false; // Biến kiểm tra xem đã đưa được chưa

        foreach (Collider2D hit in hits)
        {
            // Kiểm tra từng vật xem có vật nào là Khách hàng không
            if (hit.CompareTag("Customer"))
            {
                Customer customerScript = hit.GetComponent<Customer>();

                // Thêm điều kiện: Khách phải đang hiện (Active) mới được nhận
                if (customerScript != null && customerScript.IsActive)
                {
                    // Logic kiểm tra tên món ăn
                    string myDishName = "";
                    if (gameObject.name.Contains("MiY")) myDishName = "MiY";
                    else if (gameObject.name.Contains("GaChien")) myDishName = "GaChien";
                    else if (gameObject.name.Contains("CaChien")) myDishName = "CaChien";

                    // Gửi món cho khách
                    customerScript.ReceiveDish(myDishName);

                    // Hủy món ăn
                    Destroy(gameObject);
                    givenToCustomer = true;
                    return; // Kết thúc hàm luôn
                }
            }
        }

        // Nếu chạy hết vòng lặp mà không tìm thấy khách (hoặc khách chưa active)
        if (!givenToCustomer)
        {
            // Bay về chỗ cũ
            transform.position = startPosition;

            // Bật lại Collider để lần sau còn kéo tiếp được
            GetComponent<Collider2D>().enabled = true;
        }
    }

}