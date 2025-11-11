using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
  
    public InventorySlotUI slotUIPrefab;
    public Inventory inventory;
    public int slotAmount = 20;
    public Transform itemSlotContainer;
    public InventoryDataBaseSO itemDataBase;
    [SerializeField] ItemInforUI itemInforUI;
    bool isLoaded = false;

    List<InventorySlotUI> cachedSlotUIs = new List<InventorySlotUI>();
    private async void Awake()
    {

        if(itemDataBase == null)
        {
            itemDataBase = Resources.Load<InventoryDataBaseSO>("Items/InventoryDataBase");
        }
        if (itemDataBase == null)
        {
            Debug.LogWarning("InventoryManager.Awake: itemDataBase is null after Resources.Load. Make sure Resources/Items/InventoryDataBase exists.");
        }

        isLoaded = false;

        var slotSnapshot = await Save_Load_Firebase.LoadData("Inventory/InventoryOfPlayer/SlotAmount");
        if (slotSnapshot != null && slotSnapshot.Exists)
        {
            int parsed;
            if (int.TryParse(slotSnapshot.Value.ToString(), out parsed))
                slotAmount = parsed;
            else
                Debug.LogWarning("InventoryManager.Awake: failed to parse SlotAmount, using default slotAmount.");
        }
        // create inventory even if itemDataBase is null — Inventory handles itemDataBase null safely now
        inventory = new Inventory(slotAmount, itemDataBase);

        // load saved slots with try/catch to catch any runtime errors and log them
        try
        {
            await inventory.LoadData("InventoryOfPlayer");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InventoryManager.Awake: inventory.LoadData threw exception: {e}");
        }

        isLoaded = true;

        inventory.OnExpandSlot += () => {
            var slotUI = Instantiate(slotUIPrefab, itemSlotContainer);
            slotUI.Init(this, cachedSlotUIs.Count);
            cachedSlotUIs.Add(slotUI);
        };
        for (int i = 0; i < slotAmount; i++)
        {
            var slotUI = Instantiate(slotUIPrefab, itemSlotContainer);
            slotUI.Init(this, i);
            cachedSlotUIs.Add(slotUI);
        }
        UpdateUI();
        GameEventManager.Ins.questEvents.onClaimReward += QuestEvents_onClaimReward;
        GameEventManager.Ins.inventoryEvent.opendBagPressed+= Ins_openBagPressed;
        GameEventManager.Ins.toolKitEvent.InitSuccess(inventory);
        gameObject.SetActive(false);
    }

   

    private void Start()
    {
        GameEventManager.Ins.cookingEvent.onGetTotalItem += GetTotalItem;
        GameEventManager.Ins.cookingEvent.onGetItem += CookingEvent_onGetItem;
        GameEventManager.Ins.inventoryEvent.checkHasItem += CheckHasItem;
        GameEventManager.Ins.inventoryEvent.onRemoveItemClick += RemoveItem;
        GameEventManager.Ins.inventoryEvent.onRemoveItemCompleted += OnRemoveItemCompleted;
        GameEventManager.Ins.inventoryEvent.onRemoveItemByData += RemoveItemByData;
        GameEventManager.Ins.inventoryEvent.onAddItem += AddItem;
        GameEventManager.Ins.inventoryEvent.onDropItem += OnDropItem;

    }

    private void Ins_openBagPressed()
    {
        if(inventory == null)
        {
            Debug.Log("Inventory null");
        }
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            UpdateUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem(Resources.Load<ItemDataSO>("Items/ConsumpItem_1/TestItem"), 1);// Giả lập thêm Item
         
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
           AddItem(Resources.Load<ItemDataSO>("Items/ConsumpItem_1/TestItem2"), 2); //Giả lập thêm item
     
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddItem(Resources.Load<SeedDataSO>("Items/PlantData/SeedData_4/Seed_Carrot"), 10);
        }
    }
    private async void OnDisable()
    {
        if(isLoaded)
         await inventory.SaveData("InventoryOfPlayer");
        if(itemInforUI != null)
        {
            itemInforUI.gameObject.SetActive(false);
        }
       
    }
    private async void OnDestroy()
    {
        //GameInput.Ins.openBagPressed -= Ins_openBagPressed;
        GameEventManager.Ins.inventoryEvent.opendBagPressed -= Ins_openBagPressed;
        GameEventManager.Ins.questEvents.onClaimReward -= QuestEvents_onClaimReward;
        GameEventManager.Ins.inventoryEvent.onRemoveItemClick -= RemoveItem;
        GameEventManager.Ins.inventoryEvent.onRemoveItemCompleted -= OnRemoveItemCompleted;
        GameEventManager.Ins.cookingEvent.onGetTotalItem -= GetTotalItem;
        GameEventManager.Ins.cookingEvent.onGetItem -= CookingEvent_onGetItem;
        GameEventManager.Ins.inventoryEvent.checkHasItem -= CheckHasItem;
        GameEventManager.Ins.inventoryEvent.onRemoveItemByData -= RemoveItemByData;
        GameEventManager.Ins.inventoryEvent.onAddItem -= AddItem;
        GameEventManager.Ins.inventoryEvent.onDropItem -= OnDropItem;
        await inventory.SaveData("InventoryOfPlayer");
    }

    private void OnValidate()
    {
        itemDataBase = Resources.Load<InventoryDataBaseSO>("Items/InventoryDataBase");
    }

  
    public void OnDropItem(PointerEventData eventData)
    {
       
        if(!eventData.pointerDrag.TryGetComponent<IDragDrop>(out IDragDrop start) || !eventData.pointerEnter.TryGetComponent<IDragDrop>(out IDragDrop end))
        {
            return;
        }
        if (start.GetIndex() == end.GetIndex())
        {
            return;
        }
        inventory.MergeItem(inventory.itemSlots[start.GetIndex()], inventory.itemSlots[end.GetIndex()]);
        UpdateUI();
      
    }



    public void OnPointerClickSlotUI(int indexSlotUI)
    {
        if(!inventory.itemSlots[indexSlotUI].IsEmpty)
        {
            ItemDataSO itemData = inventory.itemSlots[indexSlotUI].ItemData;     
            itemInforUI.UpdateUI(itemData);
            itemInforUI.gameObject.SetActive(true);
            itemInforUI.SlotIndex = indexSlotUI;
        }    

    }

   

    public bool AddItem(ItemDataSO itemDataSO, int quantity = 1)
    {
        if(itemDataSO == null)
        {
            Debug.Log($"ItemdataSo is null");
            return false;
        }
        Debug.Log($"ItemDataSO {itemDataSO.name}");
        if (inventory.AddItem(itemDataSO, quantity))
        {
            GameEventManager.Ins.TriggerDialog($"<color=green>{itemDataSO._itemName} đã được thêm vào kho đồ của bạn</color>");
            UpdateUI();
            return true;
        }
        else
        {
            GameEventManager.Ins.TriggerDialog("<color=red>Vui lòng kiểm tra kho đồ, hiện tại không thể thêm vật phẩm</color>");
            return false;
        }
        
    }



    public void UpdateUI()
    {
        foreach (var slot in cachedSlotUIs)
        {
            slot.UpdateUI();
        }
        GameEventManager.Ins.toolKitEvent.UpdateUI();
    }



    private void QuestEvents_onClaimReward(Quest quest)
    {
        if (quest == null || quest.questInforSO == null || quest.questInforSO.reward == null || quest.questInforSO.reward.items == null)
            return;

        RewardData reward = quest.questInforSO.reward;
        foreach (var item in reward.items)
        {
            if (item != null && item.itemSO != null && item.quantity > 0)
            {
                AddItem(item.itemSO, item.quantity);
            }
        }
    }

    void RemoveItem(int slotIndex, int quantity)
    {
        inventory.RemoveItem(slotIndex, quantity);
        UpdateUI();

    }

    void RemoveItemByData(ItemDataSO item, int quantity)
    {
        inventory.RemoveItem(item, quantity);
        UpdateUI();
    }

    void OnRemoveItemCompleted(bool isActiveFalse) {
        if (isActiveFalse)
        {
            itemInforUI.gameObject.SetActive(false);
        }
    }

    private int GetTotalItem(ItemDataSO itemData)
    {
        return inventory.GetTotal(itemData);
    }
    private void CookingEvent_onGetItem(ItemDataSO itemDataSO, int quantity)
    {
        AddItem(itemDataSO, quantity);
    }

    bool CheckHasItem(ItemDataSO itemDataSO, int quantity)
    {
        return inventory.HasItem(itemDataSO, quantity);
    }
}
