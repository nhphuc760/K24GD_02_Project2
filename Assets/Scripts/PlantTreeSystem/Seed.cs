using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Seed : MonoBehaviour, IInteractable
{
    private int growthProgress = 0;
    private double timePlanted = 0;//lưu lại thời điểm được trồng
    private SeedDataSO currentSeedData; // Reference to the CropData ScriptableObject
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool isMature = false;//kiểm tra cây đã trưởng thành chưa
    [Header("Harvest Indicator")]
    public GameObject harvestIndicatorPrefab; // Kéo HarvestIndicator_Prefab vào đây
    public Transform indicatorAnchor;         // Kéo "giá treo" IndicatorAnchor vào đây
    private GameObject currentIndicator;      // Biến để lưu trữ bảng hiệu đã được tạo ra
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //Hàm được gọi ngay sau khi PlayerFarming trồng cây
    public void Plant(SeedDataSO cropData)
    {
        currentSeedData = cropData;

        // Lấy thời gian chính xác từ TimeManager
        if (TimeManager.instance != null)
        {
            timePlanted = TimeManager.instance.totalTimeElapsed;
        }
        else
        {
            Debug.LogError("TimeManager chưa sẵn sàng khi Plant!");
            timePlanted = 0;
        }
        growthProgress = 0;
        isMature = false;
        // Tắt mọi bảng hiệu cũ (nếu có) khi trồng
        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
        }
        UpdateSprite();
    }

    //Hàm được gọi bởi TimeManager mỗi khi một ngày trôi qua trong trò chơi
    public void Grow()
    {
        if (isMature) return;
        //Tính toán xem đã bao nhiêu giây trôi qua từ lúc trồng
        double timeSincePlanted = TimeManager.instance.totalTimeElapsed - timePlanted;
        //Tính số giây đó tương đương bao nhiêu "ngày game"
        int newGrowthProgress = (int)(timeSincePlanted / TimeManager.instance.secondsperDay);
        //cập nhật sprite nếu tiến độ mới lớn hơn tiến độ cũ
        if (newGrowthProgress > growthProgress)
        {
            growthProgress = newGrowthProgress;

            // Kiểm tra xem đã chín chưa
            if (growthProgress >= currentSeedData.DaysToGrow)
            {
                growthProgress = currentSeedData.DaysToGrow;
                isMature = true;
                ShowHarvestIndicator(true);
            }

            // Cập nhật hình ảnh
            UpdateSprite();
        }
    }
    // Cập nhật lại UpdateSprite để dùng biến "growthProgress"
    private void UpdateSprite()
    {
        int growthStageCount = currentSeedData.growhtSprites.Count;

        if (growthStageCount > 0)
        {
            // Tính toán giai đoạn dựa trên tiến độ
            int currentStage = (int)((float)growthProgress / currentSeedData.DaysToGrow * (growthStageCount - 1));
            currentStage = Mathf.Clamp(currentStage, 0, growthStageCount - 1);
            spriteRenderer.sprite = currentSeedData.growhtSprites[currentStage];
        }
    }

    private void OnEnable()
    {
        TimeManager.OnNewDay += Grow;
    }
    private void OnDisable()
    {
        TimeManager.OnNewDay -= Grow;
    }
    public void Interact()
    {
        if(isMature)
        {
            Harvest();
        }
        else
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Cây trồng chưa thu hoạch được</color>");
        }
    }

    public bool CanInteract()
    {
        return isMature;
    }
    private void Harvest()//thu hoạch
    {
        // Logic to add the crop to the player's inventory would go here
        Debug.Log($"Harvested {currentSeedData.cropData._itemName} for {currentSeedData.sellPrice} coins!");
        GameEventManager.Ins.inventoryEvent.AddItem(currentSeedData.cropData, currentSeedData.yield);
        Destroy(gameObject); // Remove the crop from the game world after harvesting
    }
    //hàm bật tắt 'bảng hiệu'
    private void ShowHarvestIndicator(bool show)
    {
        // Nếu chưa có bảng hiệu, tạo nó ra
        if (currentIndicator == null && harvestIndicatorPrefab != null)
        {
            // Tạo bảng hiệu và đặt nó làm con của "giá treo"
            currentIndicator = Instantiate(harvestIndicatorPrefab, indicatorAnchor);
            currentIndicator.transform.position = indicatorAnchor.position; // Đảm bảo đúng vị trí
        }

        // Cập nhật icon trên bảng hiệu
        if (currentIndicator != null)
        {
            // Tìm component Image trong các con của bảng hiệu và gán icon vào
            // Lưu ý: ItemData là lớp cha của CropData, chứa biến 'icon'
            currentIndicator.GetComponentInChildren<UnityEngine.UI.Image>().sprite = currentSeedData.harvestIndicator; 

            // Bật hoặc tắt bảng hiệu
            currentIndicator.SetActive(show);
        }
    }
    public SeedSaveData GetSaveData()
    {
        SeedSaveData data = new SeedSaveData();
        data.SceneName = SceneManager.GetActiveScene().name;
        data.worldPosition = new SerializableVector3(transform.position);
        // Lưu tên của CropData để tái tạo sau này(lấy tên file Asset) 
        data.cropDataID = currentSeedData._id;
        data.timePlanted = this.timePlanted;
        return data;
    }

    public void LoadCropState(SeedDataSO dataAsset, double plantedTime)
    {
        this.currentSeedData = dataAsset;
        this.timePlanted = plantedTime;

        //Gọi lại Growth() để tính toán tiến độ dựa theo thời gian đã trôi qua
        if (TimeManager.instance != null)
        {
            //tính toán thời gian đã trôi qua từ lúc trồng đến hiện tại dựa trên timePlanted và totalTimeElapsed
            double timeSincePlanted = TimeManager.instance.totalTimeElapsed - this.timePlanted;
            growthProgress = (int)(timeSincePlanted / TimeManager.instance.secondsperDay);

            //Cập nhật isMature và sprite dựa trên growthProgress mới tính
            if (growthProgress >= currentSeedData.DaysToGrow)
            {
                growthProgress = currentSeedData.DaysToGrow;
                isMature = true;
                ShowHarvestIndicator(true);
            }
            else
            {
                isMature = false;
                ShowHarvestIndicator(false);
            }
            UpdateSprite();
        }
        else
        {
            Debug.LogWarning("TimeManager chưa sẵn sàng khi LoadCropState!");
            growthProgress = 0;
            isMature = false;
            UpdateSprite();
        }
    }
}
