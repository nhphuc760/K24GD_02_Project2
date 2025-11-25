using System;
using Newtonsoft.Json;
using UnityEngine;

public class PlantingQuestStep : QuestStep
{
    public CollectionData dataStateRuntime;
    private void Start()
    {
        
        GameEventManager.Ins.onPlanting += OnPlantingSeed;

    }

    private void OnDisable()
    {
        GameEventManager.Ins.onPlanting -= OnPlantingSeed;
    }


    private void OnPlantingSeed(SeedDataSO seedDataSO)
    {
        if (dataStateRuntime.current < dataStateRuntime.target)
        {
            dataStateRuntime.current += 1;
            UpdateState();
        }
        if (dataStateRuntime.current >= dataStateRuntime.target)
        {
            FinishQuestStep();
        }
    }


    void UpdateState()
    {

        ChangeState(new QuestStepState { questStateDynamic = dataStateRuntime.ToString() });
    }
    protected override void SetQuestStepState(QuestStepState newState)
    {
        try
        {
            var tmp = JsonConvert.DeserializeObject<CollectionData>(newState.questStateDynamic);
            if (tmp != null)
            {
                dataStateRuntime = tmp;
            }
            UpdateState();
        }
        catch
        {

        }
    }
}
