using UnityEngine;

public class QuestData 
{
    public string questID;
    public QuestState questState;
    public QuestStepState questStepState;
    public bool isClaimedReward;
    public QuestData(string questI, QuestState state, QuestStepState questStepState, bool isClaimedReward)
    {
        this.questID = questI;
        this.questState = state;
        this.questStepState = questStepState;
        this.isClaimedReward = isClaimedReward;
    }
}
