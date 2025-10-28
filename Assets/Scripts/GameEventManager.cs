using System;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Ins;
    public event Action OnCollectCoins;//cho vào questEvent
    public event Action OnFishingCaught;//
    public event Action<string> triggerDialog;


    public QuestEvents questEvents;
    public CookingEvent cookingEvent;
    public InventoryEvent inventoryEvent;
    private void Awake()
    {
       if(Ins != null && Ins != this)
        {
            Destroy(this.gameObject);
        }
        Ins = this;
        questEvents = new QuestEvents();
        cookingEvent = new CookingEvent();
        inventoryEvent = new InventoryEvent();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    if (OnCollectCoins != null)
        //    {
        //        OnCollectCoins();
        //    }
        //    else {
        //        Debug.Log("OnCollectCoins is not assgin");
        //    }

        //}
        //if(Input.GetKeyDown(KeyCode.W)) { 
        //    OnFishingCaught?.Invoke();
        //}
    }

    public void TriggerDialog(string log)
    {
        triggerDialog?.Invoke(log);
    }
 

}
