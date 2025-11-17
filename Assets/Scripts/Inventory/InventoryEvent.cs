using System;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public event Func<ItemDataSO, int, bool> onAddItem;
    public bool AddItem(ItemDataSO item, int quantity)
    {
       return onAddItem?.Invoke(item, quantity) ?? false ;
    }
    

    public event Action opendBagPressed;
    public void OpenBagPress()
    {
        opendBagPressed?.Invoke();
    }

    public event Action<PointerEventData> onDropItem;
    public void DropItem(PointerEventData eventData)
    {
        onDropItem?.Invoke(eventData);
    }

    public event Action<InventoryManager> onGetInventory;
    public void GetDataInventory(InventoryManager invenManager)
    {
        onGetInventory?.Invoke(invenManager);
    }
}
