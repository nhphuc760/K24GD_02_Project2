using System;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class FarmAnimal : MonoBehaviour, IInteractable
{
    public AnimalDataSO animalData;// được gán khi spawn
    [SerializeField] protected SpriteRenderer visual;

    protected DateTime timeProductReady; //thời điểm sản phẩm phọt raa
    protected DateTime curTime; //thời điểm hiện tại
    protected bool canHarvest = false; //có thể thu hoạch hay không
    //movement
    public float moveSpeed = 1f;
    public float TimetoChangeDirection = 3f;
    protected Rigidbody2D rb;
    protected Vector2 moveDirection;
    protected float moveTimer; //thời gian thay đổi hướng đi của animal

    [SerializeField] protected GameObject harvestIndicatorPrefab;
    [SerializeField]  protected Transform indicatorAnchor;

    protected GameObject currentIndicator;

    //thêm âm thanh cục tác
    public AudioClip[] idleSounds; // Kéo các file tiếng gà cục tác vào đây
    protected AudioSource audioSource;
    public float chanceVolume = 0.3f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        transform.SetParent(AnimalManager.Ins.transform);

        if (AudioManager.instance != null)
        {
            // Lấy nhóm SFX từ AudioManager và gán vào AudioSource của con vật
            audioSource.outputAudioMixerGroup = AudioManager.instance.sfxGroup;
        }
    }

    void Update()
    {
        //logic di chuyển
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            ChangeDirection();
        }
    }

    void FixedUpdate()
    {
        //di chuyển con vật
        if(rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
            Flip();
        }
    }
    /// <summary>
    /// Thay đổi hướng con vật 
    /// </summary>

    protected virtual void ChangeDirection()
    {
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        moveTimer = TimetoChangeDirection + UnityEngine.Random.Range(-1f,1f);

        if (UnityEngine.Random.value < chanceVolume)
        {
            PlayIdleSound();
        }
    }

    /// <summary>
    /// hàm bắt đầu gọi khi tạo con vật
    /// </summary>
    /// <param name="data"></param>
    public async void StartLife(AnimalDataSO data)

    {
        this.animalData = data;

        //lấy thgian server
        var task = await Save_Load_Firebase.GetSeverDateTime();
        if(task.HasValue) { 
            curTime = task.Value; 
        }
        else { 
            curTime = DateTime.UtcNow; 
        }

        timeProductReady = curTime.AddSeconds(animalData.secondsToProduce);
        canHarvest = false;

        StartCoroutine(ProductionCycle());
        ChangeDirection();
        if(currentIndicator != null) currentIndicator.SetActive(false);
    }

    /// <summary>
    /// Hàm tải dữ liệu con vật
    /// </summary>
    /// <param name="data"></param>
    /// <param name="timeHarvest"></param>
    /// <param name="curTime"></param>
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

    /// <summary>
    /// Hàm đếm ngược thời gian đẻ
    /// </summary>
    /// <returns></returns>
    IEnumerator ProductionCycle()
    {
        while(curTime < timeProductReady)
        {
            curTime = curTime.AddSeconds(1);
            TimeSpan cooldown = timeProductReady - curTime;
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
            GameEventManager.Ins.animalEvent.OnGetProduct(animalData.productData);
        }
    }

    public bool CanInteract()
    {
        return canHarvest;
    }

    private async void GetProduct()
    {
        GameEventManager.Ins.inventoryEvent.AddItem(animalData.productData, 1);//thêm sản phẩm vào inventory

        //bắt đầu lại chu kỳ mới 
        canHarvest = false;
        ShowHarvestIndicator(false);

        //lấy thgian server
        var task = await Save_Load_Firebase.GetSeverDateTime();
        if (task.HasValue) { curTime = task.Value; }
        else { curTime = DateTime.UtcNow; }

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

    private void OnDisable()
    {
        AnimalManager.Ins.AddDataAnimal(GetAnimalSaveData());
    }
    public AnimalSaveData GetAnimalSaveData()
    {
        AnimalSaveData data = new AnimalSaveData
        {
            _idSO = animalData._id,
            animalDataID = animalData._itemName,
            worldPosition = new SerializableVector3(this.transform.position),
            timeProductReady = this.timeProductReady,

        };
        return data;
    }
    protected virtual void Flip()
    {
        if(rb.linearVelocityX > 0)
        {
            visual.flipX = true;
        }
        else
        {
            visual.flipX = false;   
        }
    }
}