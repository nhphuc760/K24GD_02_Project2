using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolKitSlotUI : MonoBehaviour, IDragDrop
{
    int slotIndex;
    ToolKit toolKit;
    [SerializeField] Image _icon;
    [SerializeField] Image _selectCursor;

    public void Init(int index, ToolKit toolKit)
    {
        this.slotIndex = index;
        this.toolKit = toolKit;
    }
    public ItemDataSO GetItemDataSO()
    {
        return null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    public void OnDrag(PointerEventData eventData)
    {
      
    }

    public void OnDrop(PointerEventData eventData)
    {
        //if ()
        //{

        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
}
   
