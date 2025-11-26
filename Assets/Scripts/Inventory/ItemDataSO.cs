using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int _id;
    public string _itemName;
    public Sprite _icon;
    [TextArea]
    public string _description;
    public int _maxStack;
    public bool isStackable;
    public ItemType itemType;
    public RunTimeItemType runTimeItemType;
    public int sell;
    public bool isCanSell;
    public bool isCanUseBuff;

    public virtual void OnValidate()
    {
        _itemName = this.name;
    }

    public virtual void Use(GameObject tarGet)
    {

    }
}

public enum ItemType
{
    Consumable,
    Equipment,
    Tool,
}
public enum RunTimeItemType
{
    None,
    ToolRunTimeData
}