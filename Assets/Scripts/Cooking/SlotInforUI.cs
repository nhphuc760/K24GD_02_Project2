using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotInforUI : MonoBehaviour
{
    [SerializeField] KitchenInventoryManager kitchenInventoryManager;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image _icon;
    [SerializeField] Button cook;
    [SerializeField] Transform ingredientContainer;
    [SerializeField] RequireItemSlotUI _requireItemSlotUI;
    [SerializeField] RectTransform _kitchenPanel;
    KitchenItemDataSO curkitchenItemSO;
    void Start()
    {
        cook.onClick.AddListener(() => { 
            GameEventManager.Ins.cookingEvent.OnCookClick(curkitchenItemSO.recipeSO); 
        });
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        cook.onClick.RemoveAllListeners();
    }
    private void OnDisable()
    {
        curkitchenItemSO = null;
        if(GameEventManager.Ins != null)
        GameEventManager.Ins.shopEvent.HideToolTip();
    }

    public void PointerClickSlotUI(int index)
    {
          
        if(index >= kitchenInventoryManager.kitchenItemList.Count)
        {
            Debug.Log(index + "out of range kitchenItemList");
            return;
        }
        KitchenItemDataSO kitchenItem = kitchenInventoryManager.kitchenItemList[index];
        if(kitchenItem != curkitchenItemSO)
        {
            curkitchenItemSO = kitchenItem;
            UpdateUI();
        }
        Debug.Log("SetActiveSlotInfor true");
        gameObject.SetActive(true);
    }

    void UpdateUI()
    {
        if (curkitchenItemSO == null) return;
        _nameText.text = curkitchenItemSO._itemName;
        _icon.sprite = curkitchenItemSO._icon;
        description.text = curkitchenItemSO._description;
        ClearAllChildrent(ingredientContainer);
        foreach (var child in curkitchenItemSO.recipeSO.ingredients)
        {
            int curQty = GameEventManager.Ins.cookingEvent.GetTotalItem(child.itemSO);
            string text = "";
            if(curQty >= child.quantity)
            {
                text = $"<color=green>{curQty}/{child.quantity}</color>";
            }
            else
            {
                text = $"<color=red>{curQty}/{child.quantity}</color>";
            }
            RequireItemSlotUI requireSlot = Instantiate(_requireItemSlotUI, ingredientContainer);
            requireSlot.Init(child.itemSO._itemName, _kitchenPanel);
            requireSlot.UpdateUI(child.itemSO._icon, text);
         
        }
    }

    public void ClearAllChildrent(Transform parent)
    {
      
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

    }
}
