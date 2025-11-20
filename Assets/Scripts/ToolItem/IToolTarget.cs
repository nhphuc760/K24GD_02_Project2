using UnityEngine;

public interface IToolTarget 
{
    public ToolDataSO.ToolType RequireTool { get; }
   public void InteractWithTool(ToolDataSO toolDataSO, ToolRunTimeData  tool);
}
