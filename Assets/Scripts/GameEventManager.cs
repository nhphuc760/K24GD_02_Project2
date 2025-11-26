using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Ins;
   
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
    public AnimalEvent animalEvent;


    //Các phần event dưới đây sẽ tự động gọi theo diễn biến trò chơi, ko quan tâm ai subcribe
    public event Action<SeedDataSO> onPlanting;//cho vào questEvent
    public event Action<FishDataSO> onFishing;//
    public event Action<CropDataSO> onHarvest;
    public event Action<ToolDataSO> onRepairTool;
    public event Action<ResourceSO> onCutDTree;
    public event Action<ResourceSO> onMiningOre;
    public event Func<AnimalDataSO, int, bool> onCheckConDition;
    public event Action<bool> onNearSell;

    private void Awake()
    {
        if (Ins != null && Ins != this)
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
        animalEvent = new AnimalEvent();
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnNearSell(bool value)
    {
        onNearSell?.Invoke(value);
    }
    public bool CheckConDition(AnimalDataSO animalDataSO, int quantity)
    {
        return onCheckConDition?.Invoke(animalDataSO, quantity) ?? false;
    }
    public event Action<AnimalDataSO, int> onSeparate;
    public void Separate(AnimalDataSO itemData, int quantity)
    {
        onSeparate?.Invoke(itemData, quantity);
    }
    public void Planting(SeedDataSO seedDataSO)
    {
        onPlanting?.Invoke(seedDataSO);
    }
    public void OnFishing(FishDataSO sO)
    {
        onFishing?.Invoke(sO);
    }
    public void HarvestCrop(CropDataSO cropDataSO)
    {
        onHarvest?.Invoke(cropDataSO);
    }
    public void OnRepairTool(ToolDataSO tool)
    {
        onRepairTool?.Invoke(tool);
    }
    public void CutDownTree(ResourceSO rO)
    {
        onCutDTree?.Invoke(rO);
    }
    public void MiningOre(ResourceSO rO)
    {
        onMiningOre?.Invoke(rO);
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
