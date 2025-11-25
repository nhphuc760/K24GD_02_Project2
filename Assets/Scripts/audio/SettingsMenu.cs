using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button backButton; // Nút quay lại

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVol")) musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        else musicSlider.value = 1f;

        if (PlayerPrefs.HasKey("SFXVol")) sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
        else sfxSlider.value = 1f;

        musicSlider.onValueChanged.AddListener(SetMusicVol);
        sfxSlider.onValueChanged.AddListener(SetSFXVol);

        if (backButton != null)
        {
            backButton.onClick.AddListener(Hide);
        }

        // Mặc định ẩn Settings đi khi bắt đầu
        Hide();
    }

    // Hàm hiện Menu (Gọi từ MainMenuManager)
    public void Show()
    {
      gameObject.SetActive(true); // Fallback nếu quên gán panel
    }

    // Hàm ẩn Menu (Gọi từ nút Back)
    public void Hide()
    {
       gameObject.SetActive(false);
    }

    public void SetMusicVol(float value)
    {
        if (AudioManager.instance != null) AudioManager.instance.SetMusicVolume(value);
        else PlayerPrefs.SetFloat("MusicVol", value);
        UpdateFallbackMusicVolume(value);
    }

    public void SetSFXVol(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXVolume(value);
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVol", value);
            PlayerPrefs.Save();

        }
    }

    // Cập nhật âm lượng cho các SceneMusic fallback
    void UpdateFallbackMusicVolume(float value)
    {
        // Tìm tất cả script SceneMusic trong scene hiện tại
        SceneMusic[] musics = FindObjectsByType<SceneMusic>(FindObjectsSortMode.None);
        foreach (var music in musics)
        {
            // Vì SceneMusic fallback tự tạo AudioSource trên chính nó
            AudioSource source = music.GetComponent<AudioSource>();
            if (source != null)
            {
                // Cập nhật volume ngay lập tức (nhân với volume gốc của bài nhạc)
                source.volume = music.fallbackVolume * value;
            }
        }
    }
}