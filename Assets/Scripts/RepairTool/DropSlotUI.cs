using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlotUI : MonoBehaviour, IDragDrop
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI calMoney;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI durability;
    InventorySlot curSlot;
    Image dragIcon;
    public InventorySlot GetInventorySlot()
    {
        return curSlot;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
      
        if (curSlot == null || curSlot.IsEmpty) return;
        dragIcon = new GameObject("DragIcon").AddComponent<Image>();
        dragIcon.sprite = curSlot.ItemData._icon;
        dragIcon.color = Color.white;
        dragIcon.raycastTarget = false;
        dragIcon.transform.SetParent(transform.parent.parent);
        dragIcon.rectTransform.sizeDelta = new Vector2(64, 64);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (curSlot == null || curSlot.IsEmpty) return;
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        try
        {
            if(eventData.pointerDrag.TryGetComponent<IDragDrop>(out IDragDrop start) && eventData.pointerEnter.TryGetComponent<IDragDrop>(out IDragDrop end))
            {
                if(start.GetInventorySlot() == end.GetInventorySlot())
                {
                    return;
                }

               GameEventManager.Ins.inventoryEvent.DropItem(eventData);
                if (end.GetInventorySlot().IsEmpty)
                {
                    curSlot = null;
                    UpdateUI();
                }
            }
        }
        catch { 

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon.gameObject);
    }

    void UpdateUI()
    {
        if(curSlot == null || curSlot.IsEmpty)
        {
         _icon.enabled = false;
            title.enabled = true;
            calMoney.enabled = false;
        }
        else
        {
           _icon.enabled = true;
            _icon.sprite = curSlot.ItemData._icon;
            title.enabled = false;
        }
    }

    public void OnValueSliderChange(float value)
    {

    }

    public void SetInventorySlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            curSlot = null;
            UpdateUI();
        }
        else if(slot.ItemData as ToolDataSO)
        {
            curSlot = slot; 
            UpdateUI();
        }
    }
}
