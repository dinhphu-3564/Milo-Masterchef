using UnityEngine;

public static class ProficiencyData
{
    private const string PROFICIENCY_KEY = "TOTAL_PROFICIENCY";
    private const int MAX_PROFICIENCY = 1000;

    // Lấy tổng điểm tinh thông
    public static int GetProficiency()
    {
        return PlayerPrefs.GetInt(PROFICIENCY_KEY, 0);
    }

    // Ghi tổng điểm tinh thông
    public static void SetProficiency(int amount)
    {
        // Giới hạn tối đa 1000 điểm
        int clampedAmount = Mathf.Clamp(amount, 0, MAX_PROFICIENCY);
        PlayerPrefs.SetInt(PROFICIENCY_KEY, clampedAmount);
        PlayerPrefs.Save();
    }

    // Cộng thêm điểm tinh thông (từ màn 3)
    public static void AddProficiency(int amount)
    {
        int currentProficiency = GetProficiency();
        int newProficiency = currentProficiency + amount;
        
        // Nếu vượt quá 1000, chỉ lấy đến 1000
        if (newProficiency >= MAX_PROFICIENCY)
        {
            SetProficiency(MAX_PROFICIENCY);
        }
        else
        {
            SetProficiency(newProficiency);
        }
    }

    // Kiểm tra xem đã đạt đủ 1000 điểm chưa (Win Game)
    public static bool IsGameWon()
    {
        return GetProficiency() >= MAX_PROFICIENCY;
    }

    // Reset về 0 (sau khi win game)
    public static void ResetProficiency()
    {
        PlayerPrefs.SetInt(PROFICIENCY_KEY, 0);
        PlayerPrefs.Save();
    }

    // Lấy phần trăm (0-1)
    public static float GetPercentage()
    {
        return (float)GetProficiency() / MAX_PROFICIENCY;
    }
}
