using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int _id;
    public string _itemName;
    public Sprite _icon;
    public string _description;
    public int _maxStack;
    public bool isStackable;
    public ItemType itemType;

    public virtual void Use()
    {
       
    }

    private void OnValidate()
    {
        _itemName = this.name;
    }
}

public enum ItemType
{
    Consumable,
    Equipment,
   
}