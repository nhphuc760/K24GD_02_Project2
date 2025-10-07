using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    Dictionary<string, Quest> questMap;
    int currentPlayerLevel;
    private void Awake()
    {
        questMap = CreateQuestMap();
      
    }
    private void OnEnable()
    {
        
        GameEventsManager.Ins.questEvent.onStartQuest += StartQuest;
        GameEventsManager.Ins.questEvent.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Ins.questEvent.onFinishQuest += FinishQuest;
        //GameEventsManager.Ins.PlayerEvents.onPlayerLevelChange += OnPlayerLevelChange;
        GameEventsManager.Ins.questEvent.onQuestStepStateChange += QuestStepStateChange;
    }

  

    private void OnDisable()
    {
        GameEventsManager.Ins.questEvent.onStartQuest -= StartQuest;
        GameEventsManager.Ins.questEvent.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Ins.questEvent.onFinishQuest -= FinishQuest;
        //GameEventsManager.Ins.PlayerEvents.onPlayerLevelChange -= OnPlayerLevelChange;
        GameEventsManager.Ins.questEvent.onQuestStepStateChange -= QuestStepStateChange;
    }


    private void Start()
    {
        
       
        foreach (Quest quest in questMap.Values)
        {
            if(quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameEventsManager.Ins.questEvent.QuestStateChange(quest);
        }

    }

    private void Update()
    {
        foreach (var quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }


    void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestByID(id);
        quest.state = state;
        GameEventsManager.Ins.questEvent.QuestStateChange(quest);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestByID(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }
    void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;
        if(currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestByID(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                // add this break statement here so that we don't continue on to the next quest, since we've proven meetsRequirements to be false at this point.
                break;
            }
        }
        return meetsRequirements;
    }


    void StartQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    void AdvanceQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        quest.MoveToNextStep();
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }
    void FinishQuest(string id)
    {
      Quest quest = GetQuestByID(id);
        ClaimReward(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    void ClaimReward(Quest quest)
    {
        //GameEventsManager.Ins.goldEvent.GoldGained(quest.info.gold);
        //GameEventsManager.Ins.experience.ExpGained(quest.info.exp);
    }

    Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuest = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (var questInfo in allQuest)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning(questInfo.id);
                
            }
            idToQuestMap.Add(questInfo.id, LoadQuestData(questInfo));
        }
        return idToQuestMap;
    }

    Quest GetQuestByID(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogWarning("");
        }
        return quest;
    }

    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            QuestData questData = quest.GetQuestData();
            Debug.Log(JsonUtility.ToJson(questData));
            foreach (QuestStepState stepState in questData.questStepStates)
            {
                Debug.Log(stepState.state);
            }
        }
    }
    void SaveQuestData(Quest quest)
    {
        //Save quest.questData to Json.
    }

    Quest LoadQuestData(QuestInfoSO questInfoSO)
    {
        Quest quest = null;
        try
        {
            if (PlayerPrefs.HasKey(questInfoSO.id))
            {
                string serilizeData = PlayerPrefs.GetString(questInfoSO.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serilizeData);
                quest = new Quest(questInfoSO, questData.questState, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new Quest(questInfoSO);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
        return quest;
    }
}
