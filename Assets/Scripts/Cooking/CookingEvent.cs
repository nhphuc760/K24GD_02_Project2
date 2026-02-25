using System;
using UnityEngine;
using UnityEngine.Events;

public class CookingEvent 
{
    public event Action onStartCook;
    public void StartCooking()
    {
        onStartCook?.Invoke();
    }

    public event Action<RecipeSO> onCookClick;
    public void OnCookClick(RecipeSO recipe)
    {
        onCookClick?.Invoke(recipe);
    }
    public event Action<int> onPointerClickSlotUI;
    public void PointerClickSlotUI(int index)
    {
        onPointerClickSlotUI?.Invoke(index);
    }
    public event Action onPlayerEnterKitchen;
    public void PlayerEnterKitchen()
    {
        Debug.Log("PlayerEnterKitchen");
        if(onPlayerEnterKitchen == null)
        {
            Debug.Log("onPlayerEnterKitchen is null");
        }
        onPlayerEnterKitchen?.Invoke();
    }
    public event Action onPlayerLeaveKitchen;
    public void PlayerLeaveKitchen()
    {
        Debug.Log("PlayerLeaveKitchen");
        if(onPlayerLeaveKitchen == null)
        {
            Debug.Log("onPlayerLeave is null");
        }
        onPlayerLeaveKitchen?.Invoke();
    }
    public event Func<ItemDataSO, int> onGetTotalItem;
    public int GetTotalItem(ItemDataSO itemDataSO)
    {
        return onGetTotalItem?.Invoke(itemDataSO) ?? 0;
    }
    public event Action<ItemDataSO, int> onGetItem;
    public void GetItem(ItemDataSO itemDataSO, int quantity)
    {
        onGetItem?.Invoke(itemDataSO, quantity);
    }
    public event Action<RecipeSO> confirmCookingChange;
    public void ConfirmCookingChange(RecipeSO recipe)
    {
        confirmCookingChange?.Invoke(recipe);
    }
    public event Action<RecipeSO> enableDialogCookingChange;
    public void EnableDialogCookingChange(RecipeSO recipe)
    {
        enableDialogCookingChange?.Invoke(recipe);
    }
    public event Action<KitchenItemDataSO> onCookingFinish;
    public void CookingFinish(KitchenItemDataSO sO)
    {
        onCookingFinish?.Invoke(sO);
    }
}
