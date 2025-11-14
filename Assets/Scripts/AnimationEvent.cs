using System;
using UnityEngine;

public class AnimationEvent 
{
   public event Action<IToolTarget, ToolDataSO> onToolUse;
    public void ToolUse(IToolTarget toolTarget, ToolDataSO toolDataSO)
    {
        onToolUse?.Invoke(toolTarget, toolDataSO);
    }
}
