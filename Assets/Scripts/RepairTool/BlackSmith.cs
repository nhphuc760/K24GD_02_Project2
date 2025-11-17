using System.Collections.Generic;
using UnityEngine;

public class BlackSmith : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Transform container;
    InventoryManager invenManager;

    private void Awake()
    {
        canvas ??= transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if(collision.CompareTag("GroundCheck"))
         {
            if(invenManager == null){
                GameEventManager.Ins.inventoryEvent.GetDataInventory(invenManager);
                InitSlot();
            }
            ShowCanvas();
        }
    }


    void InitSlot()
    {
        for (int i = 0; i < invenManager.slotAmount; i++)
        {
            var slotUI = Instantiate(invenManager.slotUIPrefab, container);
            slotUI.Init(invenManager, i);
        }
    }

    public void ShowCanvas()
    {
        if (invenManager == null) return;
        invenManager.UpdateUI();
        canvas.SetActive(true);
        GameEventManager.Ins.gameInput.Disable_InputAction();
    }
    public void HideCanvas()
    {
        canvas.SetActive(false);
        GameEventManager.Ins.gameInput.Enable_InputAction();
    }

}
