using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    bool isFinish = false;
    string questID;
    int stepIndex;
    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questID = questId;
        this.stepIndex = stepIndex;
        if(questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }
    protected void FinishQuestStep()
    {
        if (!isFinish)
        {
            isFinish = true;
            GameEventsManager.Ins.questEvent.AdvanceQuest(questID);
            Destroy(gameObject);
        }
    }
    protected void ChangeState(string newState)
    {
        GameEventsManager.Ins.questEvent.QuestStepStateChange(questID, stepIndex, new QuestStepState(newState));
    }
    protected abstract void SetQuestStepState(string state);

}
