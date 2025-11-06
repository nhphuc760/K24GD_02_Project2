using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KitchenSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image _icon;
    public int slotIndex;
    KitchenInventoryManager kitchenInventory;
   public void Init(int slotIndex, KitchenInventoryManager kitchenInventoryManager)
    {
        this.slotIndex = slotIndex;
        this.kitchenInventory = kitchenInventoryManager;
        UpdateUI();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEventManager.Ins.cookingEvent.PointerClickSlotUI(slotIndex);
    }
    public void UpdateUI()
    {
        var itemSlot = kitchenInventory.inventory.itemSlots[slotIndex];
        if (itemSlot.IsEmpty)
        {
            _icon.enabled = false;
        }
        else
        {
            _icon.enabled = true;
            _icon.sprite = itemSlot.ItemData._icon;
        }
    }
}
