
using System.Collections;
using TMPro; 
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Header("Player Info UI")]
    public Image avatarImage;
    public TextMeshProUGUI playerNameText;
    [Header("Clock UI")]
    public TextMeshProUGUI clockText;
    //tham chiếu đến toNàn bộ canvas
    public GameObject inGameCanvas;

    [Header("Day/Night Cycle (Global Volume)")]
    //public Volume globalVolume; 
    public Gradient lightColorGradient; // Dùng để chỉnh màu (Color Filter)
    public AnimationCurve lightIntensityCurve; // Dùng để chỉnh độ sáng (Post Exposure)
    public float lightTransitionSpeed = 3f; // tốc độ chuyển ánh sáng mượt


    //Biến lưu trữ tham chiếu đến ColorAdjustments Override
    private Light2D globalLight;
    private bool nightLightsOn = false;//đèn ban đêm

    private float targetLightIntensity;
    private Color targetLightColor;

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
        if (scene.name == "LoginScene" || scene.name == "CustomizeCharacter" || scene.name == "LoadingScene")
        {
            // Nếu là các scene trên, ẩn 
            if (inGameCanvas != null) inGameCanvas.SetActive(false);
        }
        else
        {
            // hiện  giao diện game lên
            if (inGameCanvas != null) inGameCanvas.SetActive(true);
            //Tìm Global Light trong scene mới
            FindGlobalLight();

            if (TimeManager.instance != null)
            {
                int currentHour = TimeManager.instance.GetCurrentHour();
                // Thiết lập trạng thái đèn đêm
                nightLightsOn = (currentHour >= 18 || currentHour < 6);
                SetNightLights(nightLightsOn);

                // Cập nhật ánh sáng lần đầu
                UpdateLighting(currentHour, TimeManager.instance.GetCurrentMinute());
            }
        }
    }
    // Tìm và lưu tham chiếu đến Global Light trong scene hiện tại
    void FindGlobalLight()
    {
        globalLight = FindObjectOfType<Light2D>(true); // 'true' để tìm cả object bị disable
        while (globalLight != null && globalLight.lightType != Light2D.LightType.Global) 
        { 
           // Nếu tìm thấy đèn khác không phải Global, tìm tiếp
           Light2D[] allLights = FindObjectsOfType<Light2D>(true); 
           globalLight = null; // Reset
           foreach (var light in allLights) 
           { 
                if (light.lightType == Light2D.LightType.Global) 
                { 
                    globalLight = light; 
                    break; 
                } 
            }
        } 
        if (globalLight == null) 
        { 
            Debug.LogWarning("Không tìm thấy Global Light 2D trong scene này!"); 
        } 
        else 
        { 
            Debug.Log("Đã tìm thấy Global Light 2D: " + globalLight.gameObject.name); 
        }
    }
    // Hàm bật/tắt tất cả đèn có tag "NightLight".
        void SetNightLights(bool isOn)
    {
        GameObject[] nightLights = GameObject.FindGameObjectsWithTag("NightLight");
        foreach (GameObject lightObj in nightLights)
        {
            var lightComp = lightObj.GetComponent<Light2D>();
            if (lightComp != null)
            {
                StartCoroutine(FadeLight(lightComp, isOn ? 1f : 0f));
            }
        }
        if (nightLights.Length > 0)
            Debug.Log($"Đã {(isOn ? "bật" : "tắt")} {nightLights.Length} đèn đêm.");
    }
    //fade để chuyển ánh sáng cho mượt
    private System.Collections.IEnumerator FadeLight(Light2D light, float target)
    {
        float start = light.intensity;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            light.intensity = Mathf.Lerp(start, target, t);
            yield return null;
        }
        light.enabled = target > 0.1f;
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

        UpdateLighting(hour, minute);

        // Bật/tắt đèn đêm (Luôn chạy)
        bool shouldLightsBeOn = (hour >= 18 || hour < 6);
        if (shouldLightsBeOn != nightLightsOn) // Chỉ gọi SetNightLights khi trạng thái thay đổi
        {
            SetNightLights(shouldLightsBeOn);
            nightLightsOn = shouldLightsBeOn;
        }
    }
    //Hàm riêng để cập nhật Global Light (được gọi từ UpdateClock và OnSceneLoaded).
    void UpdateLighting(int hour, int minute)
    {
        if (globalLight == null && lightColorGradient == null && lightIntensityCurve == null)
            return; // Không có gì để cập nhật

        float totalMinutes = (hour * 60) + minute;
        float timePercentage = totalMinutes / 1440f;

        // Mục tiêu ánh sáng
        targetLightColor = lightColorGradient.Evaluate(timePercentage);
        targetLightIntensity = lightIntensityCurve.Evaluate(timePercentage);

        // Lerp để ánh sáng chuyển mượt
        globalLight.color = Color.Lerp(globalLight.color, targetLightColor, Time.deltaTime * lightTransitionSpeed);
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetLightIntensity, Time.deltaTime * lightTransitionSpeed);


    }
}