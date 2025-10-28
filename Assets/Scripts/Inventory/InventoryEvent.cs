using System;
using UnityEngine;

public class InventoryEvent 
{
    public event Action<int, int> onRemoveItemClick;
    public void RemoveItem(int slotIndex, int quantity)
    {
        onRemoveItemClick?.Invoke(slotIndex, quantity);
    }
    public event Action<bool> onRemoveItemCompleted;
    public void RemoveItemCompleted(bool isActiveFalse)
    {
        onRemoveItemCompleted?.Invoke(isActiveFalse);
    }
    public event Func<ItemDataSO, int, bool> checkHasItem;
    public bool CheckHasItem(ItemDataSO item, int quantity)
    {
        return checkHasItem?.Invoke(item, quantity) ?? false;
    }
    public event Action<ItemDataSO, int> onRemoveItemByData;
    public void RemoveItemByData(ItemDataSO item, int quantity)
    {
        onRemoveItemByData?.Invoke(item, quantity);
    }
    public event Action<ItemDataSO, int> onAddItem;
    public void AddItem(ItemDataSO item, int quantity)
    {
        onAddItem?.Invoke(item, quantity);
    }
}
