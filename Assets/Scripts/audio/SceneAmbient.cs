using UnityEngine;
using System.Collections;

public class SceneAmbient : MonoBehaviour
{
    public enum AmbientType { RandomInterval, ContinuousLoop }

    [Header("Settings")]
    public AmbientType type = AmbientType.RandomInterval;

    [Header("Day Sounds (6:00 - 18:00)")]
    public AudioClip[] dayClips; // Tiếng chim hót, gió nhẹ...

    [Header("Night Sounds (18:00 - 6:00)")]
    public AudioClip[] nightClips; // Tiếng dế, tiếng cú, gió lạnh...

    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("For Random Interval")]
    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.spatialBlend = 0; // 2D Sound

        if (type == AmbientType.ContinuousLoop)
        {
            PlayLoop(dayClips);
        }
        else
        {
            StartCoroutine(PlayRandomly());
        }
    }

    IEnumerator PlayRandomly()
    {
        while (true)
        {
            // 1. Chờ
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Kiểm tra giờ hiện tại để chọn danh sách âm thanh
            AudioClip[] currentPool = dayClips; // Mặc định là ngày

            if (TimeManager.instance != null)
            {
                int hour = TimeManager.instance.GetCurrentHour();
                // Nếu là đêm (từ 18h tối đến 6h sáng)
                if (hour >= 18 || hour < 6)
                {
                    currentPool = nightClips;
                }
            }

            // 3. Chọn và phát ngẫu nhiên từ danh sách phù hợp
            if (currentPool != null && currentPool.Length > 0)
            {
                int randomIndex = Random.Range(0, currentPool.Length);
                // Chỉ phát nếu có clip (để tránh lỗi nếu một trong 2 list bị trống)
                if (currentPool[randomIndex] != null)
                {
                    audioSource.PlayOneShot(currentPool[randomIndex]);
                }
            }
        }
    }

    void PlayLoop(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            audioSource.clip = clips[0];
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}