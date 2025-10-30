using System;
using UnityEngine;

public class ShopEvent 
{
    public event Action onPointerEnter;
    public void PointerEnter() { 
        onPointerEnter?.Invoke();
    }
    public event Action onPointerExit;
    public   void PointerExit() { onPointerExit?.Invoke(); }
    public event Action<string> onShowToolTip;
    public void ShowToolTip(string message)
    {
        onShowToolTip?.Invoke(message);
    }
}
