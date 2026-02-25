using Newtonsoft.Json;
using UnityEngine;

public class HarvestQuest : QuestStep
{
    public CollectionData dataStateRuntime;
    private void OnEnable()
    {
        GameEventManager.Ins.onHarvest += OnHarvest;
    }


    private void Start()
    {
        UpdateState();
    }
    private void OnHarvest(CropDataSO sO)
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

    void OnDisable()
    {
        if(GameEventManager.Ins != null)
        GameEventManager.Ins.onHarvest -= OnHarvest;
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
