using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class KitchenInventoryManager : MonoBehaviour
{
    public Inventory inventory;
    [SerializeField] KitchenSlotUI kitchenSlotUI;
    [SerializeField] Transform container;
    public List<KitchenItemDataSO> kitchenItemList;
    [SerializeField] SlotInforUI slotInforUI;
    [SerializeField] GameObject DialogCookingChange;
    private void Awake()
    {

        inventory = new Inventory(kitchenItemList.Count, null);
        for(int i = 0; i < kitchenItemList.Count; i++)
        {
            var obj = Instantiate(kitchenSlotUI, container);
            inventory.AddItem(kitchenItemList[i]);
            obj.Init(i, this); 
        }
    }

    private void OnEnable()
    {
        GameEventManager.Ins.gameInput.DisableOpenBag();
    }
    void Start()
    {
        GameEventManager.Ins.cookingEvent.onPlayerEnterKitchen += PlayerEnterKitchen;
        GameEventManager.Ins.cookingEvent.onPlayerLeaveKitchen += PlayerLeaveKitchen;
        GameEventManager.Ins.cookingEvent.onPointerClickSlotUI += slotInforUI.PointerClickSlotUI;
        GameEventManager.Ins.cookingEvent.onStartCook += StartCooking;
        gameObject.SetActive(false);
    }

    private void StartCooking()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEventManager.Ins.cookingEvent.onPlayerEnterKitchen -= PlayerEnterKitchen;
        GameEventManager.Ins.cookingEvent.onPlayerLeaveKitchen -= PlayerLeaveKitchen;
        GameEventManager.Ins.cookingEvent.onPointerClickSlotUI -= slotInforUI.PointerClickSlotUI;
        GameEventManager.Ins.cookingEvent.onStartCook -= StartCooking;
    }
    private void OnDisable()
    {
        slotInforUI.gameObject.SetActive(false);
        DialogCookingChange.SetActive(false);
        GameEventManager.Ins.gameInput.EnableOpenBag();
    }
    private void PlayerLeaveKitchen()
    {
       gameObject.SetActive(false);
    }

    private void PlayerEnterKitchen()
    {
       gameObject.SetActive(true);
    }

    private void OnValidate()
    {
        kitchenItemList = Resources.LoadAll<KitchenItemDataSO>("Items").ToList();
    }
}
