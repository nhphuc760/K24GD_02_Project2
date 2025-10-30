using UnityEngine;
using UnityEngine.EventSystems;

public interface IDragDrop : IDragHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
    public ItemDataSO GetItemDataSO();

}
