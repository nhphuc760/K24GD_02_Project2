using System;
using UnityEngine;

public class ToolKitEvent 
{
    //public event Action<int> onSelectedChange;
    //public void SelectedChange(int newIndex)
    //{
    //    onSelectedChange?.Invoke(newIndex);
    //}
    public event Action<Inventory> onInitSuccess;
    public void InitSuccess(Inventory inventory)
    {
        onInitSuccess?.Invoke(inventory);
    }
    public event Action<int> onCallInput;
    public void CallInput(int index)
    {
        onCallInput?.Invoke(index);
    }

    public event Action onUpdateUI;
    public void UpdateUI()
    {
        onUpdateUI?.Invoke();
    }

    public event Func<InventorySlot> onGetDataChooseSlot;
    public InventorySlot GetCurDataChoose()
    {
       return onGetDataChooseSlot?.Invoke();
    }

    public event Func<int> onGetCurrentIndexChoose;
    public int GetCurrentIndex()
    {
        return onGetCurrentIndexChoose?.Invoke() ?? -1;
    }
}
