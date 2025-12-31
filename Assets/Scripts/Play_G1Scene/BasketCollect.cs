//Quản lý việc thu thập vật phẩm và tính điểm cho một trò chơi
using UnityEngine;
using TMPro;

public class BasketCollect : MonoBehaviour
{
    public int score = 0;                               // điểm số hiện tại
    public TextMeshProUGUI scoreText;                   // tham chiếu đến thành phần TextMeshProUGUI để hiển thị điểm số 

    void Start()
    {
        scoreText.text = "Score: 0";                    // khởi tạo điểm số hiển thị ban đầu
    }
    
    private void OnTriggerEnter2D(Collider2D other)     // khi vật phẩm chạm vào giỏ
    {
        if (other.CompareTag("Item"))                   // kiểm tra nếu vật phẩm có tag "Item"
        {
            score++;    // tăng điểm số lên 1
            scoreText.text = "Score: " + score;         // cập nhật điểm số hiển thị
            Destroy(other.gameObject);                  // hủy vật phẩm sau khi thu thập
        }
    }
}
