using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

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
    [SerializeField] int coinLostPerDead = 500;

    // Guard to prevent multiple concurrent Die() calls / scene loads
    private bool isDying = false;

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

    public async void DecreaseStamina(float amount)
    {
        // If we're already handling death, ignore further decreases.
        if (isDying) return;

        currentStamina -= amount;
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            // set guard before awaiting Die() to avoid re-entrancy
            isDying = true;
            // reset timer so we don't immediately re-trigger after revival
            timer = 0f;
            try
            {
                await Die();
            }
            finally
            {
                // clear guard after death handling completes
                isDying = false;
            }
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

    private async UniTask Die()
    {
        Debug.Log("Nhân vật đã kiệt sức và ngất xỉu!");
        GameEventManager.Ins.gameInput.Disable_InputAction();
        if (UIManager.instance != null)
            UIManager.instance.Hide();
        GameEventManager.Ins.animationEvent.Dead();
        await UniTask.Delay(2000);
        await GameManager.Ins.StartSceneTransition("InsideHouse", new Vector3(-5f, 2.3f, 0), "Trở về từ cõi chết", 0.5f);
        await UniTask.Yield();
        GameEventManager.Ins.gameInput.Enable_InputAction();
        GameEventManager.Ins.animationEvent.Idle();
        if (UIManager.instance != null) UIManager.instance.Show();
        currentStamina = maxStamina * 0.3f;
        GameManager.Ins.Coin -= coinLostPerDead;
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