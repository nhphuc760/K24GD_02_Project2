using UnityEngine;

public class CollectCoinQuestStep : QuestStep
{
    int coinsCollect = 0;
    int coinsToComplete = 5;
    private void OnEnable()
    {
        //Subcribe event OnCoinCollect;
    }
    private void OnDisable()
    {
        //Unsubcribe event;
    }

    void OnCoinCollected()
    {
        if(coinsCollect < coinsToComplete)
        {
            coinsCollect++;
            UpdateState();
        }
        if(coinsCollect >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }
    void UpdateState()
    {
        string state = coinsCollect.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.coinsCollect = int.Parse(state);
        UpdateState();
    }
}
