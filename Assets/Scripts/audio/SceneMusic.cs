using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip backgroundMusic; // Kéo file nhạc của scene này vào đây
    [Range(0f, 1f)] public float fallbackVolume = 0.5f; // Volume dùng khi không có AudioManager
    void Start()
    {
        if (backgroundMusic == null) return;

        // th 1: Game chạy bình thường (Có AudioManager)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusic(backgroundMusic);
        }
        // th 2: Scene chạy độc lập (Chưa có AudioManager)
        else
        {
            // Tự tạo một AudioSource tạm thời ngay trên vật thể này
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = backgroundMusic;
            tempSource.loop = true;
            tempSource.volume = fallbackVolume;
            tempSource.Play();

            Debug.Log("Đang phát nhạc ở chế độ Fallback (không qua AudioManager)");
        }
    }
}
