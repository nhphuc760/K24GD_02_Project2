using System;
using UnityEngine;

[Serializable]
public class QuestData 
{
    public QuestState questState;
    public int questStepIndex;
    public QuestStepState[] questStepStates;
    public QuestData(QuestState state, int questStepIndex, QuestStepState[] questStepStates)
    {
        this.questState = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}
