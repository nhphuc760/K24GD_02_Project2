using UnityEngine;
using UnityEngine.EventSystems;

public interface IDragDrop : IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    public InventorySlot GetInventorySlot();
    public void SetInventorySlot(InventorySlot slot);
}
