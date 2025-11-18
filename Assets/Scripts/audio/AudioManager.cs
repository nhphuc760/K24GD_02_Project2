using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource; //nhạc nền 
    [SerializeField] private AudioSource sfxSource; //âm thanh hiệu ứng

    [Range(0f,10f)] public float musicVolume = 1f;
    [Range(0f,10f)] public float sfxVolume = 1f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }


    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
    

    public void PlayFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip,sfxVolume);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Hàm chỉnh volume (dùng cho Settings Menu sau này)
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
    }

}
