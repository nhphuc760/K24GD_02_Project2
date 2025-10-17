using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventorySlotUI slotUIPrefab;
    public Inventory inventory;
    public int slotAmount = 20;
    public Transform itemSlotContainer;
    public InventoryDataBaseSO itemDataBase;
    [SerializeField] ItemInforUI itemInforUI;
    DatabaseReference reference;

    List<InventorySlotUI> cachedSlotUIs = new List<InventorySlotUI>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //Lay so luong slot tu firebase
        reference.Child(Save_Load_Firebase.GetUserID()).Child("Inventory/InventoryOfPlayer/SlotAmount").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve inventory data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                   slotAmount = int.Parse(snapshot.Value.ToString());
                 
                }
                
            }
        });

        inventory = new Inventory(slotAmount, itemDataBase);
        inventory.OnExpandSlot += () =>
        {
            var slotUI = Instantiate(slotUIPrefab, itemSlotContainer);
            slotUI.Init(this, cachedSlotUIs.Count);
            cachedSlotUIs.Add(slotUI);
        };
    }
    private void Start()
    {
        //khoi tao slot UI
        for (int i = 0; i < slotAmount; i++)
        {
            var slotUI = Instantiate(slotUIPrefab, itemSlotContainer);
            slotUI.Init(this, i);
            cachedSlotUIs.Add(slotUI);

        }
        inventory.LoadData("InventoryOfPlayer");
        GameInput.Ins.openBagPressed += () =>
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
            {
                UpdateUI();
            }
           
        };
        gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.AddItem(Resources.Load<ItemDataSO>("Items/ConsumpItem/TestItem"), 1);
            UpdateUI();
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            inventory.AddItem(Resources.Load<ItemDataSO>("Items/ConsumpItem/TestItem2"), 2);
            UpdateUI();
        }
    }
    private async void OnDisable()
    {
        await inventory.SaveData("InventoryOfPlayer");
    }

    public void UpdateUI()
    {
        foreach (var slot in cachedSlotUIs)
        {
            slot.UpdateUI();
        }
    }

    public void OnDropItem(PointerEventData eventData)
    {
       
        if(!eventData.pointerDrag.TryGetComponent<InventorySlotUI>(out InventorySlotUI start) || !eventData.pointerEnter.TryGetComponent<InventorySlotUI>(out InventorySlotUI end))
        {
            return;
        }
        inventory.MergeItem(inventory.itemSlots[start.slotIndex], inventory.itemSlots[end.slotIndex]);
        UpdateUI();
    }

    private void OnValidate()
    {
        itemDataBase = Resources.Load<InventoryDataBaseSO>("Items/InventoryDataBase");
    }


    public void OnPointerClickSlotUI(int indexSlotUI)
    {
        if(!inventory.itemSlots[indexSlotUI].IsEmpty)
        {
            ItemDataSO itemData = inventory.itemSlots[indexSlotUI].ItemData;     
            itemInforUI.UpdateUI(itemData);
            itemInforUI.gameObject.SetActive(true);
            
        }

        

    }

}
