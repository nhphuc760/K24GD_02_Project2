using System;
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

    public List<InventorySlotUI> cachedSlotUIs = new List<InventorySlotUI>();
    List<InventorySlotUI> cachedSlotUIBackUP = new List<InventorySlotUI>();
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
        cachedSlotUIBackUP = cachedSlotUIs;
        UpdateUI();
        GameEventManager.Ins.questEvents.onClaimReward += QuestEvents_onClaimReward;
        GameEventManager.Ins.inventoryEvent.opendBagPressed+= Ins_openBagPressed;
        GameEventManager.Ins.toolKitEvent.InitSuccess(inventory);
        gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        GameEventManager.Ins.gameInput.DisableInterac();
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
        GameEventManager.Ins.inventoryEvent.onGetInventory += GetDataInventory;
        GameEventManager.Ins.inventoryEvent.onGetItemSOByID += GetItemDataSOByID;
        GameEventManager.Ins.inventoryEvent.onGetIndexOfSlot += GetIndexOfSlot;
        GameEventManager.Ins.inventoryEvent.onGetBaitData += OnGetBaitData;
        GameEventManager.Ins.inventoryEvent.onDisableInventory += DisableSelf;
    }

    private InventoryManager GetDataInventory()
    {
        return this;
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

    private async void OnDisable()
    {
        if(isLoaded)
         await inventory.SaveData("InventoryOfPlayer");
        if(itemInforUI != null)
        {
            itemInforUI.gameObject.SetActive(false);
        }
        if(GameEventManager.Ins != null)
        GameEventManager.Ins.gameInput.EnableInterac();
       
    }


    private async void OnDestroy()
    {
        //GameInput.Ins.openBagPressed -= Ins_openBagPressed;
        if (GameEventManager.Ins != null)
        {
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
            GameEventManager.Ins.inventoryEvent.onGetInventory -= GetDataInventory;
            GameEventManager.Ins.inventoryEvent.onGetItemSOByID -= GetItemDataSOByID;
            GameEventManager.Ins.inventoryEvent.onGetIndexOfSlot -= GetIndexOfSlot;
            GameEventManager.Ins.inventoryEvent.onDisableInventory -= DisableSelf;
            GameEventManager.Ins.inventoryEvent.onGetBaitData -= OnGetBaitData;
        }
        await inventory.SaveData("InventoryOfPlayer");
    }

    private void DisableSelf()
    {
       gameObject.SetActive(false);
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
        if (start.GetIndexSlot() == end.GetIndexSlot())
        {
            return;
        }
        if(start.GetIndexSlot() < 0)
        {
            return;
        }
        if (end.GetIndexSlot() >= 0)
        inventory.MergeItem(inventory.itemSlots[start.GetIndexSlot()], inventory.itemSlots[end.GetIndexSlot()]);
        else
            end.SetInventorySlot(inventory.itemSlots[start.GetIndexSlot()]);
        UpdateUI();
      
    }

    public int GetIndexOfSlot(InventorySlot slot)
    {
        return inventory.itemSlots.IndexOf(slot);
    }

    public void OnPointerClickSlotUI(int indexSlotUI)
    {
        if(this.gameObject.activeSelf == false)
        {
            return;
        }
        if (!inventory.itemSlots[indexSlotUI].IsEmpty)
        {   
            itemInforUI.UpdateUI(inventory.itemSlots[indexSlotUI]);
            itemInforUI.gameObject.SetActive(true);
            itemInforUI.SlotIndex = indexSlotUI;
        }    

    }

   

    public bool AddItem(ItemDataSO itemDataSO, int quantity = 1, DataRunTimeItem dataRunTimeItem = null, bool isDialog = true)
    {
        if(itemDataSO == null)
        {
            Debug.Log($"ItemdataSo is null");
            return false;
        }
        Debug.Log($"ItemDataSO {itemDataSO.name}");
        if (inventory.AddItem(itemDataSO, quantity, dataRunTimeItem))
        {
            if(isDialog)
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
            GameEventManager.Ins.gameInput.Enable_InputAction();
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
    public void SetCachedSlotUI(List<InventorySlotUI> slotUIS)
    {
        cachedSlotUIs = slotUIS;
    }

    public void RestoreDataBackup()
    {
        cachedSlotUIs = cachedSlotUIBackUP;
    }

    public ItemDataSO GetItemDataSOByID(int id)
    {
        return itemDataBase.GetDataByID(id);
    }
   BaitDataSO OnGetBaitData()
    {
        InventorySlot slot = inventory.itemSlots.Find(x => x.ItemData is BaitDataSO);
        if (slot == null) return null;
        if(slot.ItemData != null)
        {
            return slot.ItemData as BaitDataSO;
        }
        return null;  
    }
}
