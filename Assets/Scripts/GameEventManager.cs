using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Ins;
    public event Action OnCollectCoins;//cho vào questEvent
    public event Action OnFishingCaught;//
    public event Action<string> triggerDialog;
    public event Action<int> onCoinChange;
    //FishingEvent: Lười tạo script mới nên để luôn ở đây
    public event Action onFishingZoneEnter;
    public event Action onFishingZoneExit;

    public QuestEvents questEvents;
    public CookingEvent cookingEvent;
    public InventoryEvent inventoryEvent;
    public ShopEvent shopEvent;
    public ToolKitEvent toolKitEvent;
    public GameInput gameInput;
    public AnimationEvent animationEvent;
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
        animationEvent = new AnimationEvent();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        gameInput.Init();
        SceneManager.sceneLoaded += OnSceneLoad;

    }
    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        string path = arg0.path;

        if(path.StartsWith("Assets/Scenes/ScenePlay"))
        {
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
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
    

    public void FishingZoneEnter()
    {
        onFishingZoneEnter?.Invoke();
    }
    public void FishingZoneExit()
    {
        onFishingZoneExit?.Invoke();
    }

}
