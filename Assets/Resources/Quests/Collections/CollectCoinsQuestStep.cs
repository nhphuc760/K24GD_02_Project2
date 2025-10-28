using System;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    public int coinCollected = 0;
    public int coinsToCompleted = 10;
    private void Start()
    {
        
        GameEventManager.Ins.OnCollectCoins += OnCoinCollected;
    }

    private void OnDisable()
    {
        GameEventManager.Ins.OnCollectCoins -= OnCoinCollected;
    }


    private void OnCoinCollected()
    {
        if (coinCollected < coinsToCompleted)
        {
            coinCollected++;
            UpdateState();

        }
        if (coinCollected >= coinsToCompleted)
        {
            FinishQuestStep();
        }
    }


    void UpdateState()
    {
     
        ChangeState(new QuestStepState { current = coinCollected, target = coinsToCompleted});
    }
    protected override void SetQuestStepState(QuestStepState newState)
    {
        coinCollected = newState.current;
        UpdateState();
    }
}
