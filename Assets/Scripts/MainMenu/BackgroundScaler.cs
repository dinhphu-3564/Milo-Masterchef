//làm background cho full screen
//tự động co giãn (scale) một tấm ảnh (Sprite) sao cho nó luôn vừa khít với màn hình camera
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        // Kích thước sprite gốc
        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        // Kích thước màn hình World Units
        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Camera.main.aspect;

        // Tính scale để sprite phủ toàn màn hình
        transform.localScale = new Vector3(worldWidth / spriteWidth, worldHeight / spriteHeight, 1);
    }
}
