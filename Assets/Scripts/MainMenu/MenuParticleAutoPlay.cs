using UnityEngine;

public class MenuParticleAutoPlay : MonoBehaviour
{
    private ParticleSystem[] particles;

    // Gọi khi đối tượng được kích hoạt
    void OnEnable()
    {
        // Tự động chơi lại các hệ thống hạt khi đối tượng được kích hoạt
        if (particles == null || particles.Length == 0)
        {
            particles = GetComponentsInChildren<ParticleSystem>(true);
        }

        if (particles == null || particles.Length == 0)
        {
            Debug.LogWarning(gameObject.name);
            return;
        }

        foreach (var ps in particles)
        {
            if (ps == null) continue;

            ps.Clear(true);
            ps.Play(true);
        }
    }
}
