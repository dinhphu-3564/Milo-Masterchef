using UnityEngine;

public class MenuBGMController : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // đồng bộ trạng thái mute
        if (AudioManager.Instance != null)
            audioSource.mute = AudioManager.Instance.isMuted;
    }
}
