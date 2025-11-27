using System;
using UnityEngine;

public class RepairToolQuest : QuestStep
{

    protected override void SetQuestStepState(QuestStepState newState)
    {
        
    }

    private void OnEnable()
    {
        
        GameEventManager.Ins.onRepairTool += OnRepairTool;
    }

    private void OnRepairTool(ToolDataSO sO)
    {
      FinishQuestStep();
    }

    private void OnDisable()
    {
        if(GameEventManager.Ins != null)
        GameEventManager.Ins.onRepairTool -= OnRepairTool;
    }

   
}
