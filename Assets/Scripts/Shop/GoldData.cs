using UnityEngine;

public static class GoldData
{
    private const string GOLD_KEY = "TOTAL_GOLD";

    // Lấy tổng vàng
    public static int GetGold()
    {
        return PlayerPrefs.GetInt(GOLD_KEY, 0);
    }

    // Ghi tổng vàng
    public static void SetGold(int amount)
    {
        PlayerPrefs.SetInt(GOLD_KEY, amount);
        PlayerPrefs.Save();
    }

    // Cộng thêm vàng
    public static void AddGold(int amount)
    {
        int currentGold = GetGold();
        SetGold(currentGold + amount);
    }
}
