using UnityEngine;

public abstract class ToolDataSO : ItemDataSO
{
    [Header("Config")]
    public int maxDurability = 100;
    public int durabilityLossPerUse; // Hao mòn mỗi lần sử dụng
    [Tooltip("Đặt tên animation của các hành động phải trùng với từng enum")]
    public ToolType toolType;
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
}
