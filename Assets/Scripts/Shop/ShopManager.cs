using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopDataBaseSO shopDataBaseSO;
    [SerializeField] ShopUISlot slotPrefab;
    [SerializeField] Transform Container;
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
        GameEventManager.Ins.shopEvent.onShow += Show;
        GameEventManager.Ins.shopEvent.onHide += Hide;
        Hide();

    }

    private void OnDestroy()
    {
        GameEventManager.Ins.onCoinChange -= OnCoinChange;
        GameEventManager.Ins.shopEvent.onShow -= Show;
        GameEventManager.Ins.shopEvent.onHide -= Hide;
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
        if (GameManager.Ins == null) return;
        if (GameManager.Ins.Coin < price)
        {
            GameEventManager.Ins.TriggerDialog($"<color=red>Không đủ tiền để mua, còn thiếu {price - GameManager.Ins.Coin}</color>");
            return;
        }
        if(GameEventManager.Ins.inventoryEvent.AddItem(itemShopDataSO, quantity))
        {
            GameManager.Ins.Coin -= itemShopDataSO.purchase_Price;
        }
    }
    public void Sell()
    {

    }

    void Show()
    {
        gameObject.SetActive(true);
    }
    void Hide()
    {
        gameObject.SetActive(false);
    }
}
