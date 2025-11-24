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
    public event Func<ItemDataSO, int, DataRunTimeItem, bool> onAddItem;
    public bool AddItem(ItemDataSO item, int quantity, DataRunTimeItem dataRunTimeItem = null)
    {
       return onAddItem?.Invoke(item, quantity, dataRunTimeItem) ?? false ;
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

    public event Func<InventoryManager> onGetInventory;
    public InventoryManager GetDataInventory()
    {
        return onGetInventory?.Invoke();
    }
    public event Func<int, ItemDataSO> onGetItemSOByID;
    public ItemDataSO GetItemSOByID(int id)
    {
        return onGetItemSOByID?.Invoke(id);
    }
    public event Func<InventorySlot, int> onGetIndexOfSlot;
    public int GetIndexOfSlot(InventorySlot slot)
    {
        return onGetIndexOfSlot?.Invoke(slot) ?? -1 ;
    }
    public event Action onDisableInventory;
    public void DisableInventory()
    {
        onDisableInventory?.Invoke();
    }

}
