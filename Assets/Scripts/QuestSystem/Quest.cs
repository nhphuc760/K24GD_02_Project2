using System;
using UnityEngine;

[Serializable]
public class Quest 
{
    public QuestInforSO questInforSO;
    private QuestState _questState;
    private QuestStepState _questStepStates;
    public bool isClaimRewards = false;

    // Public properties use backing fields and only invoke events when state actually changes
    public QuestState questState
    {
        get => _questState;
        set
        {    
            _questState = value;
            questStateChanged?.Invoke(value);
        }
    }

    public QuestStepState questStepStates
    {
        get => _questStepStates;
        set
        {
            _questStepStates = value;
            questStepStateChanged?.Invoke(value);
        }
    }

    public event Action<QuestState> questStateChanged;
    public event Action<QuestStepState> questStepStateChanged;

    public Quest(QuestInforSO questInforSO)
    {
        this.questInforSO = questInforSO;
        this.questState = QuestState.REQUIREMENTS_NOT_MET;
        this.questStepStates = new QuestStepState();
    }

    public Quest(QuestInforSO questInforSO, QuestState questState, QuestStepState questStepStates, bool isClaimed)
    {
        this.questInforSO = questInforSO;
        this.questState = questState;
        this.questStepStates = questStepStates;
        this.isClaimRewards = isClaimed;
    }

    /// <summary>
    /// Instantitate gameObject QuestStep
    /// </summary>
    /// <param name="parent"></param>
    public void InstantiateQuestStep(Transform parent)
    {
        QuestStep questStep = GameObject.Instantiate(questInforSO.questStepPrefab, parent).GetComponent<QuestStep>();
        questStep.IniteialQuestStep(questInforSO._id, questStepStates);
    }

    /// <summary>
    ///  Cập nhật questStepState vào quest
    /// </summary>
    /// <param name="questStepState"></param>
    /// <param name="index"></param>
    public void StoreQuestStepState(QuestStepState questStepState)
    {
        this.questStepStates = questStepState;
    }

    public QuestData GetQuestData()
    {
        return new QuestData(questInforSO._id, questState, questStepStates, isClaimRewards);
    }
}
