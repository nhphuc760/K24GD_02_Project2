using System;
using UnityEngine;

public class AnimationEvent 
{
   public event Action<IToolTarget,ToolDataSO, ToolRunTimeData> onToolUse;
    public void ToolUse(IToolTarget toolTarget, ToolDataSO sO, ToolRunTimeData toolDataSO)
    {
        onToolUse?.Invoke(toolTarget,sO, toolDataSO);
    }
    public event Action onDead;
    public void Dead()
    {
        onDead?.Invoke();
    }
    public event Action onIdle;
    public void Idle()
    {
        onIdle?.Invoke();
    }
}
