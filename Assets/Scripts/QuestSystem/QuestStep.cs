using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    public bool isFinished = false;
    string questID;
   
    public void IniteialQuestStep(string id, QuestStepState questStepState)
    {
        this.questID = id;
        if(questStepState != null)
        {
            SetQuestStepState(questStepState);
        }
    }
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            GameEventManager.Ins.questEvents.FinishQuest(questID);
            Destroy(this.gameObject);
        }
       
    }

    protected void ChangeState(QuestStepState newState)
    {
        GameEventManager.Ins.questEvents.QuestStepStateChanged(questID, newState);
    }
    protected abstract void SetQuestStepState(QuestStepState newState);
   
}
