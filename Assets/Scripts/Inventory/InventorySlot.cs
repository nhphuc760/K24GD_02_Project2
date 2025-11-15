using JetBrains.Annotations;
using UnityEngine;

public class InventorySlot 
{
    public ItemDataSO ItemData;
    public int quantity;
    public DataRunTimeItem dataRuntime;
    public bool IsEmpty => ItemData == null;
    public string description;
    public InventorySlot()
    {
        ItemData = null;
        quantity = 0;
        dataRuntime = null;
        description = null;
    }
    public InventorySlot(ItemDataSO itemDataSO, int quantity, DataRunTimeItem dataRunTimeItem)
    {
        this.ItemData = itemDataSO;
        this.quantity = quantity;
        this.dataRuntime = dataRunTimeItem;
        description = dataRunTimeItem != null ? dataRunTimeItem.ToString() : null;
    }
    public void Clear()
    {
        ItemData = null;
        quantity = 0;
        dataRuntime = null;
        description = null; 
    }

    public void Remove(int quantity)
    {
       this.quantity -= quantity;
      if(this.quantity <= 0)
        {
            Clear();
        }
    }

    public void Assign(ItemDataSO itemDataSO, int newQuantity, DataRunTimeItem data)
    {
        ItemData = itemDataSO;
        quantity = newQuantity;
        this.dataRuntime = data;
        description = data != null ? data.ToString() : null;
    }



}
