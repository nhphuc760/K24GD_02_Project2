using UnityEngine;
using System.Collections;

public class SceneAmbient : MonoBehaviour
{
    public enum AmbientType { RandomInterval, ContinuousLoop }

    [Header("Settings")]
    public AmbientType type = AmbientType.RandomInterval;

    [Tooltip("Danh sách các file âm thanh (ví dụ: 3-4 tiếng chim khác nhau)")]
    public AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("For Random Interval (Birds)")]
    public float minWaitTime = 5f; // Chờ ít nhất 5 giây
    public float maxWaitTime = 15f; // Chờ nhiều nhất 15 giây

    private AudioSource audioSource;

    void Start()
    {
        // Tạo AudioSource ngay trên vật thể này
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.spatialBlend = 0; // 0 = 2D Sound (nghe đều 2 tai)

        if (type == AmbientType.ContinuousLoop)
        {
            PlayLoop();
        }
        else
        {
            StartCoroutine(PlayRandomly());
        }
    }

    // Dành cho Biển (Sóng vỗ rì rào liên tục)
    void PlayLoop()
    {
        if (clips.Length > 0)
        {
            audioSource.clip = clips[0]; // Lấy clip đầu tiên
            audioSource.loop = true;     // Bật chế độ lặp
            audioSource.Play();
        }
    }

    // Dành cho Nông Trại (Chim hót thi thoảng)
    IEnumerator PlayRandomly()
    {
        while (true) // Lặp vô tận
        {
            // 1. Chờ một khoảng thời gian ngẫu nhiên
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // 2. Chọn ngẫu nhiên một tiếng chim trong danh sách
            if (clips.Length > 0)
            {
                int randomIndex = Random.Range(0, clips.Length);
                audioSource.PlayOneShot(clips[randomIndex]);
            }
        }
    }
}