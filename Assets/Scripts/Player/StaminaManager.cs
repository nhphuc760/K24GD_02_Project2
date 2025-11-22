using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager instance;

    public float maxStamina = 100f;
    public float currentStamina;

    public float staminaDecreasePerMinute = 0.5f;

    public Slider staminaSlider;
    public Image fillImage; // Để đổi màu thanh máu (Xanh -> Đỏ)

    private float timer;
    private float secondsPerGameMinute; // Thời gian thực cho 1 phút trong game 

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        currentStamina = maxStamina;
        UpdateUI();

        // Tính toán: 1 ngày có 1440 phút
        if (TimeManager.instance != null)
        {
            secondsPerGameMinute = TimeManager.instance.secondsperDay / 1440f;
        }
    }

    void Update()
    {
        // Nếu TimeManager chưa sẵn sàng hoặc game đang pause, không làm gì cả
        if (TimeManager.instance == null || Time.timeScale == 0) return;

        // Đếm ngược thời gian thực
        timer += Time.deltaTime;

        // Nếu đã trôi qua đủ thời gian cho 1 phút trong game
        if (timer >= secondsPerGameMinute)
        {
            DecreaseStamina(staminaDecreasePerMinute);
            timer = 0;
        }


        // Test Cheat
        if (Input.GetKeyDown(KeyCode.F1)) // Nhấn H để Hồi máu (giả lập ăn)
        {
            RestoreStamina(20);
        }
        if (Input.GetKeyDown(KeyCode.F2)) // Nhấn K để Tự sát (test chết)
        {
            DecreaseStamina(10);
        }
    }

    // Hàm trừ thể lực (dùng cho cả thời gian và hành động cuốc đất)
    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            Die(); // Chết!
        }

        UpdateUI();
    }

    // Hàm hồi thể lực (dùng khi ăn)
    public void RestoreStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;

        Debug.Log($"Đã ăn! Hồi {amount} thể lực.");
        UpdateUI();
    }

    // Hàm xử lý cái chết
    private void Die()
    {
        Debug.Log("Nhân vật đã kiệt sức và ngất xỉu!");

        //Reset lại thể lực (một chút)
        currentStamina = maxStamina * 0.5f; // Hồi 50%

        //Chuyển về scene Farm 
        // SceneLoader.instance.LoadScene("Farm"); 

        //Trừ tiền phạt hoặc mất đồ

        //Chuyển sang ngày hôm sau luôn
        // GameManager.instance.EndDay();

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / maxStamina;

            // Đổi màu: Xanh khi đầy, Đỏ khi sắp chết
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(Color.red, Color.green, staminaSlider.value);
            }
        }
    }

    // Hàm hỗ trợ để Inventory gọi khi dùng item(thêm sau)
 
}