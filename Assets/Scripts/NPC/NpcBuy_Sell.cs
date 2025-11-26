using UnityEngine;

public class NpcBuy_Sell : MonoBehaviour, IInteractable
{

    bool isPlayerNear;
    public bool CanInteract()
    {
       return isPlayerNear;
    }

    public void Interact()
    {
        GameEventManager.Ins.inventoryEvent.OpenBagPress();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            GameEventManager.Ins.OnNearSell(true);
           
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEventManager.Ins.OnNearSell(false);
            isPlayerNear = false;
            GameEventManager.Ins.inventoryEvent.DisableInventory();
        }
    }

}
