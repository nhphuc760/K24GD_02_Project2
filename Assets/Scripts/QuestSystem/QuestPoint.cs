using UnityEngine;


[RequireComponent (typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    public bool PlayerIsNear { get; private set; }
    [Header("Quest")]
    [SerializeField] QuestInfoSO questInfoForPoint;
    string questID;
    QuestState currentState;
    private void Awake()
    {
        questID = questInfoForPoint.id;
    }
    private void OnEnable()
    {

        GameEventsManager.Ins.questEvent.onQuestStateChange += QuestStateChange;
        GameInput.Ins.submitPressed += SubmitPressed;

    }
    private void OnDisable()
    {
        GameEventsManager.Ins.questEvent.onQuestStateChange -= QuestStateChange;
        GameInput.Ins.submitPressed -= SubmitPressed;
    }


    void SubmitPressed()
    {
      
        if (!PlayerIsNear)
        {
            return;
        }
        if (currentState.Equals(QuestState.CAN_START))
        {
            GameEventsManager.Ins.questEvent.StartQuest(questID);
        }else if(currentState.Equals(QuestState.CAN_FINISH))
        {
            GameEventsManager.Ins.questEvent.FinishQuest(questID);
        }
       

    }
    void QuestStateChange(Quest quest)
    {
        if(quest.info.id.Equals(questID))
        {
            currentState = quest.state;
           
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerIsNear = true;
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerIsNear = false;
        }
      
    }
}
