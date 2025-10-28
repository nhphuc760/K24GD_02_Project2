using JetBrains.Annotations;
using UnityEngine;

public class InventorySlot 
{
    public ItemDataSO ItemData;
    public int quantity;
    public bool IsEmpty => ItemData == null;


    public InventorySlot()
    {
        ItemData = null;
        quantity = 0;

    }
    public InventorySlot(ItemDataSO itemDataSO, int quantity)
    {
        this.ItemData = itemDataSO;
        this.quantity = quantity;
    }
    public void Clear()
    {
        ItemData = null;
        quantity = 0;
    }

    public void Remove(int quantity)
    {
       this.quantity -= quantity;
      if(this.quantity <= 0)
        {
            Clear();
        }
    }

    public void Assign(ItemDataSO itemDataSO, int newQuantity)
    {
        ItemData = itemDataSO;
        quantity = newQuantity;
    }
}
