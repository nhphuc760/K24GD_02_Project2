using UnityEngine;

public class Crop : MonoBehaviour, IInteractable
{
    private int daysSincePlanted = 0; // Days since the crop was planted
    private CropData currentCropData; // Reference to the CropData ScriptableObject
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private bool isMature = false;//kiểm tra cây đã trưởng thành chưa
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //Hàm được gọi ngay sau khi PlayerFarming trồng cây
    public void Plant(CropData cropData)
    {
        currentCropData = cropData;
        daysSincePlanted = 0;
        isMature = false;
        UpdateSprite();
    }

    //Hàm được gọi bởi TimeManager mỗi khi một ngày trôi qua trong trò chơi
    public void Grow()
    {
        daysSincePlanted++;
        if (daysSincePlanted >= currentCropData.DaysToGrow)
        {
            daysSincePlanted = currentCropData.DaysToGrow; // Giữ nguyên nếu đã đạt đến ngày cuối cùng
            isMature = true; // Cây đã trưởng thành
        }
        UpdateSprite();
    }
    private void UpdateSprite()
    {
        int growthStageCount = currentCropData.growhtSprites.Count;

        if (currentCropData.DaysToGrow > 0)
        {
            int currentStage = (int)((float)daysSincePlanted / currentCropData.DaysToGrow * (growthStageCount - 1));
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
}
