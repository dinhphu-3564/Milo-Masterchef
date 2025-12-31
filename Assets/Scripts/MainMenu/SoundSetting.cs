//Trong khi script SettingController trước đó làm nhiệm vụ thay đổi cái chữ trên màn hình (UI), thì script SoundSetting này làm nhiệm vụ ghi nhớ lựa chọn của người chơi vào bộ nhớ máy (ngay cả khi tắt game mở lại, cài đặt vẫn còn đó).
using UnityEngine;

public class SoundSetting : MonoBehaviour
{
    const string SOUND_KEY = "SOUND_ON";

    public static bool IsSoundOn()
    {
        return PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
    }

    public static void SetSound(bool on)
    {
        PlayerPrefs.SetInt(SOUND_KEY, on ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ToggleSound()
    {
        SetSound(!IsSoundOn());
    }
}
