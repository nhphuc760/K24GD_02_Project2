using Unity.VisualScripting;
using UnityEngine;

public class Crop : MonoBehaviour, IInteractable
{
    private int growthProgress = 0;
    private double timePlanted = 0;//lưu lại thời điểm được trồng
    private CropData currentCropData; // Reference to the CropData ScriptableObject
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
    public void Plant(CropData cropData)
    {
        currentCropData = cropData;
        // Lấy thời gian chính xác từ TimeManager
        timePlanted = TimeManager.instance.totalTimeElapsed;
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
            if (growthProgress >= currentCropData.DaysToGrow)
            {
                growthProgress = currentCropData.DaysToGrow;
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
        int growthStageCount = currentCropData.growhtSprites.Count;

        if (growthStageCount > 0)
        {
            // Tính toán giai đoạn dựa trên tiến độ
            int currentStage = (int)((float)growthProgress / currentCropData.DaysToGrow * (growthStageCount - 1));
            currentStage = Mathf.Clamp(currentStage, 0, growthStageCount - 1);
            spriteRenderer.sprite = currentCropData.growhtSprites[currentStage];
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
    }
    private void Harvest()//thu hoạch
    {
        // Logic to add the crop to the player's inventory would go here
        Debug.Log($"Harvested {currentCropData.cropName} for {currentCropData.sellPrice} coins!");
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
            currentIndicator.GetComponentInChildren<UnityEngine.UI.Image>().sprite = currentCropData.icon;

            // Bật hoặc tắt bảng hiệu
            currentIndicator.SetActive(show);
        }
    }
}
