using System;
using UnityEngine;

public class ShopEvent 
{
    public event Action onPointerEnter;
    public void PointerEnter() { 
        onPointerEnter?.Invoke();
    }
    public event Action onHideToolTip;
    public   void HideToolTip() { onHideToolTip?.Invoke(); }
    public event Action<string, RectTransform> onShowToolTip;
    public void ShowToolTip(string message, RectTransform parent)
    {
        onShowToolTip?.Invoke(message, parent);
    }
    public event Action onShow;
    public void Show()
    {
        onShow?.Invoke();
    }
    public event Action onHide;
    public void Hide()
    {
        onHide?.Invoke();
    }
    public event Action onPlayerEnterShop;
    public event Action onPlayerExitShop;
    public void PlayerEnterShop()
    {
        onPlayerEnterShop?.Invoke();
    }
    public void PlayerExitShop()
    {
        onPlayerExitShop?.Invoke();
    }
}
