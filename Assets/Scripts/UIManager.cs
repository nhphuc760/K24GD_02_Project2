using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public float lightTransitionSpeed = 1f; // tốc độ chuyển ánh sáng mượt


    //Biến lưu trữ tham chiếu đến ColorAdjustments Override
    private Light2D globalLight;
  

    private float targetGlobalIntensity;
    private Color targetGlobalColor;

    [Header("Night Lights")]
    [Tooltip("Cường độ sáng tối đa cho đèn đêm")]
    public float maxNightLightIntensity = 0.8f;
    [Tooltip("Giờ bắt đầu bật đèn (ví dụ: 18 = 6 giờ tối)")]
    public int turnOnHour = 18; // Giờ bắt đầu sáng dần
    [Tooltip("Giờ đèn sáng tối đa (ví dụ: 19 = 7 giờ tối)")]
    public int fullyOnHour = 19; // Giờ sáng hẳn
    [Tooltip("Giờ bắt đầu tắt đèn (ví dụ: 5 = 5 giờ sáng)")]
    public int turnOffHour = 5; // Giờ bắt đầu tối dần
    [Tooltip("Giờ đèn tắt hẳn (ví dụ: 6 = 6 giờ sáng)")]
    public int fullyOffHour = 6; // Giờ tắt hẳn

    private List<Light2D> nightLightsInScene = new List<Light2D>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        GameManager.Ins.onLoadDataCompleted += LoadDataPlayerCompleted;
    }

    private void LoadDataPlayerCompleted(PlayerData playerData, CharacterDataSO sO)
    {
        if (playerData == null)
        {
            SetupPlayerInfo("NoName", sO.portrait);
        }
        else
        {
            SetupPlayerInfo(playerData._name, sO.portrait);
        }
    }

    // Đăng ký lắng nghe sự kiện khi scene thay đổi
    private void Start()
    {

        SceneManager.sceneLoaded += OnSceneLoaded;
        inGameCanvas.SetActive(false);
    }

    // Hủy đăng ký khi đối tượng bị hủy
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Ins.onLoadDataCompleted -= LoadDataPlayerCompleted;
    }
    // Hàm này sẽ được gọi mỗi khi một scene MỚI được tải xong
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.path.StartsWith("Assets/Scenes/ScenePlay"))
        {
            // Nếu là các scene trên, ẩn 
            Debug.Log(scene.path + "not startWith");
            if (inGameCanvas != null) inGameCanvas.SetActive(false);
        }
        else
        {
            // hiện  giao diện game lên
            Debug.Log(scene.path + "start with");
            if (inGameCanvas != null) inGameCanvas.SetActive(true);
            //Tìm Global Light trong scene mới
            FindGlobalLight();
            //tìm là lưu đèn đêm trong scene mới
            FindAndStoreNightLights();
            if (TimeManager.instance != null)
            {
                int currentHour = TimeManager.instance.GetCurrentHour();
                int currentMinute = TimeManager.instance.GetCurrentMinute();

                // Cập nhật ánh sáng lần đầu
                SetInitialLighting(currentHour, currentMinute);
                SetInitialNightLightState(currentHour, currentMinute);
            }
        }
    }
    // Tìm và lưu tham chiếu đến Global Light trong scene hiện tại
    void FindGlobalLight()
    {
        globalLight = FindFirstObjectByType<Light2D>(findObjectsInactive: FindObjectsInactive.Include);        
        // Check if the first one found is already Global
        if (!(globalLight != null && globalLight.lightType == Light2D.LightType.Global))
        {
            Light2D[] allLights = FindObjectsByType<Light2D>(findObjectsInactive: FindObjectsInactive.Include, FindObjectsSortMode.None);
            globalLight = null; // Reset
            foreach (var light in allLights)
            {
                if (light.lightType == Light2D.LightType.Global)
                {
                    globalLight = light;
                    break; // Found it, exit foreach loop
                }
            }
        }       
    }
    //tìm đèn đêm và lưu vào ds
    void FindAndStoreNightLights()
    {
        nightLightsInScene.Clear();
        // Tìm TẤT CẢ các object có tag này
        GameObject[] nightLightObjects = GameObject.FindGameObjectsWithTag("NightLight");
        // Duyệt qua danh sách tìm được
        foreach (GameObject lightObj in nightLightObjects)
        {                       
            var lightComp = lightObj.GetComponent<Light2D>();
            if (lightComp != null)
            {
                nightLightsInScene.Add(lightComp);              
            }           
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
        // Tính toán giá trị MỤC TIÊU cho ánh sáng môi trường
        CalculateTargetLighting(hour, minute);
        // Cập nhật cường độ MỤC TIÊU cho đèn đêm
        UpdateNightLightState(hour, minute);
    }
    void Update()
    {
        // Chỉ Lerp nếu Global Light tồn tại
        if (globalLight != null)
        {
            // Lerp ánh sáng môi trường
            globalLight.color = Color.Lerp(globalLight.color, targetGlobalColor, Time.deltaTime * lightTransitionSpeed);
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetGlobalIntensity, Time.deltaTime * lightTransitionSpeed);

            // Lerp đèn đêm (Logic tính toán target đã ở UpdateNightLightState)
            // Việc áp dụng cường độ đã được xử lý trong UpdateNightLightState, không cần Lerp riêng ở đây nữa
            // vì UpdateClock được gọi thường xuyên.
        }
    }
    //Tính toán giá trị mục tiêu cho Global Light.
    void CalculateTargetLighting(int hour, int minute)
    {
        if (lightColorGradient != null && lightIntensityCurve != null)
        {
            float totalMinutes = (hour * 60) + minute;
            float timePercentage = totalMinutes / 1440f;
            targetGlobalColor = lightColorGradient.Evaluate(timePercentage);
            targetGlobalIntensity = lightIntensityCurve.Evaluate(timePercentage);
        }
    }
    //thiết lập ánh sáng ban đầu ngay lập tức khi vào scene.
    void SetInitialLighting(int hour, int minute)
    {
        CalculateTargetLighting(hour, minute); // Tính giá trị
        if (globalLight != null)
        {
            globalLight.color = targetGlobalColor; // Gán trực tiếp
            globalLight.intensity = targetGlobalIntensity; // Gán trực tiếp
        }
    }
    //update Night light theo thời gian
    void UpdateNightLightState(int hour, int minute)
    {
        float targetIntensity = 0f;
        float lerpFactor = (float)minute / 60f; // Tỷ lệ phút trong giờ (0.0 đến 1.0)

        if (hour < turnOffHour || hour >= fullyOnHour) // Hoàn toàn tối -> Sáng max
        {
            targetIntensity = maxNightLightIntensity;
        }
        else if (hour >= fullyOffHour && hour < turnOnHour) // Hoàn toàn sáng -> Tắt hẳn
        {
            targetIntensity = 0f;
        }
        else if (hour == turnOffHour) // Giờ bắt đầu tắt (ví dụ: 5h sáng)
        {
            // Lerp từ sáng max xuống 0
            targetIntensity = Mathf.Lerp(maxNightLightIntensity, 0f, lerpFactor);
        }
        else if (hour == turnOnHour) // Giờ bắt đầu bật (ví dụ: 18h tối)
        {
            // Lerp từ 0 lên sáng max
            targetIntensity = Mathf.Lerp(0f, maxNightLightIntensity, lerpFactor);
        }

        // Áp dụng cường độ cho tất cả đèn đêm
        foreach (Light2D light in nightLightsInScene)
        {
            if (light != null)
            {
                light.intensity = targetIntensity;
                // Bật/tắt component để tiết kiệm hiệu năng (tùy chọn)
                light.enabled = (targetIntensity > 0.01f);
            }
        }
    }
    void SetInitialNightLightState(int hour, int minute)
    {
        // Gọi hàm tính toán và áp dụng ngay lập tức
        UpdateNightLightState(hour, minute);
    }
}
