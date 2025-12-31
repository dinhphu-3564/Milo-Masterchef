//Xóa đối tượng khi ra khỏi màn hình
using UnityEngine;

public class DestroyWhenOutOfView : MonoBehaviour
{
    public float offset = 1f; // cho rơi ra khỏi màn 1 chút mới hủy

    void Update()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        // viewPos.y < 0
        if (viewPos.y < -offset)
        {
            Destroy(gameObject);
        }
    }
}
