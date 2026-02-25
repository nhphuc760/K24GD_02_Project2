using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestEvents 
{
    public event Action<string> onStartQuest;
    public void StartQuest(string id)
    {
        onStartQuest?.Invoke(id);
    }
    public event Action<string> onFinishQuest;
    public void FinishQuest(string id)
    {
        onFinishQuest?.Invoke(id);
    }

    public event Action<Quest> onQuestStateChanged; 
    /// <summary>
    /// Có thể subcribe cho UI để theo dõi trạng thái Quest
    /// </summary>
    /// <param name="quest"></param>
    public void QuestStateChanged(Quest quest) 
    {
        onQuestStateChanged?.Invoke(quest);
    }

    public event Action<string, QuestStepState> onQuestStepStateChanged;
    public void QuestStepStateChanged(string id, QuestStepState questStepState)
    {
        onQuestStepStateChanged?.Invoke(id, questStepState);
    }

    public event Action<Quest> onClaimReward;
    public void ClaimReward(Quest quest)
    {
        onClaimReward?.Invoke(quest);
            
    }

    public event Action<Quest> onQuestInforClick;
    public void QuestInforClick(Quest quest)
    {
        onQuestInforClick?.Invoke(quest);
    }
    public event Action<Quest> onRewardClick;
    public void RewardClick(Quest quest)
    {
        onRewardClick?.Invoke(quest);
    }
    public event Action<List<Quest>> OnLoadQuestMapSuccess;
    public void LoadQuestMapSuccess(List<Quest> quests)
    {
        OnLoadQuestMapSuccess?.Invoke(quests);
    }
    public event Action onMenuQuestPress;
    public void TriggerQuestUI()
    {
        onMenuQuestPress?.Invoke();
    }
    public event Action onHideQuestUI;
    public void HideQuestUI()
    {
        onHideQuestUI?.Invoke();
    }
}
