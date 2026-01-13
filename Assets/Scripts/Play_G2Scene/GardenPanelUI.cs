using UnityEngine;

public class GardenPanelUI : MonoBehaviour
{
    // Hàm này gắn vào nút Trồng Rau
    public void OnClickPlantCabbage()
    {
        // Kiểm tra xem có ô đất nào đang được kích hoạt không
        if (SoilSlot.activeSlot != null)
        {
            SoilSlot.activeSlot.SelectCabbageToPlant();
        }
    }

    // Hàm này gắn vào nút Trồng Cà Chua
    public void OnClickPlantTomato()
    {
        if (SoilSlot.activeSlot != null)
        {
            SoilSlot.activeSlot.SelectTomatoToPlant();
        }
    }

    // Hàm này gắn vào nút Đóng (X)
    public void OnClickClose()
    {
        if (SoilSlot.activeSlot != null)
        {
            SoilSlot.activeSlot.ClosePanel();
        }
        else
        {
            // Phòng hờ lỗi, tự tắt chính mình
            gameObject.SetActive(false);
        }
    }
}