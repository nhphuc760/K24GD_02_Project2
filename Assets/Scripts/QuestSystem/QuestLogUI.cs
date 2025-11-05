using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] QuestManger manager;
    [SerializeField] QuestUI prefab;
    [SerializeField] Transform container;
    private void Start()
    {
        if (manager == null)
        {
            Debug.Log("[QuestLogUI] QuestManger reference is null. Assign it in the Inspector.");
            return;
        }
        if (prefab == null)
        {
            Debug.Log("[QuestLogUI] QuestUI prefab is null. Assign it in the Inspector.");
            return;
        }

        GameEventManager.Ins.questEvents.OnLoadQuestMapSuccess += Manger_OnLoadQuestMapSuccess;
        GameEventManager.Ins.questEvents.onQuestInforClick += QuestEvents_onQuestInforClick;
        GameEventManager.Ins.questEvents.onRewardClick += QuestEvents_onRewardClick;
        Hide();
        //sub event <string id>
    }

    private void QuestEvents_onRewardClick(Quest quest)
    {
        if (quest == null) {
            Debug.Log($"Quset is null");
            return;
        }
        if (!quest.questState.Equals(QuestState.FINISHED)) {
            GameEventManager.Ins.TriggerDialog("<color=red>Bạn chưa làm xong nhiệm vụ để nhận thưởng</color>");
            return; 
        }

        if (quest.isClaimRewards)
        {
            //if (isCoroutine || dialogText == null) return;
            //StartCoroutine(DisplayDialogText("Phần thưởng đã được nhận trước đó", Color.red));
            GameEventManager.Ins.TriggerDialog("<color=red>Phần thưởng đã được nhận trước đó</color>");
        }
        else
        {
            GameEventManager.Ins.questEvents.ClaimReward(quest);
            //if (!isCoroutine) {
            //    if (dialogText == null) return;
            //    StartCoroutine(DisplayDialogText("Phần thưởng đã được thêm vào kho đồ", Color.green));
            //}
            GameEventManager.Ins.TriggerDialog("<color=green>Phần thưởng đã được thêm vào kho đồ</color>");
           
        }
    }

    private void QuestEvents_onQuestInforClick(Quest quest)
    {   
       // if (isCoroutine) return;
        if (quest.questState.Equals(QuestState.CAN_START))
        {
            GameEventManager.Ins.questEvents.StartQuest(quest.questInforSO._id);
            //StartCoroutine(DisplayDialogText($"Bắt đầu nhiệm vụ {quest.questInforSO.displayName}", Color.cyan));
            GameEventManager.Ins.TriggerDialog($"<color=cyan>Bắt đầu nhiệm vụ {quest.questInforSO.displayName}</color>");

        }
        else if (quest.questState.Equals(QuestState.FINISHED))
        {


            //StartCoroutine(DisplayDialogText("Nhiệm vụ đã hoàn thành, kiểm tra phần thưởng", Color.green));
            GameEventManager.Ins.TriggerDialog("<color=green>Nhiệm vụ đã hoàn thành, kiểm tra phần thưởng</color>");
           
        }
        else
        {
            //StartCoroutine(DisplayDialogText("Nhiệm vụ chưa hoàn thành", Color.red));
            GameEventManager.Ins.TriggerDialog("<color=red>Nhiệm vụ chưa hoàn thành</color>");
        }
    }

    private void OnDestroy()
    {
        GameEventManager.Ins.questEvents.OnLoadQuestMapSuccess -= Manger_OnLoadQuestMapSuccess;
        GameEventManager.Ins.questEvents.onQuestInforClick -= QuestEvents_onQuestInforClick;
        GameEventManager.Ins.questEvents.onRewardClick -= QuestEvents_onRewardClick;
    }

    private void Manger_OnLoadQuestMapSuccess(List<Quest> questMap)
    {
        if (questMap == null) return;

        foreach (Quest quest in questMap)
        {
            if (quest == null) continue;

            QuestUI questUI = Instantiate(prefab, container);
            questUI.Init(manager, quest.questInforSO._id);
            questUI.ChangedState(quest.questState);
            if(quest.questStepStates == null)
            {
                questUI.ChangeStepState(new QuestStepState { current = 0, target = 0});
            }
            else
            {
                questUI.ChangeStepState(quest.questStepStates);
            }

                questUI.SetPlaynameQuest(quest.questInforSO.displayName);
            // safe-subscribe to quest events
            quest.questStepStateChanged += questUI.ChangeStepState;
            quest.questStateChanged += questUI.ChangedState;

            // update reward UI safely
            if (questUI.rewardUI != null && quest.questInforSO != null && quest.questInforSO.reward != null)
            {
                questUI.rewardUI.UpdateUI(quest.questInforSO.reward);
            }
        }
    }

    void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
}
