using Newtonsoft.Json;
using UnityEngine;

public class MineOreQuest : QuestStep
{
    public CollectionData dataStateRuntime;
    private void OnEnable()
    {
        GameEventManager.Ins.onMiningOre += OnMineOre;
    }

    private void OnMineOre(ResourceSO sO)
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
        GameEventManager.Ins.onMiningOre -= OnMineOre;
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
