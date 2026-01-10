using UnityEngine;

public class IngredientDrag : MonoBehaviour
{
    [Header("Tên nguyên liệu (Ví dụ: Mi, Thit, Ca)")]
    public string ingredientName;

    private GameObject currentDragObject; // Biến lưu bản sao đang được kéo
    private Vector3 screenPoint;
    private Vector3 offset;

    // Khi người chơi BẤM chuột vào nguyên liệu gốc
    void OnMouseDown()
    {
        // 1. Kiểm tra xem trong kho GameManager còn nguyên liệu không
        // (Tạm thời bỏ qua để test cơ chế kéo thả trước, sau này mở comment ra)
        /*
        if (ingredientName == "Mi" && GameManager.Instance.miCount <= 0) return;
        */

        // 2. Tạo ra một bản sao (Clone)
        currentDragObject = Instantiate(gameObject, transform.position, Quaternion.identity);

        // Tắt script này trên bản sao
        Destroy(currentDragObject.GetComponent<IngredientDrag>());

        // --- THÊM DÒNG NÀY ĐỂ SỬA LỖI ---
        // Xóa luôn Collider của bản sao để nó không che mất cái chảo bên dưới
        Destroy(currentDragObject.GetComponent<Collider2D>());
        // -------------------------------

        // Làm bản sao mờ đi một chút
        currentDragObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);

        // Đưa bản sao lên lớp trên cùng để không bị che khuất
        currentDragObject.GetComponent<SpriteRenderer>().sortingOrder = 100;

        // Tính toán vị trí chuột
        offset = currentDragObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
    }

    // Khi người chơi GIỮ và DI CHUYỂN chuột
    void OnMouseDrag()
    {
        if (currentDragObject != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            currentDragObject.transform.position = curPosition;
        }
    }

    // Khi người chơi THẢ chuột ra
    void OnMouseUp()
    {
        if (currentDragObject != null)
        {
            // Tắt collider để raycast xuyên qua (đã làm ở bước trước)
            // Destroy(currentDragObject.GetComponent<Collider2D>()); <--- Dòng này bạn đã làm rồi, giữ nguyên hoặc bỏ qua nếu đã xóa ở OnMouseDown

            Collider2D hitCollider = Physics2D.OverlapPoint(currentDragObject.transform.position);

            if (hitCollider != null && hitCollider.CompareTag("Pan"))
            {
                // Lấy sprite của nguyên liệu hiện tại để gửi sang chảo
                Sprite mySprite = GetComponent<SpriteRenderer>().sprite;

                // Gửi Tên + Hình sang chảo
                hitCollider.GetComponent<CookingPan>().AddIngredient(ingredientName, mySprite);

                Debug.Log("Đã bỏ " + ingredientName + " vào bếp!");
            }

            Destroy(currentDragObject);
        }
    }
}