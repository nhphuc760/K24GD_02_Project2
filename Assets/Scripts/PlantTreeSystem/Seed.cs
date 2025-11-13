using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Seed : MonoBehaviour, IInteractable
{
    private int curGrowthProgress = 0; //index currentSeedData.growhtSprites
    DateTime timeHarvest; //lưu thời điểm thu hoạch được cây trồng
    DateTime curTime;
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
    public async void Plant(SeedDataSO cropData)
    {
        currentSeedData = cropData;

        // Lấy thời gian chính xác từ Sever
        var task = await Save_Load_Firebase.GetSeverDateTime();
    
        if (task.HasValue)
        {

             curTime = task.Value;
        }
        else
        {
            curTime = DateTime.UtcNow;
        }
        timeHarvest = curTime.AddSeconds(currentSeedData.timeSpandHarvest);
        curGrowthProgress = 0;
        isMature = false;
        StartCoroutine(Growth());
        // Tắt mọi bảng hiệu cũ (nếu có) khi trồng
        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
        }
        UpdateSprite();
    }
    // Cập nhật lại UpdateSprite để dùng biến "growthProgress"
    private void UpdateSprite()
    {
        int growthStageCount = currentSeedData.growhtSprites.Count;

        if (curGrowthProgress - 1 <= growthStageCount)
        {     
            spriteRenderer.sprite = currentSeedData.growhtSprites[curGrowthProgress];
        }
        else
        {
            spriteRenderer.sprite = currentSeedData.growhtSprites[growthStageCount - 1];
        }
        if (isMature)
        {
            spriteRenderer.sprite = currentSeedData.growhtSprites[growthStageCount - 1];
        }
    }
    public void Interact()
    {
        if (isMature)
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
        data.timeHarvest = this.timeHarvest;
        return data;
    }

    public void LoadCropState(SeedDataSO dataAsset, DateTime timeHarvest, DateTime curTime)
    {
        this.currentSeedData = dataAsset;
        this.timeHarvest = timeHarvest;
        this.curTime = curTime;
        TimeSpan t = timeHarvest - curTime;
        if (t < TimeSpan.Zero)
        {

            isMature = true;
            ShowHarvestIndicator(true);
        }
        else
        {
            curGrowthProgress = CalCulateProgress(curTime);
            isMature = false;
            ShowHarvestIndicator(false);
            StartCoroutine(Growth());
        }
        UpdateSprite();

    }

    IEnumerator Growth()
    {
        while (curTime < timeHarvest)
        {
            curTime = curTime.AddSeconds(1);
            TimeSpan coolDown = timeHarvest - curTime;
            int progress = CalCulateProgress(curTime);
            if (progress > curGrowthProgress)
            {
                
                curGrowthProgress = progress;
                if(curGrowthProgress >= currentSeedData.growhtSprites.Count - 1)
                {
                    isMature = true;
                    ShowHarvestIndicator(true);
                    UpdateSprite();
                    yield break; //Thoát nếu cây trưởng thành
                }
                UpdateSprite();
            }
            yield return new WaitForSeconds(1);
        }
        isMature = true;
        ShowHarvestIndicator(true);
        UpdateSprite();

    }

    int CalCulateProgress(DateTime curTime)
    {
        try
        {
            TimeSpan t = timeHarvest - curTime;
            float timeSpan = currentSeedData.GetTimeSpanPerProgress();
            int nev_progress = Mathf.CeilToInt((float)t.TotalSeconds / timeSpan);
            return currentSeedData.growhtSprites.Count - 1 - nev_progress;
        }
        catch
        {
            return 0;
        }
    }
}
