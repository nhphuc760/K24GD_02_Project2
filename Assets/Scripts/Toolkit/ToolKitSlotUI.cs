using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolKitSlotUI : MonoBehaviour, IDragDrop, IPointerClickHandler
{
    int slotIndex;
    ToolKit toolKitManager;
    [SerializeField] Image _icon;
    [SerializeField] Image _selectCursor;
    [SerializeField] TextMeshProUGUI _quantityText;
    Image dragIcon;

    private void Start()
    {
        
    }

    public void Init(int index, ToolKit toolKit)
    {
        this.slotIndex = index;
        this.toolKitManager = toolKit;
    }
    public ItemDataSO GetItemDataSO()
    {
        return null;
    }
    public void OnDrag(PointerEventData eventData) // Ham callback unity, duoc goi khi keo
    {
        if (toolKitManager.inventory.itemSlots[slotIndex].IsEmpty) return;
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;


    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = toolKitManager.inventory.itemSlots[slotIndex];
        if (slot.IsEmpty) return;
        dragIcon = new GameObject("DragIcon").AddComponent<Image>();
        dragIcon.sprite = slot.ItemData._icon;
        dragIcon.color = Color.white;
        dragIcon.raycastTarget = false;
        dragIcon.transform.SetParent(toolKitManager.transform);
        dragIcon.rectTransform.sizeDelta = new Vector2(64, 64);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (dragIcon != null)
            Destroy(dragIcon.gameObject);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        toolKitManager.OnPointerClickSlotUI(slotIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {

        GameEventManager.Ins.inventoryEvent.DropItem(eventData);
    }




    public void UpdateUI()
    {
        InventorySlot slot = toolKitManager.inventory.itemSlots[slotIndex];
        if (slot.IsEmpty)
        {
            _icon.enabled = false;

            _quantityText.text = "";
        }
        else
        {

            _icon.enabled = true;
            _icon.sprite = slot.ItemData._icon;

            _quantityText.text = slot.ItemData.isStackable ? slot.quantity.ToString() : "";
        }
    }
    
    public void DisableSelected()
    {
        _selectCursor.gameObject.SetActive(false);
    }
    public void EnableSelected()
    {
        _selectCursor.gameObject.SetActive(true);
    }

    public int GetIndex()
    {
       return slotIndex;
    }
}
   
