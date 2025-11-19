using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IDragDrop, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
   public Image backGround;
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _quantityText;
    Image dragIcon;
    InventoryManager inventoryManager;
    public int slotIndex;
    Transform parentIcon;
    public void Init(InventoryManager manager, int index)
    {
        inventoryManager = manager;
        slotIndex = index;
        UpdateUI();
    }
    public void UpdateUI()
    {
        var itemSlot = inventoryManager.inventory.itemSlots[slotIndex];
        if (itemSlot.IsEmpty)
        {
            _icon.enabled = false;
          
            _quantityText.text = "";
        }
        else
        {

            _icon.enabled = true;
            _icon.sprite = itemSlot.ItemData._icon;
          
            _quantityText.text = itemSlot.ItemData.isStackable ? itemSlot.quantity.ToString() : "";
        }

    }

    public void OnDrag(PointerEventData eventData) // Ham callback unity, duoc goi khi keo
    {
        if (inventoryManager.inventory.itemSlots[slotIndex].IsEmpty) return;
        if (dragIcon != null)
            dragIcon.transform.position = eventData.position;
      

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = inventoryManager.inventory.itemSlots[slotIndex];
        if (slot.IsEmpty) return;
        dragIcon = new GameObject("DragIcon").AddComponent<Image>();
        dragIcon.sprite = slot.ItemData._icon;
        dragIcon.color = Color.white;
        dragIcon.raycastTarget = false;
        var parent = parentIcon != null ? parentIcon : inventoryManager.transform.parent;
        dragIcon.transform.SetParent(parent);
        dragIcon.rectTransform.sizeDelta = new Vector2(128 , 128);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (dragIcon != null)
            Destroy(dragIcon.gameObject);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryManager.OnPointerClickSlotUI(slotIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {

        inventoryManager.OnDropItem(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            backGround.color = Color.gray;
            return;
        } 
        if (!eventData.pointerDrag.TryGetComponent<IDragDrop>(out IDragDrop start))
        {
            return;
        }
        if( start.GetIndexSlot() < 0)
        {
            return;
        }
        if (inventoryManager.inventory.TryDropItem(inventoryManager.inventory.itemSlots[start.GetIndexSlot()], inventoryManager.inventory.itemSlots[slotIndex]))
        {
            backGround.color = Color.green;
        }
        else
        {
            backGround.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      backGround.color = Color.white;
    }

    public InventorySlot GetInventorySlot()
    {
        return inventoryManager.inventory.itemSlots[slotIndex];
    }

    public int GetIndexSlot()
    {
       return slotIndex;    
    }

    public void SetIconParent(Transform parent)
    {
        this.parentIcon = parent;
    }

    public void SetInventorySlot(InventorySlot slot)
    {
       //not needed;
    }
}
