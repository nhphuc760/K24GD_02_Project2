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

    private void Awake()
    {
        GameEventManager.Ins.onCoinChange += OnCoinChange;
    }
    private void Start()
    {
        for (int i = 0; i< shopDataBaseSO.shopDatabase.Count; i++)
        {
            var item = Instantiate(slotPrefab, Container);
            item.Init(i, this);
            item.UpdateUI(shopDataBaseSO.shopDatabase[i]);
        }
     
        GameEventManager.Ins.shopEvent.onShow += Show;
        GameEventManager.Ins.shopEvent.onHide += Hide;
        GameEventManager.Ins.shopEvent.onPlayerEnterShop += PlayerEnterShop;
        GameEventManager.Ins.shopEvent.onPlayerExitShop += PlayerExitShop;
        Hide();

    }

    private void PlayerExitShop()
    {
        Hide();
    }

    private void PlayerEnterShop()
    {
        Show();
        GameEventManager.Ins.inventoryEvent.DisableInventory();
    }

    private void OnEnable()
    {
        GameEventManager.Ins.gameInput.DisableOpenBag();
    }
    private void OnDisable()
    {
        GameEventManager.Ins.gameInput.EnableOpenBag();
    }
    private void OnDestroy()
    {
        GameEventManager.Ins.onCoinChange -= OnCoinChange;
        GameEventManager.Ins.shopEvent.onShow -= Show;
        GameEventManager.Ins.shopEvent.onHide -= Hide;
        GameEventManager.Ins.shopEvent.onPlayerEnterShop -= PlayerEnterShop;
        GameEventManager.Ins.shopEvent.onPlayerExitShop -= PlayerExitShop;
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
        int price = itemShopDataSO.purchase_Price * quantity;
        if (GameManager.Ins == null) return;
        if (GameManager.Ins.Coin < price)
        {
            GameEventManager.Ins.TriggerDialog($"<color=red>Không đủ tiền để mua, còn thiếu {price - GameManager.Ins.Coin}</color>");
            return;
        }
        // kiểm tra loại item mua, thêm vào inven hoặc gọi logic riêng
        if(itemShopDataSO.typeItemShop == ItemShopDataSO.TypeItemShop.AddInven){
            if (GameEventManager.Ins.inventoryEvent.AddItem(itemShopDataSO, quantity))
            {
                GameManager.Ins.Coin -= price;
            }
        }
        else if(itemShopDataSO.typeItemShop == ItemShopDataSO.TypeItemShop.Separate && GameEventManager.Ins.CheckConDition(itemShopDataSO as AnimalDataSO, quantity))
        {
            GameManager.Ins.Coin -= price;
            GameEventManager.Ins.Separate(itemShopDataSO as AnimalDataSO, quantity);
        }
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
