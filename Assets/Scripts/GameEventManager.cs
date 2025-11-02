using System;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Ins;
    public event Action OnCollectCoins;//cho vào questEvent
    public event Action OnFishingCaught;//
    public event Action<string> triggerDialog;
    public event Action<int> onCoinChange;

    public QuestEvents questEvents;
    public CookingEvent cookingEvent;
    public InventoryEvent inventoryEvent;
    public ShopEvent shopEvent;
    public ToolKitEvent toolKitEvent;
    public GameInput gameInput;
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
        shopEvent = new ShopEvent();
        toolKitEvent = new ToolKitEvent();
        gameInput = new GameInput();
    }

    private void Start()
    {
        gameInput.Init();
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
    public void CoinChange(int coin)
    {
        onCoinChange?.Invoke(coin);
    }
    public void TriggerDialog(string log)
    {
        triggerDialog?.Invoke(log);
    }
 

}
