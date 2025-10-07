using UnityEngine;

public class Quest
{
    public QuestInfoSO info;
    public QuestState state;
    int currentQuestStepIndex;
    QuestStepState[] questStepStates;
    public Quest(QuestInfoSO infor)
    {
        this.info = infor;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[infor.questStepPrefabs.Length];
        currentQuestStepIndex = 0;
        for(int i = 0; i <  infor.questStepPrefabs.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }
    public Quest(QuestInfoSO questInfoSO, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates )
    {
        this.info = questInfoSO;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        if(this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("");
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
       

    }
    public bool CurrentStepExists()
    {
        return currentQuestStepIndex < info.questStepPrefabs.Length;
    }
    public void InstantiateCurrentQuestStep(Transform parent)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if(questStepPrefab != null)
        {
            QuestStep questStep =  Object.Instantiate<GameObject>(questStepPrefab, parent).GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
        }
    }

    public GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {

        }
        return questStepPrefab; 
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if(stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
        }
        else
        {
            Debug.Log("Tried to access out of range");
        }
    }
    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }


}
