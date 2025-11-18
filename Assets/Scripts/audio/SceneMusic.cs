using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip backgroundMusic; // Kéo file nhạc của scene này vào đây

    void Start()
    {
        // Khi scene này bắt đầu, yêu cầu AudioManager phát nhạc này
        if (AudioManager.instance != null && backgroundMusic != null)
        {
            AudioManager.instance.PlayMusic(backgroundMusic);
        }
    }
}
