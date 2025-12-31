//Audio cho tất cả các scene và lưu trạng thái mute
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public bool isMuted = false;

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
