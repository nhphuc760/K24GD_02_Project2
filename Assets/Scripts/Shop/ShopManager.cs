using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopDataBaseSO shopDataBaseSO;
    [SerializeField] ShopUISlot slotPrefab;
    [SerializeField] Transform Container;
    [SerializeField] Canvas Canvas;
    [SerializeField] TextMeshProUGUI coinText;
    
    private void Start()
    {
        for (int i = 0; i< shopDataBaseSO.shopDatabase.Count; i++)
        {
            var item = Instantiate(slotPrefab, Container);
            item.Init(i, this);
            item.UpdateUI(shopDataBaseSO.shopDatabase[i]);
        }
        GameEventManager.Ins.onCoinChange += OnCoinChange;
       
    }

    private void OnDestroy()
    {
        GameEventManager.Ins.onCoinChange -= OnCoinChange;
    }

    private void OnCoinChange(int coin)
    {
        coinText.text = coin.ToString();
    }

    public void ShopEvent_onPointerExit()
    {
        GameEventManager.Ins.shopEvent.PointerExit();
    }

    public void ShopEvent_onPointerEnter(int index)
    {
     
        ItemShopDataSO shopData = shopDataBaseSO.shopDatabase[index];
        GameEventManager.Ins.shopEvent.ShowToolTip(shopData._description);
    }

    public void Buy(int slotIndex, int quantity = 1)
    {
        ItemShopDataSO itemShopData = shopDataBaseSO.shopDatabase[slotIndex];
        Buy(itemShopData, quantity);  
    }
    public void Buy(ItemShopDataSO itemShopDataSO, int quantity = 1)
    {
        int price = itemShopDataSO.purchase_Price;
        if(GameEventManager.Ins.inventoryEvent.AddItem(itemShopDataSO, quantity))
        {
            GameManager.Ins.Coin -= itemShopDataSO.purchase_Price;
        }
    }
    public void Sell()
    {

    }
}
