using System;
using UnityEngine;

public class AnimationEvent 
{
   public event Action<IToolTarget,ToolDataSO, ToolRunTimeData> onToolUse;
    public void ToolUse(IToolTarget toolTarget, ToolDataSO sO, ToolRunTimeData toolDataSO)
    {
        onToolUse?.Invoke(toolTarget,sO, toolDataSO);
    }
}
