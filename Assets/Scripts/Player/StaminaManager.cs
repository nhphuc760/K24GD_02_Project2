using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager instance;
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDecreasePerMinute = 0.5f;

    public Image staminaFillImage;
    public Image iconImage;
    public Sprite[] iconSprites;

    private float timer;
    private float secondsPerGameMinute;

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        currentStamina = maxStamina;
        // Bắt đầu Coroutine để chờ TimeManager
        StartCoroutine(InitializeStaminaSystem());
    }

    // Coroutine chờ TimeManager
    IEnumerator InitializeStaminaSystem()
    {
        // Chờ đến khi TimeManager sẵn sàng
        while (TimeManager.instance == null)
        {
            yield return null;
        }

        // Lấy giá trị từ TimeManager
        secondsPerGameMinute = TimeManager.instance.secondsperDay / 1440f;

        UpdateUI();
    }

    // Dùng Update() thay vì Coroutine while(true)
    void Update()
    {
        // Nếu TimeManager chưa sẵn sàng, chưa làm gì cả
        if (TimeManager.instance == null || secondsPerGameMinute <= 0) return;

        //Logic Trừ Thể Lực Theo Thời Gian
        timer += Time.deltaTime;
        if (timer >= secondsPerGameMinute)
        {
            DecreaseStamina(staminaDecreasePerMinute);
            timer = 0;
        }
    }

    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            Die();
        }
        UpdateUI();
    }

    public void RestoreStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        Debug.Log($"Đã ăn! Hồi {amount} thể lực.");
        UpdateUI();
    }

    private void Die()
    {
        Debug.Log("Nhân vật đã kiệt sức và ngất xỉu!");
        currentStamina = maxStamina * 0.5f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        //tính tỉ lệ % thể lực
        float fillRatio = currentStamina / maxStamina;
        // Đảm bảo ratio không bao giờ vượt quá 0-1 để tránh lỗi
        fillRatio = Mathf.Clamp01(fillRatio);
        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = fillRatio;
        }
        if (iconImage != null && iconSprites.Length > 0)
        {
            // Thuật toán ánh xạ từ Tỉ lệ (0-1) sang Chỉ số mảng (0 -> n-1)
            //có 6 ảnh. Ratio 1.0 -> index 5. Ratio 0.0 -> index 0.
            int spriteCount = iconSprites.Length;

            // Công thức: nhân tỉ lệ với số lượng ảnh, rồi làm tròn xuống
            int index = Mathf.FloorToInt(fillRatio * spriteCount);

            // Xử lý trường hợp đặc biệt: khi ratio là 1.0, công thức trên ra bằng spriteCount (ví dụ ra 6)
            index = Mathf.Clamp(index, 0, spriteCount - 1);

            // Gán sprite tương ứng vào Image
            iconImage.sprite = iconSprites[index];
        }
    }
}