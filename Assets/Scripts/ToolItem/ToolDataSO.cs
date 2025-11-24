using UnityEngine;

public abstract class ToolDataSO : ItemShopDataSO
{
    [Header("Config")]
    public int maxDurability = 100;
    [Tooltip("Hao mòn mỗi lần sử dụng")]
    public int durabilityLossPerUse; // Hao mòn mỗi lần sử dụng
    [Tooltip("Đặt tên animation của các hành động phải trùng với từng enum")]
    public ToolType toolType;
    [Tooltip("Chi phí sửa chữa mỗi điểm độ bền")]
    public int repairCostPerPoint;// Chi phí sửa chữa mỗi điểm độ bền
    [Tooltip("Thời gian sửa chữa mỗi điểm độ bền")]
    public float timePerpointRepair;
    public enum ToolType
    {
        Axe,
        PickAxe,
        Shovel,
        Hoe,
        WateringCan,
        FishingRod,
        Sickle
    }
    public override void OnValidate()
    {
        
    }
}
