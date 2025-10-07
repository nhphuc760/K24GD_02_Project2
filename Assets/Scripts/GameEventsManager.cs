using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Ins { get; private set; }
    public QuestEvent questEvent;
    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }

        questEvent = new QuestEvent();
    }
}
