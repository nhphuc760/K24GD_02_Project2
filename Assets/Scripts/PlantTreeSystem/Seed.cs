using System;
using System.Collections;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Seed : MonoBehaviour, IToolTarget
{
    [Header("Harvest Indicator")]
    public GameObject harvestIndicatorPrefab; // Kéo HarvestIndicator_Prefab vào đây
    public Transform indicatorAnchor;         // Kéo "giá treo" IndicatorAnchor vào đây
    public ToolDataSO.ToolType requireTool;
    [SerializeField] GameObject canvas;
    [SerializeField] TextMeshProUGUI timeGrowthTXT;
    [SerializeField] Image fillGrowth;
    
    private int curGrowthProgress = 0; //index currentSeedData.growhtSprites
    DateTime timeHarvest; //lưu thời điểm thu hoạch được cây trồng
    DateTime curTime;
    private SeedDataSO currentSeedData; // Reference to the CropData ScriptableObject
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool isMature = false;//kiểm tra cây đã trưởng thành chưa
   
    private GameObject currentIndicator;      // Biến để lưu trữ bảng hiệu đã được tạo ra
 
    public ToolDataSO.ToolType RequireTool => requireTool;


    private void Start()
    {
        //sự kiện tua thời gian từ sleep
        if (TimeManager.instance != null)
        {
            TimeManager.instance.OnTimeSkipped += HandleTimeSkipped;
        }
    }
    private void OnDestroy()
    {
        // Hủy đăng ký để tránh lỗi
        if (TimeManager.instance != null)
        {
            TimeManager.instance.OnTimeSkipped -= HandleTimeSkipped;
        }
    }
    private void HandleTimeSkipped(float secondsSkipped)
    {
        if (isMature) return; // Nếu chín rồi thì thôi

        // Cộng thêm số giây đã ngủ vào thời gian hiện tại của cây
        //curTime là biến DateTime nội bộ của cây để đếm ngược
        curTime = curTime.AddSeconds(secondsSkipped);

        // Cập nhật ngay lập tức trạng thái cây (để UI và hình ảnh update luôn)
        CheckGrowthStatus();
    }
    private void CheckGrowthStatus()
    {
        TimeSpan coolDown = timeHarvest - curTime;

        // Cập nhật UI Fill
        if (fillGrowth != null)
        {
            fillGrowth.fillAmount = 1 - (float)(coolDown.TotalSeconds / currentSeedData.timeSpandHarvest);
        }
        if (timeGrowthTXT != null) timeGrowthTXT.text = coolDown.ToString(@"hh\:mm\:ss");

        // Cập nhật tiến độ lớn
        int progress = CalCulateProgress(curTime);
        if (progress > curGrowthProgress)
        {
            curGrowthProgress = progress;
            if (curGrowthProgress >= currentSeedData.growhtSprites.Count - 1)
            {
                isMature = true;
                requireTool = ToolDataSO.ToolType.Sickle;
                ShowHarvestIndicator(true);
                HideCoolDown();
                UpdateSprite();
                // Nếu đang chạy coroutine thì có thể dừng, nhưng để tự nhiên cũng được
            }
            UpdateSprite();
        }

        // Kiểm tra nếu đã vượt quá thời gian thu hoạch (trường hợp ngủ dậy là chín luôn)
        if (curTime >= timeHarvest && !isMature)
        {
            isMature = true;
            requireTool = ToolDataSO.ToolType.Sickle;
            ShowHarvestIndicator(true);
            HideCoolDown();
            UpdateSprite();
        }
    }



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
        requireTool = ToolDataSO.ToolType.Shovel;
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

    public bool CanInteract()
    {
        return isMature;
    }
    private void Harvest()//thu hoạch
    {
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
            requireTool = ToolDataSO.ToolType.Sickle;
            ShowHarvestIndicator(true);
        }
        else
        {
            curGrowthProgress = CalCulateProgress(curTime);
            isMature = false;
            requireTool = ToolDataSO.ToolType.Shovel;
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
            fillGrowth.fillAmount = 1 - (float)(coolDown.TotalSeconds / currentSeedData.timeSpandHarvest);
            fillGrowth.color = Color.Lerp(Color.red, Color.green, fillGrowth.fillAmount);
            timeGrowthTXT.text = coolDown.ToString(@"hh\:mm\:ss");
            int progress = CalCulateProgress(curTime);
            if (progress > curGrowthProgress)
            {
                
                curGrowthProgress = progress;
                if(curGrowthProgress >= currentSeedData.growhtSprites.Count - 1)
                {
                    isMature = true;
                    requireTool = ToolDataSO.ToolType.Sickle;
                    ShowHarvestIndicator(true);
                    HideCoolDown();
                    UpdateSprite();
                    yield break; //Thoát nếu cây trưởng thành
                }
                UpdateSprite();
            }
            yield return new WaitForSeconds(1);
        }
        requireTool = ToolDataSO.ToolType.Sickle;
        isMature = true;
        HideCoolDown();
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

    public void InteractWithTool(ToolDataSO toolDataSO, ToolRunTimeData tool)
    {
        // Defensive: ensure runtime exists and initialized
        if (tool == null)
        {
            if (toolDataSO != null)
            {
                tool = new ToolRunTimeData();
                tool.Init(toolDataSO);
            }
            else
            {
                // Nothing to do without toolDataSO
                Debug.LogWarning("InteractWithTool called with null toolDataSO and null runtime data.");
                return;
            }
        }

        switch (requireTool)
        {
            case ToolDataSO.ToolType.Shovel:
                tool.currentDurability -= toolDataSO.durabilityLossPerUse;
                Destroy(gameObject);
                break;
            case ToolDataSO.ToolType.Sickle:
                if (isMature)
                {
                    Harvest();
                    tool.currentDurability -= toolDataSO.durabilityLossPerUse;
                }
                else
                {
                    GameEventManager.Ins.TriggerDialog("<color=red>Cây trồng chưa thu hoạch được</color>");
                }
                break;
            default:
                break;
        }
    }
    public void ShowCoolDown()
    {
        if (isMature) return;
        canvas.SetActive(true);
    }
    public void HideCoolDown()
    {
        canvas.SetActive(false);
    }
}
