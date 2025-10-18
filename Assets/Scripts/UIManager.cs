// UIManager.cs
using TMPro; // Bắt buộc phải có để dùng TextMeshPro
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player Info UI")]
    public Image avatarImage;
    public TextMeshProUGUI playerNameText;

    [Header("Clock UI")]
    public TextMeshProUGUI clockText;

    // Singleton Pattern để dễ dàng truy cập từ mọi nơi
    public static UIManager instance;

    //tham chiếu đến toNàn bộ canvas
    public GameObject inGameCanvas;

    [Header("Day/Night Cycle (Global Volume)")]
    public Volume globalVolume; 
    public Gradient colorGradient; // Dùng để chỉnh màu (Color Filter)
    public AnimationCurve exposureCurve; // Dùng để chỉnh độ sáng (Post Exposure)

    private ColorAdjustments colorAdjustments;
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

        // Tìm 1 Override trong Profile và lưu lại
        if (globalVolume != null)
        {
            // Chỉ cần tìm "ColorAdjustments"
            if (globalVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.Log("<color=green>[UIManager] Đã tìm thấy ColorAdjustments!</color>");
            }
            else
            {
                Debug.LogError("<color=red>[UIManager] KHÔNG TÌM THẤY ColorAdjustments trong Profile! Hãy kiểm tra lại Global Volume!</color>");
            }
        }
        else
        {
            Debug.LogError("<color=red>[UIManager] Global Volume CHƯA ĐƯỢC GÁN vào Inspector!</color>");
        }
    }
    // Đăng ký lắng nghe sự kiện khi scene thay đổi
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Hủy đăng ký khi đối tượng bị hủy
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Hàm này sẽ được gọi mỗi khi một scene MỚI được tải xong
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //tắt UI khi khởi động scene LOGIN, CUStome hoặc chọn nhân vật
        if (scene.name == "LoginScene" || scene.name == "CustomizeCharacter" || scene.name =="LoadingScene")
        {
            // Nếu là các scene trên, ẩn 
            inGameCanvas.SetActive(false);
        }
        else
        {
            // hiện  giao diện game lên
            inGameCanvas.SetActive(true);
        }
    }
    // Hàm để thiết lập thông tin ban đầu
    public void SetupPlayerInfo(string playerName, Sprite playerAvatar)
    {
        // Kiểm tra để chắc chắn các đối tượng UI đã được gán trong Inspector
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }

        if (avatarImage != null)
        {
            avatarImage.sprite = playerAvatar;
        }
    }

    // Hàm để cập nhật đồng hồ
    public void UpdateClock(int hour, int minute)
    {
        if (clockText != null)
        {
            // Định dạng lại giờ và phút để luôn có 2 chữ số (ví dụ: 08:05)
            clockText.text = $"{hour:00}:{minute:00}";
        }
        //CẬP NHẬT ÁNH SÁNG THẾ GIỚI
        if (colorAdjustments != null)
        {
            float totalMinutes = (hour * 60) + minute;
            float timePercentage = totalMinutes / 1440f;

            // Cập nhật Tông Màu (Color Filter)
            colorAdjustments.colorFilter.value = colorGradient.Evaluate(timePercentage);

            // Cập nhật Độ Sáng (Post Exposure)
            colorAdjustments.postExposure.value = exposureCurve.Evaluate(timePercentage);
        }
    }
}