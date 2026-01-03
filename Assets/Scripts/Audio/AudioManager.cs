//Audio cho tất cả các scene và lưu trạng thái mute
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;       // Nguồn âm thanh nền
    public AudioSource sfxSource;       // Nguồn âm thanh hiệu ứng


    public bool isMuted = false;
    public AudioClip buttonClickClip;
    public AudioClip buySuccessClip;
    public AudioClip buyFailClip;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager khi chuyển scene

            isMuted = PlayerPrefs.GetInt("MUTED", 0) == 1;
        
            ApplyMute();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Phát âm thanh nút bấm
    public void PlayButtonClick()
    {
        if (isMuted) return;

        if (sfxSource != null && buttonClickClip != null)
            sfxSource.PlayOneShot(buttonClickClip);
    }

    // Phát âm thanh mua thành công
    public void PlayBuySuccess()
    {
        if (isMuted) return;

        if (sfxSource != null && buySuccessClip != null)
            sfxSource.PlayOneShot(buySuccessClip);
    }

    // Phát âm thanh mua thất bại
    public void PlayBuyFail()
    {
        if (isMuted) return;

        if (sfxSource != null && buyFailClip != null)
            sfxSource.PlayOneShot(buyFailClip);
    }

    // Chuyển đổi trạng thái mute
    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("MUTED", isMuted ? 1 : 0);
        ApplyMute();
    }

    void ApplyMute()
    {
        bgmSource.mute = isMuted;
        sfxSource.mute = isMuted;
    }
    
}
