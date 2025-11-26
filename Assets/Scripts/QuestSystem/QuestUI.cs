using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] Image backGroundState;
    [SerializeField] TextMeshProUGUI displayNameQuest;
    [SerializeField] TextMeshProUGUI descriptionQuest;
    [SerializeField] TextMeshProUGUI stateQuestStep;
    [SerializeField] Button inforButton;
    [SerializeField] Button rewardButton;
     QuestManger questManager;
    public RewardUI rewardUI;
     Quest quest;

    private void Start()
    {
        inforButton.onClick.AddListener(InforButton_Onclick);
        rewardButton.onClick.AddListener(RewardButton_Onclick);

    }
    private void OnDestroy()
    {
        inforButton.onClick.RemoveAllListeners();
        rewardButton.onClick.RemoveAllListeners();
    }

    public void Init(QuestManger manager, string id)
    {
        this.questManager = manager;
        quest = questManager.GetQuestByID(id);
    }

    void InforButton_Onclick()
    {
        if (quest == null) return;
        GameEventManager.Ins.questEvents.QuestInforClick(quest);
    }

    void RewardButton_Onclick()
    {
        if(quest == null) return;


        GameEventManager.Ins.questEvents.RewardClick(quest);
    }

    public void ChangedState(QuestState state)
    {

        if (state.Equals(QuestState.FINISHED))
        {
            Color color = Color.white;
           ColorUtility.TryParseHtmlString("#84FF00", out color);
            backGroundState.color = color;
        }
        else
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString("#FFA402", out color);
            backGroundState.color = color;
        }
    }
    public void ChangeStepState(QuestStepState questStepState)
    {
        if(questStepState  == null) return;
        stateQuestStep.text = questStepState.ToString();
    }

    public void SetPlaynameQuest(string nameText)
    {
        if (displayNameQuest != null)
        {
            displayNameQuest.text = nameText;
        }
    }

}
