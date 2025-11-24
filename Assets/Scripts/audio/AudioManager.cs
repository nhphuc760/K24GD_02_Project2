using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mainMixer;
    [SerializeField] private AudioSource musicSource; //nhạc nền 
    [SerializeField] private AudioSource sfxSource; //âm thanh hiệu ứng

    [Range(0f,10f)] public float musicVolume = 1f;
    [Range(0f,10f)] public float sfxVolume = 1f;

    // Tên biến để lưu vào PlayerPrefs
    private const string MIXER_MUSIC = "MusicVol";
    private const string MIXER_SFX = "SFXVol";
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
        LoadVolume();
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

    public void PlayFX(AudioClip[] clip)
    {
        if (clip == null || clip.Length == 0) return;
        PlayFX(clip[Random.Range(0, clip.Length - 1)]);
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }

    //// Hàm chỉnh volume (dùng cho Settings Menu sau này)
    //public void SetMusicVolume(float volume)
    //{
    //    musicVolume = volume;
    //    musicSource.volume = musicVolume;
    //}

    // Hàm chỉnh volume Nhạc (được gọi từ Slider UI)
    public void SetMusicVolume(float sliderValue)
    {
        float mixerValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat(MIXER_MUSIC, mixerValue);
        // Lưu lại ngay lập tức
        PlayerPrefs.SetFloat(MIXER_MUSIC, sliderValue);
        PlayerPrefs.Save();
    }

    // Hàm chỉnh volume SFX
    public void SetSFXVolume(float sliderValue)
    {
        float mixerValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat(MIXER_SFX, mixerValue);
        PlayerPrefs.SetFloat(MIXER_SFX, sliderValue);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        // Lấy giá trị đã lưu (mặc định là 1 nếu chưa lưu)
        float musicVal = PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
        float sfxVal = PlayerPrefs.GetFloat(MIXER_SFX, 1f);

        // Áp dụng vào Mixer
        // Lưu ý: Phải set cả mixer lẫn biến nội bộ nếu cần
        SetMusicVolume(musicVal);
        SetSFXVolume(sfxVal);
    }
}
