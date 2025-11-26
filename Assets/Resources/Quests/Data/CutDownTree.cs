using Newtonsoft.Json;
using UnityEngine;

public class CutDownTree : QuestStep
{
    public CollectionData dataStateRuntime;
    private void OnEnable()
    {
        GameEventManager.Ins.onCutDTree += OnCutDownTree;
    }

    private void Start()
    {
        UpdateState();
    }

    private void OnCutDownTree(ResourceSO sO)
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
        GameEventManager.Ins.onCutDTree -= OnCutDownTree;
    }

    void UpdateState()
    {

        ChangeState(new QuestStepState {questStateDynamic = dataStateRuntime.ToString()});
    }

    protected override void SetQuestStepState(QuestStepState newState)
    {
        try
        {
            var tmp = JsonConvert.DeserializeObject<CollectionData>(newState.questStateDynamic);
            if(tmp != null)
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
