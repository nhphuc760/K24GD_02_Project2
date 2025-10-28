using System;
using UnityEngine;

public class FishingQuest : QuestStep
{

    public int currentCaught = 0;   
    public int caughtToComplete = 5;    
    private void OnEnable()
    {
        GameEventManager.Ins.OnFishingCaught += OnFishingCaught;
    }

    private void OnFishingCaught()
    {
       if(currentCaught < caughtToComplete)
        {
            currentCaught++;
            UpdateState();
        }
        if(currentCaught >= caughtToComplete)
        {
            FinishQuestStep();
        }
    }

    void OnDisable()
    {
        GameEventManager.Ins.OnFishingCaught -= OnFishingCaught;
    }

   void UpdateState()
    {
       
        ChangeState(new QuestStepState { current = currentCaught, target = caughtToComplete });
    }

    protected override void SetQuestStepState(QuestStepState newState)
    {
      currentCaught = newState.current;
        UpdateState();
    }
}
