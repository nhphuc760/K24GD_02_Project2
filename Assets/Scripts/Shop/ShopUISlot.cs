using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _itemName;
    [SerializeField] TextMeshProUGUI purchase_Price;
    [SerializeField] GameObject multiBuy;
    [SerializeField] Button buy;
    [SerializeField] TMP_InputField quantityInput;
    [SerializeField] TextMeshProUGUI totalPrice;
    int slotIndex;
    ShopManager manager;
    int price;

    private void Start()
    {
        buy.onClick.AddListener(Buy);
    }
    public void Init(int index, ShopManager manager)
    {
        slotIndex = index;
        this.manager = manager;
    }
   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            manager.Buy(slotIndex, 1); 
        }else if (eventData.button.Equals(PointerEventData.InputButton.Right))
        {
            price = manager.GetPriceItem(slotIndex);
            totalPrice.text = price.ToString();
            multiBuy.SetActive(!multiBuy.activeSelf);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.ShopEvent_onPointerEnter(slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.ShopEvent_onPointerExit();
        multiBuy.SetActive(false);
    }

    public void UpdateUI(ItemShopDataSO item)
    {
        _icon.sprite = item._icon;
        _itemName.text = item._itemName;
        purchase_Price.text = item.purchase_Price.ToString();
    }
    public void Buy()
    {
        int quantity = int.Parse(quantityInput.text);
        if (quantity <= 0) return;
        manager.Buy(slotIndex, quantity);
    }
    public void InputMultiBuy(string quantity)
    {
        if(string.IsNullOrEmpty(quantity)) return;
        int _quantity = int.Parse(quantity);
        totalPrice.text = $"{_quantity * price}";
    }
}
