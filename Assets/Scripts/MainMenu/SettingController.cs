//Quản lý logic cho màn hình Cài đặt (Settings), cụ thể là chức năng bật/tắt âm thanh và hiển thị trạng thái đó lên giao diện người dùng (UI)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingController : MonoBehaviour
{
    public Button soundButton;
    public Text soundText;

    void Start()
    {
        UpdateSoundText();
        soundButton.onClick.AddListener(ToggleSound);
    }

    void ToggleSound()
    {
        AudioManager.Instance.ToggleMute();
        UpdateSoundText();
    }

    void UpdateSoundText()
    {
        if (AudioManager.Instance.isMuted)
            soundText.text = "SOUND: OFF";
        else
            soundText.text = "SOUND: ON";
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
