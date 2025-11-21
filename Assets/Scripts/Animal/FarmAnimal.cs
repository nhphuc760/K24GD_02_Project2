using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FarmAnimal : MonoBehaviour, IInteractable
{
    public AnimalDataSO animalData;// được gán khi spawn

    private DateTime timeProductReady; //thời điểm sản phẩm phọt raa
    private DateTime curTime; //thời điểm hiện tại
    private bool canHarvest = false; //có thể thu hoạch hay không

    //movement
    public float moveSpeed = 1f;
    public float TimetoChangeDirection = 3f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float moveTimer;
    [SerializeField] TextMeshProUGUI timeRemainingTXT;
    [SerializeField] UnityEngine.UI.Image fillProduction;
    [SerializeField] GameObject harvestIndicatorPrefab;
    [SerializeField] Transform indicatorAnchor;

    private GameObject currentIndicator;

    //thêm âm thanh cục tác
    public AudioClip[] idleSounds; // Kéo các file tiếng gà cục tác vào đây
    private AudioSource audioSource;
    public float chanceVolume = 0.3f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //logic di chuyển
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Debug.Log("Hết giờ! Đang gọi ChangeDirection...");
            ChangeDirection();
        }
    }

    void FixedUpdate()
    {
        //di chuyển con vật
        if(rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    void ChangeDirection()
    {
        Debug.Log("ChangeDirection đã được gọi!");
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        moveTimer = TimetoChangeDirection + UnityEngine.Random.Range(-1f,1f);

        if (UnityEngine.Random.value < chanceVolume)
        {
            Debug.Log("GÀ ĐANG GÁY! (Gọi PlayIdleSound)");
            PlayIdleSound();
        }
    }


    //khi mua con vật mới
    public async void Plant(AnimalDataSO data)
    {
        this.animalData = data;

        //lấy thgian server
        var task = await Save_Load_Firebase.GetSeverDateTime();
        if(task.HasValue) { 
            curTime = task.Value; 
        }
        else { 
            curTime = DateTime.Now; 
        }

        timeProductReady = curTime.AddSeconds(animalData.secondsToProduce);
        canHarvest = false;

        StartCoroutine(ProductionCycle());
        ChangeDirection();
        if(currentIndicator != null) currentIndicator.SetActive(false);
    }

    //tải con vật đã lưu
    public void LoadAnimalState(AnimalDataSO data, DateTime timeHarvest, DateTime curTime)
    {
        this.animalData = data;
        this.timeProductReady = timeHarvest;
        this.curTime = curTime;

        TimeSpan t = timeHarvest - curTime;
        if (t < TimeSpan.Zero) // Nếu đã chín
        {
            canHarvest = true;
            ShowHarvestIndicator(true);
        }
        else
        {
            canHarvest = false;
            StartCoroutine(ProductionCycle());
        }
        ChangeDirection();
    }

    //Đếm ngược thời giand đẻ
    IEnumerator ProductionCycle()
    {
        while(curTime < timeProductReady)
        {
            curTime = curTime.AddSeconds(1);
            TimeSpan cooldown = timeProductReady - curTime;

            //Cập nhật UI
            if (fillProduction != null)
            {
                fillProduction.fillAmount = 1 - (float)(cooldown.TotalSeconds / animalData.secondsToProduce);
            }
            if(timeRemainingTXT != null)
            {
                timeRemainingTXT.text = cooldown.ToString(@"hh\:mm\:ss");
            }
            yield return new WaitForSeconds(1f);
        }

        //Khi hoàn thành
        canHarvest = true;
        ShowHarvestIndicator(true);
    }

    //tương tác và thu hoạch
    public void Interact()
    {
        if(canHarvest)
        {
            GetProduct();
        }
    }

    public bool CanInteract()
    {
        return canHarvest;
    }

    private async void GetProduct()
    {
        Debug.Log("Đã thu hoạch " + animalData.productData._id);
        GameEventManager.Ins.inventoryEvent.AddItem(animalData.productData, 1);//thêm sản phẩm vào inventory

        //bắt đầu lại chu kỳ mới 
        canHarvest = false;
        ShowHarvestIndicator(false);

        //lấy thgian server
        var task = await Save_Load_Firebase.GetSeverDateTime();
        if (task.HasValue) { curTime = task.Value; }
        else { curTime = DateTime.Now; }

        timeProductReady = curTime.AddSeconds(animalData.secondsToProduce);
        StartCoroutine(ProductionCycle());
    }


    //các hàm hỗ trợ UI
    private void ShowHarvestIndicator(bool show)
    {
        if(currentIndicator == null && harvestIndicatorPrefab != null)
        {
            currentIndicator = Instantiate(harvestIndicatorPrefab, indicatorAnchor);
            currentIndicator.transform.position = indicatorAnchor.position;
        }
        if (currentIndicator != null)
        {
            currentIndicator.GetComponentInChildren<UnityEngine.UI.Image>().sprite = animalData.harvestIndicator;
            currentIndicator.SetActive(show);
        }
    }

    //// --- HÀM LƯU GAME ---
    //public AnimalSaveData GetSaveData()
    //{
    //    AnimalSaveData data = new AnimalSaveData();
    //    data.sceneName = SceneManager.GetActiveScene().name;
    //    data.worldPosition = new SerializableVector3(transform.position);
    //    data.animalDataID = animalData._id; // Lưu ID
    //    data.timeProductReady = this.timeProductReady;
    //    return data;
    //}

    void PlayIdleSound()
    {
        if (audioSource != null && idleSounds.Length > 0)
        {
            // Chọn ngẫu nhiên 1 âm thanh
            int randomIndex = UnityEngine.Random.Range(0, idleSounds.Length);

            // Thay đổi cao độ (Pitch) một chút cho tự nhiên (0.9 - 1.1)
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

            // Phát âm thanh 3D tại chỗ
            audioSource.PlayOneShot(idleSounds[randomIndex]);

            Debug.Log("Gà đã gáy");
        }
    }
}