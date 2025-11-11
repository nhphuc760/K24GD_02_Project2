using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManger : MonoBehaviour
{

    [SerializeField]
    Dictionary<string, Quest> questMap;
 
    [SerializeField] int currentPlayerLevel = 10;
   
    QuestInforSO[] questInforSOs;
    private  void Awake()
    {
     
    }

    private void OnEnable()
    {
        GameEventManager.Ins.questEvents.onStartQuest += StartQuest;
        GameEventManager.Ins.questEvents.onFinishQuest += FinishQuest;
        GameEventManager.Ins.questEvents.onQuestStepStateChanged += QuestStepStateChanged;
    }

    

    private void OnDisable()
    {
        GameEventManager.Ins.questEvents.onStartQuest -= StartQuest;
        GameEventManager.Ins.questEvents.onFinishQuest -= FinishQuest;
        GameEventManager.Ins.questEvents.onQuestStepStateChanged -= QuestStepStateChanged;
    }

    private async void Start()
    {
        questMap = await CreateQuestMap();
        if (questMap == null)
        {
            Debug.Log("QuestMap is null");
            return;
        }
        foreach (var quest in questMap.Values)
        {
            if (quest.questState.Equals(QuestState.IN_PROGRESS))
            {
                quest.InstantiateQuestStep(this.transform);
            }
            if (quest.questState.Equals(QuestState.REQUIREMENTS_NOT_MET) && CheckQuestCondition(quest))
            {
                ChangeQuestState(quest.questInforSO._id, QuestState.CAN_START);

            }
            GameEventManager.Ins.questEvents.QuestStateChanged(quest);
        }
        GameEventManager.Ins.questEvents.LoadQuestMapSuccess(questMap.Values.ToList());

    }

    void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestByID(id);
        quest.questState = state;
        Debug.Log(id + state.ToString());
        GameEventManager.Ins.questEvents.QuestStateChanged(quest);
    }

    bool CheckQuestCondition(Quest quest)
    {
        
        if (currentPlayerLevel < quest.questInforSO.requiredLevel) // Example level check
        {
           return false;
        }
        foreach (var child in quest.questInforSO.prerequisiteQuests) { 
            if(GetQuestByID(child._id).questState != QuestState.FINISHED)
            {
              return false;
            }
        }

        return true;
    }   

    void StartQuest(string id)
    {
        Quest quest  = GetQuestByID(id);
        quest.InstantiateQuestStep(this.transform);
        ChangeQuestState(quest.questInforSO._id, QuestState.IN_PROGRESS);
    }
    void FinishQuest(string id)
    {
       Quest quest = GetQuestByID(id);
         ChangeQuestState(quest.questInforSO._id, QuestState.FINISHED);
        foreach (var child in questMap.Values)
        {
            if(child.questState.Equals( QuestState.REQUIREMENTS_NOT_MET) && CheckQuestCondition(child))
            {
                ChangeQuestState(child.questInforSO._id, QuestState.CAN_START);

            }
        }
    }
    private void QuestStepStateChanged(string id, QuestStepState questStepState)
    {
        Quest quest = GetQuestByID(id);
        quest.StoreQuestStepState(questStepState);
    }

   
    async Task<Dictionary<string, Quest>> CreateQuestMap()
    {
        questInforSOs = Resources.LoadAll<QuestInforSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        List<Quest>  quests = await LoadQuest();
        foreach (var quest in quests)
        {
            idToQuestMap[quest.questInforSO._id] = quest;
        }
        return idToQuestMap;
    }

    /// <summary>
    /// Lấy Quest theo id trong questMap
    /// </summary>
    /// <param name="questID"></param>
    /// <returns></returns>
    public Quest GetQuestByID(string questID)
    {
        Quest quest = null;
        questMap.TryGetValue(questID, out quest);
        return quest;
    }

    private async void OnDestroy()
    {
        try {
            await SaveQuest();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
      
    }

    async Task SaveQuest()
    {
        try
        {
            List<QuestData> questDatas = new List<QuestData> ();
            foreach (var child in questMap.Values)
            {
                questDatas.Add(child.GetQuestData());
            }
            string serilizeJson = JsonConvert.SerializeObject(questDatas);
            await Save_Load_Firebase.SaveData("Quest/QuestDatabase", serilizeJson);
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
     async Task<List<Quest>> LoadQuest()
    {
      
        List<Quest> quests = new();
        try
        {
            var snapshot = await Save_Load_Firebase.LoadData("Quest/QuestDatabase");
            if (snapshot.Exists)
            {
                Debug.Log("QuestDatabase is Exists");
                List<QuestData> questDatas = JsonConvert.DeserializeObject<List<QuestData>>(snapshot.Value.ToString());
                foreach (var child in questDatas)
                {
                    QuestInforSO infor = GetQuestInforSoByQuestID(child.questID);
                    Quest quest = new Quest(infor, child.questState, child.questStepState, child.isClaimedReward);
                    quests.Add(quest);
                }
            }
            else
            {
                Debug.Log("QuestDatabase is not exists");
                foreach (var info in questInforSOs)
                {
                    quests.Add(new Quest(info));
                }
            }
            return quests;
        }
        catch 
        {
            quests.Clear();
            foreach (var info in questInforSOs)
            {
                quests.Add(new Quest(info));
            }
            return quests;
        }

    }


    public QuestInforSO GetQuestInforSoByQuestID(string id)
    {
       return questInforSOs.ToList().Find(x => x._id == id);
    }

}
